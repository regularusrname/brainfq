using System.Text.RegularExpressions;
using Brainfq.Exceptions;

const int ERR_CODE = 1;
const int SUCCESS_CODE = 0;
const int ERR_USAGE_CODE = 64;

const int BUFFER_LENGTH = 30_000;
const int LAST_BYTE_INDEX = BUFFER_LENGTH - 1;

const string IMPROPER_USAGE_MESSAGE =
    "No proper arguments. Usage example:\nbrainfq <file[.b or .bf]>";

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
    string source = BrainfqLexer.ExtractInstructions(fileContent);
    
    // close ']' position - start '[' position
    Dictionary<int, int> bracketsMap = BrainfqParser.ParseLoop(source);

    #if DEBUG
        Console.WriteLine(source);
        //Console.WriteLine($"{fileArg}:\n{fileContent}");
    #endif

    int dataPointer = 0;
    for (int index = 0; index < source.Length; ++index)
    {
        switch (source[index])
        {
            case '>':
                dataPointer =
                    dataPointer == LAST_BYTE_INDEX
                        ? 0                 //then: back to first byte of buffer
                        : dataPointer + 1;  //or: go to next byte of buffer
                break;

            case '<':
                dataPointer =
                    dataPointer == 0
                        ? LAST_BYTE_INDEX   //then: go to last byte of buffer
                        : dataPointer - 1;  //or: go to previous byte of buffer
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
                // if current byte at the data pointer is NOT zero,
                // then move the instruction pointer forward to the next command
                if (brainfqBuffer[dataPointer] != 0)
                {
                    continue;
                }

                //If the byte at the data pointer IS ZERO,
                //then jump it forward to the command after the matching ] command
                index = bracketsMap[index];
                break;

            case ']':
                // If the byte at the data pointer is zero,
                // then move the instruction pointer forward to the next command
                if (brainfqBuffer[dataPointer] == 0)
                {
                    continue;
                }

                // If the byte at the data pointer IS NONzero,
                // then jump it back to the command after the matching [ command.
                index = bracketsMap[index];
                break;
        }
    }

    return SUCCESS_CODE;
}
catch (LexAnalyzerOperationException lexException)
{
    Console.WriteLine(lexException.Message);
    return ERR_CODE;
}
catch (ParseSyntaxException syntaxException)
{
    Console.WriteLine(syntaxException.Message);
    return ERR_CODE;
}

#if DEBUG
    catch (Exception e)
    {
        Console.WriteLine(e);
        return ERR_CODE;
    }
#else
    catch (Exception)
    {
        Console.WriteLine("An error occurred during execution.");
        return ERR_CODE;
    }
#endif

internal partial class Program
{
    [GeneratedRegex(@"\.(b|bf)$")]
    private static partial Regex FileExtensionRegex();
}
