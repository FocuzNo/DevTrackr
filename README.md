# DevTrackr

DevTrackr is a small but professional .NET microservices backend for tracking a developer's learning progress. It is intentionally scoped as a portfolio project: real architectural patterns, clear boundaries, and enough infrastructure to show backend depth without burying the reader in enterprise-sized complexity.

## What the project demonstrates

- Clean Architecture inside each microservice
- DDD-style domain boundaries and shared kernel primitives
- CQRS with a custom in-process mediator and FluentValidation pipeline
- Event-driven communication with RabbitMQ and MassTransit
- Transactional outbox preparation for event-producing services
- PostgreSQL database-per-service setup
- Redis caching only for statistics/dashboard read models
- Containerized local development with Docker Compose

## Microservices

1. `IdentityService`
   - registration, login, JWT, current user
2. `GoalsService`
   - learning goals, goal progress, goal completion
3. `ActivityService`
   - study session logging, querying, and event publishing
4. `StatisticsService`
   - statistics projections, dashboard queries, streaks, Redis-backed dashboard cache

## Solution structure

```text
src/
  building-blocks/
    DevTrackr.SharedKernel/
    DevTrackr.Contracts/
    DevTrackr.Messaging/
  services/
    IdentityService/
      IdentityService.Api/
      IdentityService.Application/
      IdentityService.Domain/
      IdentityService.Infrastructure/
    GoalsService/
      GoalsService.Api/
      GoalsService.Application/
      GoalsService.Domain/
      GoalsService.Infrastructure/
    ActivityService/
      ActivityService.Api/
      ActivityService.Application/
      ActivityService.Domain/
      ActivityService.Infrastructure/
    StatisticsService/
      StatisticsService.Api/
      StatisticsService.Application/
      StatisticsService.Domain/
      StatisticsService.Infrastructure/
tests/
  IdentityService.UnitTests/
  GoalsService.UnitTests/
  ActivityService.UnitTests/
  StatisticsService.UnitTests/
```

## Architecture overview

Each service follows the same vertical slice:

- `Api`: HTTP endpoints, startup, docs, health checks
- `Application`: use-case contracts, validators, orchestration
- `Domain`: aggregates, entities, domain rules
- `Infrastructure`: EF Core, messaging, persistence, cache, auth adapters

Shared projects keep cross-service coupling disciplined:

- `DevTrackr.SharedKernel`: `Entity`, `AggregateRoot`, `ValueObject`, `DomainEvent`, `Result`, `Error`
- `DevTrackr.Contracts`: immutable integration events
- `DevTrackr.Messaging`: shared RabbitMQ and MassTransit bootstrap
- `DevTrackr.Cqrs`: commands, queries, handlers, mediator, and validation pipeline

## Event-driven communication

Current integration events:

- `StudySessionLoggedIntegrationEvent`
- `GoalProgressUpdatedIntegrationEvent`
- `GoalCompletedIntegrationEvent`

Planned flow:

1. `ActivityService` logs a study session and publishes `StudySessionLoggedIntegrationEvent` through the EF Core outbox.
2. `GoalsService` consumes that event, applies progress idempotently, and publishes progress/completion events through its outbox.
3. `StatisticsService` consumes all integration events, updates statistics read models, and invalidates Redis dashboard cache.

RabbitMQ is the broker, MassTransit is the bus, the custom mediator is used only for in-process request dispatch, and Redis is used only for cached dashboard/statistics data.

## Technology stack

- .NET 10
- ASP.NET Core Web API
- FastEndpoints
- Scalar
- PostgreSQL
- Redis
- RabbitMQ
- MassTransit v8
- Entity Framework Core with Npgsql
- FluentValidation
- Serilog
- Health Checks
- Docker Compose

## What is implemented in this initial scaffold

- Solution and project structure for all services, shared libraries, and tests
- Project references that preserve service boundaries
- Shared kernel primitives
- Immutable integration event contracts
- Shared CQRS abstractions and lightweight mediator pipeline
- Shared MassTransit + RabbitMQ registration with:
  - kebab-case endpoint naming
  - retry policy
  - publish/consume logging observers
  - EF Core outbox registration support
- Per-service `DbContext` setup
- Redis cache abstraction for `StatisticsService`
- FastEndpoints-based HTTP surfaces for implemented services
- Scalar docs and `/health` endpoint per API
- Dockerfiles for each API
- Docker Compose with PostgreSQL, Redis, RabbitMQ, and all four APIs
- EF Core migration startup path for service databases and tables
- Sample domain-level unit test target structure

This is intentionally a scaffold, not the finished product. Business logic and cross-service behavior are still lightweight on purpose, but the infrastructure path now assumes EF Core migrations are responsible for creating and updating service databases.

## CQRS and custom mediator

DevTrackr uses CQRS for application use cases, but deliberately does not use the `MediatR` NuGet package or any external mediator library.

Instead, the solution has a small shared building block:

- `DevTrackr.Cqrs`

It contains:

- `ICommand`
- `ICommand<TResult>`
- `IQuery<TResult>`
- `ICommandHandler<TCommand>`
- `ICommandHandler<TCommand, TResult>`
- `IQueryHandler<TQuery, TResult>`
- `IAppMediator`
- `IPipelineBehavior<TRequest, TResult>`
- `ValidationBehavior<TRequest, TResult>`

Current request flow:

`FastEndpoints -> IAppMediator -> ValidationBehavior -> Command/Query Handler -> Domain + Infrastructure`

Why this approach:

- keeps the project small and understandable
- keeps CQRS explicit
- avoids an extra external mediator dependency
- still gives us pipeline behaviors and FluentValidation

MassTransit remains the cross-service messaging mechanism. The custom mediator is only for in-process request handling.

## GoalsService

`GoalsService` is the first fully implemented microservice in the solution.

GoalsService now uses FastEndpoints for its HTTP surface and the shared custom mediator for all command/query dispatch.

### Endpoints

- `POST /api/goals`
- `GET /api/goals`
- `GET /api/goals/{id}`
- `PUT /api/goals/{id}`
- `POST /api/goals/{id}/complete`
- `POST /api/goals/{id}/cancel`
- `GET /health`
- `GET /scalar/v1`
- `GET /api/system/ping`

### GoalsService domain rules

- title is required
- title max length is 100 characters
- description max length is 1000 characters
- target minutes must be greater than zero
- current minutes cannot be negative
- deadline cannot be earlier than start date
- completed goals cannot be modified
- cancelled goals cannot be modified
- cancelled goals cannot receive progress
- completed goals cannot receive progress
- goal can be completed only if progress is at least 80%
- when current minutes reach target minutes, status becomes `ReadyToComplete`
- completing a goal sets `CompletedAt`
- cancelling a goal sets status to `Cancelled`

### GoalsService event flow

1. `ActivityService` will publish `StudySessionLoggedIntegrationEvent`.
2. `GoalsService` consumes the event, checks whether the event was already processed, and skips duplicates safely.
3. The MassTransit consumer sends `AddGoalProgressCommand` through `IAppMediator`.
4. The validation pipeline runs before `AddGoalProgressCommandHandler` mutates the aggregate.
5. `GoalsService` publishes `GoalProgressUpdatedIntegrationEvent` through the EF Core outbox.
5. When a user completes a goal explicitly, `GoalsService` publishes `GoalCompletedIntegrationEvent`.

### Current user handling

GoalsService reads the current user id from JWT claims when available. During local development, it also supports a development-only fallback user id through configuration:

- `CurrentUser:DevelopmentUserId`
- `GOALS_DEVELOPMENT_USER_ID` in Docker Compose

## ActivityService

`ActivityService` is the second fully implemented microservice in the solution.

ActivityService uses FastEndpoints for its HTTP surface and the shared custom mediator for all command/query dispatch.

### Endpoints

- `POST /api/study-sessions`
- `GET /api/study-sessions`
- `GET /api/study-sessions/{id}`
- `GET /api/study-sessions/by-goal/{goalId}`
- `GET /api/study-sessions/by-date-range?from=yyyy-MM-dd&to=yyyy-MM-dd`
- `GET /health`
- `GET /scalar/v1`
- `GET /api/system/ping`

### ActivityService domain rules

- topic is required
- topic max length is 100 characters
- note max length is 1000 characters
- duration must be greater than zero
- duration cannot be more than 480 minutes
- difficulty must be between 1 and 5
- session date cannot be in the future
- goal id is required
- user id is required

### ActivityService event flow

1. `POST /api/study-sessions` creates a `StudySession` aggregate.
2. The endpoint sends `LogStudySessionCommand` through `IAppMediator`.
3. The FluentValidation pipeline validates request shape before the handler runs.
4. `ActivityService` persists the aggregate to PostgreSQL.
5. `ActivityService` publishes `StudySessionLoggedIntegrationEvent` through MassTransit using the EF Core outbox.
6. `GoalsService` consumes the event and adds `DurationMinutes` to the matching goal.
7. `GoalsService` then publishes `GoalProgressUpdatedIntegrationEvent` when progress changes.

### Current user handling

ActivityService reads the current user id from JWT claims when available. During local development, it supports the same development-only fallback user id pattern as GoalsService:

- `CurrentUser:DevelopmentUserId`
- `CurrentUser__DevelopmentUserId` in Docker Compose

## How to run locally

### Prerequisites

- .NET 10 SDK
- Docker Desktop
- Internet access for first `dotnet restore`

### 1. Restore dependencies

```bash
dotnet restore
```

### Option A: Infrastructure only with Docker Compose

Start only the shared infrastructure:

```bash
Copy-Item .env.example .env
docker compose up -d postgres redis rabbitmq
```

Then run one or more API projects from Visual Studio or Rider:

- `IdentityService.Api`
- `GoalsService.Api`
- `ActivityService.Api`
- `StatisticsService.Api`

Each API project has its own `launchSettings.json` and fixed local ports:

- IdentityService.Api
  - HTTP: [http://localhost:5101](http://localhost:5101)
  - HTTPS: `https://localhost:7101`
- GoalsService.Api
  - HTTP: [http://localhost:5102](http://localhost:5102)
  - HTTPS: `https://localhost:7102`
- ActivityService.Api
  - HTTP: [http://localhost:5103](http://localhost:5103)
  - HTTPS: `https://localhost:7103`
- StatisticsService.Api
  - HTTP: [http://localhost:5104](http://localhost:5104)
  - HTTPS: `https://localhost:7104`

### Option B: Run everything with Docker Compose

```bash
Copy-Item .env.example .env
docker compose up --build
```

This starts PostgreSQL, Redis, RabbitMQ, and all four API services in containers.

### Local URLs

- IdentityService: [http://localhost:5101](http://localhost:5101)
- GoalsService: [http://localhost:5102](http://localhost:5102)
- ActivityService: [http://localhost:5103](http://localhost:5103)
- StatisticsService: [http://localhost:5104](http://localhost:5104)

### API docs

- IdentityService Scalar: [http://localhost:5101/scalar/v1](http://localhost:5101/scalar/v1)
- GoalsService Scalar: [http://localhost:5102/scalar/v1](http://localhost:5102/scalar/v1)
- ActivityService Scalar: [http://localhost:5103/scalar/v1](http://localhost:5103/scalar/v1)
- StatisticsService Scalar: [http://localhost:5104/scalar/v1](http://localhost:5104/scalar/v1)

### Infrastructure

- RabbitMQ Management UI: [http://localhost:15672](http://localhost:15672)
  - username: `guest`
  - password: `guest`

## Running GoalsService only

From the repository root:

```bash
dotnet restore
dotnet run --project src/services/GoalsService/GoalsService.Api/GoalsService.Api.csproj
```

Then open:

- API base: [http://localhost:5102](http://localhost:5102)
- Scalar: [http://localhost:5102/scalar/v1](http://localhost:5102/scalar/v1)
- Health: [http://localhost:5102/health](http://localhost:5102/health)

## How to run with Docker

From the repository root:

### Infrastructure only

```bash
docker compose up -d postgres redis rabbitmq
```

### Run one API service

```bash
docker compose up --build goals-service-api
```

You can replace `goals-service-api` with:

- `identity-service-api`
- `goals-service-api`
- `activity-service-api`
- `statistics-service-api`

### Run all services

```bash
docker compose up --build
```

At this stage, `GoalsService`, `ActivityService`, and `StatisticsService` are fully wired microservices. `IdentityService` is still a startup-ready API stub so the full solution can boot cleanly in Docker while its business and infrastructure logic is still being built out.

### Stop services

```bash
docker compose down
```

### Stop services and remove volumes

```bash
docker compose down -v
```

### Check status

```bash
docker compose ps -a
```

### Check logs

```bash
docker compose logs --tail=150 goals-service-api
```

### RabbitMQ Management UI

- URL: [http://localhost:15672](http://localhost:15672)
- Credentials: `guest` / `guest`

### API documentation links

- IdentityService: [http://localhost:5101/scalar/v1](http://localhost:5101/scalar/v1)
- GoalsService: [http://localhost:5102/scalar/v1](http://localhost:5102/scalar/v1)
- ActivityService: [http://localhost:5103/scalar/v1](http://localhost:5103/scalar/v1)
- StatisticsService: [http://localhost:5104/scalar/v1](http://localhost:5104/scalar/v1)

### Ping endpoints

- IdentityService: [http://localhost:5101/api/system/ping](http://localhost:5101/api/system/ping)
- GoalsService: [http://localhost:5102/api/system/ping](http://localhost:5102/api/system/ping)
- ActivityService: [http://localhost:5103/api/system/ping](http://localhost:5103/api/system/ping)
- StatisticsService: [http://localhost:5104/api/system/ping](http://localhost:5104/api/system/ping)

## GoalsService migrations

In local and Docker development environments, the services apply EF Core migrations automatically on startup. For PostgreSQL, each service first ensures its own database exists and then runs migrations for its own schema.

The GoalsService infrastructure project includes a first EF Core migration scaffold, and you can still manage migrations manually when needed.

Apply the migration manually with:

```bash
dotnet ef database update --project src/services/GoalsService/GoalsService.Infrastructure --startup-project src/services/GoalsService/GoalsService.Api
```

Create a new migration later with:

```bash
dotnet ef migrations add InitialCreate --project src/services/GoalsService/GoalsService.Infrastructure --startup-project src/services/GoalsService/GoalsService.Api
```

You can also replace `InitialCreate` with any later migration name you want.

## ActivityService migrations

In local and Docker development environments, ActivityService applies EF Core migrations automatically on startup. You can also manage the schema manually when needed.

Apply the ActivityService database migration manually with:

```bash
dotnet ef database update --project src/services/ActivityService/ActivityService.Infrastructure --startup-project src/services/ActivityService/ActivityService.Api
```

Create a new ActivityService migration later with:

```bash
dotnet ef migrations add InitialCreate --project src/services/ActivityService/ActivityService.Infrastructure --startup-project src/services/ActivityService/ActivityService.Api
```

## StatisticsService

`StatisticsService` is the third fully implemented microservice in the solution.

StatisticsService uses FastEndpoints for its HTTP surface, the shared custom mediator for all in-process queries, PostgreSQL for read models, and Redis only for dashboard caching.

### Endpoints

- `GET /api/statistics/dashboard`
- `GET /api/statistics/weekly`
- `GET /api/statistics/topics`
- `GET /api/statistics/streak`
- `GET /health`
- `GET /scalar/v1`
- `GET /api/system/ping`

### Statistics read models

StatisticsService stores three main read models in PostgreSQL:

- `user_statistics`
- `topic_statistics`
- `daily_statistics`

They are updated only from integration events and are optimized for fast dashboard-style reads.

### Redis caching strategy

- cache key: `devtrackr:statistics:dashboard:{userId}`
- cache TTL: 5 minutes
- dashboard responses are cached
- any consumed study-session or goal-related integration event invalidates the cached dashboard for that user

### Statistics event flow

1. `ActivityService` publishes `StudySessionLoggedIntegrationEvent`.
2. `GoalsService` consumes it and updates goal progress.
3. `StatisticsService` also consumes it and updates:
   - `user_statistics`
   - `topic_statistics`
   - `daily_statistics`
4. `StatisticsService` consumes goal progress and completion events as follow-up signals.
5. Relevant events invalidate the Redis dashboard cache for the affected user.

### Current user handling

StatisticsService reads the current user id from JWT claims when available. During local development, it supports the same development-only fallback user id pattern as the other implemented services:

- `CurrentUser:DevelopmentUserId`
- `CurrentUser__DevelopmentUserId` in Docker Compose

## StatisticsService migrations

In local and Docker development environments, StatisticsService applies EF Core migrations automatically on startup. You can also manage the schema manually when needed.

Apply the StatisticsService database migration manually with:

```bash
dotnet ef database update --project src/services/StatisticsService/StatisticsService.Infrastructure --startup-project src/services/StatisticsService/StatisticsService.Api
```

Create a new StatisticsService migration later with:

```bash
dotnet ef migrations add InitialCreate --project src/services/StatisticsService/StatisticsService.Infrastructure --startup-project src/services/StatisticsService/StatisticsService.Api
```

## Database setup notes

Docker Compose runs a single PostgreSQL container. The application does not use SQL init scripts for schema creation.

Instead, each microservice owns its own database and applies EF Core migrations on startup in Development or local Docker-style environments:

- `devtrackr_identity`
- `devtrackr_goals`
- `devtrackr_activity`
- `devtrackr_statistics`

Each service only points to its own database. No tables are shared across services, and application tables are created through EF Core migrations rather than raw SQL bootstrap scripts.

## Troubleshooting

- If MassTransit outbox tables are missing in `GoalsService`, run the EF Core migrations again or recreate local Docker volumes with `docker compose down -v`.
- If MassTransit outbox tables are missing in `ActivityService`, run the EF Core migrations again or recreate local Docker volumes with `docker compose down -v`.
- If statistics look stale, trigger a new study session or goal event to invalidate the dashboard cache, or clear Redis locally when debugging cached dashboard responses.
- If Docker build fails with `rpc error: code = Unavailable desc = error reading from server: EOF`, try:

```powershell
$env:COMPOSE_PARALLEL_LIMIT=1
docker compose --progress=plain build
docker compose up
```

- You can also build one service in isolation:

```powershell
docker compose --progress=plain build --no-cache goals-service-api
```

- If Docker still behaves oddly, restart Docker Desktop and run:

```powershell
wsl --shutdown
```

## Next implementation steps

1. Implement `IdentityService` registration/login and real JWT issuance.
2. Add integration tests around cross-service event processing and Redis cache invalidation.
3. Add authentication/authorization coverage across the APIs instead of relying on development user fallbacks.
4. Expand CI verification for migrations, Docker startup, and cross-service event flow.
5. Add observability around consumer lag, projection failures, and cache hit rate.

## Roadmap

- [x] EF Core migrations per service
- [ ] Real registration/login flow
- [ ] Goal lifecycle and progress updates
- [x] Study session persistence and outbox publishing
- [x] Statistics projections and dashboard endpoints
- [x] Cache invalidation and dashboard optimization
- [ ] Better authentication/authorization coverage
- [ ] CI pipeline
- [ ] Observability improvements

## Notes

This repository was designed to stay understandable. The code favors explicit structure over clever abstractions, and the business logic is intentionally unfinished so the next iterations can be implemented clearly in public commits.

## End-to-end check: Goals + Activity

Once Docker is running, you can verify the current event-driven flow with this sequence:

1. Start the full stack:

```bash
docker compose up --build
```

2. Create a goal in GoalsService using Scalar:
   - [http://localhost:5102/scalar/v1](http://localhost:5102/scalar/v1)
   - for `category`, use the enum value currently shown by Scalar

3. Log a study session in ActivityService using Scalar:
   - [http://localhost:5103/scalar/v1](http://localhost:5103/scalar/v1)
   - sample request:

```json
{
  "goalId": "PUT-GOAL-ID-HERE",
  "topic": "MassTransit EF outbox wiring",
  "durationMinutes": 75,
  "difficulty": "Hard",
  "note": "Verified publish + consume path.",
  "sessionDate": "2026-06-06"
}
```

4. Fetch the goal again from GoalsService and confirm `currentMinutes` increased.

5. If you want to see the event flow in logs:

```bash
docker compose logs --tail=150 activity-service-api goals-service-api
```

## End-to-end check: Dashboard

Once Docker is running, you can verify the statistics flow with this sequence:

1. Start the full stack:

```bash
docker compose up --build
```

2. Create a goal in GoalsService:
   - [http://localhost:5102/scalar/v1](http://localhost:5102/scalar/v1)

3. Log a study session in ActivityService:
   - [http://localhost:5103/scalar/v1](http://localhost:5103/scalar/v1)

4. Open StatisticsService Scalar:
   - [http://localhost:5104/scalar/v1](http://localhost:5104/scalar/v1)

5. Call:
   - `GET /api/statistics/dashboard`
   - `GET /api/statistics/weekly`
   - `GET /api/statistics/topics`
   - `GET /api/statistics/streak`

6. Call `GET /api/statistics/dashboard` again to verify the cached path still returns the same response.

7. Log another study session and call `GET /api/statistics/dashboard` again. The new event should invalidate the cached dashboard and the response should reflect the updated totals.
