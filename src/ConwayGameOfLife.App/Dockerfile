
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ConwayGameOfLife.App/ConwayGameOfLife.App.csproj", "ConwayGameOfLife.App/"]
COPY ["ConwayGameOfLife.Application/ConwayGameOfLife.Application.csproj", "ConwayGameOfLife.Application/"]
COPY ["ConwayGameOfLife.Data/ConwayGameOfLife.Data.csproj", "ConwayGameOfLife.Data/"]
COPY ["ConwayGameOfLife.Web/ConwayGameOfLife.Web.csproj", "ConwayGameOfLife.Web/"]
RUN dotnet restore "ConwayGameOfLife.App/ConwayGameOfLife.App.csproj"
COPY . .
WORKDIR "/src/ConwayGameOfLife.App"
RUN dotnet build "ConwayGameOfLife.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ConwayGameOfLife.App.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConwayGameOfLife.App.dll"]