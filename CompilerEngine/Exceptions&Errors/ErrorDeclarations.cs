namespace Geo_Wall_E;

public class Error
{
    //Linea del error
    public int Line { get; private set; }

    //Archivo del error
    public string File { get; private set; }

    //Posicion del error
    public int Shift { get; private set; }

    //Mensaje de Error
    public string Message { get; private set; }

    public Error(int line, string file, int shift, string message)
    {
        Line = line;
        File = file;
        Shift = shift;
        Message = message;
    }

}

//Clase base para las excepcciones de tiempo de ejecucion
class RuntimeException : GSharpException
{
    public int Line { get; private set; }
    public int Shift { get; private set; }
    public string File { get; private set; }
    public RuntimeException(int line, int shift, string file, string message) : base(message)
    {
        Line = line;
        Shift = shift;
        File = file;
    }
    public RuntimeException(IGetError error, string message) : base(message)
    {
        Line = error.Line;
        Shift = error.Shift;
        File = error.File;
    }
}

//Declaracion de Excepciones 
abstract class GSharpException : Exception
{
    public GSharpException(string message = "") : base(message)
    { }
}
abstract class TimeException : GSharpException
{
    public TimeException(string message = "") : base(message)
    { }
}
class TokenizerException : TimeException { }
class NeedsException : TimeException { }
class ParserException : TimeException { }
class ContinueException : TimeException { }
class ASTCheckException : TimeException { }
class ScopeException : GSharpException
{
    public ScopeException(string message) : base(message)
    { }
}
class CircularException : TimeException
{
    public string File { get; private set; }

    public CircularException(string file, string message) : base(message)
    {
        File = file;
    }
}
