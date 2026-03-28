using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using WindowsAPICodePack.Dialogs;

namespace LS_Shop_App.Views
{
    /// <summary>
    /// Interaction logic for CreateBoardInputWindow.xaml
    /// </summary>
    public partial class CreateBoardInputWindow : Window
    {
        public double boardWidth { get; private set; }
        public double boardHeight { get; private set; }
        public double lineMargin { get; private set; }
        public double boardMargin { get; private set; }
        public string boardName { get; private set; }
        public string targetDir { get; private set; }

        public CreateBoardInputWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (BoardWidth.Text.Length == 0 || BoardHeight.Text.Length == 0 || LineMargin.Text.Length == 0 || BoardMargin.Text.Length == 0
                || TargetDir.Text.Length == 0) return;
            if (Double.Parse(BoardMargin.Text) < .75) return;
            if (!Directory.Exists(TargetDir.Text)) return;
            Close();
        }

        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string newLMText = LineMargin.Text.Insert(LineMargin.SelectionStart, e.Text); 
            if (!double.TryParse(newLMText, out _)) 
            { 
                e.Handled = true; 
            }

            string newBMText = BoardMargin.Text.Insert(BoardMargin.SelectionStart, e.Text);
            if (!double.TryParse(newBMText, out _))
            {
                e.Handled = true;
            }
        }

        private void Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                string text = (string)e.DataObject.GetData(typeof(String));
                if (!double.TryParse(text, out _))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            boardWidth = Double.Parse(BoardWidth.Text);
            boardHeight = Double.Parse(BoardHeight.Text);
            lineMargin = Double.Parse(LineMargin.Text);
            boardMargin = Double.Parse(BoardMargin.Text);
            targetDir = TargetDir.Text;
            boardName = BoardName.Text;
        }

        private void TargetDirTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            var dialog = new CommonOpenFileDialog { IsFolderPicker = true, Title = "Select the target directory" };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                TargetDir.Text = dialog.FileName;
            }
        }
    }
}
