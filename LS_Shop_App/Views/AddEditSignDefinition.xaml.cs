using Dapper;
using Ghostscript.NET.Rasterizer;
using LS_Shop_App.Data;
using LS_Shop_App.Models;
using LS_Shop_App.Utilities;
using LS_Shop_App.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing.Imaging;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using SixLabors.ImageSharp.PixelFormats;
using Image = SixLabors.ImageSharp.Image;
using System.Windows.Navigation;
using LS_Shop_App.Utilities.Converters;

namespace LS_Shop_App.Views
{
    /// <summary>
    /// Interaction logic for AddEditSignDefinition.xaml
    /// </summary>
    public partial class AddEditSignDefinition : Window
    {
        Database db;
        public static event Action WindowClosing = delegate { };
        bool isUpdateMode = false;

        public AddEditSignDefinition()
        {
            InitializeComponent();
            this.DataContext = new AddEditSignDefinitionsViewModel();              
            db = new Database();
        }

        public AddEditSignDefinition(SignDefinition sign)
        {
            InitializeComponent();
            this.DataContext = new AddEditSignDefinitionsViewModel(sign.ImagePath);
            db = new Database();
            this.isUpdateMode = true;
            LoadView(sign);
        }

        public void LoadView(SignDefinition sign) 
        {
            isUpdateMode = true;
            AddEditSignDefinitionsNameTextBox.Text = sign.Sku;
            AddEditSignDefinitionsImagePathTextBox.Text = sign.ImagePath;
            WidthText.Text = sign.Width.ToString()+"in";
            HeightText.Text = sign.Height.ToString()+"in";
            AddEditSignDefinitionsCheckBox.IsChecked = false;
            if (sign.PrintTwice.Equals("1"))
            {
                AddEditSignDefinitionsCheckBox.IsChecked = true;
            }
            AddSignButton.Content = "Update Sign";
            AddEditSignDefinitionsNameTextBox.IsEnabled = false;
        }

        private void AddEditSignDefinitionsImagePathTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                AddEditSignDefinitionsImagePathTextBox.Text = openFileDialog.FileName;
            }

            Regex regex = new Regex(@"\s");

            if (regex.IsMatch(AddEditSignDefinitionsImagePathTextBox.Text))
            {
                ShowError("Name Or ImagePath cannot have spaces. \nMay have to move location of imagePath");
            }
        }

        private void AddSignButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                int printColor = (bool)AddEditSignDefinitionsCheckBox.IsChecked ? 1 : 0;

                SignDefinition def = new SignDefinition();
                if (!isUpdateMode)
                {
                    db.CreateSign(AddEditSignDefinitionsNameTextBox.Text,
                    AddEditSignDefinitionsImagePathTextBox.Text,
                    WidthText.Text.Replace("in",""),
                    HeightText.Text.Replace("in",""),
                    printColor);
                } else
                {
                    db.UpdateSign(AddEditSignDefinitionsNameTextBox.Text,
                    AddEditSignDefinitionsImagePathTextBox.Text,
                    WidthText.Text.Replace("in",""),
                    HeightText.Text.Replace("in",""),
                    printColor); ;
                }
                

                this.Close();
            }
        }

        private bool ValidateInput() 
        {
            bool isValid = true;

            //validate name
            if (AddEditSignDefinitionsNameTextBox.Text.Length <= 0)
            {
                ShowError("Each Sign Needs a name");
                isValid = false;
            }
            
            if (db.GetSignDefinition(AddEditSignDefinitionsNameTextBox.Text) != null && isUpdateMode != true)
            {
                ShowError("Proposed name of new sign already exists");
                isValid = false;
            }

            //validate image path
            if (AddEditSignDefinitionsImagePathTextBox.Text.Length <= 0) 
            { 
                ShowError("Each sign needs a valid Image"); 
                isValid = false;
            }

            //validate sizes
            if (WidthText.Text.Length <= 0 || HeightText.Text.Length <= 0)
            {
                ShowError("Must have both width and height defined");
                isValid = false;
            }

            return isValid;
        }

        private void NameImagePath_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"\s");

            if (regex.IsMatch(e.Text))
            {
                ShowError("Name Or ImagePath cannot have spaces. \nMay have to move location of imagePath.");
            }

            e.Handled = regex.IsMatch(e.Text);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SuggestionText.Visibility = Visibility.Hidden;
            ErrorLabel.Visibility = Visibility.Hidden;

            if (AddEditSignDefinitionsNameTextBox.Text.Count(c => c == '-') == 1 || AddEditSignDefinitionsNameTextBox.Text.Contains("-black"))
            {
                SuggestionText.Visibility = Visibility.Visible;
                SuggestionText.Foreground = Brushes.Green;
                SuggestionText.Content = "<- Will this board to be printed twice?";
            }
        }

        private void ShowError(string error)
        {
            ErrorLabel.Visibility = Visibility.Visible;
            ErrorLabel.Content = error;
            ErrorLabel.Foreground = Brushes.Red;
        }

        private void AddEditSignDefinitionImage_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            ShowError("Failed to load image. \nMake sure ImagePath is correct.");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            WindowClosing.Invoke();
        }

        private void AddEditSignDefinitionsImagePathTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(AddEditSignDefinitionsImagePathTextBox.Text))
            {
                ShowError("Unable to find Image for defined path!");
                return;
            }

            Regex regex = new Regex(@"\s");
            if (regex.IsMatch(AddEditSignDefinitionsImagePathTextBox.Text))
            {
                ShowError("Name Or ImagePath cannot have spaces. \nMay have to move location of imagePath.");
                return;
            }

            string newFile = AddEditSignDefinitionsImagePathTextBox.Text.Substring(0, AddEditSignDefinitionsImagePathTextBox.Text.Length - 4);
            newFile += ".png";

            if (AddEditSignDefinitionsImagePathTextBox.Text.Substring(AddEditSignDefinitionsImagePathTextBox.Text.Length - 4).Equals(".pdf"))
            {
                if (!File.Exists(newFile))
                {
                    string ghostScriptPath = @"C:\Program Files\gs\gs10.02.1\bin\gswin64.exe";
                    String ars = "-dNOPAUSE -q -r300 -sDEVICE=png16m -o" + newFile + " " + AddEditSignDefinitionsImagePathTextBox.Text;
                    Process proc = new Process();
                    proc.StartInfo.FileName = ghostScriptPath;
                    proc.StartInfo.Arguments = ars;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    proc.Start();
                    proc.WaitForExit();

                    AddEditSignDefinitionsImagePathTextBox.Text = newFile;
                    PopulateImageSizeFields(newFile);
                }
                else
                {
                    AddEditSignDefinitionsImagePathTextBox.Text = newFile;
                }
            } 
            else
            {
                PopulateImageSizeFields(AddEditSignDefinitionsImagePathTextBox.Text);
            }
        }

        private void PopulateImageSizeFields(string imgPath)
        {
            try
            {
                var imgObj = Image.Load<Rgba32>(imgPath);
                if (imgObj != null)
                {
                    double calcWidth = Math.Round((double)imgObj.Width / 300, 2);
                    double calcHeight = Math.Round((double)imgObj.Height / 300, 2);
                    WidthText.Text = calcWidth.ToString()+"in";
                    HeightText.Text = calcHeight.ToString()+"in";
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex.ToString());
            }
                
        }
    }
}
