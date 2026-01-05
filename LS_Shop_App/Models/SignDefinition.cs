using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

// Copyright (C) 2024 Lily and Sparrow
// This program is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation, either
// version 3 of the License, or (at your option) any later version.

namespace LS_Shop_App.Models
{
    public class SignDefinition
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string ImagePath { get; set; }
        public bool IsSelected { get; set; }
        public string Style {
            get
            {
                if (!string.IsNullOrEmpty(Sku))
                {
                    var _style = "watercolor";

                    // Find the first hyphen
                    int firstHyphen = Sku.IndexOf('-');

                    // If the first hyphen is not found, return empty.
                    if (firstHyphen == -1)
                    {
                        return "";
                    }

                    // Find the second hyphen starting after the first one.
                    int secondHyphen = Sku.IndexOf('-', firstHyphen + 1);

                    // If the second hyphen is found and is not the last character, return the substring after it.
                    if (secondHyphen != -1 && secondHyphen < Sku.Length - 1)
                    { 
                        _style = Sku.Substring(secondHyphen + 1);
                    }
                    return _style;
                }
                return "";
            }
        }
        public string Trim
        {
            get
            {
                if (!string.IsNullOrEmpty(Sku))
                {

                    int index = Sku.IndexOf('-');

                    // Check if the hyphen exists and is not at the start of the string
                    if (index > 0)
                    {
                        char candidate = Sku[index - 1];

                        // Verify that the candidate character is an alphabet letter
                        if (char.IsLetter(candidate))
                        {
                            return candidate.ToString();
                        }
                    }
                }
                return "";
            }
        }
        public string ActualDimensions
        {
            get { return this.Width + "x" + this.Height; }
        }

        public string Dimensions
        {
            get
            {
                if (string.IsNullOrEmpty(Sku))
                    return "";

                // Find the index of the first hyphen
                int hyphenIndex = Sku.IndexOf('-');
                if (hyphenIndex == -1 || hyphenIndex == 0)
                    return "";

                // If the hyphen is at index 1, then the character immediately left of hyphen is at index 0,
                // but nothing leads it, so we return an empty string.
                if (hyphenIndex == 1)
                    return "";

                char candidate = Sku[hyphenIndex - 1];

                // Verify that the candidate character is an alphabet letter
                if (char.IsLetter(candidate))
                {
                    return Sku.Substring(0, hyphenIndex - 1);
                }
                else
                {
                    return Sku.Substring(0, hyphenIndex);
                }
            }
        }

        public string Name
        {
            get
            {
                string[] parts = this.Sku.Split('-');
                return parts[1];

                // Find the index of the first hyphen
                int firstHyphenIndex = Sku.IndexOf('-');
                if (firstHyphenIndex == -1)
                {
                    return "";
                }

                // Get the substring after the first hyphen
                string substring = Sku.Substring(firstHyphenIndex + 1);

                // Check if this substring contains another hyphen
                int nextHyphenIndex = substring.IndexOf('-');
                if (nextHyphenIndex != -1)
                {
                    // Get everything before the next hyphen
                    substring = substring.Substring(0, nextHyphenIndex);
                }

                // Output the result
                return substring;
                
            }
        }

        public SignDefinition() { }

        public SignDefinition(string Sku, double Width, double Height, string ImagePath, string PrintInBW) 
        { 
            this.Sku = Sku;
            this.Width = Width;
            this.Height = Height;
            this.ImagePath = ImagePath;
        }
    }
}
