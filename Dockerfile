# ── Build stage ──
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ReservasXYZ.sln ./
COPY ReservasXYZ.Domain/ReservasXYZ.Domain.csproj ReservasXYZ.Domain/
COPY ReservasXYZ.Application/ReservasXYZ.Application.csproj ReservasXYZ.Application/
COPY ReservasXYZ.Infrastructure/ReservasXYZ.Infrastructure.csproj ReservasXYZ.Infrastructure/
COPY ReservasXYZ.Web/ReservasXYZ.Web.csproj ReservasXYZ.Web/
RUN dotnet restore

COPY . .

WORKDIR /src/ReservasXYZ.Web
RUN apt-get update && apt-get install -y nodejs npm && npm ci && npm run css:build
RUN dotnet publish -c Release -o /app/publish --no-restore

# ── Runtime stage ──
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5000

EXPOSE 5000
ENTRYPOINT ["dotnet", "ReservasXYZ.Web.dll"]
