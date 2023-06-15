FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BigBossServerSimple/BigBossServerSimple.csproj", "BigBossServerSimple/"]
RUN dotnet restore "BigBossServerSimple/BigBossServerSimple.csproj"
COPY . .
WORKDIR "/src/BigBossServerSimple"
RUN dotnet build "BigBossServerSimple.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BigBossServerSimple.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BigBossServerSimple.dll"]
