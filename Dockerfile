FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ENV WebHookUrl=http://your-webhook-url
ENV MongoConnectionString=mongodb://admin:password@mongo_db:27017/

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GestaoPedidosPayment/GestaoPedidosPayment.csproj", "GestaoPedidosPayment/"]
RUN dotnet restore "./GestaoPedidosPayment/GestaoPedidosPayment.csproj"
COPY . .
WORKDIR "/src/GestaoPedidosPayment"
RUN dotnet build "./GestaoPedidosPayment.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GestaoPedidosPayment.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GestaoPedidosPayment.dll"]