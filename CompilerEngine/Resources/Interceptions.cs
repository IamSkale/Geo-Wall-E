namespace Geo_Wall_E;

static class Interceptions
{
    public static Object Intercept(Object Object1, Object Object2)
    {
        List<Object> resultado = new List<Object>();

        if (Object1.Type == ObjectsTypes.POINT)
        {
            switch (Object2.Type)
            {
                case ObjectsTypes.LINE:
                    resultado = InterceptPointLine((Object.Point)Object1, (Object.Line)Object2);
                    break;
                case ObjectsTypes.RAY:
                    resultado = InterceptPointRay((Object.Point)Object1, (Object.Ray)Object2);
                    break;
                case ObjectsTypes.SEGMENT:
                    resultado = InterceptPointSegment((Object.Point)Object1, (Object.Segment)Object2);
                    break;
                case ObjectsTypes.CIRCLE:
                    resultado = InterceptPointCircle((Object.Point)Object1, (Object.Circle)Object2);
                    break;
                case ObjectsTypes.POINT:
                    resultado = InterceptPointPoint((Object.Point)Object1, (Object.Point)Object2);
                    break;
            }

        }
        else if (Object1.Type == ObjectsTypes.LINE)
        {
            switch (Object2.Type)
            {
                case ObjectsTypes.CIRCLE:
                    resultado = InterceptLineCircle((Object.Line)Object1, (Object.Circle)Object2);
                    break;
                case ObjectsTypes.RAY:
                    resultado = InterceptLineRay((Object.Line)Object1, (Object.Ray)Object2);
                    break;
                case ObjectsTypes.LINE:
                    resultado = InterceptLineLine((Object.Line)Object1, (Object.Line)Object2);
                    break;
                case ObjectsTypes.SEGMENT:
                    resultado = InterceptLineSegment((Object.Line)Object1, (Object.Segment)Object2);
                    break;
                case ObjectsTypes.POINT:
                    resultado = InterceptPointLine((Object.Point)Object2, (Object.Line)Object1);
                    break;
            }
        }
        else if (Object1.Type == ObjectsTypes.SEGMENT)
        {
            switch (Object2.Type)
            {
                case ObjectsTypes.CIRCLE:
                    resultado = InterceptSegmentCircle((Object.Segment)Object1, (Object.Circle)Object2);
                    break;
                case ObjectsTypes.SEGMENT:
                    resultado = InterceptSegmentSegment((Object.Segment)Object1, (Object.Segment)Object2);
                    break;
                case ObjectsTypes.LINE:
                    resultado = InterceptLineSegment((Object.Line)Object2, (Object.Segment)Object1);
                    break;
                case ObjectsTypes.POINT:
                    resultado = InterceptPointSegment((Object.Point)Object2, (Object.Segment)Object1);
                    break;
                case ObjectsTypes.RAY:
                    resultado = InterceptSegmentRay((Object.Segment)Object1, (Object.Ray)Object2);
                    break;

            }
        }
        else if (Object1.Type == ObjectsTypes.RAY)
        {
            switch (Object2.Type)
            {
                case ObjectsTypes.POINT:
                    resultado = InterceptPointRay((Object.Point)Object2, (Object.Ray)Object1);
                    break;
                case ObjectsTypes.LINE:
                    resultado = InterceptLineRay((Object.Line)Object2, (Object.Ray)Object1);
                    break;
                case ObjectsTypes.SEGMENT:
                    resultado = InterceptSegmentRay((Object.Segment)Object2, (Object.Ray)Object1);
                    break;
                case ObjectsTypes.RAY:
                    resultado = InterceptRayRay((Object.Ray)Object2, (Object.Ray)Object1);
                    break;
                case ObjectsTypes.CIRCLE:
                    resultado = InterceptRayCircle((Object.Ray)Object1, (Object.Circle)Object2);
                    break;
            }
        }
        else if (Object1.Type == ObjectsTypes.CIRCLE)
        {
            switch (Object2.Type)
            {
                case ObjectsTypes.POINT:
                    resultado = InterceptPointCircle((Object.Point)Object2, (Object.Circle)Object1);
                    break;
                case ObjectsTypes.LINE:
                    resultado = InterceptLineCircle((Object.Line)Object2, (Object.Circle)Object1);
                    break;
                case ObjectsTypes.SEGMENT:
                    resultado = InterceptSegmentCircle((Object.Segment)Object2, (Object.Circle)Object1);
                    break;
                case ObjectsTypes.RAY:
                    resultado = InterceptRayCircle((Object.Ray)Object2, (Object.Circle)Object1);
                    break;
                case ObjectsTypes.CIRCLE:
                    resultado = InterceptCircleCircle((Object.Circle)Object2, (Object.Circle)Object1);
                    break;
            }
        }


        if (resultado.Count >= 1 && resultado[0].Type == ObjectsTypes.UNDEFINED)
        {
            return Object.UNDEFINED;
        }
        return new Object.Sequence.Listing(resultado);



    }


    ///Point
    public static List<Object> InterceptPointPoint(Object.Point Object1, Object.Point Object2)
    {
        List<Object> resultado = new List<Object>();

        if (IOperations.Operate("==", Object1, Object2) == Object.TRUE)
        {
            resultado.Add(Object1);
        }
        return resultado;

    }
    public static List<Object> InterceptPointLine(Object.Point Object1, Object.Line Object2)
    {
        List<Object> resultado = new List<Object>();
        float pendiente = CalcularPendiente(Object2.a, Object2.b);
        float n = Object2.a.y.Value - (Object2.a.x.Value * pendiente);
        var ramon = Math.Abs((Object1.x.Value * pendiente + n) - Object1.y.Value);
        if (Math.Abs((Object1.x.Value * pendiente + n) - Object1.y.Value) <= Resources.ZeroLimit)
        {
            resultado.Add(Object1);
        }
        else if (Object2.b.x.Value == Object2.a.x.Value && Object1.x.Value == Object2.a.x.Value)
        {
            resultado.Add(Object1);
        }
        return resultado;
    }
    public static List<Object> InterceptPointRay(Object.Point Object1, Object.Ray Object2)
    {
        List<Object> resultado = new List<Object>();
        float pendiente = CalcularPendiente(Object2.a, Object2.b);
        float n = Object2.a.y.Value - (Object2.a.x.Value * pendiente);
        if (Math.Abs((Object1.x.Value * pendiente + n) - Object1.y.Value) <= Resources.ZeroLimit)
        {
            if (Object2.b.x.Value < Object2.a.x.Value && ((Object2.b.x.Value >= Object1.x.Value) || (Object2.b.x.Value < Object1.x.Value && Object2.a.x.Value >= Object1.x.Value)))
            {
                resultado.Add(Object1);
            }
            else if (Object2.b.x.Value > Object2.a.x.Value && ((Object2.b.x.Value <= Object1.x.Value) || (Object2.b.x.Value > Object1.x.Value && Object2.a.x.Value <= Object1.x.Value)))
            {
                resultado.Add(Object1);
            }

        }
        else if (Object2.b.x.Value == Object2.a.x.Value && Object1.x.Value == Object2.a.x.Value)
        {
            if (Object2.b.y.Value < Object2.a.y.Value && ((Object1.y.Value <= Object2.b.y.Value) || (Object1.y.Value > Object2.b.y.Value && Object1.y.Value <= Object2.a.y.Value)))
            {
                resultado.Add(Object1);
            }
            else if (Object2.b.y.Value > Object2.a.y.Value && ((Object1.y.Value >= Object2.b.y.Value) || (Object1.y.Value < Object2.b.y.Value && Object1.y.Value >= Object2.a.y.Value)))
            {
                resultado.Add(Object1);
            }
        }
        return resultado;
    }
    public static List<Object> InterceptPointSegment(Object.Point Object1, Object.Segment Object2)
    {
        List<Object> resultado = new List<Object>();
        float pendiente = CalcularPendiente(Object2.a, Object2.b);
        float n = Object2.a.y.Value - (Object2.a.x.Value * pendiente);
        if (Math.Abs((Object1.x.Value * pendiente + n) - Object1.y.Value) <= Resources.ZeroLimit)
        {
            if (Object2.b.x.Value < Object2.a.x.Value && ((Object2.b.x.Value <= Object1.x.Value && Object2.a.x.Value >= Object1.x.Value)))
            {
                resultado.Add(Object1);
            }
            else if (Object2.b.x.Value > Object2.a.x.Value && ((Object2.b.x.Value > Object1.x.Value && Object2.a.x.Value <= Object1.x.Value)))
            {
                resultado.Add(Object1);
            }

        }
        else if (Object2.b.x.Value == Object2.a.x.Value && Object1.x.Value == Object2.a.x.Value)
        {
            if (Object2.b.y.Value < Object2.a.y.Value && ((Object1.y.Value >= Object2.b.y.Value && Object1.y.Value <= Object2.a.y.Value)))
            {
                resultado.Add(Object1);
            }
            else if (Object2.b.y.Value > Object2.a.y.Value && (Object1.y.Value <= Object2.b.y.Value && Object1.y.Value >= Object2.a.y.Value))
            {
                resultado.Add(Object1);
            }
        }
        return resultado;
    }
    public static List<Object> InterceptPointCircle(Object.Point Object1, Object.Circle Object2)
    {
        List<Object> resultado = new List<Object>();
        if (Math.Abs(DistanciaPuntoPunto(Object1, Object2.o) - Object2.radius.Value) <= Resources.ZeroLimit)
        {

            resultado.Add(Object1);
        }
        return resultado;
    }
    public static List<Object> InterceptPointArc(Object.Point Object1, Object.Arc Object2, List<Object> resultado)
    {
        if (Math.Abs(DistanciaPuntoPunto(Object1, Object2.a) - Object2.radius.Value) <= Resources.ZeroLimit)
        {

        }
        return resultado;
    }

    ///Segment
    public static List<Object> InterceptLineSegment(Object.Line Object1, Object.Segment Object2)
    {
        List<Object> resultado = new List<Object>();
        List<Object> resultadoaux = InterceptLineLine(Object1, new Object.Line(new Object.String(""), Object2.a, Object2.b, new Object.String(""), Colors.BLACK));
        if (resultadoaux[0] != Object.UNDEFINED)
        {
            foreach (Object Object in resultadoaux)
            {
                List<Object> inObject1 = InterceptPointLine((Object.Point)Object, Object1);
                List<Object> inObject2 = InterceptPointSegment((Object.Point)Object, Object2);
                if (inObject1.Count == 1 && inObject2.Count == 1) resultado.Add(Object);
            }
            return resultado;
        }
        return resultadoaux;
    }
    public static List<Object> InterceptLineLine(Object.Line Object1, Object.Line Object2)
    {
        List<Object> resultado = new List<Object>();
        float pendiente1 = CalcularPendiente(Object1.a, Object1.b);
        float n1 = Object1.a.y.Value - (Object1.a.x.Value * pendiente1);
        float pendiente2 = CalcularPendiente(Object2.a, Object2.b);
        float n2 = Object2.a.y.Value - (Object2.a.x.Value * pendiente2);
        float newx = 0;
        float newy = 0;

        if (pendiente1 == pendiente2)
        {
            resultado.Add(Object.UNDEFINED);
            return resultado;
        }
        else if (pendiente1 == float.NegativeInfinity)
        {
            newx = (n2 - n1) / (pendiente1 - pendiente2);
            newy = pendiente2 * newx + n2;
            resultado = InterceptPointLine(new Object.Point(new Object.String(""), new Object.Number(newx), new Object.Number(newy), new Object.String(""), Colors.BLACK), Object2);
            return resultado;
        }

        newx = (n2 - n1) / (pendiente1 - pendiente2);
        newy = pendiente2 * newx + n2;
        resultado = InterceptPointLine(new Object.Point(new Object.String(""), new Object.Number(newx), new Object.Number(newy), new Object.String(""), Colors.BLACK), Object2);

        return resultado;
    }
    public static List<Object> InterceptLineRay(Object.Line Object1, Object.Ray Object2)
    {
        List<Object> resultado = new List<Object>();
        List<Object> resultadoaux = InterceptLineLine(Object1, new Object.Line(new Object.String(""), Object2.a, Object2.b, new Object.String(""), Colors.BLACK));
        if (resultadoaux[0] != Object.UNDEFINED)
        {
            foreach (Object Object in resultadoaux)
            {
                List<Object> inObject1 = InterceptPointLine((Object.Point)Object, Object1);
                List<Object> inObject2 = InterceptPointRay((Object.Point)Object, Object2);
                if (inObject1.Count == 1 && inObject2.Count == 1) resultado.Add(Object);
            }
            return resultado;
        }
        return resultadoaux;
    }
    static List<Object> InterceptLineCircle(Object.Line Object1, Object.Circle Object2)
    {
        List<Object> resultado = new List<Object>();
        float pendiente = CalcularPendiente(Object1.a, Object1.b);
        float n = Object1.a.y.Value - (Object1.a.x.Value * pendiente);

        float A = 1 + pendiente * pendiente;
        float B = 2 * pendiente * (n - Object2.o.y.Value) - 2 * Object2.o.x.Value;
        float C = Object2.o.x.Value * Object2.o.x.Value + (n - Object2.o.y.Value) * (n - Object2.o.y.Value) - Object2.radius.Value * Object2.radius.Value;
        float discriminante = B * B - 4 * A * C;
        if (pendiente == float.NegativeInfinity)
        {
            float x = Object1.a.x.Value;
            float discriminanteVertical = Object2.radius.Value * Object2.radius.Value - (x - Object2.o.x.Value) * (x - Object2.o.x.Value);
            if (discriminanteVertical >= 0)
            {
                float y1 = (float)Math.Sqrt(discriminanteVertical) + Object2.o.y.Value;
                float y2 = Object2.o.y.Value - y1;
                resultado.Add(new Object.Point(new Object.String(""), new Object.Number(x), new Object.Number(y1), new Object.String(""), Colors.BLACK));
                if (discriminanteVertical > 0)
                    resultado.Add(new Object.Point(new Object.String(""), new Object.Number(x), new Object.Number(y2), new Object.String(""), Colors.BLACK));
            }
        }
        else
        {
            if (discriminante < 0)
            {
                return resultado;
            }
            else if (discriminante == 0)
            {
                float x = -B / (2 * A);
                float y = pendiente * x + n;
                resultado.Add(new Object.Point(new Object.String(""), new Object.Number(x), new Object.Number(y), new Object.String(""), Colors.BLACK));
                return resultado;
            }
            else
            {
                float x1 = (float)(-B + Math.Sqrt(discriminante)) / (2 * A);
                float y1 = pendiente * x1 + n;
                float x2 = (float)(-B - Math.Sqrt(discriminante)) / (2 * A);
                float y2 = pendiente * x2 + n;
                resultado.Add(new Object.Point(new Object.String(""), new Object.Number(x1), new Object.Number(y1), new Object.String(""), Colors.BLACK));
                resultado.Add(new Object.Point(new Object.String(""), new Object.Number(x2), new Object.Number(y2), new Object.String(""), Colors.BLACK));
            }
        }
        return resultado;
    }
    // Segment
    public static List<Object> InterceptSegmentSegment(Object.Segment Object1, Object.Segment Object2)
    {
        List<Object> resultado = new List<Object>();
        List<Object> resultadocopia = new List<Object>(resultado);
        float pendiente1 = CalcularPendiente(Object1.a, Object1.b);
        float n1 = Object1.a.y.Value - (Object1.a.x.Value * pendiente1);
        float pendiente2 = CalcularPendiente(Object2.a, Object2.b);
        float n2 = Object2.a.y.Value - (Object2.a.x.Value * pendiente2);
        float newx = 0;
        float newy = 0;
        List<Object> resultadoaux1;
        List<Object> resultadoaux2;

        if (pendiente1 == pendiente2)
        {
            if (pendiente1 == float.NegativeInfinity)
            {
                if (Object2.a.y.Value < Object2.b.y.Value)
                {
                    if (Object1.a.y.Value < Object1.a.y.Value)
                    {
                        if (Object2.a.y.Value <= Object1.a.y.Value && Object2.b.y.Value >= Object1.b.y.Value)
                        {
                            resultado.Add(Object.UNDEFINED);
                        }
                        else
                        {
                            if (Object1.b.y.Value < Object2.b.y.Value && Object1.b.y.Value >= Object2.a.y.Value)
                            {
                                if (Object1.b.y.Value == Object2.a.y.Value)
                                {
                                    resultado.Add(Object1.b);
                                }
                                else
                                {
                                    resultado.Add(Object.UNDEFINED);
                                }
                            }

                        }

                    }
                    else
                    {
                        if (Object2.a.y.Value <= Object1.b.y.Value && Object2.b.y.Value >= Object1.a.y.Value)
                        {
                            resultado.Add(Object.UNDEFINED);
                        }
                        else
                        {
                            if (Object1.a.y.Value < Object2.b.y.Value && Object1.a.y.Value >= Object2.a.y.Value)
                            {
                                if (Object1.a.y.Value == Object2.a.y.Value)
                                {
                                    resultado.Add(Object1.a);
                                }
                                else
                                {
                                    resultado.Add(Object.UNDEFINED);
                                }
                            }

                        }
                    }
                }
                else
                {
                    if (Object1.a.y.Value < Object1.a.y.Value)
                    {
                        if (Object2.b.y.Value <= Object1.a.y.Value && Object2.a.y.Value >= Object1.b.y.Value)
                        {
                            resultado.Add(Object.UNDEFINED);
                        }
                        else
                        {
                            if (Object1.b.y.Value < Object2.a.y.Value && Object1.b.y.Value >= Object2.b.y.Value)
                            {
                                if (Object1.b.y.Value == Object2.b.y.Value)
                                {
                                    resultado.Add(Object1.b);
                                }
                                else
                                {
                                    resultado.Add(Object.UNDEFINED);
                                }
                            }

                        }

                    }
                    else
                    {
                        if (Object2.b.y.Value <= Object1.b.y.Value && Object2.a.y.Value >= Object1.a.y.Value)
                        {
                            resultado.Add(Object.UNDEFINED);
                        }
                        else
                        {
                            if (Object1.a.y.Value < Object2.a.y.Value && Object1.a.y.Value >= Object2.b.y.Value)
                            {
                                if (Object1.a.y.Value == Object2.b.y.Value)
                                {
                                    resultado.Add(Object1.a);
                                }
                                else
                                {
                                    resultado.Add(Object.UNDEFINED);
                                }
                            }

                        }
                    }
                }
                return resultado;
            }
            else
            {
                if (Object2.a.x.Value < Object2.b.x.Value)
                {
                    if (Object1.a.x.Value < Object1.a.x.Value)
                    {
                        if (Object2.a.x.Value <= Object1.a.x.Value && Object2.b.x.Value >= Object1.b.x.Value)
                        {
                            resultado.Add(Object.UNDEFINED);
                        }
                        else
                        {
                            if (Object1.b.x.Value < Object2.b.x.Value && Object1.b.x.Value >= Object2.a.x.Value)
                            {
                                if (Object1.b.x.Value == Object2.a.x.Value)
                                {
                                    resultado.Add(Object1.b);
                                }
                                else
                                {
                                    resultado.Add(Object.UNDEFINED);
                                }
                            }

                        }

                    }
                    else
                    {
                        if (Object2.a.x.Value <= Object1.b.x.Value && Object2.b.x.Value >= Object1.a.x.Value)
                        {
                            resultado.Add(Object.UNDEFINED);
                        }
                        else
                        {
                            if (Object1.a.x.Value < Object2.b.x.Value && Object1.a.x.Value >= Object2.a.x.Value)
                            {
                                if (Object1.a.x.Value == Object2.a.x.Value)
                                {
                                    resultado.Add(Object1.a);
                                }
                                else
                                {
                                    resultado.Add(Object.UNDEFINED);
                                }
                            }

                        }
                    }
                }
                else
                {
                    if (Object1.a.x.Value < Object1.a.x.Value)
                    {
                        if (Object2.b.x.Value <= Object1.a.x.Value && Object2.a.x.Value >= Object1.b.x.Value)
                        {
                            resultado.Add(Object.UNDEFINED);
                        }
                        else
                        {
                            if (Object1.b.x.Value < Object2.a.x.Value && Object1.b.x.Value >= Object2.b.x.Value)
                            {
                                if (Object1.b.x.Value == Object2.b.x.Value)
                                {
                                    resultado.Add(Object1.b);
                                }
                                else
                                {
                                    resultado.Add(Object.UNDEFINED);
                                }
                            }

                        }

                    }
                    else
                    {
                        if (Object2.b.x.Value <= Object1.b.x.Value && Object2.a.x.Value >= Object1.a.x.Value)
                        {
                            resultado.Add(Object.UNDEFINED);
                        }
                        else
                        {
                            if (Object1.a.x.Value < Object2.a.x.Value && Object1.a.x.Value >= Object2.b.x.Value)
                            {
                                if (Object1.a.x.Value == Object2.b.x.Value)
                                {
                                    resultado.Add(Object1.a);
                                }
                                else
                                {
                                    resultado.Add(Object.UNDEFINED);
                                }
                            }

                        }
                    }
                }
            }
            return resultado;
        }
        else if (pendiente1 == float.NegativeInfinity)
        {
            newx = (n2 - n1) / (pendiente1 - pendiente2);
            newy = pendiente2 * newx + n2;
            resultadoaux1 = InterceptPointSegment(new Object.Point(new Object.String(""), new Object.Number(newx), new Object.Number(newy), new Object.String(""), Colors.BLACK), Object1);
            resultadocopia = new List<Object>(resultado);
            resultadoaux2 = InterceptPointSegment(new Object.Point(new Object.String(""), new Object.Number(newx), new Object.Number(newy), new Object.String(""), Colors.BLACK), Object2);
            if (resultadoaux1.Count == resultadoaux2.Count && resultadoaux1.Count == 1)
            {

                resultado = resultadoaux1;

            }
            return resultado;
        }

        newx = (n2 - n1) / (pendiente1 - pendiente2);
        newy = pendiente1 * newx + n1;
        resultadoaux1 = InterceptPointSegment(new Object.Point(new Object.String(""), new Object.Number(newx), new Object.Number(newy), new Object.String(""), Colors.BLACK), Object1);
        resultadocopia = new List<Object>(resultado);
        resultadoaux2 = InterceptPointSegment(new Object.Point(new Object.String(""), new Object.Number(newx), new Object.Number(newy), new Object.String(""), Colors.BLACK), Object2);
        if (resultadoaux1.Count == resultadoaux2.Count && resultadoaux1.Count == 1)
        {
            resultado = resultadoaux1;
        }
        return resultado;
    }
    public static List<Object> InterceptSegmentRay(Object.Segment Object1, Object.Ray Object2)
    {
        List<Object> resultado = new List<Object>();
        List<Object> resultadocopia = new List<Object>(resultado);
        float pendiente1 = CalcularPendiente(Object1.a, Object1.b);
        float n1 = Object1.a.y.Value - (Object1.a.x.Value * pendiente1);
        float pendiente2 = CalcularPendiente(Object2.a, Object2.b);
        float n2 = Object2.a.y.Value - (Object2.a.x.Value * pendiente2);
        float newx = 0;
        float newy = 0;
        List<Object> resultadoaux1;
        List<Object> resultadoaux2;

        if (pendiente1 == pendiente2)
        {
            if (pendiente1 == float.NegativeInfinity)
            {
                if (Object1.a.y.Value < Object1.b.y.Value)
                {
                    if (Object2.a.y.Value < Object2.b.y.Value)
                    {
                        if (Object2.a.y.Value <= Object1.b.y.Value)
                        {
                            resultado.Add(Object.UNDEFINED);
                        }
                        else if (Object2.a.y.Value == Object1.a.y.Value)
                        {
                            resultado.Add(Object2.a);
                        }
                    }
                    else
                    {
                        if (Object2.a.y.Value >= Object1.b.y.Value)
                        {
                            resultado.Add(Object.UNDEFINED);
                        }
                        else if (Object2.a.y.Value == Object1.a.y.Value)
                        {
                            resultado.Add(Object2.a);
                        }
                    }
                }
                else
                {
                    if (Object2.a.y.Value < Object2.b.y.Value)
                    {
                        if (Object2.a.y.Value < Object1.a.y.Value)
                        {
                            resultado.Add(Object.UNDEFINED);
                        }
                        else if (Object2.a.y.Value == Object1.b.y.Value)
                        {
                            resultado.Add(Object2.a);
                        }
                        if (Object2.a.y.Value <= Object1.a.y.Value)
                        {
                            resultado.Add(Object.UNDEFINED);
                        }
                    }
                    else
                    {
                        if (Object2.a.y.Value >= Object1.a.y.Value)
                        {
                            resultado.Add(Object.UNDEFINED);
                        }
                        else if (Object2.a.y.Value == Object1.b.y.Value)
                        {
                            resultado.Add(Object2.a);
                        }
                    }
                }
            }
            else
            {
                if (Object1.a.x.Value < Object1.b.x.Value)
                {
                    if (Object2.a.x.Value < Object2.b.x.Value)
                    {
                        if (Object2.a.x.Value <= Object1.b.x.Value)
                        {
                            resultado.Add(Object.UNDEFINED);
                        }
                        else if (Object2.a.x.Value == Object1.a.x.Value)
                        {
                            resultado.Add(Object2.a);
                        }
                    }
                    else
                    {
                        if (Object2.a.x.Value >= Object1.b.x.Value)
                        {
                            resultado.Add(Object.UNDEFINED);
                        }
                        else if (Object2.a.x.Value == Object1.a.x.Value)
                        {
                            resultado.Add(Object2.a);
                        }
                    }
                }
                else
                {
                    if (Object2.a.x.Value < Object2.b.x.Value)
                    {
                        if (Object2.a.x.Value <= Object1.a.x.Value)
                        {
                            resultado.Add(Object.UNDEFINED);
                        }
                        else if (Object2.a.x.Value == Object1.b.x.Value)
                        {
                            resultado.Add(Object2.a);
                        }
                    }
                    else
                    {
                        if (Object2.a.x.Value >= Object1.a.x.Value)
                        {
                            resultado.Add(Object.UNDEFINED);
                        }
                        else if (Object2.a.x.Value == Object1.b.x.Value)
                        {
                            resultado.Add(Object2.a);
                        }
                    }

                }
            }
            return resultado;
        }
        else if (pendiente1 == float.NegativeInfinity)
        {
            newx = (n2 - n1) / (pendiente1 - pendiente2);
            newy = pendiente2 * newx + n2;
            resultadoaux1 = InterceptPointSegment(new Object.Point(new Object.String(""), new Object.Number(newx), new Object.Number(newy), new Object.String(""), Colors.BLACK), Object1);
            resultadocopia = new List<Object>(resultado);
            resultadoaux2 = InterceptPointRay(new Object.Point(new Object.String(""), new Object.Number(newx), new Object.Number(newy), new Object.String(""), Colors.BLACK), Object2);
            if (resultadoaux1.Count == resultadoaux2.Count && resultadoaux1.Count == 1)
            {
                resultado = resultadoaux1;

            }
            return resultado;
        }

        newx = (n2 - n1) / (pendiente1 - pendiente2);
        newy = pendiente1 * newx + n1;
        resultadoaux1 = InterceptPointSegment(new Object.Point(new Object.String(""), new Object.Number(newx), new Object.Number(newy), new Object.String(""), Colors.BLACK), Object1);
        resultadocopia = new List<Object>(resultado);
        resultadoaux2 = InterceptPointRay(new Object.Point(new Object.String(""), new Object.Number(newx), new Object.Number(newy), new Object.String(""), Colors.BLACK), Object2);
        if (resultadoaux1.Count == resultadoaux2.Count && resultadoaux1.Count == 1)
        {
            resultado = resultadoaux1;
        }
        return resultado;
    }
    static List<Object> InterceptSegmentCircle(Object.Segment Object1, Object.Circle Object2)
    {
        List<Object> resultado = new List<Object>();
        List<Object> resultadoaux1 = new List<Object>();

        Object.Line templine = new Object.Line(new Object.String(""), Object1.a, Object1.b, new Object.String(""), Colors.BLACK);
        resultadoaux1 = InterceptLineCircle(templine, Object2);
        foreach (Object Object in resultadoaux1)
        {
            resultado = resultado.Concat(InterceptPointSegment((Object.Point)Object, Object1)).ToList();

        }
        return resultado;
    }

    public static List<Object> InterceptRayRay(Object.Ray Object1, Object.Ray Object2)
    {
        List<Object> resultado = new List<Object>();
        List<Object> resultadocopia = new List<Object>(resultado);
        float pendiente1 = CalcularPendiente(Object1.a, Object1.b);
        float n1 = Object1.a.y.Value - (Object1.a.x.Value * pendiente1);
        float pendiente2 = CalcularPendiente(Object2.a, Object2.b);
        float n2 = Object2.a.y.Value - (Object2.a.x.Value * pendiente2);
        float newx = 0;
        float newy = 0;
        List<Object> resultadoaux1;
        List<Object> resultadoaux2;

        if (pendiente1 == pendiente2)
        {
            if (pendiente1 == float.NegativeInfinity)
            {
                if ((Object1.a.y.Value < Object1.b.y.Value && Object2.a.y.Value > Object2.b.y.Value))
                {
                    if ((Object1.a.y.Value == Object2.a.y.Value))
                    {
                        resultado.Add(Object1.a);
                    }
                    else if (Object1.a.y.Value < Object2.a.y.Value)
                    {
                        resultado.Add(Object.UNDEFINED);
                    }


                }
                else if ((Object1.a.y.Value > Object1.b.y.Value && Object2.a.y.Value < Object2.b.y.Value))
                {
                    if ((Object1.a.y.Value == Object2.a.y.Value))
                    {
                        resultado.Add(Object1.a);
                    }
                    else
                    {
                        resultado.Add(Object.UNDEFINED);
                    }
                }
                return resultado;
            }
            else
            {
                if ((Object1.a.x.Value < Object1.b.x.Value && Object2.a.x.Value > Object2.b.x.Value))
                {
                    if ((Object1.a.x.Value == Object2.a.x.Value))
                    {
                        resultado.Add(Object1.a);
                    }
                    else if (Object1.a.x.Value < Object2.a.x.Value)
                    {
                        resultado.Add(Object.UNDEFINED);
                    }

                }
                else if ((Object1.a.x.Value > Object1.b.x.Value && Object2.a.x.Value < Object2.b.x.Value))
                {
                    if ((Object1.a.x.Value == Object2.a.x.Value))
                    {
                        resultado.Add(Object1.a);
                    }
                    else
                    {
                        resultado.Add(Object.UNDEFINED);
                    }
                }
                return resultado;
            }
        }
        else if (pendiente1 == float.NegativeInfinity)
        {
            newx = (n2 - n1) / (pendiente1 - pendiente2);
            newy = pendiente2 * newx + n2;
            resultadoaux1 = InterceptPointRay(new Object.Point(new Object.String(""), new Object.Number(newx), new Object.Number(newy), new Object.String(""), Colors.BLACK), Object1);
            resultadocopia = new List<Object>(resultado);
            resultadoaux2 = InterceptPointRay(new Object.Point(new Object.String(""), new Object.Number(newx), new Object.Number(newy), new Object.String(""), Colors.BLACK), Object2);
            if (resultadoaux1.Count == resultadoaux2.Count && resultadoaux1.Count == 1)
            {
                resultado = resultadoaux1;

            }
            return resultado;
        }

        newx = (n2 - n1) / (pendiente1 - pendiente2);
        newy = pendiente1 * newx + n1;
        resultadoaux1 = InterceptPointRay(new Object.Point(new Object.String(""), new Object.Number(newx), new Object.Number(newy), new Object.String(""), Colors.BLACK), Object1);
        resultadocopia = new List<Object>(resultado);
        resultadoaux2 = InterceptPointRay(new Object.Point(new Object.String(""), new Object.Number(newx), new Object.Number(newy), new Object.String(""), Colors.BLACK), Object2);
        if (resultadoaux1.Count == resultadoaux2.Count && resultadoaux1.Count == 1)
        {

            resultado = resultadoaux1;
        }
        return resultado;
    }
    static List<Object> InterceptRayCircle(Object.Ray Object1, Object.Circle Object2)
    {
        List<Object> resultado = new List<Object>();
        List<Object> resultadoaux1 = new List<Object>();

        Object.Line templine = new Object.Line(new Object.String(""), Object1.a, Object1.b, new Object.String(""), Colors.BLACK);
        resultadoaux1 = InterceptLineCircle(templine, Object2);
        foreach (Object Object in resultadoaux1)
        {
            resultado = resultado.Concat(InterceptPointRay((Object.Point)Object, Object1)).ToList();
        }
        return resultado;
    }
    static List<Object> InterceptCircleCircle(Object.Circle Object1, Object.Circle Object2)
    {
        List<Object> resultado = new List<Object>();

        if (IOperations.Operate("==", Object1.o, Object2.o) == Object.TRUE && Math.Abs(Object1.radius.Value - Object2.radius.Value) <= Resources.ZeroLimit)
        {
            resultado.Add(Object.UNDEFINED);
            return resultado;
        }
        List<Object> resultadoaux1 = new List<Object>();
        List<Object> resultadoaux2 = new List<Object>();
        float h = Object1.o.x.Value;
        float k = Object1.o.y.Value;
        float p = Object2.o.x.Value;
        float q = Object2.o.y.Value;
        float r1 = Object1.radius.Value;
        float r2 = Object2.radius.Value;
        float A = p - h;
        float B = q - k;
        float C = (h * h + k * k - p * p - q * q + r2 * r2 - r1 * r1) / 2;
        Object.Line line;
        if (Math.Abs(B) <= Resources.ZeroLimit)
        {
            line = new Object.Line(new Object.String(), new Object.Point(new Object.String(), new Object.Number(-C / A), new Object.Number(0), new Object.String(), Colors.BLACK),
                                                        new Object.Point(new Object.String(), new Object.Number(-C / A), new Object.Number(1), new Object.String(), Colors.BLACK)
                                                        , new Object.String(), Colors.BLACK);
        }
        else
        {
            line = new Object.Line(new Object.String(), new Object.Point(new Object.String(), new Object.Number(0), new Object.Number(-C / B), new Object.String(), Colors.BLACK),
                                                       new Object.Point(new Object.String(), new Object.Number(1), new Object.Number(-(C + A) / B), new Object.String(), Colors.BLACK)
                                                       , new Object.String(), Colors.BLACK);
        }
        resultadoaux1 = InterceptLineCircle(line, Object1);
        foreach (Object Object in resultadoaux1)
        {
            List<Object> inCircle = InterceptPointCircle((Object.Point)Object, Object1);
            if (inCircle.Count == 1) resultadoaux2.Add(Object);
        }
        foreach (Object Object in resultadoaux2)
        {
            List<Object> inCircle = InterceptPointCircle((Object.Point)Object, Object2);
            if (inCircle.Count == 1) resultado.Add(Object);
        }

        return resultadoaux2;
    }

    static float CalcularPendiente(Object.Point a, Object.Point b)
    {
        if (b.x.Value - a.x.Value == 0)
        {
            return float.NegativeInfinity;
        }
        return (b.y.Value - a.y.Value) / (b.x.Value - a.x.Value);
    }
    static float DistanciaPuntoPunto(Object.Point a, Object.Point b)
    {
        float distancia = (float)Math.Sqrt(Math.Pow(b.x.Value - a.x.Value, 2) + Math.Pow(b.y.Value - a.y.Value, 2));
        return distancia;
    }
}