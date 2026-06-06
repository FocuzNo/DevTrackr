namespace StatisticsService.Api.Auth;

public interface ICurrentUserProvider
{
    Guid GetRequiredUserId();
}
