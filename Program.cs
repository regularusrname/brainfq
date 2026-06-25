using System.Text.RegularExpressions;

const string IMPROPER_USAGE_MESSAGE = "No arguments. Usage example:\nbrainfq <file[.b or .bf]>";
const int BUFFER_LENGTH = 30_000;
if (args.Length == 0 || args.Length > 1)
{
    Console.WriteLine(IMPROPER_USAGE_MESSAGE);
    return 1;
}

string fileArg = args[0];

if (!FileExtensionRegex().IsMatch(fileArg))
{
    Console.WriteLine(IMPROPER_USAGE_MESSAGE);
}

Memory<byte> brainfqBuffer = new byte[BUFFER_LENGTH];
try
{
    // Console.WriteLine(fileArg);
    StreamReader fileContentReader = new(fileArg);
    string fileContent = await fileContentReader.ReadToEndAsync();

#if DEBUG
    // Console.WriteLine($"{fileArg}:\n{fileContent}");
#endif
    ushort dataPointer = 0;
    short brackets = 0;
    for (ushort index = 0; index < fileContent.Length; ++index)
    {
        switch (fileContent[index])
        {
            case '>':
                if (dataPointer >= BUFFER_LENGTH)
                    dataPointer = 0;
                ++dataPointer;
                break;

            case '<':
                if (dataPointer <= 0)
                    dataPointer = BUFFER_LENGTH - 1;
                --dataPointer;
                break;

            case '+':
                ++brainfqBuffer.Span[dataPointer];
                break;

            case '-':
                --brainfqBuffer.Span[dataPointer];
                break;

            case '.':
                Console.Write((char)brainfqBuffer.Span[dataPointer]);
                break;

            case ',':
                var input = Console.Read();
                if (input == -1)
                    Console.WriteLine("\nAn error occurred while reading the input value");
                brainfqBuffer.Span[dataPointer] = (byte)input;
                break;

            case '[':
                brackets++;
                // if current byte at the data pointer is NOT zero, 
                // then move the instruction pointer forward to the next command
                if (brainfqBuffer.Span[dataPointer] != 0)
                {
                    continue;
                }

                //If the byte at the data pointer IS ZERO, 
                //then jump it forward to the command after the matching ] command
                while(brackets > 0)
                {
                    ++index;
                    switch(fileContent[index])
                    {
                        case '[': ++brackets; break;
                        case ']': --brackets; break;
                    }
                }
            break;

            case ']':
                --brackets;
                // If the byte at the data pointer is zero,
                // then move the instruction pointer forward to the next command
                if (brainfqBuffer.Span[dataPointer] == 0)
                {
                    continue;
                }

                // If the byte at the data pointer IS NONzero,
                // then jump it back to the command after the matching [ command.
                while(brackets != 1)
                {
                    --index;
                    switch(fileContent[index])
                    {
                        case ']': --brackets; break;
                        case '[': ++brackets; break;
                    }
                }
            break;
        }
    }

    return 0;
}
catch (Exception e)
{
    Console.WriteLine("An error occurred during execution.");

#if DEBUG
    Console.WriteLine(e.Message);
#endif

    return 1;
}

partial class Program
{
    [GeneratedRegex("\\.(b|bf)$")]
    private static partial Regex FileExtensionRegex();
}
