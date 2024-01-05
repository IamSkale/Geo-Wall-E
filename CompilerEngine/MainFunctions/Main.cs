//Clase Fundamental del Compilador

using System.Security.Cryptography.X509Certificates;

namespace Geo_Wall_E;

public static class Compiler
{
    public class Action
    {
        List<Error> errors = new List<Error>();
        List<IDraw> objects = new List<IDraw>();
        public bool FoundError { get => errors.Count > 0; }
        public bool CanReadFile { get; private set; }
        public List<Error> Errors { get => errors; }
        public List<IDraw> Objects { get => objects; }

        public List<Object.String> Prints;

        public Action(List<Error> terror, List<IDraw> tobjects, List<Object.String> prints, bool canReadFile = false)
        {
            errors = terror;
            objects = tobjects;
            Prints = prints;
            CanReadFile = canReadFile;
        }

    }

    public static Action CompileInput(string input, string inputId, Banner? banner = null)
    {
        if (banner == null)
        {
            banner = new Banner();
        }
        Resources.SetCoordinateLimit(banner.MaxCoordinate);
        List<Error> errors = new List<Error>();
        List<IDraw> objects = new List<IDraw>();
        List<Object.String> prints = new List<Object.String>();

        try
        {
            //Revisa el codigo introducido y crea los tokens
            List<Token> tokens = new Needs(input, inputId, errors, banner).CheckandSolve();

            if (errors.Count > 0)
            {
                throw new NeedsException();
            }

            Program program = new Parser(tokens, banner.MaxErrorCount, errors).Parse();

            if (errors.Count > 0)
            {
                throw new ParserException();
            }

            new TypeCheck(banner.MaxErrorCount, errors).Check(program);

            if (errors.Count > 0)
            {
                throw new ASTCheckException();
            }

            (objects, prints) = new Interpreter(banner.OutputStream).Inter(program);
        }
        catch (TimeException) { }
        catch (RuntimeException ex)
        {
            errors.Add(new Error(ex.Line, ex.File, ex.Shift, ex.Message));
        }
        catch (Exception ex)
        {
            errors.Add(new Error(0, "", 0, "Unexpected Error"));
        }

        return new Action(errors, objects, prints);
    }

    public static Action CompileLoaded(string path)
    {
        string loadedInput = "";
        List<Error> errors = new List<Error>();
        List<IDraw> draws = new List<IDraw>();
        List<Object.String> prints = new List<Object.String>();

        try
        {
            loadedInput = Resources.LoadFile(path);
        }
        catch (Exception ex)
        {
            errors.Add(new Error(-1, path, -1, ex.Message));
            return new Action(errors, draws, prints, true);
        }
        return CompileInput(loadedInput, path);
    }

    public class Banner
    {
        public bool PrintDebugInfo { get; set; }
        public TextWriter OutputStream { get; set; }
        private int maxErrorCount;
        public int MaxErrorCount
        {
            get => maxErrorCount;
            set
            {
                if (value > 1) maxErrorCount = value;
                else maxErrorCount = 1;
            }
        }
        public int MaxCoordinate
        {
            get => maxCoordinate;
            set
            {
                if (value > 0) maxCoordinate = value;
            }
        }
        private int maxCoordinate;
        public Banner()
        {
            PrintDebugInfo = true;
            OutputStream = System.Console.Out;
            MaxErrorCount = 1;
            maxCoordinate = 1000;
        }
    }

}


