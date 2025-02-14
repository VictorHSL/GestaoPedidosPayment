using GestaoPedidosPayment.Core.Shared.Infra;
using GestaoPedidosPayment.ExternalServices;
using GestaoPedidosPayment.Middlewares;
using GestaoPedidosPayment.Startup;
using System.Text.Json.Serialization;

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

builder.Services.AddSingleton<IPaymentGatewayService, MercadoPagoPaymentGatewayService>();
builder.Services.AddHttpClient();

builder
    .AddMongoDb()
    .AddAutoMapper()
    .AddUnitOfWork();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
