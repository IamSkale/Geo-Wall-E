namespace Geo_Wall_E;

public partial class Form2 : Form
{
    List<IDraw> Objects;
    List<Object.String> Prints;

    Graphics graphics;
    Pen pen;
    SolidBrush solidBrush;
    RichTextBox richTextBox;

    public Form2(List<IDraw> objects, List<Object.String> prints)
    {
        Objects = objects;
        Prints = prints;
        richTextBox = new RichTextBox();
        richTextBox.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
        richTextBox.Font = new Font("Arial", 12, FontStyle.Regular);
        richTextBox.Dock = DockStyle.Right;
        richTextBox.Text = WritePrintsText();
        richTextBox.ReadOnly = true;
        InitializeComponent();
        this.Controls.Add(richTextBox);
        this.Paint += new PaintEventHandler(Form2_Paint);
    }
    void Form2_Paint(object sender, PaintEventArgs e)
    {
        graphics = e.Graphics;

        foreach (var item in Objects)
        {
            if (item is Object.Line)
            {
                PaintLine((Object.Line)item);
            }
            else if (item is Object.Point)
            {
                PaintPoint((Object.Point)item);
            }
            else if (item.Type == ObjectsTypes.CIRCLE)
            {
                PaintCircle((Object.Circle)item);
            }
            else if (item.Type == ObjectsTypes.ARC)
            {
                PaintArc((Object.Arc)item);
            }
            else if (item.Type == ObjectsTypes.SEGMENT)
            {
                PaintSegment((Object.Segment)item);
            }
            else if (item.Type == ObjectsTypes.RAY)
            {
                PaintRay((Object.Ray)item);
            }
        }
    }
    void PaintCircle(Object.Circle circle)
    {
        switch (circle.Colors)
        {
            case Colors.RED:
                pen = new Pen(Color.Red, 2);
                break;
            case Colors.YELLOW:
                pen = new Pen(Color.Yellow, 2);
                break;
            case Colors.BLUE:
                pen = new Pen(Color.Blue, 2);
                break;
            case Colors.WHITE:
                pen = new Pen(Color.White, 2);
                break;
            case Colors.BLACK:
                pen = new Pen(Color.Black, 2);
                break;
            case Colors.GREEN:
                pen = new Pen(Color.Green, 2);
                break;
            case Colors.GRAY:
                pen = new Pen(Color.Gray, 2);
                break;
            case Colors.PURPLE:
                pen = new Pen(Color.Purple, 2);
                break;
            case Colors.CYAN:
                pen = new Pen(Color.Cyan, 2);
                break;
        }

        graphics.DrawEllipse(pen, circle.o.x.Value - circle.radius.Value, circle.o.y.Value - circle.radius.Value, circle.radius.Value * 2, circle.radius.Value * 2);
        if (circle.Text != null)
        {
            WritePaintText(circle.Text.Value, (int)circle.o.x.Value, (int)circle.o.y.Value);
        }
    }
    void PaintPoint(Object.Point point)
    {
        switch (point.Colors)
        {
            case Colors.RED:
                solidBrush = new SolidBrush(Color.Red);
                break;
            case Colors.YELLOW:
                solidBrush = new SolidBrush(Color.Yellow);
                break;
            case Colors.BLUE:
                solidBrush = new SolidBrush(Color.Blue);
                break;
            case Colors.WHITE:
                solidBrush = new SolidBrush(Color.White);
                break;
            case Colors.BLACK:
                solidBrush = new SolidBrush(Color.Black);
                break;
            case Colors.GREEN:
                solidBrush = new SolidBrush(Color.Green);
                break;
            case Colors.GRAY:
                solidBrush = new SolidBrush(Color.Gray);
                break;
            case Colors.PURPLE:
                solidBrush = new SolidBrush(Color.Purple);
                break;
            case Colors.CYAN:
                solidBrush = new SolidBrush(Color.Cyan);
                break;
        }

        graphics.FillEllipse(solidBrush, point.x.Value - 4, point.y.Value - 4, 8, 8);
        if (point.Text != null)
        {
            WritePaintText(point.Text.Value, (int)point.x.Value, (int)point.y.Value);
        }
    }
    void PaintLine(Object.Line line)
    {
        int textX = 0;
        int textY = 0;
        if (line.a.x.Value >= line.b.x.Value)
        {
            textX = (int)line.a.x.Value;
            textY = (int)line.a.y.Value;
        }
        else
        {
            textX = (int)line.b.x.Value;
            textY = (int)line.b.y.Value;
        }

        switch (line.Colors)
        {
            case Colors.RED:
                pen = new Pen(Color.Red, 2);
                break;
            case Colors.YELLOW:
                pen = new Pen(Color.Yellow, 2);
                break;
            case Colors.BLUE:
                pen = new Pen(Color.Blue, 2);
                break;
            case Colors.WHITE:
                pen = new Pen(Color.White, 2);
                break;
            case Colors.BLACK:
                pen = new Pen(Color.Black, 2);
                break;
            case Colors.GREEN:
                pen = new Pen(Color.Green, 2);
                break;
            case Colors.GRAY:
                pen = new Pen(Color.Gray, 2);
                break;
            case Colors.PURPLE:
                pen = new Pen(Color.Purple, 2);
                break;
            case Colors.CYAN:
                pen = new Pen(Color.Cyan, 2);
                break;
        }

        graphics.DrawLine(pen, line.a.x.Value, line.a.y.Value, line.b.x.Value, line.b.y.Value);
        if (line.Text != null)
        {
            WritePaintText(line.Text.Value, textX, textY);
        }
    }
    void PaintSegment(Object.Segment segment)
    {
        int textX = 0;
        int textY = 0;
        if (segment.a.x.Value >= segment.b.x.Value)
        {
            textX = (int)segment.a.x.Value;
            textY = (int)segment.a.y.Value;
        }
        else
        {
            textX = (int)segment.b.x.Value;
            textY = (int)segment.b.y.Value;
        }

        switch (segment.Colors)
        {
            case Colors.RED:
                pen = new Pen(Color.Red, 2);
                break;
            case Colors.YELLOW:
                pen = new Pen(Color.Yellow, 2);
                break;
            case Colors.BLUE:
                pen = new Pen(Color.Blue, 2);
                break;
            case Colors.WHITE:
                pen = new Pen(Color.White, 2);
                break;
            case Colors.BLACK:
                pen = new Pen(Color.Black, 2);
                break;
            case Colors.GREEN:
                pen = new Pen(Color.Green, 2);
                break;
            case Colors.GRAY:
                pen = new Pen(Color.Gray, 2);
                break;
            case Colors.PURPLE:
                pen = new Pen(Color.Purple, 2);
                break;
            case Colors.CYAN:
                pen = new Pen(Color.Cyan, 2);
                break;
        }

        graphics.DrawLine(pen, segment.a.x.Value, segment.a.y.Value, segment.b.x.Value, segment.b.y.Value);
        if (segment.Text != null)
        {
            WritePaintText(segment.Text.Value, textX, textY);
        }
    }
    void PaintRay(Object.Ray ray)
    {
        int textX = 0;
        int textY = 0;
        if (ray.a.x.Value >= ray.b.x.Value)
        {
            textX = (int)ray.a.x.Value;
            textY = (int)ray.a.y.Value;
        }
        else
        {
            textX = (int)ray.b.x.Value;
            textY = (int)ray.b.y.Value;
        }

        switch (ray.Colors)
        {
            case Colors.RED:
                pen = new Pen(Color.Red, 2);
                break;
            case Colors.YELLOW:
                pen = new Pen(Color.Yellow, 2);
                break;
            case Colors.BLUE:
                pen = new Pen(Color.Blue, 2);
                break;
            case Colors.WHITE:
                pen = new Pen(Color.White, 2);
                break;
            case Colors.BLACK:
                pen = new Pen(Color.Black, 2);
                break;
            case Colors.GREEN:
                pen = new Pen(Color.Green, 2);
                break;
            case Colors.GRAY:
                pen = new Pen(Color.Gray, 2);
                break;
            case Colors.PURPLE:
                pen = new Pen(Color.Purple, 2);
                break;
            case Colors.CYAN:
                pen = new Pen(Color.Cyan, 2);
                break;
        }

        graphics.DrawLine(pen, ray.a.x.Value, ray.a.y.Value, ray.b.x.Value, ray.b.y.Value);
        if (ray.Text != null)
        {
            WritePaintText(ray.Text.Value, textX, textY);
        }
    }
    void PaintArc(Object.Arc arc)
    {
        Point[] curvePoints = new Point[]
        {
            new Point((int)arc.a.x.Value,(int)arc.a.y.Value),
            new Point((int)arc.b.x.Value,(int)arc.b.y.Value),
            new Point((int)arc.c.x.Value,(int)arc.c.y.Value)
        };

        switch (arc.Colors)
        {
            case Colors.RED:
                pen = new Pen(Color.Red, 2);
                break;
            case Colors.YELLOW:
                pen = new Pen(Color.Yellow, 2);
                break;
            case Colors.BLUE:
                pen = new Pen(Color.Blue, 2);
                break;
            case Colors.WHITE:
                pen = new Pen(Color.White, 2);
                break;
            case Colors.BLACK:
                pen = new Pen(Color.Black, 2);
                break;
            case Colors.GREEN:
                pen = new Pen(Color.Green, 2);
                break;
            case Colors.GRAY:
                pen = new Pen(Color.Gray, 2);
                break;
            case Colors.PURPLE:
                pen = new Pen(Color.Purple, 2);
                break;
            case Colors.CYAN:
                pen = new Pen(Color.Cyan, 2);
                break;
        }
        graphics.DrawCurve(pen, curvePoints);
    }
    void WritePaintText(string text, int x, int y)
    {
        solidBrush = new SolidBrush(Color.Black);
        Font font = new Font("Arial", 12);
        graphics.DrawString(text, font, solidBrush, new PointF(x, y));
    }
    string WritePrintsText()
    {
        string message = "";
        if (Prints.Count > 0)
        {
            foreach (var item in Prints)
            {
                message += "> " + item.Value + Environment.NewLine;
            }
        }
        return message;
    }
}
