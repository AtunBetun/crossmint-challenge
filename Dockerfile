FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app
COPY ./src .
RUN dotnet restore

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app

COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "CrossmintChallenge.Host.dll"]
