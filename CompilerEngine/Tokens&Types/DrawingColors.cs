namespace Geo_Wall_E;

//Colores
public enum Colors
{
    RED,
    BLUE,
    YELLOW,
    WHITE,
    BLACK,
    GREEN,
    GRAY,
    PURPLE,
    CYAN,
}

//Pila de colores
class SavingColors
{
    List<Colors> pile = new List<Colors>();

    //Establecer el negro como color por defecto
    public SavingColors()
    {
        pile.Add(Colors.BLACK);
    }

    public Colors Top { get => pile.Last(); }
    public void Push(Colors color) => pile.Add(color);

    //Quitar colores siempre dejando el negro
    public void Pop()
    {
        if (pile.Count > 1)
        {
            pile.RemoveAt(pile.Count - 1);
        }
    }
}
