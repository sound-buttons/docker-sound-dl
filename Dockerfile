#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine AS base
WORKDIR /app
ENV AZURE_STORAGE_CONNECTION_STRING="ChangeThis"
ENV CHANNELS_IN_ARRAY="[\"https://www.youtube.com/channel/UCuy-kZJ7HWwUU-eKv0zUZFQ\", \"https://www.youtube.com/channel/UCBC7vYFNQoGPupe5NxPG4Bw\"]"
RUN apk add --no-cache --virtual build-deps musl-dev gcc g++ python3-dev &&\
    apk add --no-cache aria2 ffmpeg py3-pip &&\
    pip install --upgrade yt-dlp &&\
    apk del build-deps

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["docker-sound-dl.csproj", "."]
RUN dotnet restore "./docker-sound-dl.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "docker-sound-dl.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "docker-sound-dl.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "docker-sound-dl.dll"]