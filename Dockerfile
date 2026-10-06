FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["RentalFlow.API/RentalFlow.API.csproj", "RentalFlow.API/"]
COPY ["RentalFlow.Application/RentalFlow.Application.csproj", "RentalFlow.Application/"]
COPY ["RentalFlow.Domain/RentalFlow.Domain.csproj", "RentalFlow.Domain/"]
COPY ["RentalFlow.Infrastructure/RentalFlow.Infrastructure.csproj", "RentalFlow.Infrastructure/"]
COPY ["RentalFlow.Crosscutting/RentalFlow.Crosscutting.csproj", "RentalFlow.Crosscutting/"]

RUN dotnet restore "RentalFlow.API/RentalFlow.API.csproj"

COPY . .

WORKDIR "/src/RentalFlow.API"
RUN dotnet publish "RentalFlow.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

RUN mkdir -p /app/logs && chown -R app:app /app

COPY --from=build /app/publish .

USER app

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "RentalFlow.API.dll"]