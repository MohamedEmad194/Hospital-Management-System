# Use the official .NET 8.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY "Hospital Mangement System.csproj" .
RUN dotnet restore "Hospital Mangement System.csproj"

# Copy all source code
COPY . .
WORKDIR "/src/Hospital Mangement System"
RUN dotnet build "Hospital Mangement System.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Hospital Mangement System.csproj" -c Release -o /app/publish

# Use the official .NET 8.0 runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

# Expose ports
EXPOSE 80
EXPOSE 443

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80;https://+:443

# Start the application
ENTRYPOINT ["dotnet", "Hospital Mangement System.dll"]
