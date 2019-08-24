FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["DisDet.Web/DisDet.Web.csproj", "DisDet.Web/"]
COPY ["DisDet.Core/DisDet.Core.csproj", "DisDet.Core/"]
RUN dotnet restore "DisDet.Web/DisDet.Web.csproj"
COPY . .
WORKDIR "/src/DisDet.Web"
RUN dotnet build "DisDet.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "DisDet.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DisDet.Web.dll"]