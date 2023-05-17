FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Services/MCJanitor.Web/MCJanitor.Web.csproj", "src/Services/MCJanitor.Web/"]
RUN dotnet restore "src/Services/MCJanitor.Web/MCJanitor.Web.csproj"
COPY . .
WORKDIR "/src/src/Services/MCJanitor.Web"
RUN dotnet build "MCJanitor.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MCJanitor.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MCJanitor.Web.dll"]
