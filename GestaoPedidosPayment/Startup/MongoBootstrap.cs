using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;

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
                BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
                
                return new MongoClient(builder.Configuration.GetSection("MongoConnectionString").Value);
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
