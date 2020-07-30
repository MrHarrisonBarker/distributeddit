FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/PostService/PostService.csproj", "src/PostService/"]
RUN dotnet restore "src/PostService/PostService.csproj"
COPY . .
WORKDIR "/src/src/PostService"
RUN dotnet build "PostService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PostService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PostService.dll"]