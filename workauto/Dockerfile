#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

EXPOSE 7008

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["workauto/workauto.csproj", "workauto/"]
COPY ["Mysqldb/Mysqldb.csproj", "Mysqldb/"]
RUN dotnet restore "workauto/workauto.csproj"
COPY . .
WORKDIR "/src/workauto"
RUN dotnet build "workauto.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "workauto.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://*:7008
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "workauto.dll"]