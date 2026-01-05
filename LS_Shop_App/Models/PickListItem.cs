using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Copyright (C) 2024 Lily and Sparrow
// This program is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation, either
// version 3 of the License, or (at your option) any later version.

namespace LS_Shop_App.Models
{
    public class PickListItem : SignDefinition
    {
        public String PickListFN { get; set; }
        public String BoardName { get; set; }

        public double X { get; set; }
        public double Y { get; set; }

        //this is the sign directly below this sign on the given board
        public PickListItem Down { get; set; }

        //this is the sign directly to the right of this sign on the given board
        public PickListItem Right { get; set; }

        //this is the 'box' that the sign fits into on the canvas
        public PickListItem Fit { get; set; }

        public double Area
        {
            get { return (this.Width * this.Height); }
        }
        
        public string Style
        {
            get 
            {
                if (!string.IsNullOrEmpty(this.Sku))
                {
                    var _style = "watercolor";

                    // Find the first hyphen
                    int firstHyphen = this.Sku.IndexOf('-');

                    // If the first hyphen is not found, return empty.
                    if (firstHyphen == -1)
                    {
                        return "";
                    }

                    // Find the second hyphen starting after the first one.
                    int secondHyphen = this.Sku.IndexOf('-', firstHyphen + 1);

                    // If the second hyphen is found and is not the last character, return the substring after it.
                    if (secondHyphen != -1 && secondHyphen < this.Sku.Length - 1)
                    {
                        _style = this.Sku.Substring(secondHyphen + 1);
                    }
                    return _style;
                }
                return "";
            }
        }

        public PickListItem() { }

        public PickListItem(SignDefinition def)
        {
            this.Sku = def.Sku;
            this.Width = def.Width;
            this.Height = def.Height;
            //this.PrintTwice = def.PrintTwice;
            this.ImagePath= def.ImagePath;
        }
    }
}
