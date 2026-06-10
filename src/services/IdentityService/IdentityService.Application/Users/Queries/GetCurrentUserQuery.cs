using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;
using IdentityService.Application.Users.Responses;

namespace IdentityService.Application.Users.Queries;

public sealed record GetCurrentUserQuery(Guid UserId) : IQuery<Result<CurrentUserResponse>>;
