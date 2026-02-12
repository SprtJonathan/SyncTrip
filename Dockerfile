FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
RUN apt-get update && apt-get install -y --no-install-recommends libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copier les .csproj et restaurer les dépendances (cache Docker)
COPY src/SyncTrip.Core/SyncTrip.Core.csproj src/SyncTrip.Core/
COPY src/SyncTrip.Shared/SyncTrip.Shared.csproj src/SyncTrip.Shared/
COPY src/SyncTrip.Application/SyncTrip.Application.csproj src/SyncTrip.Application/
COPY src/SyncTrip.Infrastructure/SyncTrip.Infrastructure.csproj src/SyncTrip.Infrastructure/
COPY src/SyncTrip.API/SyncTrip.API.csproj src/SyncTrip.API/
RUN dotnet restore src/SyncTrip.API/SyncTrip.API.csproj

# Copier le code source et publier
COPY src/ src/
RUN dotnet publish src/SyncTrip.API/SyncTrip.API.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SyncTrip.API.dll"]
