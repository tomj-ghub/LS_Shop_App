using LS_Shop_App.Data;
using LS_Shop_App.Utilities;
using LS_Shop_App.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            if (!File.Exists(Paths.DatabaseFile))
            {
                new Database().CreateDatabaseFile();
            }

            if (!Directory.Exists(Paths.PickListDir))
                Directory.CreateDirectory(Paths.PickListDir);
            if (!Directory.Exists(Paths.Boards2Print))
                Directory.CreateDirectory(Paths.Boards2Print);
            if (!Directory.Exists(Paths.ImagesDir))
                Directory.CreateDirectory(Paths.ImagesDir);

            SetupDatabase();
        }

        private static void SetupDatabase()
        {
            Database database = new Database();
            database.CreateTables();
        }

        private void OpenJobs_HeaderButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Inventory_HeaderButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ManageSigns_HeaderButton_Click(object sender, RoutedEventArgs e)
        {
            LoadView(new ManageSignsViewModel());
        }

        private void BuildBoards_HeaderButton_Click(object sender, RoutedEventArgs e)
        {
            LoadView(new BuildBoardsViewModel());
        }

        private void LoadView(object viewModel)
        {
            try
            {
                MainWindowContentControl.DataContext = viewModel;
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex.ToString());
            }
        }
    }
}
