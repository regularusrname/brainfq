using System.Text;
using Brainfq.Exceptions;

internal static class BrainfqLexer
{
    private static readonly HashSet<char> _lexPool = [ '.', ',', '<', '>', '+', '-', '[', ']' ];

    internal static async Task<string> LexAnalyzeAsync(string rawText)
    {
        var rawTextLength = rawText.Length;

        if (rawTextLength <= 0 || rawText is null)
        {
            throw new LexAnalyzerOperationException("Empty file: cannot parse");
        }

        var strBuilder = new StringBuilder();

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
