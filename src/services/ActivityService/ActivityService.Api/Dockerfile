FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY DevTrackr.sln ./
COPY Directory.Packages.props ./
COPY Directory.Build.props ./

COPY src/building-blocks/DevTrackr.SharedKernel/DevTrackr.SharedKernel.csproj src/building-blocks/DevTrackr.SharedKernel/
COPY src/building-blocks/DevTrackr.Contracts/DevTrackr.Contracts.csproj src/building-blocks/DevTrackr.Contracts/
COPY src/building-blocks/DevTrackr.Messaging/DevTrackr.Messaging.csproj src/building-blocks/DevTrackr.Messaging/

COPY src/services/IdentityService/IdentityService.Api/IdentityService.Api.csproj src/services/IdentityService/IdentityService.Api/
COPY src/services/IdentityService/IdentityService.Application/IdentityService.Application.csproj src/services/IdentityService/IdentityService.Application/
COPY src/services/IdentityService/IdentityService.Domain/IdentityService.Domain.csproj src/services/IdentityService/IdentityService.Domain/
COPY src/services/IdentityService/IdentityService.Infrastructure/IdentityService.Infrastructure.csproj src/services/IdentityService/IdentityService.Infrastructure/

COPY src/services/GoalsService/GoalsService.Api/GoalsService.Api.csproj src/services/GoalsService/GoalsService.Api/
COPY src/services/GoalsService/GoalsService.Application/GoalsService.Application.csproj src/services/GoalsService/GoalsService.Application/
COPY src/services/GoalsService/GoalsService.Domain/GoalsService.Domain.csproj src/services/GoalsService/GoalsService.Domain/
COPY src/services/GoalsService/GoalsService.Infrastructure/GoalsService.Infrastructure.csproj src/services/GoalsService/GoalsService.Infrastructure/

COPY src/services/ActivityService/ActivityService.Api/ActivityService.Api.csproj src/services/ActivityService/ActivityService.Api/
COPY src/services/ActivityService/ActivityService.Application/ActivityService.Application.csproj src/services/ActivityService/ActivityService.Application/
COPY src/services/ActivityService/ActivityService.Domain/ActivityService.Domain.csproj src/services/ActivityService/ActivityService.Domain/
COPY src/services/ActivityService/ActivityService.Infrastructure/ActivityService.Infrastructure.csproj src/services/ActivityService/ActivityService.Infrastructure/

COPY src/services/StatisticsService/StatisticsService.Api/StatisticsService.Api.csproj src/services/StatisticsService/StatisticsService.Api/
COPY src/services/StatisticsService/StatisticsService.Application/StatisticsService.Application.csproj src/services/StatisticsService/StatisticsService.Application/
COPY src/services/StatisticsService/StatisticsService.Domain/StatisticsService.Domain.csproj src/services/StatisticsService/StatisticsService.Domain/
COPY src/services/StatisticsService/StatisticsService.Infrastructure/StatisticsService.Infrastructure.csproj src/services/StatisticsService/StatisticsService.Infrastructure/

RUN dotnet restore src/services/ActivityService/ActivityService.Api/ActivityService.Api.csproj

COPY . .

RUN dotnet publish src/services/ActivityService/ActivityService.Api/ActivityService.Api.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ActivityService.Api.dll"]
