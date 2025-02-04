using GestaoPedidosPayment.Core.Shared.Infra;
using GestaoPedidosPayment.Middlewares;
using GestaoPedidosPayment.Startup;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Scan(scan => scan
    .FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
    .AddClasses(classes => classes.AssignableTo<ITransient>())
    .AsImplementedInterfaces()
    .WithTransientLifetime()
    .AddClasses(classes => classes.AssignableTo<IScoped>())
    .AsImplementedInterfaces()
    .WithScopedLifetime()
    .AddClasses(classes => classes.AssignableTo<ISingleton>())
    .AsImplementedInterfaces()
    .WithSingletonLifetime()
);

builder.AddMongoDb()
    .AddUnitOfWork();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
