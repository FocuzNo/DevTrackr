namespace ActivityService.Api.Auth;

public interface ICurrentUserProvider
{
    Guid GetRequiredUserId();
}
