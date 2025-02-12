FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TestConsole/TestConsole.csproj", "TestConsole/"]
RUN dotnet restore "TestConsole/TestConsole.csproj"
COPY . .
WORKDIR "/src/TestConsole"
RUN dotnet build "TestConsole.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestConsole.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
ARG KEYVAULT_VAULT_URI
ARG KEYVAULT_SECRET_ID

ENV KEYVAULT_VAULT_URI=$KEYVAULT_VAULT_URI
ENV KEYVAULT_SECRET_ID=$KEYVAULT_SECRET_ID
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestConsole.dll"]
