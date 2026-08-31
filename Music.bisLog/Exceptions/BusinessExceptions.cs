namespace Music.bisLog.Exceptions;

// Базовый бизнес-исключение для централизованной обработки
public abstract class BusinessException : Exception
{
    public abstract string Code { get; }
    protected BusinessException(string message) : base(message) { }
}

public class EntityNotFoundException : BusinessException
{
    public override string Code => "ENTITY_NOT_FOUND";
    public EntityNotFoundException(string message) : base(message) { }
}

public class UserAlreadyExistsException : BusinessException
{
    public override string Code => "USER_ALREADY_EXISTS";
    public UserAlreadyExistsException(string message = "Пользователь с таким именем уже существует") : base(message) { }
}

public class GenreAlreadyExistsException : BusinessException
{
    public override string Code => "GENRE_ALREADY_EXISTS";
    public GenreAlreadyExistsException(string message) : base(message) { }
}

public class AuthorAlreadyExistsException : BusinessException
{
    public override string Code => "AUTHOR_ALREADY_EXISTS";
    public AuthorAlreadyExistsException(string message) : base(message) { }
}

public class AccessDeniedException : BusinessException
{
    public override string Code => "ACCESS_DENIED";
    public AccessDeniedException(string message) : base(message) { }
}

public class BusinessValidationException : BusinessException
{
    public override string Code => "VALIDATION_ERROR";
    public BusinessValidationException(string message) : base(message) { }
}
