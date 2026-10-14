FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Directory.Build.props ./
COPY db-migrator/DbMigrator.csproj db-migrator/
RUN dotnet restore db-migrator/DbMigrator.csproj
COPY db-migrator/ db-migrator/
RUN dotnet publish db-migrator/DbMigrator.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "DbMigrator.dll"]
