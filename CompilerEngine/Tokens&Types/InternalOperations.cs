using System.Linq.Expressions;

namespace Geo_Wall_E;

static class IOperations
{
    private static Dictionary<string, List<Delegate>> iOperations = new Dictionary<string, List<Delegate>>();

    //Guardar Operacines
    public static void SaveOperations(string operation, Delegate instructions)
    {
        if (!iOperations.ContainsKey(operation))
        {
            iOperations.Add(operation, new List<Delegate>());
        }
        iOperations[operation].Add(instructions);
    }

    //Declaracion generica de operaciones
    public static Object Operate(string operation, params Object[] param)
    {
        try
        {
            foreach (Delegate toperation in iOperations[operation])
            {
                try
                {
                    return (toperation.DynamicInvoke(param) as Object)!;
                }
                catch (System.Reflection.TargetParameterCountException)
                { }
                catch (ArgumentException)
                { }
                catch (InvalidCastException)
                { }
            }
            throw new InvalidOperationException("Invalid Operation");
        }
        catch (KeyNotFoundException)
        {
            throw new InvalidOperationException("Unrecognized Operation");
        }
    }

    //Operaciones basicas del compilador
    static IOperations()
    {
        //Suma
        SaveOperations("+", (Object.Number a, Object.Number b) => new Object.Number(a.Value + b.Value));
        SaveOperations("+", (Object.Measure a, Object.Measure b) => new Object.Measure(a.Value + b.Value));

        //Resta
        SaveOperations("-", (Object.Number a, Object.Number b) => new Object.Number(a.Value - b.Value));
        SaveOperations("-", (Object.Measure a, Object.Measure b) => new Object.Measure(float.Abs(a.Value - b.Value)));
        SaveOperations("-", (Object.Number a) => new Object.Number(-a.Value));

        //Multiplicacion
        SaveOperations("*", (Object.Number a, Object.Number b) => new Object.Number(a.Value * b.Value));
        SaveOperations("*", (Object.Measure a, Object.Number b) => new Object.Measure(a.Value * float.Abs(b.Value)));
        SaveOperations("*", (Object.Number a, Object.Measure b) => new Object.Measure(b.Value * float.Abs(a.Value)));

        //Division
        SaveOperations("/", (Object.Number a, Object.Number b) =>
        {
            if (Resources.Is0(b.Value))
            {
                throw new DivideByZeroException();
            }
            return new Object.Number(a.Value / b.Value);
        });
        SaveOperations("/", (Object.Measure a, Object.Measure b) =>
        {
            if (Resources.Is0(b.Value))
            {
                throw new DivideByZeroException();
            }
            return new Object.Number(a.Value / b.Value);
        });

        //Potencia
        SaveOperations("^", (Object.Number a, Object.Number b) => new Object.Number(float.Pow(a.Value, b.Value)));

        //Modulo
        SaveOperations("%", (Object.Number a, Object.Number b) =>
        {
            if (Resources.Is0(b.Value))
            {
                throw new DivideByZeroException();
            }
            return new Object.Number(a.Value % b.Value);
        });

        //Negacion
        SaveOperations("!", (Object a) =>
        {
            if (a is Object.Number && Resources.Is0((a as Object.Number)!.Value))
            {
                return Object.TRUE;
            }
            if (a is Object.Measure && Resources.Is0((a as Object.Measure)!.Value))
            {
                return Object.TRUE;
            }
            if (a is Object.Undefined)
            {
                return Object.TRUE;
            }
            return Object.FALSE;
        });

        //Menor que
        SaveOperations("<", (Object.Number a, Object.Number b) =>
        {
            if (Resources.Compare(a.Value, b.Value) < 0)
            {
                return Object.TRUE;
            }
            return Object.FALSE;
        });
        SaveOperations("<", (Object.Measure a, Object.Measure b) =>
        {
            if (Resources.Compare(a.Value, b.Value) < 0)
            {
                return Object.TRUE;
            }
            return Object.FALSE;
        });

        //Mayor que
        SaveOperations(">", (Object.Number a, Object.Number b) =>
        {
            if (Resources.Compare(a.Value, b.Value) > 0)
            {
                return Object.TRUE;
            }
            return Object.FALSE;
        });
        SaveOperations(">", (Object.Measure a, Object.Measure b) =>
        {
            if (Resources.Compare(a.Value, b.Value) > 0)
            {
                return Object.TRUE;
            }
            return Object.FALSE;
        });

        //Menor Igual que
        SaveOperations("<=", (Object.Number a, Object.Number b) =>
        {
            if (Resources.Compare(a.Value, b.Value) <= 0)
            {
                return Object.TRUE;
            }
            return Object.FALSE;
        });
        SaveOperations("<=", (Object.Measure a, Object.Measure b) =>
        {
            if (Resources.Compare(a.Value, b.Value) <= 0)
            {
                return Object.TRUE;
            }
            return Object.FALSE;
        });

        //Mayor Igual que
        SaveOperations(">=", (Object.Number a, Object.Number b) =>
        {
            if (Resources.Compare(a.Value, b.Value) >= 0)
            {
                return Object.TRUE;
            }
            return Object.FALSE;
        });
        SaveOperations(">=", (Object.Measure a, Object.Measure b) =>
        {
            if (Resources.Compare(a.Value, b.Value) >= 0)
            {
                return Object.TRUE;
            }
            return Object.FALSE;
        });
        SaveOperations("==", (Object.Number a, Object.Number b) =>
        {
            if (Resources.Compare(a.Value, b.Value) == 0) return Object.TRUE;
            return Object.FALSE;
        });
        SaveOperations("==", (Object.Point a, Object.Point b) =>
        {
            if (Operate("==", a.x, b.x) == Object.TRUE && Operate("==", a.y, b.y) == Object.TRUE) return Object.TRUE;
            return Object.FALSE;
        }
        );
        //Print
        SaveOperations("_", (Object a) =>
        {
            return Object.STRING;
        });


    }


}