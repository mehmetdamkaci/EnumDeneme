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
using static System.Net.Mime.MediaTypeNames;


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
        public Dictionary<string, ComboBox> comboLabel = new Dictionary<string, ComboBox>();
        public List<Label> labels = new List<Label>();
        public string csText = string.Empty;
        Dictionary<string, List<string>> enums;

        string buttonContent = string.Empty;
        List<TextBox> textBoxes = new List<TextBox>();
        List<Button> buttons = new List<Button>();

        List<string> Log = new List<string>();

        List<string> matchedEnums = new List<string>();

        List<string> comboList = new List<string>();

        Window enumWindow;
        Window kaydetWindow;

        string newPath;
        public MainWindow()
        {
            InitializeComponent();
            FileNameTextBox.Text = "C:\\Users\\PC_4232\\Desktop\\Mehmet\\newEnums.cs";
            viewEnums();
        }

        public void viewEnums(string path = null) 
        {
            comboBoxs.Clear();
            labels.Clear();
            stackPanel.Children.Clear();
            Log.Clear();

            if (path == null)
            {
                csText = File.ReadAllText("C:\\Users\\PC_4232\\Desktop\\Mehmet\\newenum1.cs");
            }
            else
            {
                csText = File.ReadAllText(path);
            }


            comboList = new List<string>();
            
            stackPanel.HorizontalAlignment = HorizontalAlignment.Center;

            enums = ProcessEnumCode(csText);

            foreach (var key in enums.Keys)
            {
                comboList.Add(key);
            }

            foreach (var kvp in enums)
            {
                
               
                StackPanel comboTextBoxPanel = new StackPanel();
                comboTextBoxPanel.Orientation = Orientation.Horizontal;

                Button enumButton = new Button();
                enumButton.Background = Brushes.DimGray;
                enumButton.Foreground = Brushes.White;
                enumButton.Content = "     " + kvp.Key + "     ";
                enumButton.FontWeight = FontWeights.Bold;   
                enumButton.Name = kvp.Key;
                buttons.Add(enumButton);
                enumButton.FontSize = 16;
                enumButton.Width = 200;
                enumButton.Margin = new Thickness(0, 10, 0, 0);

                enumButton.Click += enumButtonClick;

                TextBox enumTextBox = new TextBox();
                enumTextBox.HorizontalAlignment = HorizontalAlignment.Left;
                enumTextBox.TextWrapping = TextWrapping.NoWrap;
                enumTextBox.Name = kvp.Key;
                enumTextBox.FontWeight = FontWeights.Bold;
                enumTextBox.Width = 200;
                
                textBoxes.Add(enumTextBox);

                //comboTextBoxPanel.Children.Add(enumButton);
                //comboTextBoxPanel.Children.Add(enumTextBox);

                comboTextBoxPanel.Margin = new Thickness(0, 10, 0, 0);
                stackPanel.Children.Add(enumButton);

                //stackPanel.Children.Add(enumButton);
                //stackPanel.Children.Add(enumTextBox);

                buttons.Add(enumButton);

                //Label label = new Label();
                //label.FontSize = 16;
                //label.Foreground = Brushes.White;
                //label.Content = kvp.Key;
                //stackPanel.Children.Add(label);

                //foreach (var value in kvp.Value)
                //{
                //    StackPanel valueAndComboPanel = new StackPanel();
                //    valueAndComboPanel.Background = Brushes.LightBlue;
                //    valueAndComboPanel.Orientation = Orientation.Horizontal;
                //    valueAndComboPanel.HorizontalAlignment = HorizontalAlignment.Center;

                //    Label valueLabel = new Label();                   
                //    valueLabel.Width = 100;
                //    valueLabel.Content = value;

                //    ComboBox comboBox = new ComboBox();
                //    comboBox.Foreground = Brushes.Black;
                //    comboBox.FontWeight = FontWeights.Bold;
                //    comboBoxs.Add(comboBox);
                //    labels.Add(valueLabel);

                //    foreach (var comboValue in comboList)
                //    {
                //        if (kvp.Key != comboValue)
                //        {
                //            comboBox.Items.Add(comboValue);
                //        }
                //    }

                //    comboBox.Items.Add(string.Empty);

                //    valueAndComboPanel.Children.Add(valueLabel);
                //    valueAndComboPanel.Children.Add(comboBox);

                //    stackPanel.Children.Add(valueAndComboPanel);
                //}
            }


            Button KaydetButton = new Button();
            KaydetButton.Content = "Kaydet";
            KaydetButton.Margin = new Thickness(0, 20, 0, 0);
            KaydetButton.Click += KaydetClick;
            stackPanel.Children.Add(KaydetButton);
        }


        private void enumButtonClick(object sender, RoutedEventArgs e)
        {

            enumWindow = new Window();
            enumWindow.Width = 400;
            enumWindow.Height = 400;
            enumWindow.Background = Brushes.DimGray;

            StackPanel enumStack = new StackPanel();

            enumStack.VerticalAlignment = VerticalAlignment.Center;

            Button clickedButton = (Button)sender;
            buttonContent = clickedButton.Content.ToString().Trim();
            List<string> Value = enums[buttonContent];

            foreach (var value in Value)
            {
                StackPanel valueAndComboPanel = new StackPanel();
                valueAndComboPanel.Background = Brushes.LightBlue;
                valueAndComboPanel.Orientation = Orientation.Horizontal;
                valueAndComboPanel.HorizontalAlignment = HorizontalAlignment.Center;

                Label valueLabel = new Label();
                valueLabel.Content = value;
                valueLabel.Width = 150;                
                valueLabel.FontWeight = FontWeights.Bold;

                ComboBox comboBox = new ComboBox();
                comboBox.Foreground = Brushes.Black;
                comboBox.FontWeight = FontWeights.Bold;
                comboBox.Width = 150;
                comboBox.Name = value;
                comboBoxs.Add(comboBox);
                labels.Add(valueLabel);

                //comboLabel.Add(valueLabel.Content.ToString(), comboBox);

                foreach (var comboValue in comboList)
                {
                    if (buttonContent != comboValue)
                    {
                        comboBox.Items.Add(comboValue);
                    }
                }

                comboBox.Items.Add(string.Empty);

                valueAndComboPanel.Children.Add(valueLabel);
                valueAndComboPanel.Children.Add(comboBox);

                enumStack.Children.Add(valueAndComboPanel);
            }
            


            Button enumOK = new Button();
            enumOK.Content = "OK";
            enumOK.Width = 40;
            enumOK.Click += enumOKClick;
            enumOK.Margin = new Thickness(0, 20, 0, 0);

            enumStack.Children.Add(enumOK);

            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.Content = enumStack;

            enumWindow.Content = scrollViewer;
            enumWindow.Show();

        }

        private void enumOKClick(object sender, RoutedEventArgs e)
        {

            List<string> comboText = new List<string>();
            List<string> comboNames = new List<string>();

            foreach (ComboBox combo in comboBoxs)
            {
                comboText.Add(combo.Text);
                comboNames.Add(combo.Name);
            }



            foreach (var text in comboText)
            {

                if (text != "" & comboText.Where(x => x.Equals(text)).Count() > 1)
                {
                    MessageBox.Show(text + " elemanı birden fazla enum ile eşleşemez.");
                    return;
                }
            }
            

            for(int i = 0; i < buttons.Count; i++)
            {

                foreach (var text in comboText)
                {
                    if (text != "")
                    {
                        if (buttons[i].Name == text) stackPanel.Children.Remove(buttons[i]);
                        comboList.Remove(text);
                    }
                        
                }
                
            }

            foreach(TextBox textBox in textBoxes)
            {
                if (textBox.Name == buttonContent)
                {
                    for (int i = 0; i < comboText.Count; i++)
                    {

                        if (comboText[i] != "" & !matchedEnums.Contains(comboNames[i]))
                        {
                            matchedEnums.Add(comboNames[i]);
                            string log = comboText[i] + " => " + textBox.Name + "." + comboNames[i] + "  ";
                            textBox.Text += log;
                            Log.Add(log);
                            
                        }

                    }

                }
            }

            enumWindow.Close();
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
                    //Environment.Exit(0);
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
            kaydetWindow = new Window();

            kaydetWindow.Width = 400;
            kaydetWindow.Height = 400;
            kaydetWindow.Background = Brushes.DimGray;

            kaydetWindow.Closed += kaydetWindowClosed;

            StackPanel kaydetStack = new StackPanel();

            kaydetStack.VerticalAlignment = VerticalAlignment.Center;
            kaydetStack.HorizontalAlignment = HorizontalAlignment.Center;

            Label logLabel = new Label();
            logLabel.Content = "ENUM EŞLEŞMELERİ";
            logLabel.Foreground = Brushes.White;
            logLabel.Background = Brushes.DimGray;
            logLabel.HorizontalAlignment = HorizontalAlignment.Center;
            logLabel.FontSize = 18;
            logLabel.FontWeight = FontWeights.Bold; 
            logLabel.Margin = new Thickness(0, 0, 0, 20);
            kaydetStack.Children.Add(logLabel);

            TextBox kaydetTextBox = new TextBox();
            kaydetTextBox.Width = kaydetWindow.Width - 50;
            kaydetTextBox.Height = kaydetWindow.Height - 150;
            kaydetTextBox.Margin = new Thickness(0, 0, 0, 10);
            kaydetTextBox.FontWeight = FontWeights.Bold;

            foreach(var log in Log)
            {
                kaydetTextBox.Text += log + "\r\n";
            }

            kaydetStack.Children.Add(kaydetTextBox);

            Button OkKaydetLog = new Button();
            OkKaydetLog.Content = "OK";
            OkKaydetLog.Click += OkKaydetLog_Click;
            kaydetStack.Children.Add (OkKaydetLog);

            kaydetWindow.Content = kaydetStack;


            string newCsText = csText;
            List<string> comboText = new List<string>();

            foreach (ComboBox combo in comboBoxs) 
            {
                comboText.Add(combo.Text);
            }

            foreach (var text in comboText)
            {
                
                if (text != "" & comboText.Where(x => x.Equals(text)).Count() > 1)
                {
                    
                    MessageBox.Show(text + " elemanı birden fazla enum ile eşleşemez.");
                    return;
                }
            }

            for (int i = 0; i < labels.Count; i++)
            {
                if (comboBoxs[i].Text != "")
                {
                    newCsText = newCsText.Replace("enum " + comboBoxs[i].Text, "enum " + labels[i].Content);
                }
            }            

            string path = FileNameTextBox.Text;
            newPath = path.Substring(0, path.LastIndexOf("\\")) + "\\" + "new" + path.Substring(path.LastIndexOf('\\') + 1);
            if (File.Exists(newPath))
            {
                File.Delete(newPath);
                File.WriteAllText(newPath, newCsText);
            }
            else
            {
                File.WriteAllText(newPath, newCsText);
            }

            kaydetWindow.Show();


        }

        private void OkKaydetLog_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(newPath + " Dosyası Kaydedildi.");
            kaydetWindow.Close();
        }

        private void kaydetWindowClosed(object sender, EventArgs e)
        {
            //MessageBox.Show(newPath + " Dosyası Kaydedildi.");
        }
    }
}
