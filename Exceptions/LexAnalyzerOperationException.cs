namespace Brainfq.Exceptions;

public class LexAnalyzerOperationException : Exception
{
    public LexAnalyzerOperationException() { }

    public LexAnalyzerOperationException(string? message)
        : base(message) { }

    public LexAnalyzerOperationException(string? message, Exception? innerException)
        : base(message, innerException) { }
}
