namespace Geo_Wall_E;

public partial class Form1 : Form
{
    static int GeoFilesCount = 0;
    static string GeoFilesPath = "";
    RichTextBox richTextBox;
    Button button1;
    Button button2;
    string input;
    List<IDraw> objects;
    List<Object.String> prints;
    List<Error> errors;

    public Form1()
    {
        InitializeComponent();
        this.Text = "Geo Wall-E";
        richTextBox = new RichTextBox();
        richTextBox.Dock = DockStyle.Fill;
        richTextBox.Font = new Font("Arial", 12, FontStyle.Regular);

        button1 = new Button();
        button1.Text = "Compile";
        button1.Size = new System.Drawing.Size(100, 50);
        button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        button1.Location = new System.Drawing.Point(this.ClientSize.Width - button1.Width - 30, this.ClientSize.Height - button1.Height - 30);
        button1.Font = new Font("Arial", 12, FontStyle.Regular);
        button2 = new Button();
        button2.Text = "Import";
        button2.Size = new System.Drawing.Size(100, 50);
        button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        button2.Location = new System.Drawing.Point(this.ClientSize.Width - button1.Width - 30, this.ClientSize.Height - button1.Height - 40 - button2.Height);
        button2.Font = new Font("Arial", 12, FontStyle.Regular);

        button1.Click += new EventHandler(button1_Click);
        button2.Click += new EventHandler(button2_Click);
        this.Controls.Add(button1);
        this.Controls.Add(button2);
        this.Controls.Add(richTextBox);
    }
    private void button1_Click(object sender, EventArgs e)
    {
        GeoFilesCount++;
        GeoFilesPath = "./GeoFiles/GeoFile" + GeoFilesCount + ".geo";
        using (StreamWriter sw = File.CreateText(GeoFilesPath))
        {
            sw.WriteLine(richTextBox.Text);
        }
        Compiler.Action action = Compiler.CompileInput(richTextBox.Text, "GsFile" + GeoFilesCount + ".geo");

        objects = action.Objects;
        prints = action.Prints;
        errors = action.Errors;
        if (errors.Count != 0)
        {
            string errorsMessage = "";
            foreach (var item in errors)
            {
                errorsMessage += item.Message + Environment.NewLine;
            }
            MessageBox.Show(errorsMessage);
        }
        else if (objects.Count != 0 || prints.Count != 0)
        {
            Form2 form2 = new Form2(objects, prints);
            form2.Show();
        }
    }
    private void button2_Click(object sender, EventArgs e)
    {
        ReplaceImports();
    }
    private void ReplaceImports()
    {
        string[] textLines = richTextBox.Text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        for (int i = 0; i < textLines.Length; i++)
        {
            if (textLines[i].Length > 6 && textLines[i].Substring(0, 6) == "import")
            {
                string path = "./GeoFiles/";
                string fileName = "";
                for (int j = 6; j < textLines[i].Length; j++)
                {
                    if (textLines[i][j] == '\"')
                    {
                        for (int k = j + 1; k < textLines[i].Length; k++)
                        {
                            if (textLines[i][k] == '\"')
                            {
                                break;
                            }
                            fileName += textLines[i][k];
                        }
                        break;
                    }
                }
                path += fileName;
                if (File.Exists(path))
                {
                    string text = File.ReadAllText(path);
                    textLines[i] = text;
                }
                else
                {
                    MessageBox.Show("File doesn't exist");
                }
            }
        }
        richTextBox.Text = string.Join(Environment.NewLine, textLines);
    }
}
