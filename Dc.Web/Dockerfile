FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Dc.Web/Dc.Web.csproj", "Dc.Web/"]
COPY ["Dc.Core/Dc.Core.csproj", "Dc.Core/"]
RUN dotnet restore "Dc.Web/Dc.Web.csproj"
COPY . .
WORKDIR "/src/Dc.Web"
RUN dotnet build "Dc.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Dc.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Dc.Web.dll"]
