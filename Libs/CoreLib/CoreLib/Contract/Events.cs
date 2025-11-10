namespace CoreLib.Contract;

public record UserDeleted(Guid UserId);

public record PostDeleted(Guid PostId);

public record UserUpdateRequested(Guid UserId, string NewUsername);

public record UserUpdated(Guid UserId, string OldUsername);

public record PostUpdated(Guid UserId);

public record UpdateUserFailed(Guid UserId, string OldUsername);

public record UpdatePostFailed(Guid UserId);