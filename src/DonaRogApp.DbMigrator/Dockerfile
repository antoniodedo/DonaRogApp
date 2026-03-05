FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

RUN dotnet publish src/DonaRogApp.DbMigrator/DonaRogApp.DbMigrator.csproj \
    -c Release \
    -o /app/publish \
    --no-self-contained

FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "DonaRogApp.DbMigrator.dll"]
