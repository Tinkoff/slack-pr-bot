using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SlackPrBot.Services.Impl;

namespace SlackPrBot.HostedServices
{
    internal class StaleNotification : IHostedService, IDisposable
    {
        private readonly IConfiguration _config;
        private readonly IServiceProvider _services;
        private Timer _timer;

        public StaleNotification(IServiceProvider services, IConfiguration config)
        {
            _services = services;
            _config = config;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var checkTime = _config.GetValue<TimeSpan>("StaleTimerCheck");

            if (checkTime != TimeSpan.Zero)
            {
                _timer = new Timer(DoWorkAsync, null, TimeSpan.Zero, checkTime);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private async void DoWorkAsync(object state)
        {
            using var scope = _services.CreateScope();
            var service = scope.ServiceProvider.GetService<NotificationService>();
            await service.CheckOverdueAsync();
        }
    }
}
