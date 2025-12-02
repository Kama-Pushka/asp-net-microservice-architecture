using DistributedLock.Interfaces;
using org.apache.zookeeper;

namespace DistributedLock;

public class ZookeeperDistributedSemaphore : IDistributedSemaphore
{
    private readonly ZooKeeper _zookeeper;
    private readonly string _semaphorePath;
    private readonly int _maxPermits;
    private readonly string _lockPath;

    public ZookeeperDistributedSemaphore(ZooKeeper zookeeper, string semaphorePath, int maxPermits)
    {
        _zookeeper = zookeeper;
        _semaphorePath = semaphorePath;
        _maxPermits = maxPermits;
        _lockPath = $"{semaphorePath}/lock";
        
        if (!ExistsAsync(_semaphorePath).Result)
        {
            _zookeeper.createAsync(_semaphorePath, new byte[0], ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT).Wait();
        }
    }

    public string Name => _semaphorePath;

    public int MaxCount => _maxPermits;

    public IDistributedSynchronizationHandle? TryAcquire(TimeSpan timeout = default, CancellationToken cancellationToken = default)
    {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var task = TryAcquireInternal();
        
        if (task.Wait(timeout, cts.Token))
            return task.Result;

        cts.Cancel();
        throw new TimeoutException("Failed to acquire semaphore within the specified timeout.");
    }

    public IDistributedSynchronizationHandle Acquire(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        timeout ??= Timeout.InfiniteTimeSpan;

        var deadline = DateTime.UtcNow + timeout.Value;
        while (deadline > DateTime.UtcNow || timeout == Timeout.InfiniteTimeSpan)
        {
            var time = timeout == Timeout.InfiniteTimeSpan ? Timeout.InfiniteTimeSpan : deadline - DateTime.UtcNow;
            var handle = TryAcquire(time, cancellationToken);
            if (handle != null)
                return handle;

            Thread.Sleep(100);
        }

        throw new TimeoutException("Failed to acquire semaphore within the specified timeout.");
    }

    public async ValueTask<IDistributedSynchronizationHandle?> TryAcquireAsync(TimeSpan timeout = default, CancellationToken cancellationToken = default)
    {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var task = TryAcquireInternal();
        
        if (await Task.WhenAny(task, Task.Delay(timeout, cts.Token)) == task)
            return task.Result;

        await cts.CancelAsync();
        throw new TimeoutException("Failed to acquire semaphore within the specified timeout.");
    }

    public async ValueTask<IDistributedSynchronizationHandle> AcquireAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        timeout ??= Timeout.InfiniteTimeSpan;

        var deadline = DateTime.UtcNow + timeout.Value;
        while (deadline > DateTime.UtcNow || timeout == Timeout.InfiniteTimeSpan)
        {
            var time = timeout == Timeout.InfiniteTimeSpan ? Timeout.InfiniteTimeSpan : deadline - DateTime.UtcNow;
            var handle = await TryAcquireAsync(time, cancellationToken);
            if (handle != null)
                return handle;

            await Task.Delay(100, cancellationToken);
        }

        throw new TimeoutException("Failed to acquire semaphore within the specified timeout.");
    }
    
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
    
    private async Task<IDistributedSynchronizationHandle?> TryAcquireInternal()
    {
        try
        {
            var lockPath = await _zookeeper.createAsync(_lockPath, new byte[0], ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.EPHEMERAL_SEQUENTIAL);
            var children = await _zookeeper.getChildrenAsync(_semaphorePath);
            var count = children.Children.Count;

            if (count <= _maxPermits)
                return new ZookeeperDistributedSynchronizationHandle(_zookeeper, lockPath);

            await _zookeeper.deleteAsync(lockPath);
            return null;
        }
        catch (KeeperException.NodeExistsException)
        {
            return null;
        }
    }
}