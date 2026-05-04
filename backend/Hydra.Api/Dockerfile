# Stage 1: Build .NET API
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Hydra.Api.csproj", "./"]
RUN dotnet restore "Hydra.Api.csproj"
COPY . .
RUN dotnet publish "Hydra.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Hydra.Api.dll"]
