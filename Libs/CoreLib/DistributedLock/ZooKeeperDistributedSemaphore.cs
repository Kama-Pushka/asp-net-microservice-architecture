using DistributedLock.Interfaces;
using ZooKeeperNet;

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
        while (DateTime.UtcNow < deadline)
        {
            var handle = TryAcquire(deadline - DateTime.UtcNow, cancellationToken);
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
        while (DateTime.UtcNow < deadline)
        {
            var handle = await TryAcquireAsync(deadline - DateTime.UtcNow, cancellationToken);
            if (handle != null)
                return handle;

            await Task.Delay(100, cancellationToken);
        }

        throw new TimeoutException("Failed to acquire semaphore within the specified timeout.");
    }
    
    private async Task<IDistributedSynchronizationHandle?> TryAcquireInternal()
    {
        try
        {
            var lockPath = await Task.Run(() => _zookeeper.Create(_lockPath, new byte[0], Ids.OPEN_ACL_UNSAFE, CreateMode.EphemeralSequential));
            var children = await Task.Run(() => _zookeeper.GetChildren(_semaphorePath, false));
            var count = children.Count();

            if (count <= _maxPermits)
                return new ZookeeperDistributedSynchronizationHandle(_zookeeper, lockPath);

            await Task.Run(() => _zookeeper.Delete(lockPath, -1));
            return null;
        }
        catch (KeeperException.NodeExistsException)
        {
            return null;
        }
    }
}