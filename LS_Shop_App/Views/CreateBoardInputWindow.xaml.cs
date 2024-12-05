using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

        public CreateBoardInputWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            boardWidth = Double.Parse(BoardWidth.Text);
            boardHeight = Double.Parse(BoardHeight.Text);
            //lineMargin = Double.Parse(LineMargin.Text);
            boardMargin = 0;

            Close();
        }

        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!double.TryParse(e.Text, out _))
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
            boardMargin = 0;
        }
    }
}
