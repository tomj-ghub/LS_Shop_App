using Dapper;
using LS_Shop_App.Models;
using LS_Shop_App.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Xml.Linq;

// Copyright (C) 2024 Lily and Sparrow
// This program is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation, either
// version 3 of the License, or (at your option) any later version.

namespace LS_Shop_App.Data
{
    public class SqliteDataAccessor
    {
        private string GetDbConnectionString()
        {
            return $"Data Source={Paths.DatabaseFile};Version=3;";
        }

        public void CreateSignDefinitionsTable()
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(GetDbConnectionString()))
                {
                    cnn.Execute("CREATE TABLE IF NOT EXISTS SignDefinitions (" +
                                    "Id                     INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                    "Sku                    TEXT, " +
                                    "Width                  DOUBLE, " +
                                    "Height                 DOUBLE, " +
                                    "ImagePath              TEXT," +
                                    "PrintTwice             INT); ");
                }
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e.ToString());
                return;
            }
        }

        public void CreatePickListItemsTable()
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(GetDbConnectionString()))
                {
                    cnn.Execute("CREATE TABLE IF NOT EXISTS PickListItems (" +
                                    "Id                     INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                    "SignDefinitionId       INTEGER, " +
                                    "BoardName              TEXT, " +
                                    "PickListFN            TEXT); ");
                }
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e.ToString());
                return;
            }
        }

        public ObservableCollection<SignDefinition> GetSignDefinitions()
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(GetDbConnectionString()))
                {
                    var result = cnn.Query<SignDefinition>($"SELECT * FROM SignDefinitions").ToList();
                    return new ObservableCollection<SignDefinition>(result);
                }
            }
            catch (Exception e) 
            {
                ErrorLogger.Log(e.ToString());
                return new ObservableCollection<SignDefinition>();
            }
        }

        public void DeleteSignDefinition(string sku)
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(GetDbConnectionString()))
                {
                    cnn.Execute($"DELETE FROM SignDefinitions " +
                                $"WHERE Sku = '{sku}';");
                }
            }
            catch (Exception e) 
            {
                ErrorLogger.Log(e.ToString());
                return;
            }
        }

        public SignDefinition GetSignDefinition(string sku)
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(GetDbConnectionString()))
                {
                    return cnn.Query<SignDefinition>($"SELECT * FROM SignDefinitions WHERE Sku = '{sku}'").AsEnumerable().First();
                }
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e.ToString());
                return null;
            }
        }

        public void CreateSignDefinition(string name, string path, string width, string height, int printColor)
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(GetDbConnectionString()))
                {
                    cnn.Execute($"INSERT INTO SignDefinitions " +
                                                    $"(Sku, " +
                                                    $"Width, " +
                                                    $"Height, " +
                                                    $"ImagePath, " +
                                                    $"PrintTwice) " +
                                             $"VALUES ('{name}', " +
                                                    $" '{width}', " +
                                                    $" '{height}', " +
                                                    $" '{path}', " +
                                                    $" '{printColor}');");
                }
            }
            catch (Exception e) 
            {
                ErrorLogger.Log(e.ToString());
                return;
            }
        }

        public void UpdateSignDefinition(string sku, string path, string width, string height, int printTwice)
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(GetDbConnectionString()))
                {
                    cnn.Execute($"UPDATE SignDefinitions SET " +
                                                    $"Width = {width}, " +
                                                    $"Height = {height}, " +
                                                    $"ImagePath = '{path}', " +
                                                    $"PrintTwice = {printTwice} " +
                                             $"WHERE Sku = '{sku}'; ");
                }
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e.ToString());
                return;
            }
        }

        public void CreatePickListItems(List<PickListItem> list)
        {
            foreach (PickListItem item in list)
            {
                try
                {
                    using (IDbConnection cnn = new SQLiteConnection(GetDbConnectionString()))
                    {
                        cnn.Execute($"INSERT INTO PickListItems " +
                                                        $"(SignDefinitionId, " +
                                                        $"BoardName, " +
                                                        $"PickListFN) " +
                                                 $"VALUES ('{item.Sku}', " +
                                                        $" '{item.BoardName}', " +
                                                        $" '{item.PickListFN}');");
                    }
                }
                catch (Exception e)
                {
                    ErrorLogger.Log(e.ToString());
                    return;
                }
            }
        }

        public List<PickListItem> GetPickListItems() 
        {
            List<PickListItem> returnList = new List<PickListItem>();
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(GetDbConnectionString()))
                {
                    //select p.BoardName, p.PickList_FN, d.Sku from PickListItems p join SignDefinitions d on p.SignDefinitionId = d.Sku
                    returnList = cnn.Query<PickListItem>($"SELECT p.BoardName, p.PickListFN, p.Id, d.Sku, d.Width, d.Height, d.PrintTwice, d.ImagePath " +
                        $"FROM SignDefinitions d " +
                        $"JOIN PickListItems p " +
                        $"ON d.Sku = p.SignDefinitionId").AsEnumerable().ToList();
                }
                return returnList;
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e.ToString());
                return returnList;
            }
        }

        public void DeletePickListItem(int Id)
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(GetDbConnectionString()))
                {
                    cnn.Execute($"DELETE FROM PickListItems " +
                                $"WHERE Id = '{Id}';");
                }
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e.ToString());
                return;
            }
        }

        public void DeletePickListItemBySku(string sku)
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(GetDbConnectionString()))
                {
                    cnn.Execute($"DELETE FROM PickListItems " +
                                $"WHERE  SignDefinitionId = '{sku}';");
                }
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e.ToString());
                return;
            }
        }

        public void BoardPickListItem(int Id, string boardName)
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(GetDbConnectionString()))
                {
                    cnn.Execute($"UPDATE PickListItems SET " +
                                                    $"BoardName = '{boardName}' " +
                                                    $"WHERE Id = '{Id}'; ");
                }
            }
            catch (Exception e) 
            {
                ErrorLogger.Log(e.ToString());
            }
        }

        public List<PickListItem> GetAllBoardedSigns()
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(GetDbConnectionString()))
                {
                    return cnn.Query<PickListItem>($"SELECT * FROM PickListItems WHERE BoardName IS NOT NULL AND BoardName != '' ").AsEnumerable().ToList();
                }
            }
            catch(Exception e)
            {
                ErrorLogger.Log(e.ToString());
                return new List<PickListItem>();
            }
        }

        public void CreatePickListItem(PickListItem item)
        {
                try
                {
                    using (IDbConnection cnn = new SQLiteConnection(GetDbConnectionString()))
                    {
                        cnn.Execute($"INSERT INTO PickListItems " +
                                                        $"(SignDefinitionId, " +
                                                        $"BoardName, " +
                                                        $"PickListFN) " +
                                                 $"VALUES ('{item.Sku}', " +
                                                        $" '{item.BoardName}', " +
                                                        $" '{item.PickListFN}');");
                    }
                }
                catch (Exception e)
                {
                    ErrorLogger.Log(e.ToString());
                    return;
                }
            
        }

        public void RemoveBoardName(int id)
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(GetDbConnectionString()))
                {
                    cnn.Execute($"UPDATE PickListItems SET " +
                                                    $"BoardName = '' " +
                                                    $"WHERE Id = '{id}'; ");
                }
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e.ToString());
                return;
            }
        }
    }
}
