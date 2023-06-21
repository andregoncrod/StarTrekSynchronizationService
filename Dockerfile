#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["StarTrekSynchronizationService/StarTrekSynchronizationService.csproj", "StarTrekSynchronizationService/"]
RUN dotnet restore "StarTrekSynchronizationService/StarTrekSynchronizationService.csproj"
COPY . .
WORKDIR "/src/StarTrekSynchronizationService"
RUN dotnet build "StarTrekSynchronizationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StarTrekSynchronizationService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StarTrekSynchronizationService.dll"]