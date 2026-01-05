using Dapper;
using LS_Shop_App.Data;
using LS_Shop_App.Models;
using LS_Shop_App.Utilities;
using LS_Shop_App.ViewModels;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using iText.Kernel.Pdf;

namespace LS_Shop_App.Views
{
    // Copyright (C) 2024 Lily and Sparrow
    // This program is free software: you can redistribute it and/or modify it under the terms of
    // the GNU Affero General Public License as published by the Free Software Foundation, either
    // version 3 of the License, or (at your option) any later version.

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
            //AddEditSignDefinitionsCheckBox.IsChecked = false;
            //if (sign.PrintTwice.Equals("1"))
            //{
            //    AddEditSignDefinitionsCheckBox.IsChecked = true;
            //}
            AddSignButton.Content = "Update Sign";
            AddEditSignDefinitionsNameTextBox.IsEnabled = false;
            string newFile = AddEditSignDefinitionsImagePathTextBox.Text;
            LoadWebBrowser(newFile);
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
                //int printColor = (bool)AddEditSignDefinitionsCheckBox.IsChecked ? 1 : 0;

                SignDefinition def = new SignDefinition();
                if (!isUpdateMode)
                {
                    db.CreateSign(AddEditSignDefinitionsNameTextBox.Text,
                    AddEditSignDefinitionsImagePathTextBox.Text,
                    WidthText.Text.Replace("in", ""),
                    HeightText.Text.Replace("in", ""));
                } else
                {
                    db.UpdateSign(AddEditSignDefinitionsNameTextBox.Text,
                    AddEditSignDefinitionsImagePathTextBox.Text,
                    WidthText.Text.Replace("in",""),
                    HeightText.Text.Replace("in",""));
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
            //SuggestionText.Visibility = Visibility.Hidden;
            ErrorLabel.Visibility = Visibility.Hidden;

            //if (AddEditSignDefinitionsNameTextBox.Text.Count(c => c == '-') == 1 || AddEditSignDefinitionsNameTextBox.Text.Contains("-black"))
            //{
            //    SuggestionText.Visibility = Visibility.Visible;
            //    SuggestionText.Foreground = Brushes.Green;
            //    SuggestionText.Content = "<- Will this board to be printed twice?";
            //}
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

            string newFile = AddEditSignDefinitionsImagePathTextBox.Text;
            LoadWebBrowser(newFile);
        }

        private void LoadWebBrowser(string newFile)
        {
            if (String.IsNullOrEmpty(newFile))
            {
                return;
            }

            if (AddEditSignDefinitionsImagePathTextBox.Text.Substring(AddEditSignDefinitionsImagePathTextBox.Text.Length - 4).Equals(".pdf"))
            {
                if (File.Exists(newFile))
                {
                    AddEditSignDefinitionImage.Navigate(new Uri("about:blank"));
                    AddEditSignDefinitionImage.Navigate(newFile);
                    PopulateImageSizeFields(newFile);
                }
            }
        }

        private void PopulateImageSizeFields(string imgPath)
        {
            try
            {
                PdfDocument srcPdf = new PdfDocument(new PdfReader(imgPath));
                double calcWidth = Math.Round((double)srcPdf.GetFirstPage().GetPageSize().GetWidth()/72,2);
                double calcHeight = Math.Round((double)srcPdf.GetFirstPage().GetPageSize().GetHeight()/72,2);
                WidthText.Text = calcWidth.ToString() + "in";
                HeightText.Text = calcHeight.ToString() + "in";
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex.ToString());
            }
                
        }
    }
}
