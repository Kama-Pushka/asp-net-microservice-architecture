using DistributedLock.Interfaces;
using org.apache.zookeeper;

namespace DistributedLock;

public class ZookeeperDistributedSemaphore : IDistributedSemaphore
{
    private readonly ZooKeeper _zookeeper;
    private readonly string _semaphorePath;
    private readonly int _maxPermits;

    public ZookeeperDistributedSemaphore(ZooKeeper zookeeper, string semaphorePath, int maxPermits)
    {
        _zookeeper = zookeeper;
        _semaphorePath = semaphorePath;
        _maxPermits = maxPermits;
        
        if (!ExistsAsync(_semaphorePath).Result)
        {
            _zookeeper.createAsync(_semaphorePath, new byte[0], ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT).Wait();
        }
    }
    public string Name => _semaphorePath;

    public int MaxCount => _maxPermits;

    private async Task<bool> ExistsAsync(string path)
    {
        try
        {
            var stat = await _zookeeper.existsAsync(path);
            return stat != null;
        }
        catch (KeeperException.NoNodeException)
        {
            return false;
        }
    }
    
    public IDistributedSynchronizationHandle? TryAcquire(TimeSpan timeout = default, CancellationToken cancellationToken = default)
    {
        var handle = TryAcquireInternal(timeout, cancellationToken).AsTask().GetAwaiter().GetResult();
        return handle;
    }

    public IDistributedSynchronizationHandle Acquire(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var time = timeout ?? Timeout.InfiniteTimeSpan;
        var handle = TryAcquireInternal(time, cancellationToken).AsTask().GetAwaiter().GetResult();
        return handle ?? throw new TimeoutException("Failed to acquire semaphore within the specified timeout.");
    }

    public async ValueTask<IDistributedSynchronizationHandle?> TryAcquireAsync(TimeSpan timeout = default, CancellationToken cancellationToken = default)
    {
        var handle = await TryAcquireInternal(timeout, cancellationToken);
        return handle;
    }

    public async ValueTask<IDistributedSynchronizationHandle> AcquireAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var time = timeout ?? Timeout.InfiniteTimeSpan;
        var handle = await TryAcquireInternal(time, cancellationToken);
        return handle ?? throw new TimeoutException("Failed to acquire semaphore within the specified timeout.");
    }
    
    private async ValueTask<IDistributedSynchronizationHandle?> TryAcquireInternal(TimeSpan? timeout, CancellationToken cancellationToken = default)
    {
        var deadline = DateTime.UtcNow + timeout;
        var guid =  Guid.NewGuid().ToString();
        
        do
        {
            cancellationToken.ThrowIfCancellationRequested();
            var handle = await TryGetHandle(guid);
            if (handle != null)
                return handle;

            await Task.Delay(100, cancellationToken); // TODO константа 100, вынести в какой-то конфиг
        } while (deadline > DateTime.UtcNow || timeout == Timeout.InfiniteTimeSpan);
        
        await CleanupTemporaryNode(guid); // не дождались блокировку, удаляем созданную ноду
        return null;
    }
    
    private async Task<IDistributedSynchronizationHandle?> TryGetHandle(string guid)
    {
        try
        {
            var baseLockPath = $"{_semaphorePath}/{guid}-lock-";
            string lockPath;
            
            var children = await _zookeeper.getChildrenAsync(_semaphorePath);
            var existingNode = children.Children.FirstOrDefault(child => child.StartsWith(guid));
            if (existingNode != null)
            {
                lockPath = $"{_semaphorePath}/{existingNode}";
            }
            else
            {
                lockPath = await _zookeeper.createAsync(baseLockPath, new byte[0], 
                    ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.EPHEMERAL_SEQUENTIAL);
            }
            
            // Повторное получение дочерних узлов нужно для того, чтобы гарантировать, что во время ожидания 
            // создания ноды если кто-то успеет создать ноду раньше нас, то мы об этом знали 
            // TODO есть более оптимальный способ достичь этой гарантии?
            var updatedChildren = await _zookeeper.getChildrenAsync(_semaphorePath); 
            
            var sortedChildren = SortChildren(updatedChildren.Children);
            var nodePosition = sortedChildren.IndexOf(Path.GetFileName(lockPath));
            return nodePosition < _maxPermits ? new ZookeeperDistributedSynchronizationHandle(_zookeeper, lockPath) : null;
        }
        catch (KeeperException.NodeExistsException)
        {
            return null;
        }
    }
    
    private List<string> SortChildren(IEnumerable<string> children)
    {
        return children.OrderBy(child => ExtractNumericSuffix(child)).ToList();
    }
    
    private ulong ExtractNumericSuffix(string child)
    {
        var suffix = child.Split("-").Last();
        return Convert.ToUInt64(suffix);
    }
    
    private async Task CleanupTemporaryNode(string guid)
    {
        try
        {
            var children = await _zookeeper.getChildrenAsync(_semaphorePath);
            var existingNode = children.Children.FirstOrDefault(child => child.StartsWith(guid));
            if (existingNode != null)
            {
                await _zookeeper.deleteAsync($"{_semaphorePath}/{existingNode}");
            }
        }
        catch (KeeperException.NoNodeException)
        { // Игнорируем ошибку TODO добавить запись в лог, если будет эта ошибка?
        }
    }
}