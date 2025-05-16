using Microsoft.Extensions.Logging;

namespace tenis_pro_back.Helpers
{
    public static class HandleErrorHelper
    {
        private static ILogger? _logger;

        public static void Initialize(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("HandleErrorHelper");
        }

        public static void LogError(Exception ex)
        {
            if (_logger == null) return; // o lanzar excepción si preferís

            string message = ex.Message;
            if (ex.InnerException != null)
            {
                message += Environment.NewLine + ex.InnerException;
            }

            _logger.LogError(message);
        }
    }
}
