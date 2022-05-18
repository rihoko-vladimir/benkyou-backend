FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Notification.Api/Notification.Api.csproj", "Notification.Api/"]
RUN dotnet restore "Notification.Api/Notification.Api.csproj"
COPY . .
WORKDIR "/src/Notification.Api"
RUN dotnet build "Notification.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Notification.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notification.Api.dll"]
