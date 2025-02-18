using GestaoPedidosPayment.Core.Shared.Infra;
using GestaoPedidosPayment.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GestaoPedidosPayment.Startup
{
    public static class UnitOfWorkBootstrap
    {
        public static IHostApplicationBuilder AddUnitOfWork(this IHostApplicationBuilder builder)
        {
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
                return new UnitOfWork(client, settings.DatabaseName);
            });

            return builder;
        }
    }
}
