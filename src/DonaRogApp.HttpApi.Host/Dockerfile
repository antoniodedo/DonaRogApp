FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

RUN dotnet publish src/DonaRogApp.HttpApi.Host/DonaRogApp.HttpApi.Host.csproj \
    -c Release \
    -o /app/publish \
    --no-self-contained

RUN dotnet dev-certs https -v -ep /app/publish/openiddict.pfx -p 35c5ebd2-19a4-4b32-8b17-24d91bf11c47

FROM mcr.microsoft.com/dotnet/aspnet:9.0

RUN apt-get update && apt-get install -y \
    wkhtmltopdf \
    libfontconfig1 \
    libxrender1 \
    libxext6 \
    --no-install-recommends \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "DonaRogApp.HttpApi.Host.dll"]
