using DistributedLock.Interfaces;
using org.apache.zookeeper;

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
        
        var connectionWatcher = new ConnectionWatcher(this);
        _zookeeper.existsAsync(_lockPath, connectionWatcher); // если сервис отвалится, будет получено уведомление об этом
    }

    public CancellationToken HandleLostToken => _cancellationTokenSource.Token;

    public void Dispose()
    {
        _zookeeper.deleteAsync(_lockPath);
        _cancellationTokenSource.Cancel();
    }

    public async ValueTask DisposeAsync()
    {
        await _zookeeper.deleteAsync(_lockPath);
        await _cancellationTokenSource.CancelAsync();
    }

    internal void NotifyConnectionLoss()
    {
        _cancellationTokenSource.Cancel();
    }

    private sealed class ConnectionWatcher : Watcher
    {
        private readonly ZookeeperDistributedSynchronizationHandle _parent;

        public ConnectionWatcher(ZookeeperDistributedSynchronizationHandle parent)
        {
            _parent = parent;
        }

        public override Task process(WatchedEvent @event)
        {
            if (@event.getState() == Event.KeeperState.Disconnected)
            {
                _parent.NotifyConnectionLoss();
            }
            return Task.CompletedTask;
        }
    }
}