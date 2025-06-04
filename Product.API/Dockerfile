# Build stage
# Use the .NET 9 SDK image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files and restore dependencies first.
# This layer is only invalidated if the .csproj files change.
COPY ["Product.API/Product.API.csproj", "Product.API/"]
RUN dotnet restore "Product.API/Product.API.csproj"

# Copy the rest of the source code
COPY . .
WORKDIR "/src/Product.API"
RUN dotnet publish "Product.API.csproj" -c Release -o /app/publish

# Runtime stage
# Use the smaller ASP.NET 9 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

# Expose HTTP port
EXPOSE 80
# Expose HTTPS port
EXPOSE 443

# The volume from docker-compose will manage the Data directory,
# so the 'COPY Data ./Data' line is not needed here.

ENTRYPOINT ["dotnet", "Product.API.dll"]