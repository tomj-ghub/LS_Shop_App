using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace LS_Shop_App.Models
{
    public class SignDefinition
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string ImagePath { get; set; }
        public string PrintTwice { get; set; }
        public bool IsSelected { get; set; }
        public string Color {
            get { if (this.Sku.Contains("-walnut"))
                    return "walnut";
                if (this.Sku.Contains("-black"))
                    return "black";
                if (this.Sku.Contains("-white"))
                    return "white";
                return "watercolor";
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
                string[] parts = this.Sku.Split('-');
                return parts[0];
            }
        }

        public string Name
        {
            get
            {
                string[] parts = this.Sku.Split('-');
                return parts[1];
            }
        }

        public SignDefinition() { }

        public SignDefinition(string Sku, double Width, double Height, string ImagePath, string PrintInBW) 
        { 
            this.Sku = Sku;
            this.Width = Width;
            this.Height = Height;
            this.ImagePath = ImagePath;
            this.PrintTwice = PrintInBW;
        }
    }
}
