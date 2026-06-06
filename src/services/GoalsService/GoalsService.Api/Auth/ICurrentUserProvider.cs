namespace GoalsService.Api.Auth;

public interface ICurrentUserProvider
{
    Guid GetRequiredUserId();
}
