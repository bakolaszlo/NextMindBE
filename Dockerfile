#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NextMindBE/NextMindBE.csproj", "NextMindBE/"]
RUN dotnet restore "NextMindBE/NextMindBE.csproj"
COPY . .
WORKDIR "/src/NextMindBE"
RUN dotnet build "NextMindBE.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NextMindBE.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
RUN apt-get update && apt-get install -y openssl
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NextMindBE.dll"]