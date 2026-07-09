namespace Brainfq.Exceptions;

public class ParseSyntaxException : Exception
{
    public ParseSyntaxException() { }

    public ParseSyntaxException(string? message)
        : base(message) { }

    public ParseSyntaxException(string? message, Exception? innerException)
        : base(message, innerException) { }
}
