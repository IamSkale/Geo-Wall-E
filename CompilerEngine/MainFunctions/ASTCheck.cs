namespace Geo_Wall_E;

//Recive el AST y comprueba si cumple las condiciones impuestas

class TypeCheck : Dependecies, IVisitorStatement<object?>, IVisitorExpr<Object>
{
    Scope globalScope = new Scope();

    public TypeCheck(int MaxErrorCount, ICollection<Error> errors) : base(MaxErrorCount, errors) { }

    public override void Stop() { throw new ASTCheckException(); }

    public override void CatchError(IGetError error, string message, bool enforceAbort = false)
    {
        base.CatchError(error, message, enforceAbort);
        throw new ContinueException();
    }

    public void Check(Program program)
    {
        Check(program.Statements, globalScope);
    }
    private void Check(Statement Statement, Scope scope)
    {
        Statement.Accept(this, scope);
    }

    public object? VisitStatementList(Statement.StatementList StatementList, Scope scope)
    {
        foreach (Statement Statement in StatementList)
        {
            try
            {
                Check(Statement, scope);
            }
            catch (ContinueException)
            { }
        }
        return null;
    }
    public object? VisitEmptyStatement(Statement.Empty emptyStatement, Scope scope) { return null; }
    public object? VisitPointStatement(Statement.Point pointStatement, Scope scope)
    {

        try
        {
            try
            {
                scope.SetArgument(pointStatement.Id.TryLexer, Object.POINT);
            }
            catch (ScopeException e)
            {
                CatchError(pointStatement, e.Message);
            }
        }
        catch (ContinueException)
        { }

        return null;
    }

    public object? VisitLinesStatement(Statement.Lines linesStatement, Scope scope)
    {

        try
        {
            if (linesStatement.FullDeclarated)
            {
                CheckLineDeclaration(linesStatement, scope);
            }
            try
            {
                scope.SetArgument(linesStatement.Id.TryLexer, Object.LINE);
            }
            catch (ScopeException e)
            {
                CatchError(linesStatement, e.Message);
            }
        }
        catch (ContinueException)
        { }
        return null;
    }
    public object? VisitSegmentStatement(Statement.Segment segmentStatement, Scope scope)
    {
        try
        {
            if (segmentStatement.FullDeclarated)
            {
                CheckLineDeclaration(segmentStatement, scope);
            }
            try
            {
                scope.SetArgument(segmentStatement.Id.TryLexer, Object.SEGMENT);
            }
            catch (ScopeException e)
            {
                CatchError(segmentStatement, e.Message);
            }
        }
        catch (ContinueException)
        { }
        return null;
    }

    public object? VisitRayStatement(Statement.Ray rayStatement, Scope scope)
    {

        try
        {
            if (rayStatement.FullDeclarated)
            {
                CheckLineDeclaration(rayStatement, scope);
            }
            try
            {
                scope.SetArgument(rayStatement.Id.TryLexer, Object.RAY);
            }
            catch (ScopeException e)
            {
                CatchError(rayStatement, e.Message);
            }
        }
        catch (ContinueException)
        { }
        return null;
    }
    public object? VisitCircleStatement(Statement.Circle circleStatement, Scope scope)
    {
        try
        {
            try
            {
                scope.SetArgument(circleStatement.Id.TryLexer, Object.CIRCLE);
            }
            catch (ScopeException e)
            {
                CatchError(circleStatement, e.Message);
            }
        }
        catch (ContinueException)
        { }
        return null;
    }
    public object? VisitArcStatement(Statement.Arc arcStatement, Scope scope)
    {
        try
        {
            try
            {
                scope.SetArgument(arcStatement.Id.TryLexer, Object.ARC);
            }
            catch (ScopeException e)
            {
                CatchError(arcStatement, e.Message);
            }
        }
        catch (ContinueException)
        { }
        return null;
    }
    public object? VisitConstantDeclarationStatement(Statement.Declaration.Constant declStatement, Scope scope)
    {
        try
        {
            Object rValue = Check(declStatement.RValue, scope);
            try
            {
                scope.SetConstant(declStatement.Id.TryLexer, rValue);
            }
            catch (ScopeException e)
            {
                CatchError(declStatement, e.Message);
            }
        }
        catch (ContinueException)
        { }
        return null;
    }
    public object? VisitFunctionDeclarationStatement(Statement.Declaration.Function functionStatement, Scope scope)
    {
        try
        {
            try
            {
                scope.SetArgument(functionStatement.Id.TryLexer, Object.Function.MakeFunction(functionStatement.Arity));
            }
            catch (ScopeException e)
            {
                CatchError(functionStatement, e.Message);
            }
        }
        catch (ContinueException)
        { }
        return null;
    }
    public object? VisitPrintStatement(Statement.Print printStatement, Scope scope)
    {
        Check(printStatement._Expr, scope);
        return null;
    }
    public object? VisitColorStatement(Statement.Color colorStatement, Scope scope)
    {
        return null;
    }
    public object? VisitDrawStatement(Statement.Draw drawStatement, Scope scope)
    {
        return null;
    }
    public object? VisitMatchStatement(Statement.Declaration.Match stmt, Scope scope)
    {
        Object sequence = Check(stmt.Sequence, scope);
        try
        {
            if (sequence.Type != ObjectsTypes.SEQUENCE && sequence.Type != ObjectsTypes.RUNTIME_DEFINED) CatchError(stmt, $"Expected `sequence` after `=` but {sequence.Type} was found");
        }
        catch (ContinueException) { }
        try
        {
            List<Token> identifiers = stmt.Identifiers;
            for (int i = 0; i < stmt.Identifiers.Count; ++i)
            {
                if (identifiers[i].TryLexer == "_") continue;
                try
                {
                    scope.SetArgument(identifiers[i].TryLexer, Object.RUNTIME_DEFINED);
                }
                catch (ScopeException e)
                {
                    CatchError(stmt, e.Message);
                }
            }
        }
        catch (ContinueException) { }
        return null;
    }
    public object? VisitEvalStatement(Statement.Eval evalStatement, Scope scope)
    {
        Check(evalStatement.Expr, scope);
        return null;
    }

    private Object Check(Expr expr, Scope scope)
    {
        return expr.Accept(this, scope);
    }
    public Object VisitEmptyExpr(Expr.Empty emptyExpr, Scope scope)
    {
        return Object.UNDEFINED;
    }
    public Object VisitNumberExpr(Expr.Number numberExpr, Scope scope)
    {
        return Object.NUMBER;
    }
    public Object VisitStringExpr(Expr.String stringExpr, Scope scope)
    {
        return Object.STRING;
    }
    public Object VisitVariableExpr(Expr.Variable variableExpr, Scope scope)
    {
        try
        {
            return scope.Get(variableExpr.Id.TryLexer);
        }
        catch (ScopeException e)
        {
            CatchError(variableExpr, e.Message);
        }
        throw new Exception("Invalid excecution path reached");
    }
    public Object VisitUnaryNotExpr(Expr.Unary.Not unaryNotExpr, Scope scope)
    {
        Check(unaryNotExpr._Expr, scope);
        return Object.NUMBER;
    }
    public Object VisitUnaryMinusExpr(Expr.Unary.Minus unaryMinusExpr, Scope scope)
    {
        Object rValue = Check(unaryMinusExpr._Expr, scope);
        if (rValue.Type != ObjectsTypes.RUNTIME_DEFINED)
        {
            try
            {
                if (IOperations.Operate("-", rValue).Type == ObjectsTypes.NUMBER) unaryMinusExpr.RequiresRuntimeCheck = false;
            }
            catch (InvalidOperationException e)
            {
                try
                {
                    CatchError(unaryMinusExpr, e.Message);
                }
                catch (ContinueException) { }
            }
        }
        return Object.NUMBER;
    }
    public Object VisitUnaryPrintExpr(Expr.Unary.Print unaryPrintExpr, Scope scope)
    {
        Check(unaryPrintExpr._Expr, scope);
        return Object.NUMBER;
    }
    public Object VisitBinaryPowerExpr(Expr.Binary.Power powerExpr, Scope scope)
    {
        CheckNumberOperands(powerExpr, scope);
        return Object.NUMBER;
    }
    public Object VisitBinaryProductExpr(Expr.Binary.Product productExpr, Scope scope)
    {
        Object left = Check(productExpr.Left, scope);
        Object right = Check(productExpr.Right, scope);
        if (left.Type != ObjectsTypes.RUNTIME_DEFINED && right.Type != ObjectsTypes.RUNTIME_DEFINED)
        {
            try
            {
                switch (IOperations.Operate("*", left, right).Type)
                {
                    case ObjectsTypes.NUMBER:
                        productExpr.RequiresRuntimeCheck = false;
                        return Object.NUMBER;
                    case ObjectsTypes.MEASURE:
                        productExpr.RequiresRuntimeCheck = false;
                        return Object.MEASURE;
                    default:
                        throw new Exception("Invalid excecution path reached.");
                }
            }
            catch (InvalidOperationException e)
            {
                CatchError(productExpr, e.Message);
            }
        }
        return Object.RUNTIME_DEFINED;
    }
    public Object VisitBinaryDivisionExpr(Expr.Binary.Division divisionExpr, Scope scope)
    {
        Object left = Check(divisionExpr.Left, scope);
        Object right = Check(divisionExpr.Right, scope);
        if (left.Type != ObjectsTypes.RUNTIME_DEFINED && right.Type != ObjectsTypes.RUNTIME_DEFINED)
        {
            try
            {
                try
                {
                    switch (IOperations.Operate("/", left, right).Type)
                    {
                        case ObjectsTypes.NUMBER:
                            divisionExpr.RequiresRuntimeCheck = false;
                            return Object.NUMBER;
                        default:
                            throw new Exception("Invalid excecution path reached.");
                    }
                }
                catch (InvalidOperationException e)
                {
                    CatchError(divisionExpr, e.Message);
                }
            }
            catch (ContinueException) { }
        }
        return Object.NUMBER;
    }
    public Object VisitBinaryModulusExpr(Expr.Binary.Modulus modulusExpr, Scope scope)
    {
        CheckNumberOperands(modulusExpr, scope);
        return Object.NUMBER;
    }
    public Object VisitBinarySumExpr(Expr.Binary.Sum sumExpr, Scope scope)
    {
        Object left = Check(sumExpr.Left, scope);
        Object right = Check(sumExpr.Right, scope);
        if (left.Type != ObjectsTypes.RUNTIME_DEFINED && right.Type != ObjectsTypes.RUNTIME_DEFINED)
        {
            try
            {
                switch (IOperations.Operate("+", left, right).Type)
                {
                    case ObjectsTypes.NUMBER:
                        sumExpr.RequiresRuntimeCheck = false;
                        return Object.NUMBER;
                    case ObjectsTypes.MEASURE:
                        sumExpr.RequiresRuntimeCheck = false;
                        return Object.MEASURE;
                    default:
                        throw new Exception("Invalid excecution path reached.");
                }
            }
            catch (InvalidOperationException e)
            {
                CatchError(sumExpr, e.Message);
            }
        }
        return Object.RUNTIME_DEFINED;
    }
    public Object VisitBinaryDifferenceExpr(Expr.Binary.Difference differenceExpr, Scope scope)
    {
        Object left = Check(differenceExpr.Left, scope);
        Object right = Check(differenceExpr.Right, scope);
        if (left.Type != ObjectsTypes.RUNTIME_DEFINED && right.Type != ObjectsTypes.RUNTIME_DEFINED)
        {
            try
            {
                switch (IOperations.Operate("-", left, right).Type)
                {
                    case ObjectsTypes.NUMBER:
                        differenceExpr.RequiresRuntimeCheck = false;
                        return Object.NUMBER;
                    case ObjectsTypes.MEASURE:
                        differenceExpr.RequiresRuntimeCheck = false;
                        return Object.MEASURE;
                    default:
                        throw new Exception("Invalid excecution path reached.");
                }
            }
            catch (InvalidOperationException e)
            {
                CatchError(differenceExpr, e.Message);
            }
        }
        return Object.RUNTIME_DEFINED;
    }
    public Object VisitBinaryLessExpr(Expr.Binary.Less lessExpr, Scope scope)
    {
        Object left = Check(lessExpr.Left, scope);
        Object right = Check(lessExpr.Right, scope);
        if (left.Type != ObjectsTypes.RUNTIME_DEFINED && right.Type != ObjectsTypes.RUNTIME_DEFINED)
        {
            try
            {
                try
                {
                    switch (IOperations.Operate("<", left, right).Type)
                    {
                        case ObjectsTypes.NUMBER:
                            lessExpr.RequiresRuntimeCheck = false;
                            return Object.NUMBER;
                        default:
                            throw new Exception("Invalid excecution path reached.");
                    }
                }
                catch (InvalidOperationException e)
                {
                    CatchError(lessExpr, e.Message);
                }
            }
            catch (ContinueException) { }
        }
        return Object.NUMBER;
    }
    public Object VisitBinaryLessEqualExpr(Expr.Binary.LessEqual lessEqualExpr, Scope scope)
    {
        Object left = Check(lessEqualExpr.Left, scope);
        Object right = Check(lessEqualExpr.Right, scope);
        if (left.Type != ObjectsTypes.RUNTIME_DEFINED && right.Type != ObjectsTypes.RUNTIME_DEFINED)
        {
            try
            {
                try
                {
                    switch (IOperations.Operate("<", left, right).Type)
                    {
                        case ObjectsTypes.NUMBER:
                            lessEqualExpr.RequiresRuntimeCheck = false;
                            return Object.NUMBER;
                        default:
                            throw new Exception("Invalid excecution path reached.");
                    }
                }
                catch (InvalidOperationException e)
                {
                    CatchError(lessEqualExpr, e.Message);
                }
            }
            catch (ContinueException) { }
        }
        return Object.NUMBER;
    }
    public Object VisitBinaryGreaterExpr(Expr.Binary.Greater greaterExpr, Scope scope)
    {
        Object left = Check(greaterExpr.Left, scope);
        Object right = Check(greaterExpr.Right, scope);
        if (left.Type != ObjectsTypes.RUNTIME_DEFINED && right.Type != ObjectsTypes.RUNTIME_DEFINED)
        {
            try
            {
                try
                {
                    switch (IOperations.Operate(">", left, right).Type)
                    {
                        case ObjectsTypes.NUMBER:
                            greaterExpr.RequiresRuntimeCheck = false;
                            return Object.NUMBER;
                        default:
                            throw new Exception("Invalid excecution path reached.");
                    }
                }
                catch (InvalidOperationException e)
                {
                    CatchError(greaterExpr, e.Message);
                }
            }
            catch (ContinueException) { }
        }
        return Object.NUMBER;
    }
    public Object VisitBinaryGreaterEqualExpr(Expr.Binary.GreaterEqual greaterEqualExpr, Scope scope)
    {
        Object left = Check(greaterEqualExpr.Left, scope);
        Object right = Check(greaterEqualExpr.Right, scope);
        if (left.Type != ObjectsTypes.RUNTIME_DEFINED && right.Type != ObjectsTypes.RUNTIME_DEFINED)
        {
            try
            {
                try
                {
                    switch ((IOperations.Operate(">=", left, right)).Type)
                    {
                        case ObjectsTypes.NUMBER:
                            greaterEqualExpr.RequiresRuntimeCheck = false;
                            return Object.NUMBER;
                        default:
                            throw new Exception("Invalid excecution path reached.");
                    }
                }
                catch (InvalidOperationException e)
                {
                    CatchError(greaterEqualExpr, e.Message);
                }
            }
            catch (ContinueException) { }
        }
        return Object.NUMBER;
    }
    private void CheckNumberOperands(Expr.Binary binaryExpr, Scope scope)
    {
        bool runtimeCheck = false;
        try
        {
            Object operand = Check(binaryExpr.Left, scope);
            if (operand.Type == ObjectsTypes.RUNTIME_DEFINED)
            {
                runtimeCheck = true;
            }
            else if (operand.Type != ObjectsTypes.NUMBER) CatchError(binaryExpr, $"Left operand of `{binaryExpr.Operator.TryLexer}` is {operand.Type} and must be NUMBER");
        }
        catch (ContinueException) { }
        try
        {
            Object operand = Check(binaryExpr.Right, scope);
            if (operand.Type == ObjectsTypes.RUNTIME_DEFINED)
            {
                runtimeCheck = true;
            }
            else if (operand.Type != ObjectsTypes.NUMBER) CatchError(binaryExpr, $"Right operand of `{binaryExpr.Operator.TryLexer}` is {operand.Type} and must be NUMBER");
        }
        catch (ContinueException) { }

        binaryExpr.RequiresRuntimeCheck = runtimeCheck;
    }
    private void CheckLineDeclaration(Statement.Lines lineStatement, Scope scope)
    {
        try
        {
            Object parameter = Check(lineStatement.P1, scope);
            if (parameter.Type != ObjectsTypes.POINT && parameter.Type != ObjectsTypes.RUNTIME_DEFINED) CatchError(lineStatement, $"Expected `POINT` as first parameter but {parameter.Type} was found");
        }
        catch (ContinueException) { }
        try
        {
            Object parameter = Check(lineStatement.P2, scope);
            if (parameter.Type != ObjectsTypes.POINT && parameter.Type != ObjectsTypes.RUNTIME_DEFINED) CatchError(lineStatement, $"Expected `POINT` as second parameter but {parameter.Type} was found");
        }
        catch (ContinueException) { }
    }
    public Object VisitBinaryEqualEqualExpr(Expr.Binary.EqualEqual equalEqualExpr, Scope scope)
    {
        Check(equalEqualExpr.Left, scope);
        Check(equalEqualExpr.Right, scope);

        return Object.NUMBER;
    }
    public Object VisitBinaryNotEqualExpr(Expr.Binary.NotEqual notEqualExpr, Scope scope)
    {
        Check(notEqualExpr.Left, scope);
        Check(notEqualExpr.Right, scope);

        return Object.NUMBER;
    }
    public Object VisitBinaryAndExpr(Expr.Binary.And andExpr, Scope scope)
    {
        Check(andExpr.Left, scope);
        Check(andExpr.Right, scope);

        return Object.NUMBER;
    }
    public Object VisitBinaryOrExpr(Expr.Binary.Or orExpr, Scope scope)
    {
        Check(orExpr.Left, scope);
        Check(orExpr.Right, scope);

        return Object.NUMBER;
    }
    public Object VisitConditionalExpr(Expr.Conditional conditionalExpr, Scope scope)
    {
        Check(conditionalExpr.Condition, scope);
        Object thenBranchObject = Check(conditionalExpr.ThenBranchExpr, scope);
        Object elseBranchObject = Check(conditionalExpr.ElseBranchExpr, scope);
        if (thenBranchObject.Type == ObjectsTypes.RUNTIME_DEFINED) return elseBranchObject;
        if (elseBranchObject.Type == ObjectsTypes.RUNTIME_DEFINED) return thenBranchObject;


        if (thenBranchObject.Type != elseBranchObject.Type) CatchError(conditionalExpr, $"Expected equal return types for `if-then-else` expression branches, but {thenBranchObject.Type} and {elseBranchObject.Type} were found.");

        conditionalExpr.RequiresRuntimeCheck = false;
        return thenBranchObject;
    }
    public Object VisitLetInExpr(Expr.LetIn letInExpr, Scope scope)
    {
        Scope letInScope = new Scope(scope);
        foreach (Statement Statement in letInExpr.LetStmts) Check(Statement, letInScope);
        Object inExprObject = Check(letInExpr.InExpr, letInScope);
        return inExprObject;
    }
    public Object VisitCallExpr(Expr.Call callExpr, Scope scope)
    {
        try
        {
            scope.Get(callExpr.Id.TryLexer, callExpr.Arity);

            foreach (Expr parameterExpr in callExpr.Parameters)
            {
                try
                {
                    Check(parameterExpr, scope);
                }
                catch (ContinueException)
                { }
            }
        }
        catch (ScopeException e)
        {
            CatchError(callExpr, e.Message);
        }

        return Object.RUNTIME_DEFINED;
    }
    public Object VisitMeasureExpr(Expr.Measure expr, Scope scope)
    {
        bool requiresRuntimeCheck = false;
        try
        {
            Object p1 = Check(expr.P1, scope);
            if (p1.Type == ObjectsTypes.RUNTIME_DEFINED) requiresRuntimeCheck = true;
            else if (p1.Type != ObjectsTypes.POINT) CatchError(expr, $"Expected POINT as first parameter but {p1.Type} was found");
        }
        catch (ContinueException)
        { }
        try
        {
            Object p2 = Check(expr.P1, scope);
            if (p2.Type == ObjectsTypes.RUNTIME_DEFINED) requiresRuntimeCheck = true;
            else if (p2.Type != ObjectsTypes.POINT) CatchError(expr, $"Expected POINT as second parameter but {p2.Type} was found");
        }
        catch (ContinueException)
        { }
        expr.RequiresRuntimeCheck = requiresRuntimeCheck;
        return Object.MEASURE;
    }
    public Object VisitPointExpr(Expr.Point pointExpr, Scope scope)
    {
        if (pointExpr.FullDeclarated)
        {
            try
            {
                Check(pointExpr.X, scope);
            }
            catch (ContinueException)
            { }
            Object parameterx = Check(pointExpr.X, scope);
            if (parameterx.Type != ObjectsTypes.NUMBER) CatchError(pointExpr, $"Expected `NUMBER` as first parameter but {parameterx.Type} was found");
            try
            {
                Check(pointExpr.Y, scope);
            }
            catch (ContinueException)
            { }
            Object parametery = Check(pointExpr.Y, scope);
            if (parametery.Type != ObjectsTypes.NUMBER) CatchError(pointExpr, $"Expected `NUMBER` as first parameter but {parametery.Type} was found");
        }
        return Object.POINT;
    }
    public Object VisitLinesExpr(Expr.Lines linesexpr, Scope scope)
    {   
        if (linesexpr.FullDeclarated)
        {

            try
            {
                Check(linesexpr.P1, scope);
            }
            catch (ContinueException)
            { }
            Object parameterx = Check(linesexpr.P1, scope);
            if (parameterx.Type != ObjectsTypes.POINT && parameterx.Type != ObjectsTypes.RUNTIME_DEFINED) CatchError(linesexpr.P1, $"Expected `POINT` as first parameter but {parameterx.Type} was found");
            try
            {
                Check(linesexpr.P2, scope);
            }
            catch (ContinueException)
            { }
            Object parametery = Check(linesexpr.P2, scope);
            if (parametery.Type != ObjectsTypes.POINT && parametery.Type != ObjectsTypes.RUNTIME_DEFINED) CatchError(linesexpr.P2, $"Expected `POINT` as first parameter but {parametery.Type} was found");
        }
        
        return Object.LINE;
    }
    public Object VisitSegmentExpr(Expr.Segment linesexpr, Scope scope)
    {
        if (linesexpr.FullDeclarated)
        {
            try
            {
                Check(linesexpr.P1, scope);
            }
            catch (ContinueException)
            { }
            Object parameterx = Check(linesexpr.P1, scope);
            if (parameterx.Type != ObjectsTypes.POINT) CatchError(linesexpr.P1, $"Expected `POINT` as first parameter but {parameterx.Type} was found");
            try
            {
                Check(linesexpr.P2, scope);
            }
            catch (ContinueException)
            { }
            Object parametery = Check(linesexpr.P2, scope);
            if (parametery.Type != ObjectsTypes.POINT) CatchError(linesexpr.P2, $"Expected `POINT` as first parameter but {parametery.Type} was found");
        }
        return Object.SEGMENT;
    }
    public Object VisitRayExpr(Expr.Ray linesexpr, Scope scope)
    {
        if (linesexpr.FullDeclarated)
        {
            try
            {
                Check(linesexpr.P1, scope);
            }
            catch (ContinueException)
            { }
            Object parameterx = Check(linesexpr.P1, scope);
            if (parameterx.Type != ObjectsTypes.POINT) CatchError(linesexpr.P1, $"Expected `POINT` as first parameter but {parameterx.Type} was found");
            try
            {
                Check(linesexpr.P2, scope);
            }
            catch (ContinueException)
            { }
            Object parametery = Check(linesexpr.P2, scope);
            if (parametery.Type != ObjectsTypes.POINT) CatchError(linesexpr.P2, $"Expected `POINT` as first parameter but {parametery.Type} was found");
        }
        return Object.RAY;
    }
    public Object VisitCircleExpr(Expr.Circle circleexpr, Scope scope)
    {
        if (circleexpr.FullDeclarated)
        {
            try
            {
                Check(circleexpr.P1, scope);
            }
            catch (ContinueException)
            { }
            Object parameter1 = Check(circleexpr.P1, scope);
            if (parameter1.Type != ObjectsTypes.POINT) CatchError(circleexpr.P1, $"Expected `POINT` as first parameter but {parameter1.Type} was found");
            try
            {
                Check(circleexpr.Radius, scope);
            }
            catch (ContinueException)
            { }
            Object parameter2 = Check(circleexpr.Radius, scope);
            if (parameter2.Type != ObjectsTypes.MEASURE) CatchError(circleexpr.Radius, $"Expected `MEASURE` as first parameter but {parameter2.Type} was found");
        }
        return Object.CIRCLE;
    }
    public Object VisitArcExpr(Expr.Arc circleexpr, Scope scope)
    {
        if (circleexpr.FullDeclarated)
        {
            try
            {
                Check(circleexpr.P1, scope);
            }
            catch (ContinueException)
            { }
            Object parameter1 = Check(circleexpr.P1, scope);
            if (parameter1.Type != ObjectsTypes.POINT) CatchError(circleexpr.P1, $"Expected `POINT` as first parameter but {parameter1.Type} was found");
            try
            {
                Check(circleexpr.P2, scope);
            }
            catch (ContinueException)
            { }
            Object parameter2 = Check(circleexpr.P2, scope);
            if (parameter2.Type != ObjectsTypes.POINT) CatchError(circleexpr.P2, $"Expected `POINT` as first parameter but {parameter2.Type} was found");
            try
            {
                Check(circleexpr.P3, scope);
            }
            catch (ContinueException)
            { }
            Object parameter3 = Check(circleexpr.P3, scope);
            if (parameter3.Type != ObjectsTypes.POINT) CatchError(circleexpr.P3, $"Expected `POINT` as first parameter but {parameter3.Type} was found");
            try
            {
                Check(circleexpr.Radius, scope);
            }
            catch (ContinueException)
            { }
            Object parameter4 = Check(circleexpr.Radius, scope);
            if (parameter4.Type != ObjectsTypes.MEASURE) CatchError(circleexpr.Radius, $"Expected `MEASURE` as first parameter but {parameter4.Type} was found");
        }
        return Object.CIRCLE;
    }

    public Object VisitCountExpr(Expr.Count expr, Scope scope)
    {
        Object sequence = Check(expr.Sequence, scope);
        try
        {
            if (sequence.Type != ObjectsTypes.SEQUENCE && sequence.Type != ObjectsTypes.RUNTIME_DEFINED) CatchError(expr, $"Expected `SEQUENCE` as parameter but {sequence.Type} was found");
        }
        catch (ContinueException) { }
        return Object.NUMBER;
    }
    public Object VisitSamplesExpr(Expr.Samples expr, Scope scope)
    {
        return Object.SEQUENCE;
    }
    public Object VisitRandomsExpr(Expr.Randoms expr, Scope scope)
    {
        return Object.SEQUENCE;
    }


    public Object VisitIntersectExpr(Expr.Intersect expr, Scope scope)
    {
        return Object.RUNTIME_DEFINED;
    }

    public Object VisitSequenceExpr(Expr.Sequence expr, Scope scope)
    {
        return Object.RUNTIME_DEFINED;
    }
}
