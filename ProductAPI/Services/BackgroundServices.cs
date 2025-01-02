namespace ProductAPI.Services
{
    public class BackgroundServices : BackgroundService
    {
        private Timer? _timer;
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(_timer_Callback,
            null,
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }
        private void _timer_Callback(object? state)
        {
            // Console.WriteLine("background service is called");
        }
    }
}