#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Employee.API/Employee.API.csproj", "Employee.API/"]
COPY ["../SQS/SQS.csproj", "../SQS/"]
COPY ["../EventBusRabbitMQ/EventBusRabbitMQ.csproj", "../EventBusRabbitMQ/"]
COPY ["../EventBus/EventBus.csproj", "../EventBus/"]
COPY ["../IntegrationEventLogEF/IntegrationEventLogEF.csproj", "../IntegrationEventLogEF/"]
RUN dotnet restore "Employee.API/Employee.API.csproj"
COPY . .
WORKDIR "/src/Employee.API"
RUN dotnet build "Employee.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Employee.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Employee.API.dll"]