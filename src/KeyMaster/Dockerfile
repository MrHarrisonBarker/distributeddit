FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/KeyMaster/KeyMaster.csproj", "src/KeyMaster/"]
RUN dotnet restore "src/KeyMaster/KeyMaster.csproj"
COPY . .
WORKDIR "/src/src/KeyMaster"
RUN dotnet build "KeyMaster.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KeyMaster.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KeyMaster.dll"]