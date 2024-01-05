using System.ComponentModel;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;

namespace Geo_Wall_E;

//Recive el Input del Usuario y lo separa en tokens

class Tokenizer : Dependecies
{
    private List<Token> tokens = new List<Token>();
    private string input;
    private int start = 0;
    private int current = 0;
    private int line = 1;
    private int shift = 0;
    private char[] fileId;

    private static readonly Dictionary<string, TokenTypes> keyWords = new Dictionary<string, TokenTypes>()
    {
        //Palabras clave del compilador
        {"point",TokenTypes.POINT},
        {"line",TokenTypes.LINE},
        {"segment",TokenTypes.SEGMENT},
        {"ray",TokenTypes.RAY},
        {"circle",TokenTypes.CIRCLE},
        {"arc",TokenTypes.ARC},
        {"eval",TokenTypes.EVALUATE},
        {"measure",TokenTypes.MEASURE},
        {"draw",TokenTypes.DRAW},
        {"print",TokenTypes.PRINT},
        {"if",TokenTypes.IF},
        {"else",TokenTypes.ELSE},
        {"then",TokenTypes.THEN},
        {"and",TokenTypes.AND},
        {"or",TokenTypes.OR},
        {"let",TokenTypes.LET},
        {"in",TokenTypes.IN},
        {"import",TokenTypes.IMPORT},
        {"random",TokenTypes.RANDOMS},
        {"samples",TokenTypes.SAMPLES},
        {"count",TokenTypes.COUNT},
        {"intersect",TokenTypes.INTERSECT},

        //Colores
        {"color",TokenTypes.COLOR},
        {"red",TokenTypes.COLOR_RED},
        {"blue",TokenTypes.COLOR_BLUE},
        {"yellow",TokenTypes.COLOR_YELLOW},
        {"white",TokenTypes.COLOR_WHITE},
        {"black",TokenTypes.COLOR_BLACK},
        {"green",TokenTypes.COLOR_GREEN},
        {"grey",TokenTypes.COLOR_GRAY},
        {"cyan",TokenTypes.COLOR_CYAN},
        {"restore",TokenTypes.RESTORE}
    };

    public Tokenizer(string? tinput, string tfileId, int ErrorCountTop, ICollection<Error> errors) : base(ErrorCountTop, errors)
    {
        if (tinput == null)
        {
            input = "";
        }
        else
        {
            input = tinput;
        }
        fileId = tfileId.ToCharArray();
    }

    public override void Stop()
    {
        throw new TokenizerException();
    }

    public List<Token> Check()
    {
        while (!ReachEnd)
        {
            start = current;
            ReadTokens();
        }

        tokens.Add(new Token(TokenTypes.END, "", null, line, input.Length, fileId));
        return new List<Token>(tokens);
    }

    private void ReadTokens()
    {
        char t = Consume();

        switch (t)
        {
            //Signos clave
            case ',': GetTokens(TokenTypes.COMMA); break;
            case ';': GetTokens(TokenTypes.SEMICOLON); break;
            case '+': GetTokens(TokenTypes.SUM); break;
            case '-': GetTokens(TokenTypes.REST); break;
            case '*': GetTokens(TokenTypes.PROD); break;
            case '%': GetTokens(TokenTypes.PERCENT); break;
            case '^': GetTokens(TokenTypes.POWER); break;
            case '(': GetTokens(TokenTypes.OPEN_BRACKET); break;
            case ')': GetTokens(TokenTypes.CLOSED_BRACKET); break;
            case '{': GetTokens(TokenTypes.OPEN_KEY); break;
            case '}': GetTokens(TokenTypes.CLOSED_KEY); break;
            case '&': GetTokens(TokenTypes.AND); break;
            case '|': GetTokens(TokenTypes.OR); break;
            case '=':
                if (Pair('='))
                {
                    GetTokens(TokenTypes.EQUAL_EQUAL);
                }
                else
                {
                    GetTokens(TokenTypes.EQUAL);
                }
                break;
            case '"': CheckString(); break;
            case '<':
                if (Pair('='))
                {
                    GetTokens(TokenTypes.LOWER_EQUAL);
                }
                else
                {
                    GetTokens(TokenTypes.LOWER);
                }
                break;
            case '>':
                if (Pair('='))
                {
                    GetTokens(TokenTypes.GREATER_EQUAL);
                }
                else
                {
                    GetTokens(TokenTypes.GREATER);
                }
                break;
            case '!':
                if (Pair('='))
                {
                    GetTokens(TokenTypes.NO_EQUAL);
                }
                else
                {
                    GetTokens(TokenTypes.NO);
                }
                break;

            //Expresiones regulares
            case ' ':
            case '\t':
                break;

            case '\r': break;
            case '\n': NewLine(); break;

            default:

                if (CheckNumber(t))
                {
                    GetNumberToken();
                    break;
                }
                else if (CheckChars(t))
                {
                    CheckIdentifier();
                    break;
                }
                else if (t == '.')
                {
                    if (!CheckDots())
                    {
                        CatchError(line, GetShift, new string(fileId), "Missing digit before '.'");
                    }
                    break;
                }
                CatchError(line, GetShift, new string(fileId), "Invalid char");
                break;

        }
    }

    //Token de string
    private void CheckString()
    {
        int initline = line;
        int initshift = GetShift;

        while (!ReachEnd && look != '"')
        {
            char t = Consume();
            if (t == '\n')
            {
                NewLine();
            }
        }

        if (ReachEnd)
        {
            CatchError(initline, initshift, new string(fileId), "Missing closing quote", true);
        }
        Consume();

        string value = input.Substring(start + 1, current - start - 2);

        GetTokens(TokenTypes.STRING, value);
    }

    //Token de literales
    private void GetNumberToken()
    {
        while (CheckNumber(look))
        {
            Consume();
        }

        if (look == '.')
        {
            Consume();

            if (!CheckNumber(look))
            {
                CatchError(line, GetShift, new string(fileId), "Missing digit after .(point)");
            }
            while (CheckNumber(look))
            {
                Consume();
            }
        }

        if (CheckChars(look))
        {
            CatchError(line, GetShift, new string(fileId), "Identifier cant start with number");
        }

        GetTokens(TokenTypes.NUMBER, float.Parse(input.Substring(start, current - start)));

    }

    private bool ReachEnd { get => current >= input.Length; }

    //Revisa si es un numero del 0 al 9
    private bool CheckNumber(char t)
    {
        if ('0' <= t && t <= '9')
        {
            return true;
        }
        return false;
    }

    //Revisa si es una letra o signo
    private bool CheckChars(char t)
    {
        return ('a' <= t && t <= 'z') || ('A' <= t && t <= 'Z') || (t == '_');
    }

    //Revisa si es letra, signo o numero
    private bool CheckNumberChars(char t)
    {
        return CheckNumber(t) || CheckChars(t);
    }

    //Reconoce los identificadores
    private void CheckIdentifier()
    {
        while (CheckNumberChars(look)) Consume();

        string trylex = input.Substring(start, current - start);
        TokenTypes type;

        try
        {
            type = keyWords[trylex];
        }
        catch (KeyNotFoundException)
        {
            type = TokenTypes.ID;
        }
        GetTokens(type);
    }

    //Comprueba si son iguales
    private bool Pair(char t)
    {
        if (ReachEnd || input[current] != t)
        {
            return false;
        }
        ++current;
        return true;
    }

    //Devuelve el caracter actual y avanza al siguiente
    private char Consume()
    {
        ++current;
        return input[current - 1];
    }


    //Devuelve el caracter en la posicion actual
    private char look
    {
        get
        {
            if (ReachEnd)
            {
                return '\0';
            }
            return input[current];
        }
    }

    //Devuelve el caracter en la siguiente posicion sin moverse
    private char lookForward
    {
        get
        {
            if (current + 1 >= input.Length)
            {
                return '\0';
            }
            return input[current + 1];
        }
    }

    //Salto de linea
    private void NewLine()
    {
        ++line;
        shift = current;
    }

    //Guardar el token
    private void GetTokens(TokenTypes types)
    {
        GetTokens(types, null);
    }
    private void GetTokens(TokenTypes types, object? literales)
    {
        string trylex = input.Substring(start, current - start);
        tokens.Add(new Token(types, trylex, literales, line, GetShift, fileId));
    }

    //Parsear tres puntos
    private bool CheckDots()
    {
        if (look == '.' && lookForward == '.')
        {
            Consume(); Consume();
            GetTokens(TokenTypes.THREE_DOTS);
            return true;
        }
        return false;
    }

    //Calcula la posicion del caracter dado en la linea actual
    private int GetShift { get => start - shift; }
}