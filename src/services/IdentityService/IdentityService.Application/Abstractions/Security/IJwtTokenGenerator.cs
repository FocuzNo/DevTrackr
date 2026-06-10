using IdentityService.Domain.Users;

namespace IdentityService.Application.Abstractions.Security;

public interface IJwtTokenGenerator
{
    AccessTokenResult Generate(User user);
}
