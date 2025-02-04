using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GestaoPedidosPayment.Startup
{
    public static class MongoBootstrap
    {
        public static WebApplicationBuilder AddMongoDb(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<MongoDBSettings>(
                builder.Configuration.GetSection("MongoDBSettings"));

            builder.Services.AddSingleton<IMongoClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
                return new MongoClient(settings.ConnectionString);
            });

            builder.Services.AddSingleton(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
                return client.GetDatabase(settings.DatabaseName);
            });

            return builder;
        }
    }
}
