namespace GestaoPedidosPayment.Startup
{
    public static class AutoMapper
    {
        public static WebApplicationBuilder AddAutoMapper(this WebApplicationBuilder builder)
        {
            builder.Services.AddAutoMapper(assemblies: new[]
            {
                typeof(Program).Assembly,
            });

            return builder;
        }
    }
}
