namespace CoreLib.Contract;

public record DeleteUserCommand(Guid UserId);

// public record DeletePostCommand(Guid PostId); пока нет необходимости

public record RevertUserUpdateCommand(Guid UserId,  string OldUsername);

// public record RevertPostUpdateCommand(Guid PostId, string OldUsername);

public record UpdateUserCommand(Guid UserId, string NewUsername);

public record UpdatePostCommand(Guid UserId, string NewUsername);