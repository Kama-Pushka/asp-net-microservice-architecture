namespace CoreLib.Contract;

public record UserDeleted(Guid UserId, string Username, string Email); 

// public record PostDeleted(Guid UserId); пока не нужен, у нас всего 2 микросервиса

public record PostDeleteFailed(UserDeleted UserInfo); // TODO а правильно ли сохранять инфу для компенсирующего действия через прокидывание по ивентам?

public record UserUpdateRequested(Guid UserId, string NewUsername);

public record UserUpdated(Guid UserId, string OldUsername);

public record PostUpdated(Guid UserId);

public record UpdateUserFailed(Guid UserId);

public record UpdatePostFailed(Guid UserId);