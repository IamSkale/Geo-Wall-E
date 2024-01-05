using System.Collections;
namespace Geo_Wall_E;

interface IVisitorExpr<S>
{
    public S VisitEmptyExpr(Expr.Empty expr, Scope scope);
    public S VisitNumberExpr(Expr.Number expr, Scope scope);
    public S VisitStringExpr(Expr.String expr, Scope scope);
    public S VisitVariableExpr(Expr.Variable expr, Scope scope);
    public S VisitUnaryNotExpr(Expr.Unary.Not expr, Scope scope);
    public S VisitUnaryMinusExpr(Expr.Unary.Minus expr, Scope scope);
    public S VisitUnaryPrintExpr(Expr.Unary.Print expr, Scope scope);
    public S VisitBinaryPowerExpr(Expr.Binary.Power expr, Scope scope);
    public S VisitBinaryProductExpr(Expr.Binary.Product expr, Scope scope);
    public S VisitBinaryDivisionExpr(Expr.Binary.Division expr, Scope scope);
    public S VisitBinaryModulusExpr(Expr.Binary.Modulus expr, Scope scope);
    public S VisitBinarySumExpr(Expr.Binary.Sum expr, Scope scope);
    public S VisitBinaryDifferenceExpr(Expr.Binary.Difference expr, Scope scope);
    public S VisitBinaryLessExpr(Expr.Binary.Less expr, Scope scope);
    public S VisitBinaryLessEqualExpr(Expr.Binary.LessEqual expr, Scope scope);
    public S VisitBinaryGreaterExpr(Expr.Binary.Greater expr, Scope scope);
    public S VisitBinaryGreaterEqualExpr(Expr.Binary.GreaterEqual expr, Scope scope);
    public S VisitBinaryEqualEqualExpr(Expr.Binary.EqualEqual expr, Scope scope);
    public S VisitBinaryNotEqualExpr(Expr.Binary.NotEqual expr, Scope scope);
    public S VisitBinaryAndExpr(Expr.Binary.And expr, Scope scope);
    public S VisitBinaryOrExpr(Expr.Binary.Or expr, Scope scope);
    public S VisitConditionalExpr(Expr.Conditional expr, Scope scope);
    public S VisitLetInExpr(Expr.LetIn expr, Scope scope);
    public S VisitCallExpr(Expr.Call expr, Scope scope);
    public S VisitMeasureExpr(Expr.Measure expr, Scope scope);
    public S VisitPointExpr(Expr.Point expr, Scope scope);
    public S VisitLinesExpr(Expr.Lines expr, Scope scope);
    public S VisitSegmentExpr(Expr.Segment expr, Scope scope);
    public S VisitRayExpr(Expr.Ray expr, Scope scope);
    public S VisitCircleExpr(Expr.Circle expr, Scope scope);
    public S VisitArcExpr(Expr.Arc expr, Scope scope);
    public S VisitIntersectExpr(Expr.Intersect expr, Scope scope);
    public S VisitCountExpr(Expr.Count expr, Scope scope);
    public S VisitRandomsExpr(Expr.Randoms expr, Scope scope);
    public S VisitSamplesExpr(Expr.Samples expr, Scope scope);
    public S VisitSequenceExpr(Expr.Sequence expr, Scope scope);
}

interface IVisitableExpr
{
    public S Accept<S>(IVisitorExpr<S> visitor, Scope scope);
}

//Clase Base de las expresiones
abstract class Expr : IVisitableExpr, IGetError
{
    public int Line { get; private set; }
    public int Shift { get; private set; }
    public string File { get => new string(fileName); }
    private char[] fileName;
    public char[] ShowFile { get => fileName; }
    public virtual bool RequiresRuntimeCheck { get; set; }
    protected Expr(int tline, int tshift, char[] tfileName)
    {
        Line = tline;
        Shift = tshift;
        fileName = tfileName;
        RequiresRuntimeCheck = true;
    }

    abstract public S Accept<S>(IVisitorExpr<S> visitor, Scope scope);

    //Expresiones vacias
    public class Empty : Expr
    {
        public Empty() : base(0, 0, new char[] { 'E', 'M', 'P', 'T', 'Y' }) { }
        public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
        {
            return visitor.VisitEmptyExpr(this, scope);
        }
        public override bool RequiresRuntimeCheck { get => false; set => base.RequiresRuntimeCheck = false; }
    }
    public static Empty EMPTY = new Empty();

    //Llamada
    public class Call : Expr
    {
        public Token Id { get; private set; }
        public List<Expr> Parameters { get; private set; }
        public int Arity { get => Parameters.Count; }
        public Call(Token id, char[] fileName, List<Expr> parameters) : base(id.Line, id.Shift, fileName)
        {
            Id = id;
            Parameters = parameters;
        }
        public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
        {
            return visitor.VisitCallExpr(this, scope);
        }
        public override bool RequiresRuntimeCheck { get => false; set => base.RequiresRuntimeCheck = false; }
    }

    //Numeros
    public class Number : Expr
    {
        public Object.Number Value { get; private set; }
        public Number(int line, int shift, char[] fileName, float tvalue) : base(line, shift, fileName)
        {
            Value = new Object.Number(tvalue);
        }
        public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
        {
            return visitor.VisitNumberExpr(this, scope);
        }
        public override bool RequiresRuntimeCheck { get => false; set => base.RequiresRuntimeCheck = false; }
    }

    //Medida
    public class Measure : Expr
    {
        public Expr P1 { get; private set; }
        public Expr P2 { get; private set; }
        public Measure(Token measureToken, Expr firstPoint, Expr secondPoint) : base(measureToken.Line, measureToken.Shift, measureToken.Showfile)
        {
            P1 = firstPoint;
            P2 = secondPoint;
        }
        public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
        {
            return visitor.VisitMeasureExpr(this, scope);
        }
    }

    //Strings
    public class String : Expr
    {
        public Object.String Value { get; private set; }
        public String(int line, int shift, char[] fileName, string _value) : base(line, shift, fileName)
        {
            Value = new Object.String(_value);
        }
        public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
        {
            return visitor.VisitStringExpr(this, scope);
        }
        public override bool RequiresRuntimeCheck { get => false; set => base.RequiresRuntimeCheck = false; }
    }

    //Variables
    public class Variable : Expr
    {
        public Token Id { get; private set; }
        public Variable(Token tid, char[] fileName) : base(tid.Line, tid.Shift, fileName)
        {
            Id = tid;
        }
        public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
        {
            return visitor.VisitVariableExpr(this, scope);
        }
        public override bool RequiresRuntimeCheck { get => false; set => base.RequiresRuntimeCheck = false; }
    }

    //Operadores unarios
    public abstract class Unary : Expr
    {
        public Expr _Expr { get; private set; }
        abstract public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope);
        protected Unary(int line, int shift, char[] fileName, Expr _expr) : base(line, shift, fileName)
        {
            _Expr = _expr;
        }
        public class Not : Unary
        {
            public Not(int line, int shift, char[] fileName, Expr _expr) : base(line, shift, fileName, _expr) { }
            public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
            {
                return visitor.VisitUnaryNotExpr(this, scope);
            }
            public override bool RequiresRuntimeCheck { get => false; set => base.RequiresRuntimeCheck = false; }
        }
        public class Minus : Unary
        {
            public Minus(int line, int shift, char[] fileName, Expr _expr) : base(line, shift, fileName, _expr) { }
            public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
            {
                return visitor.VisitUnaryMinusExpr(this, scope);
            }
        }
        public class Print : Unary
        {
            public Print(int line, int shift, char[] fileName, Expr _expr) : base(line, shift, fileName, _expr) { }
            public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
            {
                return visitor.VisitUnaryPrintExpr(this, scope);
            }
        }
    }

    public abstract class Binary : Expr
    {
        private static Type GetTypeFromOperation(TokenTypes operationType)
        {
            switch (operationType)
            {
                case TokenTypes.POWER:
                    return typeof(Expr.Binary.Power);
                case TokenTypes.PROD:
                    return typeof(Expr.Binary.Product);
                case TokenTypes.DIV:
                    return typeof(Expr.Binary.Division);
                case TokenTypes.PERCENT:
                    return typeof(Expr.Binary.Modulus);
                case TokenTypes.SUM:
                    return typeof(Expr.Binary.Sum);
                case TokenTypes.REST:
                    return typeof(Expr.Binary.Difference);
                case TokenTypes.LOWER:
                    return typeof(Expr.Binary.Less);
                case TokenTypes.LOWER_EQUAL:
                    return typeof(Expr.Binary.LessEqual);
                case TokenTypes.GREATER:
                    return typeof(Expr.Binary.Greater);
                case TokenTypes.GREATER_EQUAL:
                    return typeof(Expr.Binary.GreaterEqual);
                case TokenTypes.EQUAL_EQUAL:
                    return typeof(Expr.Binary.EqualEqual);
                case TokenTypes.NO_EQUAL:
                    return typeof(Expr.Binary.NotEqual);
                case TokenTypes.AND:
                    return typeof(Expr.Binary.And);
                case TokenTypes.OR:
                    return typeof(Expr.Binary.Or);
                default: throw new ArgumentException($"No type matching operation of type {operationType}");
            }
        }

        public static Expr.Binary MakeBinaryExpr(int line, int shift, char[] fileName, Token operation, Expr left, Expr right)
        {
            Type binaryExprType = GetTypeFromOperation(operation.Type);

            Expr.Binary binaryExpr = (Activator.CreateInstance(binaryExprType, line, shift, fileName, operation, left, right) as Expr.Binary)!;
            return binaryExpr;
        }
        public Expr Left { get; private set; }
        public Expr Right { get; private set; }
        public Token Operator { get; private set; }
        abstract public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope);
        protected Binary(int line, int shift, char[] fileName, Token _operator, Expr left, Expr right) : base(line, shift, fileName)
        {
            Left = left;
            Right = right;
            Operator = _operator;
        }
        public class Power : Binary
        {
            public Power(int line, int shift, char[] fileName, Token _operator, Expr left, Expr right) : base(line, shift, fileName, _operator, left, right) { }
            public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
            {
                return visitor.VisitBinaryPowerExpr(this, scope);
            }
        }
        public class Product : Binary
        {
            public Product(int line, int shift, char[] fileName, Token _operator, Expr left, Expr right) : base(line, shift, fileName, _operator, left, right) { }
            public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
            {
                return visitor.VisitBinaryProductExpr(this, scope);
            }
        }
        public class Division : Binary
        {
            public Division(int line, int shift, char[] fileName, Token _operator, Expr left, Expr right) : base(line, shift, fileName, _operator, left, right) { }
            public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
            {
                return visitor.VisitBinaryDivisionExpr(this, scope);
            }
        }

        public class Modulus : Binary
        {
            public Modulus(int line, int shift, char[] fileName, Token _operator, Expr left, Expr right) : base(line, shift, fileName, _operator, left, right) { }
            public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
            {
                return visitor.VisitBinaryModulusExpr(this, scope);
            }
        }
        public class Sum : Binary
        {
            public Sum(int line, int shift, char[] fileName, Token _operator, Expr left, Expr right) : base(line, shift, fileName, _operator, left, right) { }
            public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
            {
                return visitor.VisitBinarySumExpr(this, scope);
            }
        }
        public class Difference : Binary
        {
            public Difference(int line, int shift, char[] fileName, Token _operator, Expr left, Expr right) : base(line, shift, fileName, _operator, left, right) { }
            public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
            {
                return visitor.VisitBinaryDifferenceExpr(this, scope);
            }
        }
        public class Less : Binary
        {
            public Less(int line, int shift, char[] fileName, Token _operator, Expr left, Expr right) : base(line, shift, fileName, _operator, left, right) { }
            public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
            {
                return visitor.VisitBinaryLessExpr(this, scope);
            }
        }

        public class LessEqual : Binary
        {
            public LessEqual(int line, int shift, char[] fileName, Token _operator, Expr left, Expr right) : base(line, shift, fileName, _operator, left, right) { }
            public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
            {
                return visitor.VisitBinaryLessEqualExpr(this, scope);
            }
        }

        public class Greater : Binary
        {
            public Greater(int line, int shift, char[] fileName, Token _operator, Expr left, Expr right) : base(line, shift, fileName, _operator, left, right) { }
            public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
            {
                return visitor.VisitBinaryGreaterExpr(this, scope);
            }
        }

        public class GreaterEqual : Binary
        {
            public GreaterEqual(int line, int shift, char[] fileName, Token _operator, Expr left, Expr right) : base(line, shift, fileName, _operator, left, right) { }
            public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
            {
                return visitor.VisitBinaryGreaterEqualExpr(this, scope);
            }
        }

        public class EqualEqual : Binary
        {
            public EqualEqual(int line, int shift, char[] fileName, Token _operator, Expr left, Expr right) : base(line, shift, fileName, _operator, left, right) { }
            public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
            {
                return visitor.VisitBinaryEqualEqualExpr(this, scope);
            }
            public override bool RequiresRuntimeCheck { get => false; set => base.RequiresRuntimeCheck = false; }
        }

        public class NotEqual : Binary
        {
            public NotEqual(int line, int shift, char[] fileName, Token _operator, Expr left, Expr right) : base(line, shift, fileName, _operator, left, right) { }
            public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
            {
                return visitor.VisitBinaryNotEqualExpr(this, scope);
            }
            public override bool RequiresRuntimeCheck { get => false; set => base.RequiresRuntimeCheck = false; }
        }

        public class And : Binary
        {
            public And(int line, int shift, char[] fileName, Token _operator, Expr left, Expr right) : base(line, shift, fileName, _operator, left, right) { }
            public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
            {
                return visitor.VisitBinaryAndExpr(this, scope);
            }
            public override bool RequiresRuntimeCheck { get => false; set => base.RequiresRuntimeCheck = false; }
        }

        public class Or : Binary
        {
            public Or(int line, int shift, char[] fileName, Token _operator, Expr left, Expr right) : base(line, shift, fileName, _operator, left, right) { }
            public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
            {
                return visitor.VisitBinaryOrExpr(this, scope);
            }
            public override bool RequiresRuntimeCheck { get => false; set => base.RequiresRuntimeCheck = false; }
        }
    }

    //Condicionales
    public class Conditional : Expr
    {
        public Expr Condition { get; private set; }
        public Expr ThenBranchExpr { get; private set; }
        public Expr ElseBranchExpr { get; private set; }
        public Conditional(int line, int shift, char[] fileName, Expr condition, Expr thenBranchExpr, Expr elseBranchExpr) : base(line, shift, fileName)
        {
            Condition = condition;
            ThenBranchExpr = thenBranchExpr;
            ElseBranchExpr = elseBranchExpr;
        }
        public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
        {
            return visitor.VisitConditionalExpr(this, scope);
        }
    }

    //Let-in
    public class LetIn : Expr
    {
        public Statement.StatementList LetStmts { get; private set; }
        public Expr InExpr { get; private set; }
        public LetIn(int line, int shift, char[] fileName, Statement.StatementList states, Expr expr) : base(line, shift, fileName)
        {
            LetStmts = states;
            InExpr = expr;
        }
        public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
        {
            return visitor.VisitLetInExpr(this, scope);
        }
        public override bool RequiresRuntimeCheck { get => false; set => base.RequiresRuntimeCheck = false; }
    }

    //Punto
    public class Point : Expr
    {
        public Expr X { get; private set; }
        public Expr Y { get; private set; }
        public bool FullDeclarated { get; private set; }
        public Point(int line, int shift, char[] fileName, Expr x, Expr y) : base(line, shift, fileName)
        {
            X = x;
            Y = y;
            FullDeclarated = true;
        }
        public Point(int line, int shift, char[] fileName) : base(line, shift, fileName)
        {
            X = Expr.EMPTY;
            Y = Expr.EMPTY;
            FullDeclarated = false;
        }
        public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
        {
            return visitor.VisitPointExpr(this, scope);
        }
    }

    //Lineas
    public class Lines : Expr
    {
        public Expr P1 { get; private set; }
        public Expr P2 { get; private set; }
        public bool FullDeclarated { get; private set; }

        public Lines(int line, int shift, char[] fileName, Expr _p1, Expr _p2) : base(line, shift, fileName)
        {
            P1 = _p1;
            P2 = _p2;
            FullDeclarated = true;
        }
        public Lines(int line, int shift, char[] fileName) : base(line, shift, fileName)
        {
            FullDeclarated = false;
            P1 = new Expr.Empty();
            P2 = new Expr.Empty();
        }

        public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
        {
            return visitor.VisitLinesExpr(this, scope);
        }
    }

    //Segmentos
    public class Segment : Expr
    {
        public Expr P1 { get; private set; }
        public Expr P2 { get; private set; }
        public bool FullDeclarated { get; private set; }


        public Segment(int line, int shift, char[] fileName, Expr _p1, Expr _p2) : base(line, shift, fileName)
        {
            P1 = _p1;
            P2 = _p2;
            FullDeclarated = true;

        }
        public Segment(int line, int shift, char[] fileName) : base(line, shift, fileName)
        {
            P1 = new Expr.Empty();
            P2 = new Expr.Empty();
            FullDeclarated = false;

        }
        public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
        {
            return visitor.VisitSegmentExpr(this, scope);
        }
    }

    //Semirrecta
    public class Ray : Expr
    {
        public Expr P1 { get; private set; }
        public Expr P2 { get; private set; }
        public bool FullDeclarated { get; private set; }

        public Ray(int line, int shift, char[] fileName, Expr _p1, Expr _p2) : base(line, shift, fileName)
        {
            P1 = _p1;
            P2 = _p2;
            FullDeclarated = true;

        }
        public Ray(int line, int shift, char[] fileName) : base(line, shift, fileName)
        {
            P1 = new Expr.Empty();
            P2 = new Expr.Empty();
            FullDeclarated = false;

        }
        public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
        {
            return visitor.VisitRayExpr(this, scope);
        }
    }

    //Circulo
    public class Circle : Expr
    {
        public Expr P1 { get; private set; }
        public Expr Radius { get; private set; }
        public bool FullDeclarated { get; private set; }
        public Circle(int line, int shift, char[] fileName, Expr _p1, Expr radius) : base(line, shift, fileName)
        {
            P1 = _p1;
            Radius = radius;
            FullDeclarated = true;
        }
        public Circle(int line, int shift, char[] fileName) : base(line, shift, fileName)
        {
            FullDeclarated = false;
            P1 = new Expr.Empty();
            Radius = new Expr.Empty();
        }
        public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
        {
            return visitor.VisitCircleExpr(this, scope);
        }
    }

    //Arco
    public class Arc : Expr
    {
        public Expr P1 { get; private set; }
        public Expr P2 { get; private set; }
        public Expr P3 { get; private set; }
        public Expr Radius { get; private set; }
        public bool FullDeclarated { get; private set; }
        public Arc(int _line, int _shift, char[] fileName, Expr _p1, Expr _p2, Expr _p3, Expr radius) : base(_line, _shift, fileName)
        {

            P1 = _p1;
            P2 = _p2;
            P3 = _p3;
            Radius = radius;
            FullDeclarated = true;

        }
        public Arc(int _line, int _shift, char[] fileName) : base(_line, _shift, fileName)
        {

            P1 = new Expr.Empty();
            P2 = new Expr.Empty();
            P3 = new Expr.Empty();
            Radius = new Expr.Empty();

            FullDeclarated = false;

        }
        public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
        {
            return visitor.VisitArcExpr(this, scope);
        }
    }

    public class Intersect : Expr
    {
        public Expr FirstExpression { get; private set; }
        public Expr SecondExpression { get; private set; }
        public Intersect(int _line, int _shift, char[] fileName, Expr first, Expr second) : base(_line, _shift, fileName)
        {
            FirstExpression = first;
            SecondExpression = second;
        }
        public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
        {
            return visitor.VisitIntersectExpr(this, scope);
        }
    }

    public class Count : Expr
    {
        public new Expr Sequence { get; private set; }
        public Count(int line, int Shift, char[] fileName, Expr sequence) : base(line, Shift, fileName)
        {
            Sequence = sequence;
        }
        public override T Accept<T>(IVisitorExpr<T> visitor, Scope scope)
        {
            return visitor.VisitCountExpr(this, scope);
        }
    }
    public class Randoms : Expr
    {
        public Randoms(Token randomToken) : base(randomToken.Line, randomToken.Shift, randomToken.Showfile) { }
        public override T Accept<T>(IVisitorExpr<T> visitor, Scope scope)
        {
            return visitor.VisitRandomsExpr(this, scope);
        }
    }
    public class Samples : Expr
    {
        public Samples(Token samplesToken) : base(samplesToken.Line, samplesToken.Shift, samplesToken.Showfile) { }
        public override T Accept<T>(IVisitorExpr<T> visitor, Scope scope)
        {
            return visitor.VisitSamplesExpr(this, scope);
        }
    }

    public class Sequence : Expr, IEnumerable<Expr>
    {
        public Expr First { get => Exprs[0]; }
        public Expr Second { get => Exprs[1]; }
        public bool HasTreeDots { get; private set; }
        public List<Expr> Exprs { get; private set; }
        public bool IsEmpty { get => Exprs.Count == 0; }
        public int Count => Exprs.Count;

        public Sequence(int line, int shift, char[] fileName, bool hasTreeDots, List<Expr> sequence) : base(line, shift, fileName)
        {
            HasTreeDots = hasTreeDots;
            Exprs = sequence;
            if (HasTreeDots && (sequence.Count == 0 || sequence.Count > 2)) throw new Exception($"Tried to form a dotted sequence with {sequence.Count} elements");
        }
        public override S Accept<S>(IVisitorExpr<S> visitor, Scope scope)
        {
            return visitor.VisitSequenceExpr(this, scope);
        }

        IEnumerator<Expr> IEnumerable<Expr>.GetEnumerator()
        {
            return ((IEnumerable<Expr>)Exprs).GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Exprs).GetEnumerator();
        }
    }
}