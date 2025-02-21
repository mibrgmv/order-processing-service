FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/OrderProcessingService/OrderProcessingService.csproj", "src/OrderProcessingService/"]
COPY ["src/Presentation/OrderProcessingService.Presentation.Grpc/OrderProcessingService.Presentation.Grpc.csproj", "src/Presentation/OrderProcessingService.Presentation.Grpc/"]
COPY ["src/Application/OrderProcessingService.Application.Contracts/OrderProcessingService.Application.Contracts.csproj", "src/Application/OrderProcessingService.Application.Contracts/"]
COPY ["src/Application/OrderProcessingService.Application.Models/OrderProcessingService.Application.Models.csproj", "src/Application/OrderProcessingService.Application.Models/"]
COPY ["src/Infrastructure/OrderProcessingService.Infrastructure.Events/OrderProcessingService.Infrastructure.Events.csproj", "src/Infrastructure/OrderProcessingService.Infrastructure.Events/"]
COPY ["src/Application/OrderProcessingService.Application.Services/OrderProcessingService.Application.Services.csproj", "src/Application/OrderProcessingService.Application.Services/"]
COPY ["src/Application/OrderProcessingService.Application.Abstractions/OrderProcessingService.Application.Abstractions.csproj", "src/Application/OrderProcessingService.Application.Abstractions/"]
COPY ["src/Infrastructure/OrderProcessingService.Infrastructure.Persistence/OrderProcessingService.Infrastructure.Persistence.csproj", "src/Infrastructure/OrderProcessingService.Infrastructure.Persistence/"]
COPY ["src/Presentation/OrderProcessingService.Presentation.Kafka/OrderProcessingService.Presentation.Kafka.csproj", "src/Presentation/OrderProcessingService.Presentation.Kafka/"]
COPY ["src/Infrastructure/OrderProcessingService.Infrastructure.Kafka/OrderProcessingService.Infrastructure.Kafka.csproj", "src/Infrastructure/OrderProcessingService.Infrastructure.Kafka/"]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
RUN dotnet restore "src/OrderProcessingService/OrderProcessingService.csproj"
COPY . .
WORKDIR "/src/src/OrderProcessingService"
RUN dotnet build "OrderProcessingService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OrderProcessingService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderProcessingService.dll"]
