using System.Text;
using Brainfq.Exceptions;

internal static class BrainfqLexer
{
    private static readonly HashSet<char> _lexPool = [ '.', ',', '<', '>', '+', '-', '[', ']' ];

    internal static string ExtractInstructions(string rawText)
    {
        var rawTextLength = rawText.Length;

        if (rawText is null)
        {
            throw new LexAnalyzerOperationException("Cannot perform reading of this file");
        }

        var strBuilder = new StringBuilder(rawTextLength);

        for (int i = 0; i < rawTextLength; ++i)
        {
            if (_lexPool.Contains(rawText[i]))
            {
                strBuilder.Append(rawText[i]);
            }
        }
        return strBuilder.ToString();
    }
}
