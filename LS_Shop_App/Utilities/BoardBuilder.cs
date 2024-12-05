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
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using LS_Shop_App.Data;
using LS_Shop_App.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;

// Copyright (C) 2024 Lily and Sparrow
// This program is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation, either
// version 3 of the License, or (at your option) any later version.

namespace LS_Shop_App.Utilities
{
    /**
     *  
    */
    public class BoardBuilder
    {
        public PickListItem Root { get; private set; }

        string boardName;
        string outputFilePath;
        double canvasWidthIn, canvasHeightIn;
        float lineWidth;
        List<PickListItem> signs2Pars;
        Database db = new Database();

        //this is the size of the board
        public BoardBuilder(double canvasWidthIn, double canvasHeightIn, string boardName, double boarderMargin, double lineWidth) 
        {
            Root = new PickListItem { X = 0, Y = canvasHeightIn, Width = canvasWidthIn, Height = canvasHeightIn };
            this.boardName = boardName;
            this.canvasWidthIn= canvasWidthIn;
            this.canvasHeightIn= canvasHeightIn;
            this.lineWidth = (float)lineWidth;
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
            node.Down = new PickListItem { X = node.X, Y = node.Y - height, Width = node.Width, Height = node.Height - height };
            node.Right = new PickListItem { X = node.X + width, Y = node.Y, Width = node.Width - width, Height = height };
            return node;
        }

        private void PaintBoard()
        {
            try
            {
                PdfWriter writer = new PdfWriter(Paths.Boards2Print + boardName + ".pdf");
                PdfDocument pdfDocument = new PdfDocument(writer);
                // Set the size of the new page
                PageSize pageSize = new PageSize((float)canvasWidthIn*72, (float)canvasHeightIn*72); // Adjust the size as necessary
                PdfPage page = pdfDocument.AddNewPage(pageSize);
                PdfCanvas pdfCanvas = new PdfCanvas(page);
                foreach (PickListItem item in signs2Pars)
                {
                    if (item.Fit != null)
                    {
                        PdfDocument srcPdf = new PdfDocument(new PdfReader(item.ImagePath));
                        PdfFormXObject pageCopy = srcPdf.GetFirstPage().CopyAsFormXObject(pdfDocument);

                        pdfCanvas.AddXObjectAt(pageCopy, (float)(item.Fit.X*72)+(lineWidth*72), (float)((item.Fit.Y*72)-(item.Height)*72));
                        //pdfCanvas.AddXObjectAt(pageCopy, 0, );
                        //pdfCanvas.Release();
                        srcPdf.Close();
                    }
                }
                pdfDocument.Close();
            } 
            catch(Exception ex)
            {
                ErrorLogger.Log(ex.ToString());
            }
        }
    }
}
