FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5225

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["QuickApp.Server/QuickApp.Server.csproj", "QuickApp.Server/"]
COPY ["QuickApp.Core/QuickApp.Core.csproj", "QuickApp.Core/"]
COPY ["quickapp.client/quickapp.client.esproj", "quickapp.client/"]
RUN dotnet restore "QuickApp.Server/QuickApp.Server.csproj"
COPY QuickApp.Server/ QuickApp.Server/
COPY QuickApp.Core/ QuickApp.Core/
WORKDIR "/src/QuickApp.Server"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:5225
ENTRYPOINT ["dotnet", "QuickApp.Server.dll"]
