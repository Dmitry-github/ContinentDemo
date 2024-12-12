namespace ContinentDemo.WebApi
{
    using System.IO;
    using Microsoft.Extensions.Configuration;
    
    public static class ConfigAppSettings
    {
        private static IConfiguration Configuration
        {
            get
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                IConfiguration config = builder.Build();
                return config;
            }
        }

        public static string NetWorkRequestHost => Configuration["AppSettings:NetWorkRequestHost"] ?? string.Empty;

        public static string NetWorkRequestUri => Configuration["AppSettings:NetWorkRequestUri"] ?? string.Empty;

        public static int NetWorkRetryCount => Convert.ToInt32(Configuration["AppSettings:NetWorkRetryCount"]);

        public static int LocalCacheInitialCapacity => Convert.ToInt32(Configuration["AppSettings:LocalCacheInitialCapacity"]);

        public static int LocalCacheExpiresAfterHours => Convert.ToInt32(Configuration["AppSettings:LocalCacheExpiresAfterHours"]);

        public static bool UseDistributedCache => Convert.ToBoolean(Configuration["AppSettings:UseDistributedCache"]);

        public static int CachingOperationTimeOut => Convert.ToInt32(Configuration["AppSettings:CachingOperationTimeOut_ms"]);

        public static string RedisCacheConfigurationString => Configuration["AppSettings:RedisCacheConfigurationString"] ?? string.Empty;

        public static string RedisCacheInstanceName => Configuration["AppSettings:RedisCacheInstanceName"] ?? string.Empty;

        public static int RedisAbsoluteExpirationHours => Convert.ToInt32(Configuration["AppSettings:RedisAbsoluteExpirationHours"]);

        public static int RedisSlidingExpirationHours => Convert.ToInt32(Configuration["AppSettings:RedisSlidingExpirationHours"]);

        public static bool ExtendedLogEnabled => Convert.ToBoolean(Configuration["AppSettings:ExtendedLogEnabled"]);

        public static bool UseTimer => Convert.ToBoolean(Configuration["AppSettings:UseTimer"]);
    }
}
