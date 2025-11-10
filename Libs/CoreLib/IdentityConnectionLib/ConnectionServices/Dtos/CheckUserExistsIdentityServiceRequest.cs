namespace IdentityConnectionLib.ConnectionServices.Dtos;

// "{name}{servicename}{request/response}"
public record CheckUserExistsIdentityServiceRequest
{
    public required Guid UserId { get; init; }
}