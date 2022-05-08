#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["War Thunder Store Scraper/War Thunder Store Scraper.csproj", "War Thunder Store Scraper/"]
RUN dotnet restore "War Thunder Store Scraper/War Thunder Store Scraper.csproj"
COPY . .
WORKDIR "/src/War Thunder Store Scraper"
RUN dotnet build "War Thunder Store Scraper.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "War Thunder Store Scraper.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV DATABASE_DIRECTORY=null
ENV REFRESH_INTERVAL=null

ENTRYPOINT dotnet "War Thunder Store Scraper.dll" --database-directory $DATABASE_DIRECTORY --refresh-interval $REFRESH_INTERVAL