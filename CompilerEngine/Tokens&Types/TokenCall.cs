namespace Geo_Wall_E;

class Token : IGetError
{
    //Tipo
    public TokenTypes Type { get; private set; }

    //Llamar al lexer
    public string TryLexer { get; private set; }

    //Linea
    public int Line { get; private set; }

    //Literales
    public object? Literals { get; private set; }

    //Archivo
    private char[] file;
    public char[] Showfile;
    public string File { get => new string(file); }

    //Desplazamiento
    public int Shift { get; private set; }


    public Token(TokenTypes type, string tryLexer, object? literals, int line, int shift, char[] fileId)
    {
        this.Type = type;
        this.TryLexer = tryLexer;
        this.Line = line;
        this.Literals = literals;
        this.Shift = shift;
        file = fileId;
    }

    public override string ToString()
    {
        return $"{Type} '{TryLexer}' L:{Line} S:{Shift}";
    }
}
