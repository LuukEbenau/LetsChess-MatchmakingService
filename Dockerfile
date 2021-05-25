#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["LetsChess-MatchmakingService/LetsChess-MatchmakingService.csproj", "LetsChess-MatchmakingService/"]
RUN dotnet restore "LetsChess-MatchmakingService/LetsChess-MatchmakingService.csproj"
COPY . .
WORKDIR "/src/LetsChess-MatchmakingService"
RUN dotnet build "LetsChess-MatchmakingService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LetsChess-MatchmakingService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LetsChess-MatchmakingService.dll"]