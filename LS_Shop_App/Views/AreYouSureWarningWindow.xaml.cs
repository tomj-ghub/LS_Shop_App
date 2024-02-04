using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for AreYouSureWarningWindow.xaml
    /// </summary>
    public partial class AreYouSureWarningWindow : Window
    {
        public event Action ContinueButtonClicked = delegate { };

        public AreYouSureWarningWindow(string msg)
        {
            InitializeComponent();
            MessageLabel.Content= msg;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ContinueButtonClicked.Invoke();
            this.Close();
        }
    }
}
