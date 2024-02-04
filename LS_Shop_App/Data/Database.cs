using LS_Shop_App.Models;
using LS_Shop_App.Utilities;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_Shop_App.Data
{
    public class Database
    {
        private SqliteDataAccessor dataAccessor;

        public Database()
        {
            dataAccessor = new SqliteDataAccessor();
        }

        public void CreateDatabaseFile()
        {
            if (!Directory.Exists(Paths.DataDir))
                Directory.CreateDirectory(Paths.DataDir);

            if (!File.Exists(Paths.DatabaseFile))
                SQLiteConnection.CreateFile(Paths.DatabaseFile);
        }

        public void CreateTables()
        {
            dataAccessor.CreateSignDefinitionsTable();
            dataAccessor.CreatePickListItemsTable();
        }

        public ObservableCollection<SignDefinition> GetSignDefinitions()
        {
            return dataAccessor.GetSignDefinitions();
        }

        public void DeleteSign(string sku)
        {
            dataAccessor.DeleteSignDefinition(sku);
            dataAccessor.DeletePickListItemBySku(sku);
        }

        public SignDefinition GetSignDefinition(string sku)
        {
            return dataAccessor.GetSignDefinition(sku);
        }

        public void CreateSign(string sku, string path, string width, string height, int printTwice)
        {
            dataAccessor.CreateSignDefinition(sku, path, width, height, printTwice);
        }

        public void UpdateSign(string sku, string path, string width, string height, int printTwice)
        {
            dataAccessor.UpdateSignDefinition(sku, path, width, height, printTwice);
        }

        public void AddPickListItem(PickListItem item)
        {
            dataAccessor.CreatePickListItem(item);
        }

        public void CreatePickListItems(List<PickListItem> list)
        {
            dataAccessor.CreatePickListItems(list);
        }
        
        public List<PickListItem> GetPickListItems()
        {
            return dataAccessor.GetPickListItems();
        }

        public void DeletePickListItem(int id) 
        { 
            dataAccessor.DeletePickListItem(id);
        }

        public void BoardPickListItem(int id, string boardName)
        {
            dataAccessor.BoardPickListItem(id, boardName);
        }

        public List<PickListItem> GetAllBoardedSigns()
        {
            return dataAccessor.GetAllBoardedSigns();
        }

        public void RemoveBoardName(int id)
        {
            dataAccessor.RemoveBoardName(id);
        }
    }
}
