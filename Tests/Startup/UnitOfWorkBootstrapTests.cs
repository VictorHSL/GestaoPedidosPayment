using GestaoPedidosPayment.Core.Shared.Infra;
using GestaoPedidosPayment.Repositories;
using GestaoPedidosPayment.Startup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NSubstitute;
using Xunit;

namespace Tests.Startup
{
    public class UnitOfWorkBootstrapTests
    {
        [Fact(DisplayName = "AddUnitOfWork should register IUnitOfWork with correct dependencies")]
        public void AddUnitOfWork_ShouldRegisterIUnitOfWork_WithCorrectDependencies()
        {
            // Arrange
            var builder = Substitute.For<IHostApplicationBuilder>();
            var services = Substitute.For<IServiceCollection>();

            builder.Services.Returns(services);
            builder.AddUnitOfWork();

            services.ReceivedWithAnyArgs(1).AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
