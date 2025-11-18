# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY *.sln .
COPY TWGKafkaConsumerService/*.csproj TWGKafkaConsumerService/
COPY LFApiClient/*.csproj LFApiClient/

# Restore dependencies
RUN dotnet restore TWGKafkaConsumerService/TWGKafkaConsumerService.csproj

# Copy the rest of the source code
COPY . .

# Publish the worker service project
WORKDIR /src/TWGKafkaConsumerService
RUN dotnet publish -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Make sure ca-certificates package is installed
RUN apt-get update && apt-get install -y ca-certificates && rm -rf /var/lib/apt/lists/*

COPY certs/WIN11DEV_2.crt /usr/local/share/ca-certificates/
COPY certs/laserfiche-test.thewarehousegroup.net.crt /usr/local/share/ca-certificates/
COPY certs/laserfiche.thewarehousegroup.net.crt /usr/local/share/ca-certificates/
COPY certs/TWG-CA.crt /usr/local/share/ca-certificates/
COPY certs/WHAKLWNGCA1P.crt /usr/local/share/ca-certificates/
COPY certs/WHAKLWNGCA2P.crt /usr/local/share/ca-certificates/
RUN update-ca-certificates

# Create logs folder
RUN mkdir -p /app/logs

COPY --from=build /app .
ENTRYPOINT ["dotnet", "TWGKafkaConsumerService.dll"]
