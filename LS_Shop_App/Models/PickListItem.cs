using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        
        public string Color
        {
            get 
            {
                string sku2Parse = this.Sku;
                string[] parts = sku2Parse.Split('-');
                if (parts.Length >= 3)
                {
                    return parts[2];
                }
                else
                {
                    return "Transparent";
                }
            }
        }

        public PickListItem() { }

        public PickListItem(SignDefinition def)
        {
            this.Sku = def.Sku;
            this.Width = def.Width;
            this.Height = def.Height;
            this.PrintTwice = def.PrintTwice;
            this.ImagePath= def.ImagePath;
        }
    }
}
