using LS_Shop_App.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LS_Shop_App.Views
{
    /// <summary>
    /// Interaction logic for SignDefinitionUserControl.xaml
    /// </summary>
    public partial class SignDefinitionUserControl : UserControl
    {
        public bool IsSelected { get; set; }
        public string SignSku { get; set; }

        public SignDefinitionUserControl()
        {
            InitializeComponent();
        }

        public SignDefinitionUserControl(string sku)
        {
            InitializeComponent();
            SignSku = sku;

            if (sku.Contains("-black"))
            {
                SignDefinitionUserControlName.Background = Brushes.Black;
                SignDefinitionUserControlName.Foreground = Brushes.White;
            }
            if (sku.Contains("-white"))
            {
                SignDefinitionUserControlName.Background = Brushes.White;
                SignDefinitionUserControlName.Foreground = Brushes.Black;
            }
            if (sku.Contains("-walnut"))
            {
                SignDefinitionUserControlName.Background = Brushes.SaddleBrown;
                SignDefinitionUserControlName.Foreground = Brushes.Black;
            }
        }

        private void SignDefinitionUserControlButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsSelected)
            {
                IsSelected = false;
                SignDefinitionUserControlButton.Opacity = 1;
            }
            else
            {
                SignDefinitionUserControlButton.Opacity = .5;
                IsSelected = true;
            }
        }

        private void SignDefinitionUserControlButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AddEditSignDefinition AddEditSignDefinitions = new AddEditSignDefinition((SignDefinition) this.DataContext);

            AddEditSignDefinitions.Show();
        }
    }
}
