# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy all files and restore dependencies
COPY . .
RUN dotnet restore

# Build and publish the app to the 'out' folder
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app

# Copy published files from build stage
COPY --from=build /app/out .

# Expose port 80 for HTTP traffic
EXPOSE 80

# Run the app
ENTRYPOINT ["dotnet", "YourAppName.dll"]