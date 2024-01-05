namespace Geo_Wall_E;

interface IGetError
{
    public int Line { get; }
    public string File { get; }
    public int Shift { get; }

}

interface CompileTimeError : IStop
{
    public int ErrorCountTop { get; }

    public ICollection<Error> Errors { get; }

    public void ErrorOrders(IStop call, bool forceStop, int ErrorCountTop, ICollection<Error> errors, string message, int line, int shift, string file);

    public void CatchError(IGetError error, string message, bool forceStop = false);
    public void CatchError(int line, int shift, string file, string message, bool forceStop = false);

}
interface IStop
{
    public void Stop();
}

abstract class Dependecies : CompileTimeError
{
    public int ErrorCountTop { get; private set; }

    public ICollection<Error> Errors { get; private set; }

    protected Dependecies(int errorCountTop, ICollection<Error> errors)
    {
        ErrorCountTop = errorCountTop;
        Errors = errors;
    }

    public virtual void ErrorOrders(IStop call, bool forceStop, int ErrorCountTop, ICollection<Error> errors, string message, int line, int shift, string file)
    {
        errors.Add(new Error(line, file, shift, message));
        if (forceStop || errors.Count >= ErrorCountTop)
        {
            call.Stop();
        }
    }

    public virtual void CatchError(int line, int shift, string file, string message, bool forceStop = false)
    {
        ErrorOrders(this, forceStop, this.ErrorCountTop, this.Errors, message, line, shift, file);
    }

    public virtual void CatchError(IGetError error, string message, bool forceStop = false)
    {
        ErrorOrders(this, forceStop, this.ErrorCountTop, this.Errors, message, error.Line, error.Shift, error.File);
    }

    public abstract void Stop();

}


