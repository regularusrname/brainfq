using Brainfq.Exceptions;

internal static class BrainfqParser
{
    internal static Dictionary<int, int> ParseLoop(string source)
    {
        Dictionary<int, int> loopMap = [];

        Stack<int> loopStarts = [];
        for (int i = 0; i < source.Length; ++i)
        {
            if (source[i] != '[' && source[i] != ']')
            {
                continue;
            }

            if (source[i] == ']')
            {
                if (loopStarts.Count == 0)
                {
                    // TODO: make positionMap of charecters in source code and lexAnalyzed code
                    // like: in source code - unexpected token in position 10:2 (character position:line)
                    //       in lexAnalyzed code (that return BrainfqLexer) in position 13:1
                    //       -> return map(10:2 and 13:1)
                    //       for error output of brainfq
                    throw new ParseSyntaxException("Unexpected token ']' in source code"); 
                }

                int startPosition = loopStarts.Pop();
                
                // close ']' position - start '[' position
                loopMap.Add(i, startPosition);
                // for test
                // same but reverted: '[' position - ']' position
                loopMap.Add(startPosition, i);
                
                continue;
            }
            
            loopStarts.Push(i);
        }

        if (loopStarts.Count > 0)
        {
            throw new ParseSyntaxException($"Expected {loopStarts.Count} of ']' tokens in source code");
        }

        return loopMap;
    }
}
