# Stage 1 — Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY *.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Stage 2 — Run
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MartinKulev.dll"]

HEALTHCHECK --interval=30s --timeout=5s \
  CMD wget -qO- http://localhost:8080/ || exit 1