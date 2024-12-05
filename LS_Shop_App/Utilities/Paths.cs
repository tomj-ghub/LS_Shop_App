using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

// Copyright (C) 2024 Lily and Sparrow
// This program is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation, either
// version 3 of the License, or (at your option) any later version.

namespace LS_Shop_App.Utilities
{
    public class Paths
    {
        private static readonly string AppName = "LS_Shop_App";
        private static readonly string PublicDocs = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);

        //directories
        public static readonly string DataDir = $@"{PublicDocs}\{AppName}\Data\";
        public static readonly string PickListDir = $@"{PublicDocs}\{AppName}\PickList2Import\";
        public static readonly string Boards2Print = $@"{PublicDocs}\{AppName}\Boards2Print\";
        public static readonly string ImagesDir = $@"{PublicDocs}\{AppName}\Sign_Images\";
        public static readonly string ErrorDir = $@"{PublicDocs}\{AppName}\Errors\";

        //files
        public static readonly string DatabaseFile = $@"{PublicDocs}\{AppName}\Data\SQLite_Database.sqlite";
    }
}
