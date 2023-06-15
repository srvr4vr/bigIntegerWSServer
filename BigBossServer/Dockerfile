FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 5083

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BigBossServer/BigBossServer.csproj", "BigBossServer/"]
RUN dotnet restore "BigBossServer/BigBossServer.csproj"
COPY . .
WORKDIR "/src/WebApplication1"
RUN dotnet build "BigBossServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BigBossServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BigBossServer.dll"]
