using Brainfq.Exceptions;

internal static class BrainfqParser
{
    internal static async Task<Dictionary<int, int>> ParseLoopAsync(string source)
    {
        Dictionary<int, int> loopMap = [];

        Stack<int> levels = [];
        for (int i = 0; i < source.Length; ++i)
        {
            if (source[i] == '[')
            {
                levels.Push(i);
                continue;
            }

            if (source[i] == ']' && levels.Count > 0)
            {
                int startPosition = levels.Pop();
                // close ']' position - start '[' position
                loopMap.Add(i, startPosition);
                // for test
                // same but reverted: '[' position - ']' position
                loopMap.Add(startPosition, i);
                continue;
            }

            if (source[i] == ']' && levels.Count == 0)
            {
                // TODO: make positionMap of charecters in source code and lexAnalyzed code
                // like: in source code - unexpected token in position 10:2 (character position:line)
                //       in lexAnalyzed code (that return BrainfqLexer) in position 13:1
                //       -> return map(10:2 and 13:1)
                //       for error output of brainfq
                throw new ParseSyntaxException("Unexpected token ']' in source code"); 
            }
        }

        if (levels.Count > 0)
        {
            throw new ParseSyntaxException($"Expected {levels.Count} of ']' tokens in source code");
        }

        return loopMap;
    }
}
