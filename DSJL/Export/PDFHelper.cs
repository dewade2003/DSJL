using System;
using System.Collections.Generic;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace DSJL.Export
{
    class PDFHelper
    {
        /// <summary>
        /// 获取pdfpcell对象
        /// </summary>
        /// <param name="str">单元格的文本</param>
        /// <param name="font">字体</param>
        /// <returns>返回pdfpcell对象</returns>
        public static PdfPCell ExtraTableCell(string str,Font font) 
        {
            PdfPCell cell = new PdfPCell(new Phrase(str, font));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;            
            cell.Padding = 5;
            cell.FixedHeight = 26;
            return cell;
        }
        /// <summary>
        /// 获取pdfpcell对象
        /// </summary>
        /// <param name="str">单元格的文本</param>
        /// <param name="font">字体</param>
        /// <param name="backColor">背景颜色</param>
        /// <returns>返回pdfpcell对象</returns>
        public static PdfPCell ExtraTableCell(string str, Font font,BaseColor backColor)
        {
            PdfPCell cell = ExtraTableCell(str, font);                   
            cell.BackgroundColor = backColor;       
            return cell;
        }

        /// <summary>
        /// 获取段落
        /// </summary>
        /// <returns>返回格式化后的段落对象</returns>
        public static Paragraph GetParagraph() 
        {
            Paragraph par = new Paragraph();
            par.SpacingBefore = 20;         
            par.IndentationLeft = 65;
            return par;
        }

        public static Paragraph GetParagraph(int spaceTop) 
        {
            Paragraph par = new Paragraph();
            par.SpacingBefore = spaceTop;
            par.IndentationLeft = 65;
            return par;
        }

        public static Paragraph GetParagraph(string str, Font font) 
        {
            Paragraph par = new Paragraph(str, font);
            par.IndentationLeft = 65;
            return par;
        }

        /// <summary>
        /// 获取格式化后的表格对象
        /// </summary>
        /// <param name="colNum">列数量</param>
        /// <returns>返回格式化后的PdfPTable对象</returns>
        public static PdfPTable GetPTable(int colNum) 
        {
            PdfPTable table = new PdfPTable(colNum);
            table.SpacingBefore = 20;
            table.HorizontalAlignment = Element.ALIGN_LEFT;
            table.TotalWidth = 664;
            table.LockedWidth = true;
            return table;
        }

        /// <summary>
        /// 获取格式化后的表格对象
        /// </summary>
        /// <param name="colNum">列数量</param>
        /// <returns>返回格式化后的PdfPTable对象</returns>
        public static PdfPTable GetPTable(int colNum,bool fixedWidth)
        {
            PdfPTable table = new PdfPTable(colNum);
            table.SpacingBefore = 20;
            table.HorizontalAlignment = Element.ALIGN_LEFT;           
            table.LockedWidth = true;
            return table;
        }
    }
}
