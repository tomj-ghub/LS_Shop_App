﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Copyright (C) 2024 Lily and Sparrow
// This program is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation, either
// version 3 of the License, or (at your option) any later version.

namespace LS_Shop_App.Utilities
{
    public class ErrorLogger
    {
        public static void Log(string message) 
        {
            if (!Directory.Exists(Paths.ErrorDir))
                Directory.CreateDirectory(Paths.ErrorDir);

            DateTime today = DateTime.Now;
            string fileName = today.ToString("yyyyMMdd") + "_ErrorLog.txt";
            string fullPath = Path.Combine(Paths.ErrorDir, today.ToString("yyyyMMdd") + "_ErrorLog.txt");


            try
            {
                // Loop through each file in the directory
                foreach (string file in Directory.GetFiles(Paths.ErrorDir))
                {
                    // Get the file information
                    FileInfo fileInfo = new FileInfo(file);

                    // Check if the file is older than 7 days
                    if (fileInfo.LastWriteTime < today.AddDays(-7))
                    {
                        // Delete the file
                        fileInfo.Delete();
                    }
                }

                File.AppendAllLines(fullPath, new List<string> { message });
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
