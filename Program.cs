namespace Geo_Wall_E;

class Program
{
    public Statement.StatementList Statements {get; private set;}
    public Program(Statement.StatementList statement)
    {
        Statements=statement;
    }
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }
}