using LS_Shop_App.Data;
using LS_Shop_App.Models;
using LS_Shop_App.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Database = LS_Shop_App.Data.Database;

namespace LS_Shop_App.Views
{

    public partial class ManageSignsView : UserControl
    {
        Database db;
        ManageSignsViewModel viewModel;
        ObservableCollection<SignDefinition> SignDefinitions;
        private delegate void UpdateUICallBack();
        List<String> ImagesSelected;

        public ManageSignsView()
        {
            InitializeComponent();
            AddEditSignDefinition.WindowClosing += AddEditSignDefinition_WindowClosing;
            db = new Database();
            viewModel = new ManageSignsViewModel();
            SignDefinitions = new ObservableCollection<SignDefinition>();
            ImagesSelected = new List<String>();
            RefreshView();
        }

        private void AddEditSignDefinition_WindowClosing()
        {
            RefreshView();
        }

        private void RefreshDataGrid()
        {
            SignDefinitionDataGrid.ItemsSource = SignDefinitions;
        }

        private void GetAllSigns()
        {
            this.SignDefinitions = db.GetSignDefinitions();
        }

        private void DeleteSignsButton_Click(object sender, RoutedEventArgs e)
        {
            AreYouSureWarningWindow AreYouSure = new AreYouSureWarningWindow("Are you sure you want to delete all selected records?");
            AreYouSure.ContinueButtonClicked += DeleteAllSelected;
            AreYouSure.Show();
        }

        private void DeleteAllSelected()
        {
            bool didDelete = false;

            foreach (SignDefinition selectedSign in SignDefinitionDataGrid.Items)
            {
                if (selectedSign.IsSelected)
                {
                    db.DeleteSign(selectedSign.Sku);
                    didDelete = true;
                }
            }
            if (didDelete) RefreshView();
        }

        public void RefreshView()
        {
            GetAllSigns();
            RefreshDataGrid();
        }

        private void CreateSignButton_Click(object sender, RoutedEventArgs e)
        {
            AddEditSignDefinition AddEditSignDefinitions= new AddEditSignDefinition();

            AddEditSignDefinitions.Show();
        }

        private void UpdateSign_Click(object sender, RoutedEventArgs e)
        {
            if (SignDefinitionDataGrid.SelectedItem != null)
            {
                AddEditSignDefinition AddEditSignDefinitions = new AddEditSignDefinition((SignDefinition)SignDefinitionDataGrid.SelectedItem);

                AddEditSignDefinitions.Show();
            }
        }

        private void AddToPickList_Click(object sender, RoutedEventArgs e)
        {
            db.AddPickListItem(new PickListItem((SignDefinition)SignDefinitionDataGrid.SelectedItem));
        }
    }
}
