FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/gatekeeper/gatekeeper.csproj", "src/gatekeeper/"]
RUN dotnet restore "src/gatekeeper/gatekeeper.csproj"
COPY . .
WORKDIR "/src/src/gatekeeper"
RUN dotnet build "gatekeeper.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "gatekeeper.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "gatekeeper.dll"]