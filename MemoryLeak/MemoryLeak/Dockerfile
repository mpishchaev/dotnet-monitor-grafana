FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY "MemoryLeak/MemoryLeak.sln" "MemoryLeak/MemoryLeak.sln"
COPY "MemoryLeak/MemoryLeak/MemoryLeak.csproj" "MemoryLeak/MemoryLeak/MemoryLeak.csproj"

RUN dotnet restore "MemoryLeak/MemoryLeak.sln"

COPY . .
WORKDIR /src/MemoryLeak/MemoryLeak
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish
FROM base AS final

WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MemoryLeak.dll"]