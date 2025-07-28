using LMS.Application.Abstractions.Loggings;
using Microsoft.Extensions.Logging;

namespace LMS.Infrastructure.Helpers.Loggings
{
    public class AppLogger<TEntity> : IAppLogger<TEntity>
    {
        private readonly ILogger<TEntity> _logger;

        public AppLogger(
            ILogger<TEntity> logger)
        {
            _logger = logger;
        }

        public void LogError(string message, Exception exception) => _logger.LogError(exception, message);

        public void LogInformation(string message) => _logger.LogInformation(message);

        public void LogWarning(string message) => _logger.LogWarning(message);
    }
}
