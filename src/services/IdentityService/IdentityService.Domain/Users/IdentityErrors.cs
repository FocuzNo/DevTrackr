using DevTrackr.SharedKernel.Primitives;

namespace IdentityService.Domain.Users;

public static class IdentityErrors
{
    public static readonly Error EmailRequired = new(
        "identity.email_required",
        "Email is required.");

    public static readonly Error DisplayNameRequired = new(
        "identity.display_name_required",
        "Display name is required.");

    public static readonly Error DisplayNameTooLong = new(
        "identity.display_name_too_long",
        "Display name must be 100 characters or fewer.");

    public static readonly Error PasswordHashRequired = new(
        "identity.password_hash_required",
        "Password hash is required.");

    public static readonly Error InvalidCredentials = new(
        "identity.invalid_credentials",
        "Invalid email or password.");

    public static readonly Error EmailAlreadyExists = new(
        "identity.email_already_exists",
        "A user with this email already exists.");

    public static readonly Error UserNotFound = new(
        "identity.user_not_found",
        "User was not found.");
}
