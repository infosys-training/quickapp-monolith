FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5225

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["QuickApp.Server/QuickApp.Server.csproj", "QuickApp.Server/"]
COPY ["QuickApp.Core/QuickApp.Core.csproj", "QuickApp.Core/"]
# Remove SpaProxy and client project references for server-only Docker build
RUN sed -i '/<ProjectReference Include=".*quickapp\.client.*"/d' QuickApp.Server/QuickApp.Server.csproj && \
    sed -i '/<ReferenceOutputAssembly>false<\/ReferenceOutputAssembly>/d' QuickApp.Server/QuickApp.Server.csproj && \
    sed -i '/<\/ProjectReference>/{ N; /^<\/ProjectReference>\n$/d; }' QuickApp.Server/QuickApp.Server.csproj && \
    sed -i '/<PackageReference Include="Microsoft.AspNetCore.SpaProxy"/,/<\/PackageReference>/d' QuickApp.Server/QuickApp.Server.csproj
RUN dotnet restore "QuickApp.Server/QuickApp.Server.csproj" --ignore-failed-sources
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
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "QuickApp.Server.dll"]
