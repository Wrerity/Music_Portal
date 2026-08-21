namespace Music.DataAccess.Utils;

public static class FileExtensions
{
    public const string Mp3 = ".mp3";
    public const string Wav = ".wav";

    public static readonly string[] Allowed = { Mp3, Wav };
}