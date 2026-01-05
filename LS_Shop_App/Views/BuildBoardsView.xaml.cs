using LS_Shop_App.Data;
using LS_Shop_App.Models;
using LS_Shop_App.Utilities;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LS_Shop_App.Views
{
    /// <summary>
    /// Interaction logic for BuildBoardsView.xaml
    /// </summary>
    public partial class BuildBoardsView : UserControl
    {
        Database db;
        List<PickListItem> pickListItems;
        CreateBoardInputWindow boardInputs;

        public BuildBoardsView()
        {
            InitializeComponent();
            this.db = new Database();
            RefreshDataGrid();          
        }

        private void ShowError(string error)
        {
            ErrorLabel.Visibility = Visibility.Visible;
            ErrorLabel.Content = error;
            ErrorLabel.Foreground = Brushes.Red;
        }

        private void ImportPickListButton_Click(object sender, RoutedEventArgs e)
        {
            List<PickListItem> proposedList = new List<PickListItem>();

            if (string.IsNullOrEmpty(File2ImportTextBox.Text)) return;
            if (!File2ImportTextBox.Text.Substring(File2ImportTextBox.Text.Length - 4).Equals(".csv"))
            {
                ShowError("Please define a csv");
                return;
            }
            if (!File.Exists(File2ImportTextBox.Text))
            {
                ShowError("Cannot find csv defined");
                return;
            }

            try
            {
                if (PickListSignsDataGrid.Items.Contains(File2ImportTextBox.Text))
                {
                    ShowError("PickList has already been imported");
                    return;
                }

                using (TextFieldParser parser = new TextFieldParser(File2ImportTextBox.Text))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.HasFieldsEnclosedInQuotes = true;
                    parser.SetDelimiters(",");
                    bool gotHeaders = false;
                    int skuIndex = 0;
                    int qtyIndex = 0;
                    while (!parser.EndOfData)
                    {
                        int colIndex = 0;
                        SignDefinition currentDef = null;
                        //Process row
                        string[] fields = parser.ReadFields();
                        foreach (string field in fields)
                        {
                            if(!gotHeaders)
                            {
                                if (field.Equals("SKU"))
                                {
                                    skuIndex = colIndex;
                                } else if (field.Equals("Quantity"))
                                {
                                    qtyIndex = colIndex;
                                }
                                colIndex++;
                                continue;
                            }

                            //parse the sku's
                            if(colIndex == skuIndex)
                            {
                                currentDef = db.GetSignDefinition(field);
                                if (currentDef == null)
                                {
                                    ShowError("Import Failed. No definition for Sku " + field);
                                    return;
                                }
                            }

                            //parse the qty's
                            if(colIndex == qtyIndex)
                            {
                                int qtyOnPickList = Int32.Parse(field);
                                for (int i = qtyOnPickList; i > 0; i--) 
                                {
                                    PickListItem newItem = new PickListItem(currentDef);
                                    newItem.PickListFN = File2ImportTextBox.Text;
                                    proposedList.Add(newItem);
                                }
                            }

                            colIndex++;
                        }
                        gotHeaders = true;
                    }
                    //if we got here, we have a list of all out picklistitem's in proposedList, now can load the dataGrid
                    db.CreatePickListItems(proposedList);
                    RefreshDataGrid();
                    File2ImportTextBox.Text = string.Empty;
                }
            } catch (Exception ex)
            {
                ShowError("Error when attempting to parse csv");
                ErrorLogger.Log(ex.ToString());
                return;
            }
        }

        private void RefreshDataGrid() 
        {
            ErrorLabel.Visibility = Visibility.Hidden;
            this.pickListItems = db.GetPickListItems();
            PickListSignsDataGrid.ItemsSource = this.pickListItems;
            PickListSignsDataGrid.Items.Refresh();
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.pickListItems.Count == 0) return;
            if (this.pickListItems[0].IsSelected)
            {
                foreach (PickListItem item in this.pickListItems) item.IsSelected = false;
            } 
            else
            {
                foreach (PickListItem item in this.pickListItems) item.IsSelected = true;
            }

            PickListSignsDataGrid.ItemsSource = this.pickListItems;
            PickListSignsDataGrid.Items.Refresh();
        }

        private void DeleteAllSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            AreYouSureWarningWindow AreYouSure = new AreYouSureWarningWindow("Are you sure you want to delete all selected records ?");
            AreYouSure.ContinueButtonClicked += DeleteAllSelected;
            AreYouSure.Show();
        }

        public void DeleteAllSelected()
        {
            bool didDelete = false;
            foreach (PickListItem item in this.pickListItems)
            {
                if (item.IsSelected)
                {
                    db.DeletePickListItem(item.Id);
                    didDelete = true;
                }
            }
            if (didDelete) RefreshDataGrid();
        }

        private void File2ImportTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                File2ImportTextBox.Text = openFileDialog.FileName;
            }
        }

        private void File2ImportTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ErrorLabel.Visibility = Visibility.Collapsed;
        }

        private void CreateBoardsButton_Click(object sender, RoutedEventArgs e)
        {

            //this is everything that has a definition "PrintTwice"
            //List<PickListItem> selectedItems_2Prints = new List<PickListItem>();
            //this is everything that has no definition of "PrintTwice"
            List<PickListItem> selectedItems_1Print = new List<PickListItem>();

            //first, only grab the signs that the user has explicitly selected
            foreach (PickListItem item in this.pickListItems)
            {
                if (item.IsSelected) { selectedItems_1Print.Add(item); }
            }

            if (selectedItems_1Print.Any())
            {
                boardInputs = new CreateBoardInputWindow();
                boardInputs.ShowDialog();
            }

            int boardNum = 1;
            if (selectedItems_1Print.Count > 0)
            {
                boardNum = CallBoardBuilder(selectedItems_1Print, boardNum, "");

                ////grab all unique sign colors out of the selected signs
                //var colors = selectedItems_1Print.Select(PickListItem => PickListItem.Style).Distinct();
                //foreach (string color in colors)
                //{
                //    var colorSpecificEnumerable = from PickListItem item in selectedItems_1Print where item.Style == color select item;
                //    List<PickListItem> colorSpecificList = colorSpecificEnumerable.ToList();
                //    boardNum = CallBoardBuilder(colorSpecificList, boardNum, color);
                //}
            }

            //if (selectedItems_2Prints.Count > 0)
            //{
            //    boardNum = CallBoardBuilder(selectedItems_2Prints, boardNum, "2Prints");
            //}
            RefreshDataGrid();
            
        }

        private int CallBoardBuilder(List<PickListItem> selectedItems, int i, string color)
        {
            List<PickListItem> sanitizedItems = SanitizePickListItems(selectedItems);
            if (sanitizedItems == null)
            {
                return i;
            }

            if (sanitizedItems.Count <= 0 ) 
            {
                return i;
            }

            //define the board name
            string currentDate = DateTime.Now.ToString("MM.dd.yy.ss");
            //string boardName = currentDate + "-" + i + "-" + color;
            string boardName = currentDate + "-" + i;

            BoardBuilder boardBuilder = new BoardBuilder(boardInputs.boardWidth, boardInputs.boardHeight, boardName, boardInputs.boardMargin, boardInputs.lineMargin);
            boardBuilder.Fit(sanitizedItems);
            i++;
            CallBoardBuilder(sanitizedItems, i, color);
            return i;
        }

        /**
         * first, grab all of the pick list items that have been placed on a board already
         * second, iterate through the unsorted list of selected records, adding them to a newList only if they haven't been boarded
         * last, sort the newList by height, then by area of the sign
         */
        private List<PickListItem> SanitizePickListItems(List<PickListItem> unsortedList)
        {
            //grab all of the signs in PickListItems that have been assigned a board
            List<PickListItem> boardedSigns = db.GetAllBoardedSigns();

            List<PickListItem> newList = new List<PickListItem>();
            foreach (PickListItem item in unsortedList)
            {
                var onBoard = (from PickListItem boardedItem in boardedSigns where boardedItem.Id == item.Id select boardedItem);
                List<PickListItem> newSign = onBoard.ToList();
                if (newSign.Count <= 0)
                {
                    newList.Add(item);
                }
            }

            if (newList.Count > 0)
            {
                return newList.OrderByDescending(o => o.Area).ThenBy(o => o.Width).ToList();
            }
            else
            {
                return newList;
            }
        }

        private void ResetSeleted_Click(object sender, RoutedEventArgs e)
        {
            AreYouSureWarningWindow AreYouSure = new AreYouSureWarningWindow("Are you sure you want to remove board names?");
            AreYouSure.ContinueButtonClicked += ResetAllSelected;
            AreYouSure.Show();
        }

        public void ResetAllSelected()
        {
            bool didReset = false;
            foreach (PickListItem item in this.pickListItems)
            {         
                if (item.IsSelected)
                {
                    db.RemoveBoardName(item.Id);
                    didReset = true;
                }
            }
            if (didReset) RefreshDataGrid();
        }
    }
}
