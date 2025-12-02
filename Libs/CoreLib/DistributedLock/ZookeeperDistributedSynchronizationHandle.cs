using DistributedLock.Interfaces;
using ZooKeeperNet;

namespace DistributedLock;

public class ZookeeperDistributedSynchronizationHandle : IDistributedSynchronizationHandle
{
    private readonly ZooKeeper _zookeeper;
    private readonly string _lockPath;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public ZookeeperDistributedSynchronizationHandle(ZooKeeper zookeeper, string lockPath)
    {
        _zookeeper = zookeeper;
        _lockPath = lockPath;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public CancellationToken HandleLostToken => _cancellationTokenSource.Token;

    public void Dispose()
    {
        _zookeeper.Delete(_lockPath, -1);
        _cancellationTokenSource.Cancel();
    }

    public async ValueTask DisposeAsync()
    {
        await Task.Run(() => _zookeeper.Delete(_lockPath, -1));
        _cancellationTokenSource.Cancel();
    }
}