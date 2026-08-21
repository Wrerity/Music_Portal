namespace Music.bisLog.Dtos;

public class OperationResult
{
    public bool Success { get; set; }
    public string Error { get; set; } = string.Empty;

    public static OperationResult Ok() => new() { Success = true };
    public static OperationResult Fail(string error) => new() { Error = error };
}