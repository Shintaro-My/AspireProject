using Microsoft.Extensions.Hosting;

namespace WebApi.Util
{
    /// <summary>
    /// （抽象クラス）バックグラウンドタスクを作成する
    /// ExecuteAsyncをオーバーライドしてタスク処理を記述する
    /// services.AddHostedService<MyBgService>();などで登録する
    /// </summary>
    public abstract class BackgroundService : IHostedService, IDisposable
    {
        private Task _executingTask;
        private readonly CancellationTokenSource _mainTaskCancellationTokenSource = new CancellationTokenSource();

        protected abstract Task ExecuteAsync(CancellationToken cancellationToken);

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            _executingTask = ExecuteAsync(_mainTaskCancellationTokenSource.Token);

            if (_executingTask.IsCompleted) return _executingTask;
            return Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null) return;

            try
            {
                _mainTaskCancellationTokenSource.Cancel();
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        public virtual void Dispose()
        {
            _mainTaskCancellationTokenSource.Cancel();
        }
    }
}
