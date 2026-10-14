# Construir desde Backend/ :  docker build -f user-service/Dockerfile .
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Directory.Build.props ./
COPY shared/ shared/
COPY user-service/src/ user-service/src/
RUN dotnet publish user-service/src/Controller/Controller.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "User.Api.dll"]
