namespace Music.bisLog.Exceptions;

public class UserNotFoundException : BusinessException
{
    public override string Code => "USER_NOT_FOUND";
    public UserNotFoundException(string message = "Пользователь не найден") : base(message) { }
}

public class InvalidCredentialsException : BusinessException
{
    public override string Code => "INVALID_CREDENTIALS";
    public InvalidCredentialsException(string message = "Неверное имя пользователя или пароль") : base(message) { }
}

public class UserNotApprovedException : BusinessException
{
    public override string Code => "USER_NOT_APPROVED";
    public UserNotApprovedException(string message = "Ваша учётная запись ещё не подтверждена администратором") : base(message) { }
}
