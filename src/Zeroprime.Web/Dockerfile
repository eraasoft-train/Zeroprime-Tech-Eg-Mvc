# Build stage ----------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Restore first (better layer caching)
COPY Zeroprime.sln ./
COPY src/Zeroprime.Domain/Zeroprime.Domain.csproj src/Zeroprime.Domain/
COPY src/Zeroprime.Application/Zeroprime.Application.csproj src/Zeroprime.Application/
COPY src/Zeroprime.Infrastructure/Zeroprime.Infrastructure.csproj src/Zeroprime.Infrastructure/
COPY src/Zeroprime.Web/Zeroprime.Web.csproj src/Zeroprime.Web/
RUN dotnet restore src/Zeroprime.Web/Zeroprime.Web.csproj

# Build & publish
COPY . .
RUN dotnet publish src/Zeroprime.Web/Zeroprime.Web.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Runtime stage ---------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    Database__MigrateOnStartup=true

EXPOSE 8080

# The aspnet base image ships a non-root 'app' user.
USER app

COPY --from=build --chown=app:app /app/publish ./

ENTRYPOINT ["dotnet", "Zeroprime.Web.dll"]
