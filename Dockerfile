# Stage 1: Build application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copy everything
COPY . .

# Restore dependencies
RUN dotnet restore

# Build and publish API
RUN dotnet publish PagePulse.API/PagePulse.API.csproj \
    -c Release \
    -o /out


# Stage 2: Run application
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Copy published files from build stage
COPY --from=build /out .

# Render uses port 8080
EXPOSE 8080

# Start API
ENTRYPOINT ["dotnet", "PagePulse.API.dll"]