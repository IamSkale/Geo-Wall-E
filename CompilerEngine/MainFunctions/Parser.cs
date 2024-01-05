using System.Linq.Expressions;

namespace Geo_Wall_E;

//Recive la secuencia de tokens y construye el AST(abtract syntax tree)

class Parser : Dependecies
{
    List<Token> tokens;

    int current = 0;

    public override void Stop()
    {
        throw new ParserException();
    }

    public Parser(List<Token> ttokens, int errorcounttop, ICollection<Error> errors) : base(errorcounttop, errors)
    {
        tokens = ttokens;
    }

    public override void CatchError(int line, int shift, string file, string message, bool forceStop = false)
    {
        base.CatchError(line, shift, file, message, forceStop);
        throw new ContinueException();
    }

    public override void CatchError(IGetError error, string message, bool forceStop = false)
    {
        base.CatchError(error, message, forceStop);
        throw new ContinueException();
    }


    //Metodo del parsing recursivo descendente
    public Program Parse()
    {
        return ParseProg();
    }

    private Program ParseProg()
    {
        return new Program(ParseStatementList());
    }

    private Statement.StatementList ParseStatementList(TokenTypes stopAtThisType = TokenTypes.END)
    {
        Statement.StatementList Statements = new Statement.StatementList(Look.Line, Look.Shift, Look.Showfile);
        while (!IsAtEnd && Look.Type != stopAtThisType)
        {
            try
            {
                Statement tmp = ParseStatement();
                if (tmp != Statement.EMPTY) Statements.Add(tmp);
            }
            catch (ContinueException)
            {
                while (!IsAtEnd && Look.Type != TokenTypes.SEMICOLON && Look.Type != stopAtThisType) Continue();
            }
        }
        if (Look.Type != stopAtThisType) CatchError(Back, $"Unexpected EOF encountered, could not reach token of type {stopAtThisType}", true);
        return Statements;
    }

    private Statement ParseStatement()
    {
        Statement? aux = null;
        switch (Look.Type)
        {
            case TokenTypes.SEMICOLON:
                aux = Statement.EMPTY;
                break;
            case TokenTypes.POINT:
                aux = ParsePointStatement();
                break;
            case TokenTypes.LINE:
                aux = ParseLinesStatement();
                break;
            case TokenTypes.SEGMENT:
                aux = ParseSegmentStatement();
                break;
            case TokenTypes.RAY:
                aux = ParseRayStatement();
                break;
            case TokenTypes.CIRCLE:
                aux = ParseCircleStatement();
                break;
            case TokenTypes.ARC:
                aux = ParseArcStatement();
                break;
            case TokenTypes.ID:
                aux = ParseDeclaration();
                break;
            case TokenTypes.PRINT:
                aux = ParsePrintStatement();
                break;
            case TokenTypes.DRAW:
                aux = ParseDrawStatement();
                break;
            case TokenTypes.COLOR:
            case TokenTypes.RESTORE:
                aux = ParseColorStatement();
                break;
            case TokenTypes.EVALUATE:
                aux = ParseEvalStatement();
                break;
            case TokenTypes.IMPORT:
                aux = ParseImportStatement();
                break;
            default:
                CatchError(Look, "Not a statement");
                break;
        }
        Consume(TokenTypes.SEMICOLON, "Semicolon expected after statement");
        return aux!;
    }

    private Expr ParseExpression()
    {
        switch (Look.Type)
        {
            case TokenTypes.LINE:
                return ParseLinesExpression();
            case TokenTypes.POINT:
                return ParsePointExpression();
            case TokenTypes.CIRCLE:
                return ParseCircleExpression();
            case TokenTypes.SEGMENT:
                return ParseSegmentExpression();
            case TokenTypes.RAY:
                return ParseRayExpression();
            case TokenTypes.ARC:
                return ParseArcExpression();
            case TokenTypes.INTERSECT:
                return ParseIntersectExpression();
            case TokenTypes.LET:
                return ParseLetInExpression();
            case TokenTypes.IF:
                return ParseConditionalExpression();
            default:
                return ParseOrExpression();
        }
    }

    private Token Look { get => tokens[current]; }

    private Token LookNext
    {
        get
        {
            if (current + 1 < tokens.Count)
            {
                return tokens[current + 1];
            }
            return tokens[tokens.Count - 1];
        }
    }

    private Token Back { get => tokens[current - 1]; }

    private Token Continue()
    {
        ++current;
        return tokens[current - 1];
    }

    private Token Consume(TokenTypes type, string message = "Errores")
    {
        if (Look.Type == type)
        {
            return Continue();
        }
        CatchError(Back.Line, Back.Shift + Back.TryLexer.Length, Back.File, message);
        return Look;
    }

    private bool IsAtEnd { get => Look.Type == TokenTypes.END; }

    private Statement.Draw ParseDrawStatement()
    {
        Token drawToken = Consume(TokenTypes.DRAW, "Expected `draw` keyword");
        Expr expr = ParseExpression();
        ErrorIfEmpty(expr, drawToken, "Expected non-empty expression after `draw`");
        string comment = "";
        if (Look.Type == TokenTypes.STRING) comment = (string)Continue().Literals!;

        return new Statement.Draw(drawToken.Line, drawToken.Shift, drawToken.Showfile, expr, new Object.String(comment));

    }

    private Statement.Point ParsePointStatement()
    {
        Token pointToken = Consume(TokenTypes.POINT, "Expected `point` keyword");
        if (Look.Type == TokenTypes.OPEN_BRACKET)
        {
            Consume(TokenTypes.OPEN_BRACKET);
            if (Look.Type != TokenTypes.CLOSED_BRACKET)
            {

                Expr x = ParseExpression();
                Consume(TokenTypes.COMMA, "Expected comma `,`");
                Expr y = ParseExpression();
                Consume(TokenTypes.CLOSED_BRACKET, "Expected `)`");

                Token id = Consume(TokenTypes.ID, "Expected identifier");

                string comment = "";
                if (Look.Type == TokenTypes.STRING) comment = (string)Continue().Literals!;
                return new Statement.Point(pointToken.Line, pointToken.Shift, pointToken.Showfile, id, x, y, new Object.String(comment));

            }
            Consume(TokenTypes.CLOSED_BRACKET, "Expected `)`");
        }

        Token id2 = Consume(TokenTypes.ID, "Expected identifier");

        return new Statement.Point(pointToken.Line, pointToken.Shift, pointToken.Showfile, id2);
    }

    private Statement.Lines ParseLinesStatement()
    {
        Token lineToken = Consume(TokenTypes.LINE, "Expected `line` keyword");

        if (Look.Type == TokenTypes.OPEN_BRACKET)
        {
            Consume(TokenTypes.OPEN_BRACKET);
            if (Look.Type != TokenTypes.CLOSED_BRACKET)
            {
                Expr p1 = ParseExpression();
                Consume(TokenTypes.COMMA, "Expected comma `,`");
                Expr p2 = ParseExpression();
                Consume(TokenTypes.CLOSED_BRACKET, "Expected `)`");


                Token id = Consume(TokenTypes.ID, "Expected identifier");
                string comment = "";
                if (Look.Type == TokenTypes.STRING) comment = (string)Continue().Literals!;

                return new Statement.Lines(lineToken.Line, lineToken.Shift, lineToken.Showfile, id, p1, p2, new Object.String(comment));
            }
            Consume(TokenTypes.CLOSED_BRACKET, "Expected `)`");
        }

        Token id2 = Consume(TokenTypes.ID, "Expected identifier");

        return new Statement.Lines(lineToken.Line, lineToken.Shift, lineToken.Showfile, id2);
    }

    private Statement.Segment ParseSegmentStatement()
    {
        Token lineToken = Consume(TokenTypes.SEGMENT, "Expected `segment` keyword");
        if (Look.Type == TokenTypes.OPEN_BRACKET)
        {
            Consume(TokenTypes.OPEN_BRACKET);
            if (Look.Type != TokenTypes.CLOSED_BRACKET)
            {
                Expr p1 = ParseExpression();
                Consume(TokenTypes.COMMA, "Expected comma `,`");
                Expr p2 = ParseExpression();
                Consume(TokenTypes.CLOSED_BRACKET, "Expected `)`");
                Token id = Consume(TokenTypes.ID, "Expected identifier");

                string comment = "";
                if (Look.Type == TokenTypes.STRING) comment = (string)Continue().Literals!;

                return new Statement.Segment(lineToken.Line, lineToken.Shift, lineToken.Showfile, id, p1, p2, new Object.String(comment));
            }
            Consume(TokenTypes.CLOSED_BRACKET, "Expected `)`");
        }
        Token id2 = Consume(TokenTypes.ID, "Expected identifier");
        return new Statement.Segment(lineToken.Line, lineToken.Shift, lineToken.Showfile, id2);
    }

    private Statement.Ray ParseRayStatement()
    {
        Token lineToken = Consume(TokenTypes.RAY, "Expected `ray` keyword");
        if (Look.Type == TokenTypes.OPEN_BRACKET)
        {
            Consume(TokenTypes.OPEN_BRACKET);
            if (Look.Type != TokenTypes.CLOSED_BRACKET)
            {
                Expr p1 = ParseExpression();
                Consume(TokenTypes.COMMA, "Expected comma `,`");
                Expr p2 = ParseExpression();
                Consume(TokenTypes.CLOSED_BRACKET, "Expected `)`");

                Token id = Consume(TokenTypes.ID, "Expected identifier");

                string comment = "";
                if (Look.Type == TokenTypes.STRING) comment = (string)Continue().Literals!;

                return new Statement.Ray(lineToken.Line, lineToken.Shift, lineToken.Showfile, id, p1, p2, new Object.String(comment));
            }

            Consume(TokenTypes.CLOSED_BRACKET, "Expected `)`");
        }
        Token id2 = Consume(TokenTypes.ID, "Expected identifier");
        return new Statement.Ray(lineToken.Line, lineToken.Shift, lineToken.Showfile, id2);

    }

    private Statement.Circle ParseCircleStatement()
    {
        Token circleToken = Consume(TokenTypes.CIRCLE, "Expected `circle` keyword");
        if (Look.Type == TokenTypes.OPEN_BRACKET)
        {
            Consume(TokenTypes.OPEN_BRACKET);
            if (Look.Type != TokenTypes.CLOSED_BRACKET)
            {
                Expr p1 = ParseExpression();
                Consume(TokenTypes.COMMA, "Expected comma `,`");
                Expr radius = ParseExpression();
                Consume(TokenTypes.CLOSED_BRACKET, "Expected `)`");

                Token id = Consume(TokenTypes.ID, "Expected identifier");
                string comment = "";
                if (Look.Type == TokenTypes.STRING) comment = (string)Continue().Literals!;
                return new Statement.Circle(circleToken.Line, circleToken.Shift, circleToken.Showfile, id, p1, radius, new Object.String(comment));
            }
            Consume(TokenTypes.CLOSED_BRACKET, "Expected `)`");
        }
        Token id2 = Consume(TokenTypes.ID, "Expected identifier");

        return new Statement.Circle(circleToken.Line, circleToken.Shift, circleToken.Showfile, id2);
    }

    private Statement.Arc ParseArcStatement()
    {
        Token arcToken = Consume(TokenTypes.ARC, "Expected `arc` keyword");
        if (Look.Type == TokenTypes.OPEN_BRACKET)
        {
            Consume(TokenTypes.OPEN_BRACKET);
            if (Look.Type != TokenTypes.CLOSED_BRACKET)
            {
                Expr p1 = ParseExpression();
                Consume(TokenTypes.COMMA, "Expected comma `,`");
                Expr p2 = ParseExpression();
                Consume(TokenTypes.COMMA, "Expected comma `,`");
                Expr p3 = ParseExpression();
                Consume(TokenTypes.COMMA, "Expected comma `,`");
                Expr radius = ParseExpression();
                Consume(TokenTypes.CLOSED_BRACKET, "Expected `)`");

                Token id = Consume(TokenTypes.ID, "Expected identifier");
                string comment = "";
                if (Look.Type == TokenTypes.STRING) comment = (string)Continue().Literals!;
                return new Statement.Arc(arcToken.Line, arcToken.Shift, arcToken.Showfile, id, p1, p2, p3, radius, new Object.String(comment));
            }
            Consume(TokenTypes.CLOSED_BRACKET, "Expected `)`");

        }
        Token id2 = Consume(TokenTypes.ID, "Expected identifier");
        return new Statement.Arc(arcToken.Line, arcToken.Shift, arcToken.Showfile, id2);
    }

    private Statement.Color ParseColorStatement()
    {
        int line = Look.Line;
        int Shift = Look.Shift;

        if (Look.Type == TokenTypes.RESTORE)
        {
            Consume(TokenTypes.RESTORE);
            return new Statement.Color(line, Shift, Back.Showfile, Colors.BLACK, true);
        }

        Colors color = Colors.BLACK;
        Consume(TokenTypes.COLOR);

        switch (Look.Type)
        {
            case TokenTypes.COLOR_BLACK:
                color = Colors.BLACK;
                break;
            case TokenTypes.COLOR_BLUE:
                color = Colors.BLUE;
                break;
            case TokenTypes.COLOR_CYAN:
                color = Colors.CYAN;
                break;
            case TokenTypes.COLOR_GRAY:
                color = Colors.GRAY;
                break;
            case TokenTypes.COLOR_GREEN:
                color = Colors.GREEN;
                break;
            case TokenTypes.COLOR_PURPLE:
                color = Colors.PURPLE;
                break;
            case TokenTypes.COLOR_RED:
                color = Colors.RED;
                break;
            case TokenTypes.COLOR_WHITE:
                color = Colors.WHITE;
                break;
            case TokenTypes.COLOR_YELLOW:
                color = Colors.YELLOW;
                break;
            default:
                CatchError(Look, $"Expected built-in color");
                break;
        }

        Continue();
        return new Statement.Color(line, Shift, Back.Showfile, color);
    }
    private Statement.Eval ParseEvalStatement()
    {
        Token evalToken = Consume(TokenTypes.EVALUATE);
        Expr expr = ParseExpression();
        return new Statement.Eval(evalToken.Line, evalToken.Shift, evalToken.Showfile, expr);
    }

    private Statement.Print ParsePrintStatement()
    {
        Token printToken = Consume(TokenTypes.PRINT);
        return new Statement.Print(printToken.Line, printToken.Shift, printToken.Showfile, ParseExpression());
    }

    private Statement.Eval ParseEvalStmt()
    {
        Token evalToken = Consume(TokenTypes.EVALUATE);
        Expr expr = ParseExpression();
        return new Statement.Eval(evalToken.Line, evalToken.Shift, evalToken.Showfile, expr);
    }

    //Expresiones
    private Expr ParseOrExpression()
    {
        Func<Expr> parseHigherPrecedence = () => ParseAndExpression();
        return ParseAssociativeBinaryOperator(parseHigherPrecedence, parseHigherPrecedence, 0, TokenTypes.OR);
    }
    private Expr ParseAndExpression()
    {
        Func<Expr> parseHigherPrecedence = () => ParseEqualityExpression();
        return ParseAssociativeBinaryOperator(parseHigherPrecedence, parseHigherPrecedence, 0, TokenTypes.AND);
    }
    private Expr ParseEqualityExpression()
    {
        Func<Expr> parseHigherPrecedence = () => ParseComparisonExpression();
        return ParseAssociativeBinaryOperator(parseHigherPrecedence, parseHigherPrecedence, 1, TokenTypes.EQUAL_EQUAL, TokenTypes.NO_EQUAL);
    }
    private Expr ParseComparisonExpression()
    {
        Func<Expr> parseHigherPrecedence = () => ParseTermExpression();
        return ParseAssociativeBinaryOperator(parseHigherPrecedence, parseHigherPrecedence, 1, TokenTypes.LOWER, TokenTypes.LOWER_EQUAL, TokenTypes.GREATER, TokenTypes.GREATER_EQUAL);
    }
    private Expr ParseTermExpression()
    {
        Func<Expr> parseHigherPrecedence = () => ParseFactorExpression();
        return ParseAssociativeBinaryOperator(parseHigherPrecedence, parseHigherPrecedence, 0, TokenTypes.SUM, TokenTypes.REST);
    }
    private Expr ParseFactorExpression()
    {
        Func<Expr> parseHigherPrecedence = () => ParsePowerExpression();
        return ParseAssociativeBinaryOperator(parseHigherPrecedence, parseHigherPrecedence, 0, TokenTypes.PROD, TokenTypes.DIV, TokenTypes.PERCENT);
    }
    private Expr ParsePowerExpression()
    {
        Func<Expr> parseLeft = () => ParseUnaryExpression();
        Func<Expr> parseRight = () => ParsePowerExpression();
        return ParseAssociativeBinaryOperator(parseLeft, parseRight, 0, TokenTypes.POWER);
    }
    private Expr ParseUnaryExpression()
    {
        Expr expr;
        switch (Look.Type)
        {
            case TokenTypes.NO:
                Continue();
                expr = ParseUnaryExpression();
                ErrorIfEmpty(expr, Back, "Expected non-empty expression as operand");
                return new Expr.Unary.Not(Back.Line, Back.Shift, Back.Showfile, expr);

            case TokenTypes.LOWER:
                Continue();
                expr = ParseUnaryExpression();
                ErrorIfEmpty(expr, Back, "Expected non-empty expression as operand");
                return new Expr.Unary.Minus(Back.Line, Back.Shift, Back.Showfile, expr);
            default:
                return ParseVariableOrCallExpression();
        }
    }

    private Expr ParsePointExpression()
    {
        Token pointToken = Consume(TokenTypes.POINT);

        Consume(TokenTypes.OPEN_BRACKET, "Expected `(` on call to `point`");
        if (Look.Type != TokenTypes.CLOSED_BRACKET)
        {
            Expr x = ParseExpression();
            Consume(TokenTypes.COMMA, "Expected comma `,`");
            Expr y = ParseExpression();
            Consume(TokenTypes.CLOSED_BRACKET, "Expected `)` on call to `point`");

            return new Expr.Point(pointToken.Line, pointToken.Shift, pointToken.Showfile, x, y);

        }
        Consume(TokenTypes.CLOSED_BRACKET, "Expected `)`");

        return new Expr.Point(pointToken.Line, pointToken.Shift, pointToken.Showfile);

    }
    private Expr.Lines ParseLinesExpression()
    {
        Token lineToken = Consume(TokenTypes.LINE, "Expected `line` keyword");

        Consume(TokenTypes.OPEN_BRACKET);
        if (Look.Type != TokenTypes.CLOSED_BRACKET)
        {
            Expr p1 = ParseExpression();
            Consume(TokenTypes.COMMA, "Expected comma `,`");
            Expr p2 = ParseExpression();
            Consume(TokenTypes.CLOSED_BRACKET, "Expected `)`");
            return new Expr.Lines(lineToken.Line, lineToken.Shift, lineToken.Showfile, p1, p2);
        }

        Consume(TokenTypes.CLOSED_BRACKET, "Expected `)`");
        return new Expr.Lines(lineToken.Line, lineToken.Shift, lineToken.Showfile);
    }

    private Expr.Circle ParseCircleExpression()
    {
        Token circleToken = Consume(TokenTypes.CIRCLE, "Expected `circle` keyword");

        Consume(TokenTypes.OPEN_BRACKET);
        if (Look.Type != TokenTypes.OPEN_BRACKET)
        {
            Expr p1 = ParseExpression();
            Consume(TokenTypes.COMMA, "Expected comma `,`");
            Expr radius = ParseExpression();
            Consume(TokenTypes.CLOSED_BRACKET, "Expected `)`");
            return new Expr.Circle(circleToken.Line, circleToken.Shift, circleToken.Showfile, p1, radius);
        }
        Consume(TokenTypes.OPEN_BRACKET, "Expected `)`");
        return new Expr.Circle(circleToken.Line, circleToken.Shift, circleToken.Showfile);
    }

    private Expr.Segment ParseSegmentExpression()
    {
        Token lineToken = Consume(TokenTypes.SEGMENT, "Expected `segment` keyword");

        Consume(TokenTypes.OPEN_BRACKET);
        if (Look.Type != TokenTypes.CLOSED_BRACKET)
        {
            Expr p1 = ParseExpression();
            Consume(TokenTypes.COMMA, "Expected comma `,`");
            Expr p2 = ParseExpression();
            Consume(TokenTypes.CLOSED_BRACKET, "Expected `)`");

            return new Expr.Segment(lineToken.Line, lineToken.Shift, lineToken.Showfile, p1, p2);
        }
        Consume(TokenTypes.CLOSED_BRACKET, "Expected `)`");

        return new Expr.Segment(lineToken.Line, lineToken.Shift, lineToken.Showfile);
    }

    private Expr.Ray ParseRayExpression()
    {
        Token lineToken = Consume(TokenTypes.RAY, "Expected `ray` keyword");

        Consume(TokenTypes.OPEN_BRACKET);
        if (Look.Type != TokenTypes.CLOSED_BRACKET)
        {
            Expr p1 = ParseExpression();
            Consume(TokenTypes.COMMA, "Expected comma `,`");
            Expr p2 = ParseExpression();
            Consume(TokenTypes.CLOSED_BRACKET, "Expected `)`");

            return new Expr.Ray(lineToken.Line, lineToken.Shift, lineToken.Showfile, p1, p2);
        }

        Consume(TokenTypes.CLOSED_BRACKET, "Expected `)`");

        return new Expr.Ray(lineToken.Line, lineToken.Shift, lineToken.Showfile);
    }

    private Expr.Arc ParseArcExpression()
    {
        Token arcToken = Consume(TokenTypes.ARC, "Expected `arc` keyword");

        Consume(TokenTypes.OPEN_BRACKET, "Expected `(` after call to `arc`");
        if (Look.Type != TokenTypes.CLOSED_BRACKET)
        {
            Expr p1 = ParseExpression();
            Consume(TokenTypes.COMMA, "Expected comma `,`");
            Expr p2 = ParseExpression();
            Consume(TokenTypes.COMMA, "Expected comma `,`");
            Expr p3 = ParseExpression();
            Consume(TokenTypes.COMMA, "Expected comma `,`");
            Expr radius = ParseExpression();
            Consume(TokenTypes.CLOSED_BRACKET, "Expected `)`");
            return new Expr.Arc(arcToken.Line, arcToken.Shift, arcToken.Showfile, p1, p2, p3, radius);
        }
        Consume(TokenTypes.CLOSED_BRACKET, "Expected `)`");
        return new Expr.Arc(arcToken.Line, arcToken.Shift, arcToken.Showfile);
    }
    private Expr ParseLetInExpression()
    {
        Token letToken = Consume(TokenTypes.LET);
        Statement.StatementList stmts = ParseStatementList(TokenTypes.IN);
        Consume(TokenTypes.IN);
        Expr expr = ParseExpression();
        ErrorIfEmpty(expr, letToken, "Expected non-empty expression after `in` keyword");
        return new Expr.LetIn(letToken.Line, letToken.Shift, letToken.Showfile, stmts, expr);
    }
    private Expr ParseConditionalExpression()
    {
        Token ifToken = Consume(TokenTypes.IF);
        Expr condition = ParseExpression();
        ErrorIfEmpty(condition, ifToken, "Expected non-empty expression for condition");
        Consume(TokenTypes.THEN, "Expected `then` keyword");
        Expr thenBranchExpr = ParseExpression();
        ErrorIfEmpty(thenBranchExpr, ifToken, "Expected non-empty expression after `then`");
        Consume(TokenTypes.ELSE, "Expected `else` keyword");
        Expr elseBranchExpr = ParseExpression();
        ErrorIfEmpty(elseBranchExpr, ifToken, "Expected non-empty expression after `else`");
        return new Expr.Conditional(ifToken.Line, ifToken.Shift, ifToken.Showfile, condition, thenBranchExpr, elseBranchExpr);
    }

    private Expr ParseIntersectExpression()
    {
        Token intesectToken = Consume(TokenTypes.INTERSECT);

        Consume(TokenTypes.OPEN_BRACKET, "Expected `(` on call to `point`");

        Expr x = ParseExpression();
        Consume(TokenTypes.COMMA, "Expected comma `,`");
        Expr y = ParseExpression();
        Consume(TokenTypes.CLOSED_BRACKET, "Expected `)` on call to `point`");

        return new Expr.Intersect(intesectToken.Line, intesectToken.Shift, intesectToken.Showfile, x, y);

    }

    private void ErrorIfEmpty(Expr expr, IGetError error, string message, bool enforceAbort = false)
    {
        if (expr == Expr.EMPTY) CatchError(error, message, enforceAbort);
    }

    private bool Match(params TokenTypes[] types)
    {
        foreach (TokenTypes type in types)
        {
            if (type == Look.Type)
            {
                Continue();
                return true;
            }
        }
        return false;
    }

    private Expr ParseAssociativeBinaryOperator(Func<Expr> parseLeft, Func<Expr> parseRight, int loopLimit, params TokenTypes[] types)
    {
        Expr left = parseLeft();
        int loopCount = 0;
        Token firstOperation = Look;
        while (Match(types))
        {
            if (loopLimit > 0 && loopCount == loopLimit) CatchError(Back, $"Cannot use '{Back.TryLexer}' after '{firstOperation.TryLexer}'. Consider using parenthesis and/or logical operators.");
            Token operation = Back;
            ErrorIfEmpty(left, operation, "Expected non-empty expression as left operand");
            Expr right = parseRight();
            ErrorIfEmpty(right, operation, "Expected non-empty expression as right operand");
            left = Expr.Binary.MakeBinaryExpr(left.Line, left.Shift, left.ShowFile, operation, left, right);
            ++loopCount;
        }
        return left;
    }

    private Expr ParseVariableOrCallExpression()
    {
        switch (Look.Type)
        {
            case TokenTypes.MEASURE:
                return ParseMeasureExpr();
            case TokenTypes.RANDOMS:
                return ParseRandomExpr();
            case TokenTypes.SAMPLES:
                return ParseSamplesExpr();
            case TokenTypes.COUNT:
                return ParseCountExpr();
            case TokenTypes.ID:
                Token id = Continue();
                if (Match(TokenTypes.OPEN_BRACKET))
                {
                    List<Expr> parameters = ParseParameters();
                    Consume(TokenTypes.CLOSED_BRACKET, $"Expected `)` after parameters on call to `{id.TryLexer}`");
                    return new Expr.Call(id, id.Showfile, parameters);
                }
                return new Expr.Variable(id, id.Showfile);

            default: return ParsePrimaryExpression();
        }
        List<Expr> ParseParameters()
        {
            List<Expr> parameters = new List<Expr>();
            if (Look.Type == TokenTypes.CLOSED_BRACKET) return parameters;
            do
            {
                parameters.Add(ParseExpression());
            } while (Match(TokenTypes.COMMA));
            return parameters;
        }
    }

    private Expr ParseMeasureExpr()
    {
        Token measureToken = Consume(TokenTypes.MEASURE);
        Consume(TokenTypes.OPEN_BRACKET, $"Expected `(` after call to function `measure`");
        Expr firstPoint = ParseExpression();
        ErrorIfEmpty(firstPoint, Back, "Expected non-empty expression as first parameter");
        Consume(TokenTypes.COMMA, "Expected `,` after parameter");
        Expr secondPoint = ParseExpression();
        ErrorIfEmpty(secondPoint, Back, "Expectedd non-empty expression as second parameter.");
        Consume(TokenTypes.CLOSED_BRACKET, "Expected `)` after parameters");
        return new Expr.Measure(measureToken, firstPoint, secondPoint);
    }

    private Expr ParseCountExpr()
    {
        Token countToken = Consume(TokenTypes.COUNT);
        Consume(TokenTypes.OPEN_BRACKET, $"Expected `(` after call to function `count`");
        Expr sequenceExpr = ParseExpression();
        ErrorIfEmpty(sequenceExpr, countToken, $"Expected non-empty expression on call to function `count`");
        Consume(TokenTypes.CLOSED_BRACKET, "Expected `)` after parameter");
        return new Expr.Count(countToken.Line, countToken.Shift, countToken.Showfile, sequenceExpr);
    }
    private Expr ParseRandomExpr()
    {
        Token randomToken = Consume(TokenTypes.RANDOMS);
        Consume(TokenTypes.OPEN_BRACKET, $"Expected `(` after call to `randoms`");
        Consume(TokenTypes.CLOSED_BRACKET, $"Expected `)`");
        return new Expr.Randoms(randomToken);
    }
    private Expr ParseSamplesExpr()
    {
        Token samplesToken = Consume(TokenTypes.SAMPLES);
        Consume(TokenTypes.OPEN_BRACKET, $"Expected `(` after call to `randoms`");
        Consume(TokenTypes.CLOSED_BRACKET, $"Expected `)`");
        return new Expr.Samples(samplesToken);
    }
    private Expr ParsePrimaryExpression()
    {
        switch (Look.Type)
        {
            case TokenTypes.NUMBER:
                return new Expr.Number(Look.Line, Look.Shift, Look.Showfile, (float)Continue().Literals!);
            case TokenTypes.STRING:
                return new Expr.String(Look.Line, Look.Shift, Look.Showfile, (string)Continue().Literals!);
            case TokenTypes.OPEN_BRACKET:
                Consume(TokenTypes.OPEN_BRACKET);
                Expr expr = ParseExpression();
                Consume(TokenTypes.CLOSED_BRACKET, "Expected `)`");
                return expr;
            case TokenTypes.OPEN_KEY:
                return ParseSequenceExpr();
            default: return Expr.EMPTY;
        }
    }

    private Expr ParseSequenceExpr()
    {
        Token leftBraceToken = Consume(TokenTypes.OPEN_KEY);
        List<Expr> expressions = new List<Expr>();
        bool hasThreeDots = false;
        if (Look.Type != TokenTypes.CLOSED_KEY)
        {
            if (Look.Type == TokenTypes.THREE_DOTS) CatchError(Look, "Cannot start sequence declaration with `...`");
            Expr expr = ParseExpression();
            ErrorIfEmpty(expr, leftBraceToken, "Expected non-empty expression on sequence declaration");
            expressions.Add(expr);
            if (Match(TokenTypes.THREE_DOTS))
            {
                hasThreeDots = true;
                if (Match(TokenTypes.CLOSED_KEY)) return new Expr.Sequence(leftBraceToken.Line, leftBraceToken.Shift, leftBraceToken.Showfile, hasThreeDots, expressions);
                expr = ParseExpression();
                ErrorIfEmpty(expr, Look, "Expected non-empty expression after `...`");
                expressions.Add(expr);
                Consume(TokenTypes.CLOSED_KEY, "Expected `}` after sequence Objects");
                return new Expr.Sequence(leftBraceToken.Line, leftBraceToken.Shift, leftBraceToken.Showfile, hasThreeDots, expressions);
            }
            if (Look.Type != TokenTypes.CLOSED_KEY)
            {
                Consume(TokenTypes.COMMA, "Expected `,` after expression");
                do
                {
                    if (Look.Type == TokenTypes.THREE_DOTS) CatchError(Look, "Three dots can only be used on sequence declarations of the form {Expr ...} or {Expr ... Expr}");
                    expr = ParseExpression();
                    ErrorIfEmpty(expr, Look, "Expected non-empty expression on sequence declaration");
                    expressions.Add(expr);
                } while (Match(TokenTypes.COMMA));
            }
        }
        Consume(TokenTypes.CLOSED_KEY, "Expected `}` after sequence Objects");
        return new Expr.Sequence(leftBraceToken.Line, leftBraceToken.Shift, leftBraceToken.Showfile, hasThreeDots, expressions);
    }

    private Statement.Declaration.Constant ParseConstantDeclaration()
    {
        Token id = Consume(TokenTypes.ID, "Expected identifier");
        Consume(TokenTypes.EQUAL, "Expected `=`");
        Expr expr = ParseExpression();
        ErrorIfEmpty(expr, id, $"Assigned empty expression to constant `{id.TryLexer}`");
        return new Statement.Declaration.Constant(id, id.Showfile, expr);
    }
    private Statement.Declaration.Function ParseFunctionDeclaration()
    {
        Token id = Consume(TokenTypes.ID, "Expected identifier on function declaration");
        Consume(TokenTypes.OPEN_BRACKET);

        List<Token> ParseFunctionArguments()
        {
            List<Token> arguments = new List<Token>();
            if (Look.Type == TokenTypes.CLOSED_BRACKET) return arguments;

            do
            {
                Consume(TokenTypes.ID, "Identifier expected as argument");
                arguments.Add(Back);
            } while (Match(TokenTypes.COMMA));
            return arguments;
        }

        List<Token> arguments = ParseFunctionArguments();
        Consume(TokenTypes.CLOSED_BRACKET, "Expected `)` on function declaration");
        Consume(TokenTypes.EQUAL, "Expected `=` after function signature");
        Expr body = ParseExpression();
        ErrorIfEmpty(body, id, "Expected non-empty expression as function body");
        return new Statement.Declaration.Function(id, id.Showfile, arguments, body);
    }

    internal List<string> ParseImports()
    {
        List<string> imports = new List<string>();
        while (Match(TokenTypes.IMPORT))
        {
            Token file = Consume(TokenTypes.STRING, $"Expected STRING after `import` but {Look.Type} found");
            imports.Add((string)file.Literals!);
            Consume(TokenTypes.SEMICOLON, "Expected `;` after `import` statement.");
        }
        while (!IsAtEnd)
        {
            if (Look.Type == TokenTypes.IMPORT)
            {
                try
                {
                    CatchError(Look, $"`import` must be placed before any other statements");
                }
                catch (ContinueException)
                {

                }
            }
            Continue();
        }
        return imports;
    }

    private Statement.Declaration ParseDeclaration()
    {
        switch (LookNext.Type)
        {
            case TokenTypes.EQUAL:
                return ParseConstantDeclaration();
            case TokenTypes.OPEN_BRACKET:
                return ParseFunctionDeclaration();
            case TokenTypes.COMMA:
                return ParseMatchDeclaration();
            default:
                CatchError(Look, "Identifier found on top level statement, but no declaration follows. If you intend to evaluate an expression use the `eval` keyword before the identifier.");
                break;
        }

        throw new Exception("Invalid execution path reached");
    }

    private Statement.Declaration.Match ParseMatchDeclaration()
    {
        List<Token> ID = new List<Token>();
        do{
            ID.Add(Consume(TokenTypes.ID,$"Expected `ID` on match declaration but {Look.Type} was found"));
        }while(Match(TokenTypes.COMMA));
        Token equal = Consume(TokenTypes.EQUAL,$"Expected `=` after match declaration");
        Expr sequence = ParseExpression();
        ErrorIfEmpty(sequence,equal,$"Expecte non-empty expression after `=` on match declaration");
        return new Statement.Declaration.Match(ID[0].Line,ID[0].Shift,ID[0].Showfile,ID,sequence);
    }
    public Statement ParseImportStatement()
    {
        Consume(TokenTypes.IMPORT);
        Consume(TokenTypes.STRING, $"Expected STRING after `import` but {Look.Type} found");
        return Statement.EMPTY;
    }
}