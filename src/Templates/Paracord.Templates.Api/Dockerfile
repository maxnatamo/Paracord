FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY ApplicationName.csproj .
RUN dotnet restore "ApplicationName.csproj"

COPY . .
RUN dotnet build "ApplicationName.csproj" --configuration Release -o /app/build
RUN dotnet publish "ApplicationName.csproj" --configuration Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS runner
WORKDIR /app

EXPOSE 8080
EXPOSE 8443

COPY --from=build /app/publish .

ENTRYPOINT [ "dotnet", "ApplicationName.dll" ]