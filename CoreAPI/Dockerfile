#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
RUN apt-get update
RUN apt-get install -y apt-utils
RUN apt-get install -y libgdiplus
RUN apt-get install -y libc6-dev
RUN ln -s /usr/lib/libgdiplus.so /usr/lib/gdiplus.dll
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["CoreAPI/CoreAPI.csproj", "CoreAPI/"]
COPY ["DAL/DAL.csproj", "DAL/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["BL/BL.csproj", "BL/"]
COPY ["DTL/DTL.csproj", "DTL/"]
RUN dotnet restore "CoreAPI/CoreAPI.csproj"
COPY . .
WORKDIR "/src/CoreAPI"
RUN dotnet build "CoreAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CoreAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoreAPI.dll"]
