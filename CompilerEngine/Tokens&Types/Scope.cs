namespace Geo_Wall_E;

class Scope
{
    Scope? parent;
    Dictionary<string, Object> objects = new Dictionary<string, Object>();
    HashSet<string> constants = new HashSet<string>();
    public Scope(Scope? _parent = null)
    {
        parent = _parent;
    }
    private bool HasParent { get => parent != null; }

    private bool IsConstant(string varName)
    {
        if (constants.Contains(varName)) return true;
        if (HasParent) return parent!.IsConstant(varName);
        return false;
    }

    public void SetArgument(string varName, Object newobject)
    {
        if (IsConstant(varName)) throw new ScopeException($"Redeclaration of constant `{varName}`");

        if (objects.ContainsKey(varName))
        {


            if (objects[varName].Type == ObjectsTypes.FUNCTION_LIST)
            {

                if (newobject.Type == ObjectsTypes.FUNCTION)
                {
                    Object.FunctionList elementAssociatedToVarNameAsFunctionList = (objects[varName] as Object.FunctionList)!;
                    Object.Function newElementAsFunction = (newobject as Object.Function)!;

                    if (elementAssociatedToVarNameAsFunctionList.ContainsArity(newElementAsFunction.Arity)) throw new ScopeException($"A definition of function `{varName}` with `{newElementAsFunction.Arity}` arguments already exist");

                    elementAssociatedToVarNameAsFunctionList.Add(newElementAsFunction);
                }
                else
                {

                    throw new ScopeException($"Variable `{varName}` is declared twice on the same scope");
                }
            }
            else
            {

                throw new ScopeException($"Variable `{varName}` is declared twice on the same scope");
            }
        }
        else
        {

            if (newobject.Type == ObjectsTypes.FUNCTION)
            {

                Object.FunctionList newFunction = new Object.FunctionList();

                newFunction.Add((Object.Function)newobject);

                objects.Add(varName, newFunction);
            }
            else
            {

                objects.Add(varName, newobject);
            }
        }
    }

    public void SetConstant(string varName, Object element)
    {
        SetArgument(varName, element);
        constants.Add(varName);
    }

    public Object Get(string varName, int arity = -1)
    {

        if (objects.ContainsKey(varName))
        {
            Object element = objects[varName];
            if (element.Type == ObjectsTypes.FUNCTION_LIST)
            {
                Object.FunctionList elementAsFunctionList = (element as Object.FunctionList)!;

                if (arity < 0)
                {
                    throw new ScopeException($"Attempted to use function `{varName}` as a variable");
                }

                if (!elementAsFunctionList.ContainsArity(arity)) throw new ScopeException($"No overload of function `{varName}` takes {arity} parameters");

                return elementAsFunctionList.GetFunction(arity);
            }
            else
            {

                if (arity >= 0) throw new ScopeException($"Attempted to use variable `{varName}` as a function");

                return element;
            }
        }

        if (HasParent) return parent!.Get(varName, arity);

        if (arity < 0) throw new ScopeException($"Variable `{varName}` not declared");
        else throw new ScopeException($"Function `{varName}` with `{arity}` parameters is not declared");
    }

    public static Scope RequestScopeForFunction(List<Object.String> Arguments, List<Object> Parameters, Scope? parentScope = null)
    {
        if (Arguments.Count != Parameters.Count) throw new ScopeException("Arguments and parameters count doesnt match");
        Scope functionScope = new Scope();
        for (int i = 0; i < Arguments.Count; ++i)
        {
            if (Parameters[i].Type == ObjectsTypes.FUNCTION) functionScope.SetArgument(Arguments[i].Value, Parameters[i]);
            else functionScope.SetConstant(Arguments[i].Value, Parameters[i]);
        }

        functionScope.parent = parentScope;

        return functionScope;
    }
}
