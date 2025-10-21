namespace IdentityConnectionLib.ConnectionServices.Dtos;

// "{name}{servicename}{request/response}"
public class CheckUserExistsIdentityServiceRequest
{
    public required Guid UserId { get; init; }
}