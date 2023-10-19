using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;


namespace EnumDeneme
{
    
    public class MyData
    {
        public string Key { get; set; }
        public List<string> Values { get; set; }
    }

    public partial class MainWindow : Window
    {
        public List<ComboBox> comboBoxs = new List<ComboBox>();
        public List<Label> labels = new List<Label>();
        public string csText = string.Empty;
        Dictionary<string, List<string>> enums;
        

        public MainWindow()
        {
            InitializeComponent();
            viewEnums();
        }

        public void viewEnums(string path = null) 
        {
            comboBoxs.Clear();
            labels.Clear();
            stackPanel.Children.Clear();

            if (path == null)
            {
                csText = File.ReadAllText("C:\\Users\\PC_4232\\Desktop\\Mehmet\\newEnums.cs");
            }
            else
            {
                csText = File.ReadAllText(path);
            }
            

            List<string> comboList = new List<string>();
            
            stackPanel.HorizontalAlignment = HorizontalAlignment.Center;

            enums = ProcessEnumCode(csText);

            foreach (var key in enums.Keys)
            {
                comboList.Add(key);
            }

            foreach (var kvp in enums)
            {
                Label label = new Label();
                label.FontSize = 16;
                label.Foreground = Brushes.White;
                label.Content = kvp.Key;
                stackPanel.Children.Add(label);

                foreach (var value in kvp.Value)
                {
                    StackPanel valueAndComboPanel = new StackPanel();
                    valueAndComboPanel.Background = Brushes.LightBlue;
                    valueAndComboPanel.Orientation = Orientation.Horizontal;
                    valueAndComboPanel.HorizontalAlignment = HorizontalAlignment.Center;

                    Label valueLabel = new Label();
                    valueLabel.Width = 100;
                    valueLabel.Content = value;

                    ComboBox comboBox = new ComboBox();
                    comboBoxs.Add(comboBox);
                    labels.Add(valueLabel);

                    foreach (var comboValue in comboList)
                    {
                        if (kvp.Key != comboValue)
                        {
                            comboBox.Items.Add(comboValue);
                        }
                    }

                    comboBox.Items.Add(string.Empty);

                    valueAndComboPanel.Children.Add(valueLabel);
                    valueAndComboPanel.Children.Add(comboBox);

                    stackPanel.Children.Add(valueAndComboPanel);
                }
            }

            TextBlock textBlock = new TextBlock();
            textBlock.Text = "\r\n\r\n";
            stackPanel.Children.Add(textBlock);

            Button KaydetButton = new Button();
            KaydetButton.Content = "Kaydet";
            KaydetButton.Click += KaydetClick;
            stackPanel.Children.Add(KaydetButton);

            TextBlock textBlock2 = new TextBlock();
            textBlock2.Text = "\r\n\r\n";
            stackPanel.Children.Add(textBlock2);
        }

        public Dictionary<string, List<string>> ProcessEnumCode(string enumCode)
        {
            Dictionary<string, List<string>> enums = new Dictionary<string, List<string>>();


            var syntaxTree = CSharpSyntaxTree.ParseText(enumCode);
            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.GetExecutingAssembly().Location)
            };

            var compilation = CSharpCompilation.Create("DynamicEnumCompilation")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(references)
                .AddSyntaxTrees(syntaxTree);

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    Console.WriteLine("Derleme hatası:\n" + string.Join("\n", result.Diagnostics));
                    Environment.Exit(0);
                }

                ms.Seek(0, SeekOrigin.Begin);
                Assembly assembly = Assembly.Load(ms.ToArray());

                //List<string> enumOutput = new List<string>();

                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsEnum)
                    {
                        List<string> enumValue = new List<string>();
                        foreach (var value in Enum.GetValues(type))
                        {

                            enumValue.Add(value.ToString());
                        }

                        enums.Add(type.Name, enumValue);
                    }
                }

                return enums;
            }
        }

        private void BrowseClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();

            Nullable<bool> result = openFileDlg.ShowDialog();

            if (result == true)
            {
                FileNameTextBox.Text = openFileDlg.FileName;               
            }
        }

        private void OKClick(object sender, RoutedEventArgs e)
        {
            viewEnums(FileNameTextBox.Text);
        }

        private void KaydetClick(object sender, RoutedEventArgs e)
        {
            string newCsText = csText;



            //for (int i = 0; i < enums.Count; i++)
            //{
            //    MessageBox.Show("enum " + enums.Values.ElementAt(i).ToString() + "  " + "enum " + enums.Keys.ElementAt(i).ToString());
            //    //Console.WriteLine(matchedEnum.Values.ElementAt(i) + "  " + matchedEnum.Keys.ElementAt(i));
            //    newCsText = newCsText.Replace("enum " + enums.Values.ElementAt(i).ToString(), "enum " + enums.Keys.ElementAt(i).ToString());
            //}

            for (int i = 0; i < labels.Count; i++)
            {
                if (comboBoxs[i].Text != "")
                {
                    newCsText = newCsText.Replace("enum " + comboBoxs[i].Text, "enum " + labels[i].Content);
                }
            }

            string path = FileNameTextBox.Text;
            string newPath = path.Substring(0, path.LastIndexOf("\\")) + "\\" + "new" + path.Substring(path.LastIndexOf('\\') + 1);
            if (File.Exists(newPath))
            {
                File.Delete(newPath);
                File.WriteAllText(newPath, newCsText);
            }
            else
            {
                File.WriteAllText(newPath, newCsText);
            }
            
            MessageBox.Show(path.Substring(0, path.LastIndexOf("\\")) + "\\" + "new" + path.Substring(path.LastIndexOf('\\') + 1) + " Dosyası Kaydedildi.");
        }


    }
}
