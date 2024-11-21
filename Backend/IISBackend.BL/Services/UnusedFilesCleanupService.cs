using IISBackend.BL.Facades.Interfaces;
using IISBackend.BL.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IISBackend.BL.Services
{
    internal class UnusedFilesCleanupService : BackgroundService
    {
        private readonly IServiceProvider _provider;

        public UnusedFilesCleanupService(IServiceProvider provider)
        {
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CleanupUnusedImagesAsync();
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // Run every hour
            }
        }

        private async Task CleanupUnusedImagesAsync()
        {
            using var scope = _provider.CreateScope();
            var fileFacade = scope.ServiceProvider.GetRequiredService<IFileFacade>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<UnusedFilesCleanupService>>();
            try
            {
                await fileFacade.DeleteUnusedFiles(TimeSpan.FromMinutes(15));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error during cleanup of unused files");
            }
        }
    }
}
