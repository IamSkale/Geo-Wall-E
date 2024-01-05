namespace Geo_Wall_E;
using System.Collections;

//Recive el AST y ejecuta la tarea especifica de cada orden

class Interpreter : IVisitorStatement<object?>, IVisitorExpr<Object>
{
    private Scope generalScope = new Scope();
    private SavingColors savedColors = new SavingColors();
    private List<IDraw> draws = new List<IDraw>();
    public List<Object.String> prints = new List<Object.String>();
    private TextWriter output;
    private int callcounter = 0;
    private int callsize = 2000;

    private Object.Sequence.Randoms randomNumbers = new Object.Sequence.Randoms();
    private Object.Sequence.Samples randomPoints = new Object.Sequence.Samples();

    public Interpreter(TextWriter _output)
    {
        output = _output;
    }

    public (List<IDraw>, List<Object.String>) Inter(Program prog)
    {
        Inter(prog.Statements, generalScope);
        return (draws, prints);
    }

    private object? Inter(Statement state, Scope scope)
    {
        state.Accept(this, scope);
        return null;
    }


    public object? VisitEmptyStatement(Statement.Empty empty, Scope scope)
    {
        return null;
    }

    public object? VisitPointStatement(Statement.Point point, Scope scope)
    {
        if (point.FullDeclarated)
        {
            scope.SetArgument(point.Id.TryLexer, new Object.Point(new Object.String(point.Id.TryLexer), (Object.Number)Evaluate(point.X, scope), (Object.Number)Evaluate(point.Y, scope), point.Comment, savedColors.Top));
        }
        else
        {
            scope.SetArgument(point.Id.TryLexer, new Object.Point(savedColors.Top));
        }
        return null;
    }
    public object? VisitLinesStatement(Statement.Lines lines, Scope scope)
    {
        if (lines.FullDeclarated)
        {
            scope.SetArgument(lines.Id.TryLexer, new Object.Line(new Object.String(lines.Id.TryLexer), (Object.Point)Evaluate(lines.P1, scope), (Object.Point)(Evaluate(lines.P2, scope)), lines.Comment, savedColors.Top));
        }
        else
        {
            scope.SetArgument(lines.Id.TryLexer, new Object.Line(savedColors.Top));
        }
        return null;
    }
    public object? VisitSegmentStatement(Statement.Segment segment, Scope scope)
    {
        if (segment.FullDeclarated)
        {
            scope.SetArgument(segment.Id.TryLexer, new Object.Segment(new Object.String(segment.Id.TryLexer), (Object.Point)Evaluate(segment.P1, scope), (Object.Point)(Evaluate(segment.P2, scope)), segment.Comment, savedColors.Top));
        }
        else { scope.SetArgument(segment.Id.TryLexer, new Object.Segment(savedColors.Top)); }
        return null;
    }
    public object? VisitRayStatement(Statement.Ray ray, Scope scope)
    {
        if (ray.FullDeclarated)
        {
            scope.SetArgument(ray.Id.TryLexer, new Object.Ray(new Object.String(ray.Id.TryLexer), (Object.Point)Evaluate(ray.P1, scope), (Object.Point)(Evaluate(ray.P2, scope)), ray.Comment, savedColors.Top));
        }
        else { scope.SetArgument(ray.Id.TryLexer, new Object.Ray(savedColors.Top)); }
        return null;
    }
    public object? VisitCircleStatement(Statement.Circle circle, Scope scope)
    {
        if (circle.FullDeclarated)
        {
            scope.SetArgument(circle.Id.TryLexer, new Object.Circle(new Object.String(circle.Id.TryLexer), (Object.Point)Evaluate(circle.P1, scope), (Object.Measure)Evaluate(circle.Radius, scope), circle.Comment, savedColors.Top));
        }
        else { scope.SetArgument(circle.Id.TryLexer, new Object.Circle(savedColors.Top)); }
        return null;
    }
    public object? VisitArcStatement(Statement.Arc arc, Scope scope)
    {
        if (arc.FullDeclarated)
        {
            scope.SetArgument(arc.Id.TryLexer, new Object.Arc(new Object.String(arc.Id.TryLexer), (Object.Point)Evaluate(arc.P1, scope), (Object.Point)Evaluate(arc.P2, scope), (Object.Point)Evaluate(arc.P3, scope), (Object.Measure)Evaluate(arc.Radius, scope), arc.Comment, savedColors.Top));
        }
        else
        {
            scope.SetArgument(arc.Id.TryLexer, new Object.Arc(savedColors.Top));
        }

        return null;
    }
    public object? VisitConstantDeclarationStatement(Statement.Declaration.Constant declaration, Scope scope)
    {
        scope.SetConstant(declaration.Id.TryLexer, Evaluate(declaration.RValue, scope));
        return null;
    }
    public object? VisitFunctionDeclarationStatement(Statement.Declaration.Function functionStatement, Scope scope)
    {
        scope.SetArgument(functionStatement.Id.TryLexer, Object.Function.MakeFunction(functionStatement));
        return null;
    }
    private Object.String NumberToString(Object.Number number)
    {
        return (new Object.String(number.Value.ToString()));
    }
    public object? VisitPrintStatement(Statement.Print Statement, Scope scope)
    {
        if (Statement._Expr != Expr.EMPTY)
        {
            if(Evaluate(Statement._Expr, scope) is Object.Number)
            {
                prints.Add(NumberToString((Object.Number)Evaluate(Statement._Expr, scope)));
            }
            else
            {
                prints.Add((Object.String)Evaluate(Statement._Expr, scope));
            }
        }
        return null;
    }
    public object? VisitColorStatement(Statement.Color Statement, Scope scope)
    {
        if (Statement.IsRestore) savedColors.Pop();
        else savedColors.Push(Statement._Color);
        return null;
    }
    public object? VisitDrawStatement(Statement.Draw statement, Scope scope)
    {
        Object evaluated = Evaluate(statement._Expr, scope);
        if (evaluated.Type == ObjectsTypes.SEQUENCE)
        {
            foreach (Object objects in (evaluated as Object.Sequence)!)
            {
                if (!(objects is IDraw)) throw new RuntimeException(statement, $"Cannot draw `{objects.Type}`");
                IDraw temp = (objects as IDraw)!;
                temp.Colors = savedColors.Top;
                draws.Add((objects as IDraw)!);
            }
            return null;
        }
        if (!(evaluated is IDraw)) throw new RuntimeException(statement, $"Cannot draw `{evaluated.Type}`");
        IDraw drawableObject = (IDraw)evaluated;
        drawableObject.Text = statement.Comment;
        draws.Add(drawableObject);
        return null;
    }
    public object? VisitEvalStatement(Statement.Eval stmt, Scope scope)
    {
        Evaluate(stmt.Expr, scope);
        return null;
    }

    public object? VisitMatchStatement(Statement.Declaration.Match stmt,Scope scope)
    {
        Object evaluated = Evaluate(stmt.Sequence,scope);
        if(evaluated.Type == ObjectsTypes.UNDEFINED){
            foreach(Token id in stmt.Identifiers)if(id.TryLexer != "_")scope.SetArgument(id.TryLexer,Object.UNDEFINED);
            return null;
        }
        Object.Sequence sequence = (evaluated as Object.Sequence)!;
        Object.Sequence.SequenceEnumerator enumerator = (Object.Sequence.SequenceEnumerator)sequence.GetEnumerator();
        for (int i = 0; i < stmt.Identifiers.Count - 1; ++i)
        {
            Object sequenceObject;
            if (stmt.Identifiers[i].TryLexer == "_")
            {
                enumerator.MoveNext();
                continue;
            }
            if (enumerator.MoveNext()) sequenceObject = enumerator.Current;
            else sequenceObject = Object.UNDEFINED;
            scope.SetArgument(stmt.Identifiers[i].TryLexer,sequenceObject);
        }
        if(stmt.Identifiers[stmt.Identifiers.Count - 1].TryLexer != "_") scope.SetArgument(stmt.Identifiers[stmt.Identifiers.Count - 1].TryLexer,enumerator.Resto);
        return null;
    }

    public object? VisitStatementList(Statement.StatementList stmtList, Scope scope)
    {
        foreach (Statement stmt in stmtList)
        {
            Inter(stmt, scope);
        }
        return null;
    }


    public Object Evaluate(Expr expr, Scope scope)
    {
        return expr.Accept(this, scope);
    }
    public Object VisitEmptyExpr(Expr.Empty expr, Scope scope)
    {
        return Object.UNDEFINED;
    }

    public Object VisitNumberExpr(Expr.Number expr, Scope scope)
    {
        return expr.Value;
    }

    public Object VisitStringExpr(Expr.String expr, Scope scope)
    {
        return expr.Value;
    }

    public Object VisitVariableExpr(Expr.Variable expr, Scope scope)
    {
        return scope.Get(expr.Id.TryLexer);
    }

    public Object VisitUnaryNotExpr(Expr.Unary.Not unaryNot, Scope scope)
    {
        return IOperations.Operate("!", Evaluate(unaryNot._Expr, scope));
    }

    public Object VisitUnaryMinusExpr(Expr.Unary.Minus expr, Scope scope)
    {
        Object rvalue = Evaluate(expr._Expr, scope);
        try
        {
            return IOperations.Operate("-", rvalue);
        }
        catch (InvalidOperationException)
        {
            throw new RuntimeException(expr, $"Cant apply operation `-` on {rvalue.Type}");
        }
    }
    public Object VisitUnaryPrintExpr(Expr.Unary.Print unaryPrint, Scope scope)
    {
        return IOperations.Operate("_", Evaluate(unaryPrint._Expr, scope));
    }

    public Object VisitBinaryPowerExpr(Expr.Binary.Power expr, Scope scope)
    {
        Object left = Evaluate(expr.Left, scope);
        Object right = Evaluate(expr.Right, scope);
        try
        {
            return IOperations.Operate("^", left, right);
        }
        catch (InvalidOperationException)
        {
            throw new RuntimeException(expr, $"Cant apply operation `^` on {left.Type} and {right.Type}");
        }
    }

    public Object VisitBinaryProductExpr(Expr.Binary.Product expr, Scope scope)
    {
        Object left = Evaluate(expr.Left, scope);
        Object right = Evaluate(expr.Right, scope);
        try
        {
            return IOperations.Operate("*", left, right);
        }
        catch (InvalidOperationException)
        {
            throw new RuntimeException(expr, $"Cant apply operation `*` on {left.Type} and {right.Type}");
        }
    }

    public Object VisitBinaryDivisionExpr(Expr.Binary.Division expr, Scope scope)
    {
        Object left = Evaluate(expr.Left, scope);
        Object right = Evaluate(expr.Right, scope);
        try
        {
            return IOperations.Operate("/", left, right);
        }
        catch (InvalidOperationException)
        {
            throw new RuntimeException(expr, $"Cant apply operation `/` on {left.Type} and {right.Type}");
        }
        catch (DivideByZeroException)
        {
            throw new RuntimeException(expr, "Division by 0");
        }
    }

    public Object VisitBinaryModulusExpr(Expr.Binary.Modulus expr, Scope scope)
    {
        Object left = Evaluate(expr.Left, scope);
        Object right = Evaluate(expr.Right, scope);
        try
        {
            return IOperations.Operate("%", left, right);
        }
        catch (InvalidOperationException)
        {
            throw new RuntimeException(expr, $"Cant apply operation `%` on {left.Type} and {right.Type}");
        }
        catch (DivideByZeroException)
        {
            throw new RuntimeException(expr, "Division by 0");
        }
    }

    public Object VisitBinarySumExpr(Expr.Binary.Sum expr, Scope scope)
    {
        Object left = Evaluate(expr.Left, scope);
        Object right = Evaluate(expr.Right, scope);
        try
        {
            return IOperations.Operate("+", left, right);
        }
        catch (InvalidOperationException)
        {
            throw new RuntimeException(expr, $"Cant apply operation `+` on {left.Type} and {right.Type}");
        }
    }

    public Object VisitBinaryDifferenceExpr(Expr.Binary.Difference expr, Scope scope)
    {
        Object left = Evaluate(expr.Left, scope);
        Object right = Evaluate(expr.Right, scope);
        try
        {
            return IOperations.Operate("-", left, right);
        }
        catch (InvalidOperationException)
        {
            throw new RuntimeException(expr, $"Cant apply operation `-` on {left.Type} and {right.Type}");
        }
    }

    public Object VisitBinaryLessExpr(Expr.Binary.Less expr, Scope scope)
    {
        Object left = Evaluate(expr.Left, scope);
        Object right = Evaluate(expr.Right, scope);
        try
        {
            return IOperations.Operate("<", left, right);
        }
        catch (InvalidOperationException)
        {
            throw new RuntimeException(expr, $"Cant apply operation `<` on {left.Type} and {right.Type}");
        }
    }

    public Object VisitBinaryLessEqualExpr(Expr.Binary.LessEqual expr, Scope scope)
    {
        Object left = Evaluate(expr.Left, scope);
        Object right = Evaluate(expr.Right, scope);
        try
        {
            return IOperations.Operate("<=", left, right);
        }
        catch (InvalidOperationException)
        {
            throw new RuntimeException(expr, $"Cant apply operation `<=` on {left.Type} and {right.Type}");
        }
    }

    public Object VisitBinaryGreaterExpr(Expr.Binary.Greater expr, Scope scope)
    {
        Object left = Evaluate(expr.Left, scope);
        Object right = Evaluate(expr.Right, scope);
        try
        {
            return IOperations.Operate(">", left, right);
        }
        catch (InvalidOperationException)
        {
            throw new RuntimeException(expr, $"Cant apply operation `>` on {left.Type} and {right.Type}");
        }
    }

    public Object VisitBinaryGreaterEqualExpr(Expr.Binary.GreaterEqual expr, Scope scope)
    {
        Object left = Evaluate(expr.Left, scope);
        Object right = Evaluate(expr.Right, scope);
        try
        {
            return IOperations.Operate(">=", left, right);
        }
        catch (InvalidOperationException)
        {
            throw new RuntimeException(expr, $"Cant apply operation `>=` on {left.Type} and {right.Type}");
        }
    }

    public Object VisitBinaryEqualEqualExpr(Expr.Binary.EqualEqual equalEqualExpr, Scope scope)
    {
        Object left = Evaluate(equalEqualExpr.Left, scope);
        Object right = Evaluate(equalEqualExpr.Right, scope);
        return left.EqualTo(right);
    }

    public Object VisitBinaryNotEqualExpr(Expr.Binary.NotEqual notEqualExpr, Scope scope)
    {
        Object left = Evaluate(notEqualExpr.Left, scope);
        Object right = Evaluate(notEqualExpr.Right, scope);
        return left.NotEqual(right);
    }

    public Object VisitBinaryAndExpr(Expr.Binary.And andExpr, Scope scope)
    {
        if (TruthValue(Evaluate(andExpr.Left, scope)) == Object.FALSE) return Object.FALSE;
        return TruthValue(Evaluate(andExpr.Right, scope));
    }

    public Object VisitBinaryOrExpr(Expr.Binary.Or orExpr, Scope scope)
    {
        if (TruthValue(Evaluate(orExpr.Left, scope)) == Object.TRUE) return Object.TRUE;
        return TruthValue(Evaluate(orExpr.Right, scope));
    }

    public Object VisitConditionalExpr(Expr.Conditional conditionalExpr, Scope scope)
    {
        if (TruthValue(Evaluate(conditionalExpr.Condition, scope)) == Object.TRUE) return Evaluate(conditionalExpr.ThenBranchExpr, scope);
        return Evaluate(conditionalExpr.ElseBranchExpr, scope);
    }

    public Object VisitLetInExpr(Expr.LetIn letInExpr, Scope scope)
    {
        Scope letInScope = new Scope(scope);
        foreach (Statement stmt in letInExpr.LetStmts) Inter(stmt, letInScope);
        return Evaluate(letInExpr.InExpr, letInScope);
    }
    public Object VisitPointExpr(Expr.Point pointExpr, Scope scope)
    {
        try
        {
            if (pointExpr.FullDeclarated)
            {
                return new Object.Point(new Object.String(""), (Object.Number)Evaluate(pointExpr.X, scope), (Object.Number)Evaluate(pointExpr.Y, scope), new Object.String(""), savedColors.Top);
            }
            else
            {
                return new Object.Point(savedColors.Top);
            }
        }
        catch (RuntimeException e)
        {
            throw new RuntimeException(pointExpr, e.Message);
        }
    }
    public Object VisitLinesExpr(Expr.Lines linesExpr, Scope scope)
    {
        try
        {
            if (linesExpr.FullDeclarated)
            {
                return new Object.Line(new Object.String(""), (Object.Point)Evaluate(linesExpr.P1, scope), (Object.Point)Evaluate(linesExpr.P2, scope), new Object.String(""), savedColors.Top);
            }
            else
            {
                return new Object.Line(savedColors.Top);
            }
        }
        catch (RuntimeException e)
        {
            throw new RuntimeException(linesExpr, e.Message);
        }

    }
    public Object VisitSegmentExpr(Expr.Segment linesExpr, Scope scope)
    {
        try
        {
            if (linesExpr.FullDeclarated)
            { return new Object.Segment(new Object.String(""), (Object.Point)Evaluate(linesExpr.P1, scope), (Object.Point)Evaluate(linesExpr.P2, scope), new Object.String(""), savedColors.Top); }
            else { return new Object.Segment(savedColors.Top); }
        }
        catch (RuntimeException e)
        {
            throw new RuntimeException(linesExpr, e.Message);
        }
    }
    public Object VisitRayExpr(Expr.Ray linesExpr, Scope scope)
    {
        try
        {
            if (linesExpr.FullDeclarated)
            { return new Object.Ray(new Object.String(""), (Object.Point)Evaluate(linesExpr.P1, scope), (Object.Point)Evaluate(linesExpr.P2, scope), new Object.String(""), savedColors.Top); }
            else
            { return new Object.Ray(savedColors.Top); }
        }
        catch (RuntimeException e)
        {
            throw new RuntimeException(linesExpr, e.Message);
        }
    }
    public Object VisitCircleExpr(Expr.Circle circleExpr, Scope scope)
    {
        try
        {
            if (circleExpr.FullDeclarated)
            {
                return new Object.Circle(new Object.String(""), (Object.Point)Evaluate(circleExpr.P1, scope), (Object.Measure)Evaluate(circleExpr.Radius, scope), new Object.String(""), savedColors.Top);
            }
            else { return new Object.Circle(savedColors.Top); }
        }
        catch (RuntimeException e)
        {
            throw new RuntimeException(circleExpr, e.Message);
        }
    }
    public Object VisitArcExpr(Expr.Arc circleExpr, Scope scope)
    {
        try
        {
            if (circleExpr.FullDeclarated)
            {
                return new Object.Arc(new Object.String(""), (Object.Point)Evaluate(circleExpr.P1, scope), (Object.Point)Evaluate(circleExpr.P2, scope), (Object.Point)Evaluate(circleExpr.P3, scope), (Object.Measure)Evaluate(circleExpr.Radius, scope), new Object.String(""), savedColors.Top);
            }
            else
            {
                return new Object.Arc(savedColors.Top);
            }
        }
        catch (RuntimeException e)
        {
            throw new RuntimeException(circleExpr, e.Message);
        }
    }

    public Object VisitSamplesExpr(Expr.Samples expr, Scope scope)
    {
        return this.randomPoints;
    }
    public Object VisitRandomsExpr(Expr.Randoms expr, Scope scope)
    {
        return this.randomNumbers;
    }

    public Object VisitCountExpr(Expr.Count expr, Scope scope)
    {
        Object evaluated = Evaluate(expr.Sequence, scope);
        if (evaluated.Type != ObjectsTypes.SEQUENCE) throw new RuntimeException(expr, $"Expected `SEQUENCE` as parameter but {evaluated.Type} was found");
        Object.Sequence sequence;
        if (evaluated is Object.Sequence.Listing)
        {
            sequence = (evaluated as Object.Sequence.Listing)!;
        }
        else
        {
            sequence = (evaluated as Object.Sequence.Interval)!;
        }
        return sequence.Count;
    }

    public Object VisitCallExpr(Expr.Call callExpr, Scope scope)
    {
        Object.Function calledFunction = (Object.Function)scope.Get(callExpr.Id.TryLexer, callExpr.Arity);
        List<Object> parameters = new List<Object>(callExpr.Arity);
        foreach (Expr expr in callExpr.Parameters) parameters.Add(Evaluate(expr, scope));
        Scope functionScope = Scope.RequestScopeForFunction(calledFunction.Arguments, parameters, scope);
        ++callcounter;
        if (callcounter > callsize) throw new RuntimeException(callExpr, $"Stack overflow. Last called function was {callExpr.Id.TryLexer}");
        Object result = Evaluate(calledFunction.Body, functionScope);
        --callcounter;
        return result;
    }
    public Object VisitMeasureExpr(Expr.Measure expr, Scope scope)
    {
        if (expr.RequiresRuntimeCheck)
        {
            Object p1 = Evaluate(expr.P1, scope);
            if (p1.Type != ObjectsTypes.POINT) throw new RuntimeException(expr, $"Expected POINT as first parameter but {p1.Type} was found");
            Object p2 = Evaluate(expr.P2, scope);
            if (p2.Type != ObjectsTypes.POINT) throw new RuntimeException(expr, $"Expected POINT as second parameter but {p2.Type} was found");
            return Object.Point.DistanceBetweenPoints((p1 as Object.Point)!, (p2 as Object.Point)!);
        }
        else
        {
            Object.Point p1 = (Evaluate(expr.P1, scope) as Object.Point)!;
            Object.Point p2 = (Evaluate(expr.P2, scope) as Object.Point)!;
            return Object.Point.DistanceBetweenPoints(p1, p2);
        }
    }

    public Object VisitSequenceExpr(Expr.Sequence sequence, Scope scope)
    {
        if (sequence.HasTreeDots)
        {
            Object start = Evaluate(sequence.First, scope);
            if (start.Type != ObjectsTypes.NUMBER) throw new RuntimeException(sequence.First, $"Dotted sequece contains {start.Type} but can only contain NUMBER");
            float startValue = (start as Object.Number)!.Value;
            if (sequence.Count == 2)
            {
                Object end = Evaluate(sequence.Second, scope);
                if (end.Type != ObjectsTypes.NUMBER) throw new RuntimeException(sequence.Second, $"Dotted sequece contains {end.Type} but can only contain NUMBER");
                float endValue = (end as Object.Number)!.Value;
                if (startValue > endValue) throw new RuntimeException(sequence, $"Sequence range is inverted : [{startValue} , {endValue}]");
                return new Object.Sequence.Interval(startValue, endValue);
            }
            return new Object.Sequence.Interval(startValue, float.PositiveInfinity);
        }
        ObjectsTypes? type = null;
        List<Object> Objects = new List<Object>();
        foreach (Expr expr in sequence)
        {
            Object Object = Evaluate(expr, scope);
            if (type == null) type = Object.Type;
            if (type != Object.Type) throw new RuntimeException(sequence, $"Sequence has Objects of type {type} and {Object.Type}. Only one type is allowed.");
            Objects.Add(Object);
        }
        return new Object.Sequence.Listing(Objects);
    }
    private Object.Number TruthValue(Object Object)
    {
        switch (Object.Type)
        {
            case ObjectsTypes.NUMBER:
                if (((Object.Number)Object).Value != 0f) return Object.TRUE;
                return Object.FALSE;
            case ObjectsTypes.UNDEFINED:
                return Object.FALSE;
            default:
                return Object.TRUE;
        }
    }
    public Object VisitIntersectExpr(Expr.Intersect intersect, Scope scope)
    {
        Object Object1 = Evaluate(intersect.FirstExpression, scope);
        Object Object2 = Evaluate(intersect.SecondExpression, scope);
        if (Resources.IsDraweable(Object1) && Resources.IsDraweable(Object2))
        {
            Object Objects = Interceptions.Intercept(Object1, Object2);
            return Objects;
        }
        else
        {
            throw new RuntimeException(intersect, $"Only Draweables Objects can be intercepted");
        }
    }
    private Object.Number OppossiteTruthValue(Object Object)
    {
        Object.Number truthValue = TruthValue(Object);
        if (truthValue == Object.TRUE) return Object.FALSE;
        return Object.TRUE;
    }
}