using Microsoft.Extensions.Configuration;

namespace Bot.Settings
{
    internal class ConfigReader
    {
        public static AppSettings Read()
        {
            var builder = new ConfigurationBuilder()
                //.SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();

            return configuration.Get<AppSettings>();
        }
    }
}
