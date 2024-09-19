# Base image for the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build image for the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Presentation/Dumps.API/Dumps.API.csproj", "src/Presentation/Dumps.API/"]
COPY ["src/Infrastructure/Dumps.Infrastructure/Dumps.Infrastructure.csproj", "src/Infrastructure/Dumps.Infrastructure/"]
COPY ["src/Infrastructure/Dumps.Persistence/Dumps.Persistence.csproj", "src/Infrastructure/Dumps.Persistence/"]
COPY ["src/Core/Dumps.Application/Dumps.Application.csproj", "src/Core/Dumps.Application/"]
COPY ["src/Core/Dumps.Domain/Dumps.Domain.csproj", "src/Core/Dumps.Domain/"]
RUN dotnet restore "./src/Presentation/Dumps.API/Dumps.API.csproj"
COPY . .
WORKDIR "/src/src/Presentation/Dumps.API"
RUN dotnet build "./Dumps.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Dumps.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Install EF Core tools and apply migrations
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS migrations
WORKDIR /src
COPY ["src/Infrastructure/Dumps.Infrastructure/Dumps.Infrastructure.csproj", "src/Infrastructure/Dumps.Infrastructure/"]
COPY ["src/Infrastructure/Dumps.Persistence/Dumps.Persistence.csproj", "src/Infrastructure/Dumps.Persistence/"]
COPY ["src/Presentation/Dumps.API/Dumps.API.csproj", "src/Presentation/Dumps.API/"]
COPY ["src/Core/Dumps.Application/Dumps.Application.csproj", "src/Core/Dumps.Application/"]
COPY ["src/Core/Dumps.Domain/Dumps.Domain.csproj", "src/Core/Dumps.Domain/"]
RUN dotnet restore "./src/Infrastructure/Dumps.Infrastructure/Dumps.Infrastructure.csproj"
RUN dotnet tool install --version 8.0.0 --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
COPY . .
ENTRYPOINT ["dotnet-ef", "database", "update", "--project", "src/Infrastructure/Dumps.Persistence/", "--startup-project", "src/Presentation/Dumps.API/"]

# Final image for the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dumps.API.dll"]
