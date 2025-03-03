using Ocelot.DependencyInjection;
using System.Text;

namespace OcelotApiGateway
{
    public static class OcelotConfigurationExtensions
    {
        public static IServiceCollection AddCustomOcelot(this IServiceCollection services, IConfiguration configuration, string ocelotConfigPath)
        {
            var ocelotJson = File.ReadAllText(ocelotConfigPath);
            var replacements = new List<string>
            {
                "PRODUCT_SERVICE_HOST",
                "PRODUCT_SERVICE_PORT",
                "COUPON_SERVICE_HOST",
                "COUPON_SERVICE_PORT",
                "AUTH_SERVICE_HOST",
                "AUTH_SERVICE_PORT",
                "CART_SERVICE_HOST",
                "CART_SERVICE_PORT"
            };

            foreach (var i in replacements)
            {
                ocelotJson = ocelotJson
                    .Replace("{" + i + "}", Environment.GetEnvironmentVariable(i));
            }

            var configStream = new MemoryStream(Encoding.UTF8.GetBytes(ocelotJson));
            var newConfiguration = new ConfigurationBuilder()
                .AddJsonStream(configStream)
                .Build();

            services.AddOcelot(newConfiguration);

            return services;
        }
    }
}