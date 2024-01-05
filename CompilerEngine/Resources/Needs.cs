namespace Geo_Wall_E;

//Recive el input y lo procesa para resolver las necesidades
class Needs : Dependecies
{
    private string mainfile;
    Queue<(string, string)> inputfiles = new Queue<(string, string)>();
    Dictionary<string, FileDetails> AllFilesDetails = new Dictionary<string, FileDetails>();
    private Compiler.Banner Banner;

    public override void Stop()
    {
        throw new NeedsException();
    }

    public Needs(string input, string fileid, ICollection<Error> errors, Compiler.Banner banner) : base(banner.MaxErrorCount, errors)
    {
        inputfiles.Enqueue((fileid, input));
        mainfile = fileid;
        this.Banner = banner;
    }

    public List<Token> CheckandSolve()
    {
        while (inputfiles.Count > 0)
        {
            string currentfileId, inputcode;

            (currentfileId, inputcode) = inputfiles.Dequeue();

            if (AllFilesDetails.ContainsKey(currentfileId))
            {
                continue;
            }

            List<Token> tokens = (new Tokenizer(inputcode, currentfileId, ErrorCountTop, Errors)).Check();

            if(Banner.PrintDebugInfo)
            {
                Resources.PrintTokens(tokens,Banner.OutputStream,currentfileId);
            }

            if (Errors.Count > 0)
            {
                throw new TokenizerException();
            }

            List<string> import = ParceImport(tokens);

            if (Errors.Count > 0)
            {
                throw new ParserException();
            }

            AllFilesDetails.Add(currentfileId, new FileDetails(import, tokens));

            foreach (string file in import)
            {
                string codeofFile = "";

                try
                {
                    codeofFile = Resources.LoadFile(codeofFile);
                }
                catch (Exception ex)
                {
                    Errors.Add(new Error(-1, currentfileId, -1, ex.Message));
                    throw new NeedsException();
                }

                inputfiles.Enqueue((file, codeofFile));

                AllFilesDetails[currentfileId].Depends.Add(file);
            }

        }

        //Indica que los archivos debe concatenarse
        List<string> concat = new List<string>();

        try
        {
            concat = ConcatOrder();
        }
        catch (CircularException ex)
        {
            CatchError(-1, -1, ex.File, ex.Message, true);
        }

        List<Token> finalreturn = new List<Token>();

        foreach (string file in concat)
        {
            foreach (Token token in AllFilesDetails[file].Input)
            {
                if (token.Type != TokenTypes.END)
                {
                    finalreturn.Add(token);
                }
            }
        }

        finalreturn.Add(AllFilesDetails[mainfile].Input.Last());
        return finalreturn;
    }

    //Detalles del Documento
    private class FileDetails
    {
        public List<string> Depends { get; set; }

        public List<string> Import { get; private set; }

        public List<Token> Input { get; private set; }

        public FileDetails(List<string> imp, List<Token> inp)
        {
            Depends = new List<string>();
            Import = imp;
            Input = inp;
        }
    }

    private List<string> ParceImport(List<Token> tokens)
    {
        List<string> impports = (new Parser(tokens, ErrorCountTop, Errors)).ParseImports();
        return impports;
    }

    List<string> ConcatOrder()
    {
        Stack<string> Calls = new Stack<string>();
        List<string> disorder = new List<string>();
        Dictionary<string, FilesState> state = new Dictionary<string, FilesState>();

        void ConcatOrder(string file)
        {
            Calls.Push(file);
            state[file] = FilesState.PROCESSING;

            foreach (string bring in AllFilesDetails[file].Depends)
            {
                switch (state[bring])
                {
                    case FilesState.UNPROCESSED:
                        {
                            ConcatOrder(bring);
                            break;
                        }
                    case FilesState.PROCESSING:
                        {
                            string message = $">{bring}<";

                            while (Calls.Peek() != bring)
                            {
                                message += $">{Calls.Pop()}<";
                            }

                            message += bring;
                            message = "Circular dependecies  : " + message;

                            throw new CircularException(bring, message);
                        }
                }
            }

            disorder.Add(file);
            state[file] = FilesState.PROCESSED;
            Calls.Pop();
        }

        foreach (string file in AllFilesDetails.Keys) state[file] = FilesState.UNPROCESSED;
        ConcatOrder(mainfile);
        return disorder;
    }

    enum FilesState
    {
        UNPROCESSED, PROCESSING, PROCESSED
    };
}