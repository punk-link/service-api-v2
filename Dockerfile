FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base

ARG PNKL_VAULT_TOKEN

ENV PNKL_VAULT_TOKEN=$PNKL_VAULT_TOKEN


WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0  AS build
ARG GITHUB_TOKEN
WORKDIR /src
COPY *.sln ./
COPY . .
RUN dotnet restore
WORKDIR /src/Api
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app

COPY --from=publish /app .

#HEALTHCHECK --interval=6s --timeout=10s --retries=3 CMD curl -sS 127.0.0.1/health || exit 1

ENTRYPOINT ["dotnet", "Api.dll"]


