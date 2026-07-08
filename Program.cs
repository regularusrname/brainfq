using System.Text.RegularExpressions;

const int ERR_CODE = 1;
const int SUCCESS_CODE = 0;
const int ERR_USAGE_CODE = 64;

const short BUFFER_LENGTH = 30_000;
const string IMPROPER_USAGE_MESSAGE = "No proper arguments. Usage example:\nbrainfq <file[.b or .bf]>";


if (args.Length == 0 || args.Length > 1)
{
    Console.WriteLine(IMPROPER_USAGE_MESSAGE);
    return ERR_USAGE_CODE;
}

string fileArg = args[0];

if (!FileExtensionRegex().IsMatch(fileArg))
{
    Console.WriteLine(IMPROPER_USAGE_MESSAGE);
    return ERR_USAGE_CODE;
}

byte[] brainfqBuffer = new byte[BUFFER_LENGTH];

try
{
    string fileContent = await File.ReadAllTextAsync(fileArg);
    string source = await BrainfqLexer.LexAnalyzeAsync(fileContent);

    #if DEBUG
        Console.WriteLine(source);
        //Console.WriteLine($"{fileArg}:\n{fileContent}");
    #endif

    int dataPointer = 0;
    int brackets = 0;
    for (ushort index = 0; index < source.Length; ++index)
    {
        switch (source[index])
        {
            case '>':
                if (dataPointer >= BUFFER_LENGTH)
                    dataPointer = 0;
                else
                    ++dataPointer;
                break;

            case '<':
                if (dataPointer <= 0)
                    dataPointer = BUFFER_LENGTH - 1;
                else
                    --dataPointer;
                break;

            case '+':
                ++brainfqBuffer[dataPointer];
                break;

            case '-':
                --brainfqBuffer[dataPointer];
                break;

            case '.':
                Console.Write((char)brainfqBuffer[dataPointer]);
                break;

            case ',':
                var input = Console.Read();
                if (input == -1)
                {
                    Console.WriteLine("\nAn error occurred while reading the input value");
                    break;
                }
                brainfqBuffer[dataPointer] = (byte)input;
                break;

            case '[':
                ++brackets;
                // if current byte at the data pointer is NOT zero,
                // then move the instruction pointer forward to the next command
                if (brainfqBuffer[dataPointer] != 0)
                {
                    continue;
                }

                //If the byte at the data pointer IS ZERO,
                //then jump it forward to the command after the matching ] command
                while (brackets > 0)
                {
                    ++index;
                    switch (source[index])
                    {
                        case '[':
                            ++brackets;
                            break;
                        case ']':
                            --brackets;
                            break;
                    }
                }
                break;

            case ']':
                --brackets;
                // If the byte at the data pointer is zero,
                // then move the instruction pointer forward to the next command
                if (brainfqBuffer[dataPointer] == 0)
                {
                    continue;
                }

                // If the byte at the data pointer IS NONzero,
                // then jump it back to the command after the matching [ command.
                while (brackets != 1)
                {
                    --index;
                    switch (source[index])
                    {
                        case ']':
                            --brackets;
                            break;
                        case '[':
                            ++brackets;
                            break;
                    }
                }
                break;
        }
    }

    return SUCCESS_CODE;
}
catch (Exception e)
{
    Console.WriteLine("An error occurred during execution.");

    #if DEBUG
        Console.WriteLine(e.Message);
    #endif

    return ERR_CODE;
}

internal partial class Program
{
    [GeneratedRegex("\\.(b|bf)$")]
    private static partial Regex FileExtensionRegex();
}
