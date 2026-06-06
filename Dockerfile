# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["ShoppeFake.sln", "."]
COPY ["ShoppeFake.API/ShoppeFake.API.csproj", "ShoppeFake.API/"]
COPY ["ShoppeFake.Application/ShoppeFake.Application.csproj", "ShoppeFake.Application/"]
COPY ["ShoppeFake.Domain/ShoppeFake.Domain.csproj", "ShoppeFake.Domain/"]
COPY ["ShoppeFake.Infrastructure/ShoppeFake.Infrastructure.csproj", "ShoppeFake.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "ShoppeFake.sln"

# Copy the rest of the source code
COPY . .

# Build the application
RUN dotnet build "ShoppeFake.sln" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ShoppeFake.API/ShoppeFake.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install additional dependencies if needed
RUN apt-get update && apt-get install -y --no-install-recommends \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=publish /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/swagger/index.html || exit 1

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["dotnet", "ShoppeFake.API.dll"]
