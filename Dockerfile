FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src
COPY ["src/MobileShopBilling.Domain/MobileShopBilling.Domain.csproj", "MobileShopBilling.Domain/"]
COPY ["src/MobileShopBilling.Application/MobileShopBilling.Application.csproj", "MobileShopBilling.Application/"]
COPY ["src/MobileShopBilling.Infrastructure/MobileShopBilling.Infrastructure.csproj", "MobileShopBilling.Infrastructure/"]
COPY ["src/MobileShopBilling.Web/MobileShopBilling.Web.csproj", "MobileShopBilling.Web/"]
RUN dotnet restore "MobileShopBilling.Web/MobileShopBilling.Web.csproj"
COPY src/ .
RUN dotnet publish "MobileShopBilling.Web/MobileShopBilling.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# QuestPDF requires libfontconfig
RUN apt-get update && apt-get install -y libfontconfig1 && rm -rf /var/lib/apt/lists/*

ENTRYPOINT ["dotnet", "MobileShopBilling.Web.dll"]
