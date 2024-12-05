using LS_Shop_App.Data;
using LS_Shop_App.Utilities;
using LS_Shop_App.ViewModels;
using System;
using System.IO;
using System.Windows;

// Copyright (C) 2024 Lily and Sparrow
// This program is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation, either
// version 3 of the License, or (at your option) any later version.

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
