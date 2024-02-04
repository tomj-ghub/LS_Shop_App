using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using LS_Shop_App.Data;
using LS_Shop_App.Models;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;

namespace LS_Shop_App.Utilities
{
    /**
     *  
    */
    public class BoardBuilder
    {
        public PickListItem Root { get; private set; }

        XGraphics gfx;
        string boardName;
        string outputFilePath;
        double canvasWidthIn, canvasHeightIn;
        List<PickListItem> signs2Pars;
        Database db = new Database();

        //this is the size of the board
        public BoardBuilder(double canvasWidthIn, double canvasHeightIn, string boardName) 
        { 
            Root = new PickListItem { X = 0.405, Y = 0.405, Width = canvasWidthIn, Height = canvasHeightIn };
            this.boardName = boardName;
            this.canvasWidthIn= canvasWidthIn;
            this.canvasHeightIn= canvasHeightIn;
        }

        public void Fit(List<PickListItem> signs)
        {
            this.signs2Pars = signs;
            foreach (var sign in signs2Pars)
            {
                PickListItem node = FindNode(Root, sign.Width, sign.Height);
                if (node != null)
                {
                    sign.Fit = SplitNode(node, sign.Width, sign.Height);
                    db.BoardPickListItem(sign.Id, this.boardName);
                }
                else
                {
                    PaintBoard();
                    return;
                }
            }
            PaintBoard();
        }

        private PickListItem FindNode(PickListItem root, double width, double height)
        {
            if (!String.IsNullOrEmpty(root.BoardName))
            {
                var down = FindNode(root.Down, width, height);
                var right = FindNode(root.Right, width, height);          

                return right ?? down;
            }

            if (width <= root.Width && height <= root.Height)
            {
                return root;
            }

            return null;
        }

        private PickListItem SplitNode(PickListItem node, double width, double height)
        {
            node.BoardName = this.boardName;
            node.Down = new PickListItem { X = node.X, Y = node.Y + height, Width = node.Width, Height = node.Height - height };
            node.Right = new PickListItem { X = node.X + width, Y = node.Y, Width = node.Width - width, Height = height };
            return node;
        }

        private void PaintBoard()
        {
            try
            {
                this.outputFilePath = Paths.Boards2Print + boardName + ".pdf";
                PdfDocument pdf = new PdfDocument();
                PdfPage page = pdf.AddPage();
                page.Width = XUnit.FromInch(canvasWidthIn);
                page.Height = XUnit.FromInch(canvasHeightIn);
                this.gfx = XGraphics.FromPdfPage(page);

                int imageIndex = 0;
                foreach (PickListItem item in signs2Pars)
                {
                    if (item.Fit != null)
                    {
                        Image<Rgba32> currentSignImage = Image.Load<Rgba32>(item.ImagePath);
                        double widthPts1 = ((double)currentSignImage.Width / 300) * 72;
                        double heightPts1 = ((double)currentSignImage.Height / 300) * 72;

                        string tempFilePath = Paths.Boards2Print + imageIndex + "RawBoardFile.png";
                        currentSignImage.Save(tempFilePath);
                        DrawImage(gfx, tempFilePath, item.Fit.X * 72, item.Fit.Y * 72, widthPts1, heightPts1);
                        // Delete the temporary image file
                        File.Delete(tempFilePath);
                        currentSignImage.Dispose();
                        imageIndex++;
                    }
                }
                pdf.Save(outputFilePath);
            } 
            catch(Exception ex)
            {
                ErrorLogger.Log(ex.ToString());
            }
        }

        static void DrawImage(XGraphics gfx, string imagePath, double x, double y, double width, double height)
        {
            XImage image = XImage.FromFile(imagePath);
            gfx.DrawImage(image, x, y, width, height);
        }
    }
}
