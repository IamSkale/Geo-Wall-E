using System.Drawing;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Collections;
using System.Drawing.Design;

namespace Geo_Wall_E;

//Declaracion de objetos del compilador

//Tipos de Objetos
public enum ObjectsTypes
{
    UNDEFINED,
    RUNTIME_DEFINED,
    NUMBER,
    STRING,
    POINT,
    LINE,
    SEGMENT,
    RAY,
    CIRCLE,
    ARC,
    MEASURE,
    SEQUENCE,
    FUNCTION,
    FUNCTION_LIST,
}


public interface IDraw
{
    public ObjectsTypes Type { get; }
    public Colors Colors { get; set; }
    public Object.String Text { get; set; }
}

//Declaracion base de todos los objetos del conpilador
public abstract class Object
{
    //Declaracion arbitraria del tipo del objeto
    public ObjectsTypes Type { get; private set; }


    //Declaracion de objetos constantes
    public static Object.Undefined UNDEFINED = new Object.Undefined();
    public static Object.Runtimedefined RUNTIME_DEFINED = new Object.Runtimedefined();
    public static Object.Measure MEASURE = new Object.Measure(1);
    public static Object.Number NUMBER = new Object.Number(1);
    public static Object.String STRING = new Object.String("");
    public static Object.Point POINT = new Object.Point(STRING, NUMBER, NUMBER, STRING, Colors.BLACK);
    public static Object.Line LINE = new Object.Line(STRING, POINT, POINT, STRING, Colors.BLACK);
    public static Object.Segment SEGMENT = new Object.Segment(STRING, POINT, POINT, STRING, Colors.BLACK);
    public static Object.Ray RAY = new Object.Ray(STRING, POINT, POINT, STRING, Colors.BLACK);
    public static Object.Circle CIRCLE = new Object.Circle(STRING, POINT, MEASURE, STRING, Colors.BLACK);
    public static Object.Arc ARC = new Object.Arc(STRING, POINT, POINT, POINT, MEASURE, STRING, Colors.BLACK);
    public static Object.Sequence SEQUENCE = new Object.Sequence.Listing(new List<Object>());


    //Representacion numerica de booleanos
    public static Object.Number TRUE = new Object.Number(1);
    public static Object.Number FALSE = new Object.Number(0);

    protected Object(ObjectsTypes type)
    {
        Type = type;
    }

    //Igualdad
    public abstract Object.Number EqualTo(Object arbitrary);

    public Object.Number NotEqual(Object i)
    {
        if (this.EqualTo(i) == Object.TRUE)
        {
            return Object.FALSE;
        }
        return Object.TRUE;
    }


    //Objetos de tipo Indefinido
    public class Undefined : Object
    {
        public Undefined() : base(ObjectsTypes.UNDEFINED)
        { }

        public override Number EqualTo(Object arbitrary)
        {
            if (arbitrary.Type == this.Type)
            {
                return TRUE;
            }
            return FALSE;
        }
    }

    public class Runtimedefined : Object
    {
        public Runtimedefined() : base(ObjectsTypes.RUNTIME_DEFINED)
        { }

        public override Number EqualTo(Object arbitrary)
        {
            throw new InvalidOperationException($"Not defined operation for{this.Type}");
        }
    }

    //Numeros
    public class Number : Object
    {
        float value;

        public float Value
        {
            get
            {
                return value;
            }
        }

        public Number(float tvalue) : base(ObjectsTypes.NUMBER)
        {
            value = tvalue;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public override Number EqualTo(Object arbitrary)
        {
            if (arbitrary.Type != this.Type)
            {
                return Object.FALSE;
            }
            if (Resources.Compare(this.Value, (arbitrary as Object.Number)!.Value) == 0)
            {
                return Object.TRUE;
            }
            return Object.FALSE;
        }
    }

    //Strings
    public class String : Object
    {
        string value;
        public string Value
        {
            get
            {
                return value;
            }
        }
        public String(string tvalue = "") : base(ObjectsTypes.STRING)
        {
            value = tvalue;
        }
        public override string ToString()
        {
            return value;
        }
        public override Number EqualTo(Object arbitrary)
        {
            if (arbitrary.Type != this.Type)
            {
                return Object.FALSE;
            }
            if (((Object.String)arbitrary).value == this.value)
            {
                return Object.TRUE;
            }
            return Object.FALSE;
        }
    }

    //Medicion
    public class Measure : Object
    {
        public float Value { get; private set; }

        //Asegurarnos de q la distancia sea un valor positivo
        public Measure(float value) : base(ObjectsTypes.MEASURE)
        {
            Value = Math.Abs(value);
        }

        public Measure() : base(ObjectsTypes.MEASURE)
        {
            Value = Math.Abs(Resources.RandomCoordinates());
        }

        public override Number EqualTo(Object arbitrary)
        {
            if (arbitrary.Type != this.Type)
            {
                return Object.FALSE;
            }
            if (Resources.Compare(this.Value, (arbitrary as Object.Measure)!.Value) == 0)
            {
                return Object.TRUE;
            }
            return Object.FALSE;
        }

        public override string ToString()
        {
            return Value.ToString() + 'u';
        }
    }

    //Punto
    public class Point : Object, IDraw
    {
        public Object.String Id;
        public Object.String Text { get; set; }
        public Object.Number x;
        public Object.Number y;

        public Colors Colors { get; set; }

        public Point(Object.String tId, Object.Number tx, Object.Number ty, Object.String tText, Colors color) : base(ObjectsTypes.POINT)
        {
            Id = tId;
            Text = tText;
            x = tx;
            y = ty;
            Colors = color;
        }

        public Point() : base(ObjectsTypes.POINT)
        {
            Id = new String();
            Text = new String();
            x = new Object.Number(Resources.RandomCoordinates());
            y = new Object.Number(Resources.RandomCoordinates());
            Colors = Colors.BLACK;
        }

        public Point(Colors color) : base(ObjectsTypes.POINT)
        {
            Id = new String();
            Text = new String();
            x = new Object.Number(Resources.RandomCoordinates());
            y = new Object.Number(Resources.RandomCoordinates());
            Colors = color;
        }

        public override string ToString()
        {
            return $"{Id} ({x},{y}) {Text}";
        }

        public override Number EqualTo(Object arbitrary)
        {
            if (arbitrary.Type != this.Type)
            {
                return Object.FALSE;
            }
            if (((Object.Point)arbitrary).x == this.x && ((Object.Point)arbitrary).y == this.y)
            {
                return Object.TRUE;
            }
            return Object.FALSE;
        }

        public static Object.Measure DistanceBetweenPoints(Object.Point a, Object.Point b)
        {
            float xs = (IOperations.Operate("-", a.x, b.x) as Object.Number)!.Value;
            xs *= xs;

            float ys = (IOperations.Operate("-", a.y, b.y) as Object.Number)!.Value;
            ys *= ys;

            return new Object.Measure(float.Sqrt(xs + ys));
        }
    }

    //Linea
    public class Line : Object, IDraw
    {
        public Object.String Id;
        public Object.Point a;
        public Object.Point b;
        public Object.String Text { get; set; }

        public Colors Colors { get; set; }

        public Line(Object.String tId, Object.Point ta, Object.Point tb, Object.String tText, Colors color) : base(ObjectsTypes.LINE)
        {
            Id = tId;
            Text = tText;
            a = ta;
            b = tb;
            Colors = color;
        }

        public Line(Colors color) : base(ObjectsTypes.LINE)
        {
            Id = new String();
            Text = new String();
            a = new Object.Point();
            b = new Object.Point();
            Colors = color;
        }

        public override string ToString()
        {
            return $"{Id} (({a.x},{a.y}),({b.x},{b.y})){Text}";
        }

        public override Number EqualTo(Object arbitrary)
        {
            if (arbitrary.Type != this.Type)
            {
                return Object.FALSE;
            }
            if (((Object.Line)arbitrary).a == this.a && ((Object.Line)arbitrary).b == this.b)
            {
                return Object.TRUE;
            }
            return Object.FALSE;
        }
    }

    //Segmento
    public class Segment : Object, IDraw
    {
        public Object.String Id;
        public Object.String Text { get; set; }
        public Object.Point a;
        public Object.Point b;

        public Colors Colors { get; set; }

        public Segment(Object.String tId, Object.Point ta, Object.Point tb, Object.String tText, Colors color) : base(ObjectsTypes.SEGMENT)
        {
            Id = tId;
            Text = tText;
            a = ta;
            b = tb;
            Colors = color;
        }

        public Segment(Colors color) : base(ObjectsTypes.SEGMENT)
        {
            Id = new String();
            Text = new String();
            a = new Object.Point();
            b = new Object.Point();
            Colors = color;
        }

        public override string ToString()
        {
            return $"{Id}(({a.x},{a.y}),({b.x},{b.y})){Text}";
        }

        public override Number EqualTo(Object arbitrary)
        {
            if (arbitrary.Type != this.Type)
            {
                return Object.FALSE;
            }
            if (((Object.Segment)arbitrary).a == this.a && ((Object.Segment)arbitrary).b == this.b)
            {
                return Object.TRUE;
            }
            return Object.FALSE;
        }
    }

    //Semirrecta
    public class Ray : Object, IDraw
    {
        public Object.String Id;
        public Object.String Text { get; set; }
        public Object.Point a;
        public Object.Point b;

        public Colors Colors { get; set; }

        public Ray(Object.String tId, Object.Point ta, Object.Point tb, Object.String tText, Colors color) : base(ObjectsTypes.RAY)
        {
            Id = tId;
            Text = tText;
            a = ta;
            b = tb;
            Colors = color;
        }
        public Ray(Colors color) : base(ObjectsTypes.RAY)
        {
            Id = new String();
            Text = new String();
            a = new Object.Point();
            b = new Object.Point();
            Colors = color;

        }

        public override string ToString()
        {
            return $"{Id}(({a.x},{a.y}),({b.x},{b.y})){Text}";
        }

        public override Number EqualTo(Object arbitrary)
        {
            if (arbitrary.Type != this.Type)
            {
                return Object.FALSE;
            }
            if (((Object.Ray)arbitrary).a == this.a && ((Object.Ray)arbitrary).b == this.b)
            {
                return Object.TRUE;
            }
            return Object.FALSE;
        }
    }

    //Circulo
    public class Circle : Object, IDraw
    {
        public Object.String Id;
        public Object.Point o;
        public Object.Measure radius;
        public Object.String Text { get; set; }

        public Colors Colors { get; set; }

        public Circle(Object.String tId, Object.Point to, Object.Measure tradius, Object.String tText, Colors color) : base(ObjectsTypes.CIRCLE)
        {
            Id = tId;
            Text = tText;
            o = to;
            radius = tradius;
            Colors = color;
        }
        public Circle(Colors color) : base(ObjectsTypes.CIRCLE)
        {
            o = new Object.Point();
            Id = new String();
            Text = new String();
            radius = new Object.Measure();
            Colors = color;

        }

        public override string ToString()
        {
            return $"{Id}({o.x},{o.y}){radius}{Text}";
        }

        public override Number EqualTo(Object arbitrary)
        {
            if (arbitrary.Type != this.Type)
            {
                return Object.FALSE;
            }
            if (((Object.Circle)arbitrary).o == this.o && ((Object.Circle)arbitrary).radius == this.radius)
            {
                return Object.TRUE;
            }
            return Object.FALSE;
        }
    }

    //Arco
    public class Arc : Object, IDraw
    {
        public Object.String name;
        public Object.String Text { get; set; }
        public Object.Point a;
        public Object.Point b;
        public Object.Point c;
        public Object.Measure radius;

        public Colors Colors { get; set; }

        public Arc(Object.String _name, Object.Point _p1, Object.Point _p2, Object.Point _p3, Object.Measure _radius, Object.String _Text, Colors color) : base(ObjectsTypes.ARC)
        {
            name = _name;
            Text = _Text;
            a = _p1;
            b = _p2;
            c = _p3;
            radius = _radius;
            Colors = color;
        }
        public Arc(Colors color) : base(ObjectsTypes.ARC)
        {
            a = new Object.Point();
            b = new Object.Point();
            c = new Object.Point();
            radius = new Object.Measure();
            name = new String();
            Text = new String();
            Colors = color;
        }
        public override string ToString()
        {
            return $"{name}({a.x},{a.y})({b.x},{b.y})({c.x},{c.y}){radius}{Text}";
        }
        public override Number EqualTo(Object other)
        {
            if (other.Type != this.Type) return Object.FALSE;
            if (((Object.Arc)other).a == this.a && ((Object.Arc)other).b == this.b && ((Object.Arc)other).c == this.c && ((Object.Arc)other).radius == this.radius) return Object.TRUE;
            return Object.FALSE;
        }
    }

    //Funcion
    internal class Function : Object
    {

        public List<Object.String> Arguments { get; private set; }
        public Expr Body { get; private set; }
        public Function(List<Token> arguments, Expr body) : base(ObjectsTypes.FUNCTION)
        {
            Body = body;
            Arguments = new List<Object.String>(arguments.Count);
            foreach (Token identifier in arguments) Arguments.Add(new Object.String(identifier.TryLexer));
        }
        public override Number EqualTo(Object other)
        {
            throw new NotImplementedException("Equality isnt valid on functions");
        }

        public int Arity { get => Arguments.Count; }

        public static Object.Function MakeFunction(int arity)
        {
            if (arity < 0) throw new ArgumentException("Arity must be a non-negative integer");
            List<Token> arguments = new List<Token>(arity);
            for (int i = 0; i < arity; ++i) arguments.Add(DUMMY);
            return new Object.Function(arguments, Expr.EMPTY);
        }

        public static Object.Function MakeFunction(Statement.Declaration.Function funcDecl)
        {
            return new Object.Function(funcDecl.Arguments, funcDecl.Body);
        }

        //Para crear funciones que no seran usadas
        private static Token DUMMY = new Token(TokenTypes.END, "", null, -1, -1, new char[] { 'D', 'U', 'M', 'M', 'Y' });
    }

    //Familia de funciones
    internal class FunctionList : Object, IList<Object.Function>
    {
        public List<Object.Function> functions;
        public FunctionList() : base(ObjectsTypes.FUNCTION_LIST)
        {
            functions = new List<Object.Function>();
        }

        public bool ContainsArity(int arity)
        {
            if (arity < 0) throw new ArgumentException("Arity must be non-negative");
            foreach (Object.Function function in functions) if (function.Arity == arity) return true;
            return false;
        }
        public Function GetFunction(int arity)
        {
            if (arity < 0) throw new ArgumentException("Arity must be non-negative");
            foreach (Object.Function function in functions) if (function.Arity == arity) return function;
            throw new InvalidOperationException("Cant find a function with the provided arity");
        }
        public Function this[int index] { get => functions[index]; set => functions[index] = value; }

        public int Count => functions.Count;

        public bool IsReadOnly => ((ICollection<Function>)functions).IsReadOnly;

        public void Add(Function item)
        {
            functions.Add(item);
        }

        public void Clear()
        {
            functions.Clear();
        }

        public bool Contains(Function item)
        {
            return functions.Contains(item);
        }

        public void CopyTo(Function[] array, int arrayIndex)
        {
            functions.CopyTo(array, arrayIndex);
        }

        public override Number EqualTo(Object other)
        {
            throw new NotImplementedException("Equality isnt valid in FunctionList");
        }

        public IEnumerator<Function> GetEnumerator()
        {
            return functions.GetEnumerator();
        }

        public int IndexOf(Function item)
        {
            return functions.IndexOf(item);
        }

        public void Insert(int index, Function item)
        {
            functions.Insert(index, item);
        }

        public bool Remove(Function item)
        {
            return functions.Remove(item);
        }

        public void RemoveAt(int index)
        {
            functions.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return functions.GetEnumerator();
        }
    }

    //Secuencias
    public abstract class Sequence : Object, IEnumerable<Object>
    {
        protected Sequence() : base(ObjectsTypes.SEQUENCE) { }
        public abstract Object Count { get; }
        public abstract bool IsFinite { get; }
        public abstract bool IsEmpty { get; }
        public abstract IEnumerator<Object> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public abstract class SequenceEnumerator : IEnumerator<Object>
        {
            public abstract Object Current { get; }
            object IEnumerator.Current => Current;
            public abstract void Dispose();
            public abstract bool MoveNext();
            public abstract void Reset();
            public abstract Object.Sequence Resto { get; }
        }

        public class Interval : Sequence
        {
            public float Min { get; private set; }
            public float Max { get; private set; }
            public override bool IsFinite => !float.IsPositiveInfinity(Max);
            public override bool IsEmpty => false;
            public override Number EqualTo(Object other)
            {
                throw new NotImplementedException();
            }
            public override Object Count
            {
                get
                {
                    if (IsFinite) return new Object.Number(Max - Min + 1);
                    return Object.UNDEFINED;
                }
            }
            public Interval(float min, float max = float.PositiveInfinity)
            {
                if (float.IsInfinity(min)) throw new Exception("Cannot create a interval with infite minimum value");
                if (float.IsNegativeInfinity(max)) throw new Exception("Maximum cannot be negative infinity");
                if (min > max) throw new Exception("Interval order is exchanged");
                Min = float.Truncate(min);
                Max = float.Truncate(max);
            }
            public override string ToString()
            {
                if (!IsFinite) return "Infinite Sequence";
                System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
                stringBuilder.Append('{');
                for (float i = Min; i <= Max; ++i)
                {
                    stringBuilder.Append(i);
                    if (i < Max) stringBuilder.Append(',');
                }
                stringBuilder.Append('}');
                return stringBuilder.ToString();
            }
            public override IEnumerator<Object> GetEnumerator() => new IntervalEnumerator(Min, Max, IsFinite);
            class IntervalEnumerator : SequenceEnumerator
            {
                float Min;
                float Max;
                float current;
                bool NCurrent;
                bool Finite;
                public IntervalEnumerator(float min, float max, bool finite)
                {
                    Min = min;
                    Max = max;
                    Finite = finite;
                    Reset();
                }

                override public Object Current
                {
                    get
                    {
                        if (NCurrent)
                        {
                            return new Object.Number(current);
                        }
                        throw new InvalidOperationException("The enumerator is empty");
                    }
                }

                override public void Dispose()
                { }

                public override bool MoveNext()
                {
                    if (Resources.Compare(current + 1, Max) <= 0)
                    {
                        ++current;
                        NCurrent = true;
                    }
                    else
                    {
                        NCurrent = false;
                    }
                    return NCurrent;
                }

                public override void Reset()
                {
                    current = Min - 1;
                    NCurrent = false;
                }

                public override Sequence Resto
                {
                    get
                    {
                        if (Finite)
                        {
                            return new Sequence.Interval(Min, Max);
                        }
                        return new Sequence.Interval(current + 1);
                    }
                }
            }
        }

        //Secuencia finita
        public class Listing : Sequence
        {
            List<Object> sequence;
            public Listing(List<Object> Objects)
            {
                sequence = Objects;
            }
            public override bool IsFinite => true;
            public override bool IsEmpty => sequence.Count == 0;
            public override Object.Number Count => new Object.Number(sequence.Count);
            public override Number EqualTo(Object other)
            {
                throw new NotImplementedException();
            }
            public override IEnumerator<Object> GetEnumerator()
            {
                return new ListingEnumerator(sequence);
            }
            public override string ToString()
            {
                System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
                stringBuilder.Append('{');
                for (int i = 0; i < sequence.Count; ++i)
                {
                    stringBuilder.Append(sequence[i].ToString());
                    if (i < sequence.Count - 1) stringBuilder.Append(',');
                }
                stringBuilder.Append('}');
                return stringBuilder.ToString();
            }

            class ListingEnumerator : SequenceEnumerator
            {
                List<Object> sequence;
                bool NCurrent;
                int actual;
                public ListingEnumerator(List<Object> sequence)
                {
                    this.sequence = sequence;
                    Reset();
                }
                public override void Reset()
                {
                    NCurrent = false;
                    actual = -1;
                }
                public override Object Current
                {
                    get
                    {
                        if (NCurrent)
                        {
                            return sequence[actual];
                        }
                        throw new InvalidOperationException("The enumerator doesnt have remaining Objects.");
                    }
                }
                public override void Dispose() { }
                public override bool MoveNext()
                {
                    if (actual + 1 < sequence.Count)
                    {
                        ++actual;
                        NCurrent = true;
                    }
                    else NCurrent = false;
                    return NCurrent;
                }
                public override Sequence Resto
                {
                    get
                    {
                        List<Object> resto = new List<Object>();
                        for (int i = actual + 1; i < sequence.Count; ++i) resto.Add(sequence[i]);
                        return new Object.Sequence.Listing(resto);
                    }
                }
            }
        }

        public class Randoms : Sequence
        {
            private int seed;
            public Randoms()
            {
                seed = new Random().Next();
            }
            public override Object Count => Object.UNDEFINED;
            public override bool IsFinite => true;
            public override Number EqualTo(Object other)
            {
                throw new NotImplementedException();
            }
            public override bool IsEmpty => false;
            public override IEnumerator<Object> GetEnumerator() => new RandomsEnumerator(seed);

            class RandomsEnumerator : SequenceEnumerator
            {
                Random random;
                int seed;
                public RandomsEnumerator(int seed)
                {
                    this.seed = seed;
                    random = new Random(seed);
                }
                public override Object Current => new Object.Number(random.NextSingle());
                public override void Dispose() { }
                public override void Reset() => random = new Random(seed);
                public override bool MoveNext() => true;
                public override Sequence Resto => throw new Exception("Cannot obtain a rest from randoms");
            }
        }

        public class Samples : Sequence
        {
            private int seed;
            public Samples()
            {
                seed = new Random().Next();
            }
            public override Object Count => Object.UNDEFINED;
            public override bool IsFinite => true;
            public override Number EqualTo(Object other)
            {
                throw new NotImplementedException();
            }
            public override bool IsEmpty => false;
            public override IEnumerator<Object> GetEnumerator() => new SamplesEnumerator(seed);

            class SamplesEnumerator : SequenceEnumerator
            {
                Random random;
                int seed;
                public SamplesEnumerator(int seed)
                {
                    this.seed = seed;
                    random = new Random(seed);
                }
                public override Object Current => Resources.RandomPoint(random);
                public override void Dispose() { }
                public override void Reset() => random = new Random(seed);
                public override bool MoveNext() => true;
                public override Sequence Resto => throw new Exception("Cannot obtain a rest from samples");
            }
        }

        public class CompositeSequence : Sequence
        {
            private List<Sequence> sequences;
            public CompositeSequence(List<Sequence> sequences)
            {
                this.sequences = sequences;
            }
            public CompositeSequence(params Sequence[] sequences)
            {
                this.sequences = new List<Sequence>(sequences);
            }
            public override bool IsEmpty
            {
                get
                {
                    foreach (Sequence seq in sequences) if (!seq.IsEmpty) return true;
                    return true;
                }
            }
            public override Object Count
            {
                get
                {
                    float count = 0;
                    foreach (Sequence seq in sequences)
                    {
                        if (!seq.IsFinite) return Object.UNDEFINED;
                        count += (seq.Count as Object.Number)!.Value;
                    }
                    return new Object.Number(count);
                }
            }
            public override bool IsFinite
            {
                get
                {
                    if (Count == Object.UNDEFINED) return false;
                    return true;
                }
            }
            public override string ToString()
            {
                if (!IsFinite) return "infinite sequence";
                List<Object> Objects = new List<Object>();
                foreach (Sequence sequence in sequences)
                {
                    foreach (Object Object in sequence)
                    {
                        Objects.Add(Object);
                    }
                }

                System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
                stringBuilder.Append('{');
                for (int i = 0; i < Objects.Count; ++i)
                {
                    stringBuilder.Append(Objects[i].ToString());
                    if (i < Objects.Count - 1) stringBuilder.Append(',');
                }
                stringBuilder.Append('}');
                return stringBuilder.ToString();
            }
            public override Number EqualTo(Object other)
            {
                throw new NotImplementedException();
            }
            public override IEnumerator<Object> GetEnumerator()
            {
                return new CompositeSequenceEnumerator(sequences);
            }
            class CompositeSequenceEnumerator : SequenceEnumerator
            {
                private List<SequenceEnumerator> enumerators;
                private bool hayCurrent;
                private Object current;
                public CompositeSequenceEnumerator(List<Sequence> sequences)
                {
                    enumerators = new List<SequenceEnumerator>(sequences.Count);
                    foreach (Sequence seq in sequences) enumerators.Add((seq.GetEnumerator() as SequenceEnumerator)!);
                    current = Object.UNDEFINED;
                    Reset();
                }
                public override void Reset()
                {
                    hayCurrent = false;
                    current = Object.UNDEFINED;
                }
                public override void Dispose() { }
                public override Sequence Resto
                {
                    get
                    {
                        List<Sequence> resto = new List<Sequence>();
                        foreach (SequenceEnumerator enumerator in enumerators) resto.Add(enumerator.Resto);
                        return new Sequence.CompositeSequence(resto);
                    }
                }
                public override bool MoveNext()
                {
                    foreach (SequenceEnumerator enumerator in enumerators)
                    {
                        if (enumerator.MoveNext())
                        {
                            hayCurrent = true;
                            current = enumerator.Current;
                            return true;
                        }
                    }
                    hayCurrent = false;
                    current = Object.UNDEFINED;
                    return false;
                }
                public override Object Current => current;
            }
        }
    }
}

