namespace Geo_Wall_E;
using System.Collections;
using Microsoft.VisualBasic;


interface IVisitorStatement<S>
{
    public S VisitEmptyStatement(Statement.Empty Statement, Scope scope);
    public S VisitPointStatement(Statement.Point Statement, Scope scope);
    public S VisitConstantDeclarationStatement(Statement.Declaration.Constant Statement, Scope scope);
    public S VisitFunctionDeclarationStatement(Statement.Declaration.Function Statement, Scope scope);
    public S VisitPrintStatement(Statement.Print Statement, Scope scope);
    public S VisitColorStatement(Statement.Color Statement, Scope scope);
    public S VisitDrawStatement(Statement.Draw Statement, Scope scope);
    public S VisitStatementList(Statement.StatementList Statement, Scope scope);
    public S VisitLinesStatement(Statement.Lines Statement, Scope scope);
    public S VisitSegmentStatement(Statement.Segment Statement, Scope scope);
    public S VisitRayStatement(Statement.Ray Statement, Scope scope);
    public S VisitEvalStatement(Statement.Eval Statement, Scope scope);
    public S VisitCircleStatement(Statement.Circle Statement, Scope scope);
    public S VisitArcStatement(Statement.Arc Statement, Scope scope);
    public S VisitMatchStatement(Statement.Declaration.Match Statement, Scope scope);
}

interface IVisitableStatement
{
    public S Accept<S>(IVisitorStatement<S> visitor, Scope scope);
}

abstract class Statement : IVisitableStatement, IGetError
{

    public int Line { get; private set; }
    public int Shift { get; private set; }
    public string File { get => new string(fileName); }
    private char[] fileName;
    protected Statement(int tline, int tshift, char[] fileName)
    {
        Line = tline;
        Shift = tshift;
        this.fileName = fileName;
    }

    abstract public S Accept<S>(IVisitorStatement<S> visitor, Scope scope);

    public class Empty : Statement
    {
        public Empty() : base(0, 0, new char[] { 'E', 'M', 'P', 'T', 'Y' }) { }
        public override S Accept<S>(IVisitorStatement<S> visitor, Scope scope)
        {
            return visitor.VisitEmptyStatement(this, scope);
        }
    }

    static public Statement.Empty EMPTY = new Statement.Empty();

    public class Point : Statement
    {
        public Token Id { get; private set; }
        public Object.String Comment { get; set; }

        public Expr X { get; private set; }
        public Expr Y { get; private set; }
        public bool FullDeclarated { get; private set; }
        public Point(int _line, int _shift, char[] fileName, Token _id, Expr _x, Expr _y, Object.String _comment) : base(_line, _shift, fileName)
        {
            Id = _id;
            X = _x;
            Y = _y;
            Comment = _comment;
            FullDeclarated = true;
        }
        public Point(int _line, int _shift, char[] fileName, Token _id) : base(_line, _shift, fileName)
        {
            X = Expr.EMPTY;
            Y = Expr.EMPTY;
            Comment = Object.STRING;
            Id = _id;
            FullDeclarated = false;
        }

        public override S Accept<S>(IVisitorStatement<S> visitor, Scope scope)
        {
            return visitor.VisitPointStatement(this, scope);
        }
    }


    public class Lines : Statement
    {
        public Token Id { get; protected set; }
        public Object.String Comment { get; protected set; }
        public Expr P1 { get; protected set; }
        public Expr P2 { get; protected set; }
        public bool FullDeclarated { get; protected set; }
        public Lines(int _line, int _shift, char[] fileName, Token _id, Expr _p1, Expr _p2, Object.String _comment) : base(_line, _shift, fileName)
        {
            Id = _id;
            P1 = _p1;
            P2 = _p2;
            Comment = _comment;
            FullDeclarated = true;
        }
        public Lines(int _line, int _shift, char[] fileName, Token _id) : base(_line, _shift, fileName)
        {
            Id = _id;
            P1 = new Expr.Empty();
            P2 = new Expr.Empty();
            FullDeclarated = false;
        }

        public override S Accept<S>(IVisitorStatement<S> visitor, Scope scope)
        {
            return visitor.VisitLinesStatement(this, scope);
        }
    }

    public class Segment : Lines
    {
        public Segment(int _line, int _shift, char[] fileName, Token _id, Expr _p1, Expr _p2, Object.String _comment) : base(_line, _shift, fileName, _id, _p1, _p2, _comment)
        {
            FullDeclarated = true;

        }
        public Segment(int _line, int _shift, char[] fileName, Token _id) : base(_line, _shift, fileName, _id)
        {
            Id = _id;
            FullDeclarated = false;
            P1 = new Expr.Empty();
            P2 = new Expr.Empty();

        }
        public override S Accept<S>(IVisitorStatement<S> visitor, Scope scope)
        {
            return visitor.VisitSegmentStatement(this, scope);
        }
    }

    public class Ray : Lines
    {
        public Ray(int _line, int _shift, char[] fileName, Token _id, Expr _p1, Expr _p2, Object.String _comment) : base(_line, _shift, fileName, _id, _p1, _p2, _comment)
        {
            FullDeclarated = true;
        }
        public Ray(int _line, int _shift, char[] fileName, Token _id) : base(_line, _shift, fileName, _id)
        {
            Id = _id;
            FullDeclarated = false;
            P1 = new Expr.Empty();
            P2 = new Expr.Empty();
        }
        public override S Accept<S>(IVisitorStatement<S> visitor, Scope scope)
        {
            return visitor.VisitRayStatement(this, scope);
        }
    }

    public class Circle : Statement
    {
        public Token Id { get; private set; }
        public Object.String Comment { get; private set; }

        public Expr P1 { get; private set; }
        public Expr Radius { get; private set; }
        public bool FullDeclarated { get; private set; }
        public Circle(int _line, int _shift, char[] fileName, Token _id, Expr _p1, Expr radius, Object.String _comment) : base(_line, _shift, fileName)
        {
            Id = _id;
            P1 = _p1;
            Radius = radius;
            Comment = _comment;
            FullDeclarated = true;
        }
        public Circle(int _line, int _shift, char[] fileName, Token _id) : base(_line, _shift, fileName)
        {
            Id = _id;
            P1 = new Expr.Empty();
            Radius = new Expr.Empty();
            FullDeclarated = false;

        }

        public override S Accept<S>(IVisitorStatement<S> visitor, Scope scope)
        {
            return visitor.VisitCircleStatement(this, scope);
        }
    }

    public class Arc : Statement
    {
        public Token Id { get; private set; }
        public Object.String Comment { get; private set; }

        public Expr P1 { get; private set; }
        public Expr P2 { get; private set; }
        public Expr P3 { get; private set; }
        public Expr Radius { get; private set; }
        public bool FullDeclarated { get; private set; }
        public Arc(int _line, int _shift, char[] fileName, Token _id, Expr _p1, Expr _p2, Expr _p3, Expr radius, Object.String _comment) : base(_line, _shift, fileName)
        {
            Id = _id;
            P1 = _p1;
            P2 = _p2;
            P3 = _p3;
            Radius = radius;
            Comment = _comment;
            FullDeclarated = true;
        }
        public Arc(int _line, int _shift, char[] fileName, Token _id) : base(_line, _shift, fileName)
        {
            Id = _id;
            P1 = new Expr.Empty();
            P2 = new Expr.Empty();
            P3 = new Expr.Empty();
            Radius = new Expr.Empty();
            FullDeclarated = false;

        }

        public override S Accept<S>(IVisitorStatement<S> visitor, Scope scope)
        {
            return visitor.VisitArcStatement(this, scope);
        }
    }


    public abstract class Declaration : Statement
    {
        public Token Id { get; protected set; }
        protected Declaration(Token identifier, char[] fileName) : base(identifier.Line, identifier.Shift, fileName)
        {
            Id = identifier;
        }

        abstract public override S Accept<S>(IVisitorStatement<S> visitor, Scope scope);
        public class Constant : Declaration
        {
            public Expr RValue { get; private set; }
            public Constant(Token identifier, char[] fileName, Expr rvalue) : base(identifier, fileName)
            {
                RValue = rvalue;
            }
            public override S Accept<S>(IVisitorStatement<S> visitor, Scope scope)
            {
                return visitor.VisitConstantDeclarationStatement(this, scope);
            }
        }
        public class Function : Declaration
        {
            public List<Token> Arguments { get; private set; }
            public Expr Body { get; private set; }
            public Function(Token identifier, char[] fileName, List<Token> arguments, Expr body) : base(identifier, fileName)
            {
                Arguments = arguments;
                Body = body;
            }
            public override S Accept<S>(IVisitorStatement<S> visitor, Scope scope)
            {
                return visitor.VisitFunctionDeclarationStatement(this, scope);
            }
            public int Arity { get => Arguments.Count; }
        }
        
        public class Match : Declaration
        {
        public Expr Sequence {get; private set;}
            public List<Token> Identifiers {get; private set;}
            public Match(int line,int offset,char[] fileName,List<Token> identifiers,Expr sequence):base(identifiers[0],identifiers[0].Showfile){
                Identifiers = identifiers;
                Sequence = sequence;
            }
            public override T Accept<T>(IVisitorStatement<T> visitor, Scope scope)
            {
                return visitor.VisitMatchStatement(this,scope);
            }       
        }
    }

    public class Draw : Statement
    {
        public Expr _Expr { get; private set; }
        public Object.String Comment { get; private set; }

        public Draw(int line, int shift, char[] fileName, Expr _expr, Object.String comment) : base(line, shift, fileName)
        {
            _Expr = _expr;
            Comment = comment;
        }

        public override S Accept<S>(IVisitorStatement<S> visitor, Scope scope)
        {
            return visitor.VisitDrawStatement(this, scope);
        }
    }

    public class Eval : Statement
    {
        public Expr Expr { get; private set; }
        public Eval(int line, int shift, char[] fileName, Expr expr) : base(line, shift, fileName)
        {
            Expr = expr;
        }
        public override S Accept<S>(IVisitorStatement<S> visitor, Scope scope)
        {
            return visitor.VisitEvalStatement(this, scope);
        }
    }

    public class Print : Statement
    {
        public Expr _Expr { get; private set; }
        public Print(int _line, int _shift, char[] fileName, Expr _expr) : base(_line, _shift, fileName)
        {
            _Expr = _expr;
        }
        public override S Accept<S>(IVisitorStatement<S> visitor, Scope scope)
        {
            return visitor.VisitPrintStatement(this, scope);
        }
    }

    public class Color : Statement
    {
        public Colors _Color { get; private set; }
        public bool IsRestore { get; private set; }
        public Color(int line, int shift, char[] fileName, Colors _color, bool _restore = false) : base(line, shift, fileName)
        {
            _Color = _color;
            IsRestore = _restore;
        }

        public override S Accept<S>(IVisitorStatement<S> visitor, Scope scope)
        {
            return visitor.VisitColorStatement(this, scope);
        }
    }

    public class StatementList : Statement, ICollection<Statement>
    {
        private List<Statement> stmts;
        public StatementList(int line, int offset, char[] fileName) : base(line, offset, fileName)
        {
            this.stmts = new List<Statement>();
        }
        public override S Accept<S>(IVisitorStatement<S> visitor, Scope scope)
        {
            return visitor.VisitStatementList(this, scope);
        }
        public IEnumerator<Statement> GetEnumerator()
        {
            return stmts.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public int Count { get => stmts.Count; }

        public void Add(Statement stmt)
        {
            stmts.Add(stmt);
        }
        public void Clear()
        {
            stmts.Clear();
        }
        public bool Contains(Statement stmt)
        {
            return stmts.Contains(stmt);
        }
        public bool Remove(Statement stmt)
        {
            return stmts.Remove(stmt);
        }
        public bool IsReadOnly { get => false; }
        public void CopyTo(Statement[] stmt, int arrayIndex)
        {
            stmts.CopyTo(stmt, arrayIndex);
        }
    }
}
