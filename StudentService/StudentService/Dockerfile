# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the project file to restore dependencies
COPY ["./StudentService.csproj", "./"]
RUN dotnet restore "./StudentService.csproj"

# Copy the entire project and build it
COPY . .
RUN dotnet build "./StudentService.csproj" -c Release -o /app/build

# Publish the project
FROM build AS publish
RUN dotnet publish "./StudentService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StudentService.dll"]
