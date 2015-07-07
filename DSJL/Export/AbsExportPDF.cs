using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text;
using System.Xml.Linq;
using iTextSharp.text.pdf;

namespace DSJL.Export
{
    /// <summary>
    /// 导出pdf报告抽象类
    /// </summary>
    abstract class AbsExportPDF
    {
        protected Document pdfDoc;

        protected XDocument xdoc;

        protected Font fontTitle = new Font();//表格头字体
        protected Font fontContent = new Font();//内容字体
        protected Font fontLabel = new Font();//测试名称字体
        protected Font fontTableRemark = new Font();//表格备注字体

        /// <summary>
        /// 用report.xml的xdocument实例化
        /// </summary>
        /// <param name="doc">report内容</param>
        public AbsExportPDF(XDocument doc) {
            xdoc = doc;

            pdfDoc = new Document(PageSize.A4, 0, 0, 0, 0);
            Rectangle rect = new Rectangle(794, 1123);
            pdfDoc.SetPageSize(rect);
            pdfDoc.SetMargins(0, 0, 20, 0);

            BaseFont.AddToResourceSearch("iTextAsian.dll");
            BaseFont.AddToResourceSearch("iTextAsianCmaps.dll");
            BaseFont bsFont;
            bsFont = BaseFont.CreateFont(@"c:\WINDOWS\fonts\SIMSUN.TTC,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            fontTitle = new Font(bsFont, 20);
            fontContent = new Font(bsFont, 12);
            fontLabel = new Font(bsFont, 12);
            fontTableRemark = new Font(bsFont, 14);
            fontLabel.SetStyle(Font.BOLD);
            fontTitle.SetStyle(Font.BOLD);
            fontTableRemark.SetStyle(Font.BOLD);
        }

        /// <summary>
        /// 导出报告
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>导出结果</returns>
        public abstract bool Export(string fileName);

        /// <summary>
        /// 添加标题
        /// </summary>
        protected void AddTitle()
        {
            string title = xdoc.Descendants("title").ElementAt(0).Value;
            string reportdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            Paragraph parTitle = new Paragraph(title, fontTitle);
            parTitle.Alignment = Element.ALIGN_CENTER;
            parTitle.SpacingBefore = 50;
            Paragraph parReportDate = new Paragraph("报告时间：" + reportdate, fontContent);
            parReportDate.Alignment = Element.ALIGN_RIGHT;
            parReportDate.IndentationRight = 24;
            pdfDoc.Add(parTitle);
            pdfDoc.Add(parReportDate);
        }

        /// <summary>
        /// 添加图片
        /// </summary>
        /// <param name="parName"></param>
        /// <param name="imgName"></param>
        protected void AddImg(string parName, string imgName)
        {
            Paragraph titlePar = new Paragraph(parName, fontTableRemark);
            titlePar.IndentationLeft = 24;
            titlePar.SpacingBefore = 5;
            pdfDoc.Add(titlePar);

            Paragraph imgPar = new Paragraph();
            imgPar.IndentationLeft = 20;
            imgPar.IndentationRight = 20;
            Image imgCla = Image.GetInstance(imgName);
            imgCla.ScalePercent(24f);
            imgCla.ScaleAbsoluteHeight(240);
            imgPar.Add(imgCla);
            pdfDoc.Add(imgPar);
        }
        /// <summary>
        /// 创建测试者信息和测试信息表格
        /// </summary>
        /// <param name="tableEle"></param>
        /// <param name="columnCount"></param>
        protected void CreateTestInfoTable(XElement tableEle, int columnCount)
        {
            string remark = tableEle.Attribute("remark").Value;
            Paragraph parTableRemark = new Paragraph(remark, fontTableRemark);
            parTableRemark.IndentationLeft = 24;
            parTableRemark.SpacingBefore = 20;
            pdfDoc.Add(parTableRemark);

            PdfPTable table = new iTextSharp.text.pdf.PdfPTable(columnCount);
            List<PdfPRow> rowList = new List<iTextSharp.text.pdf.PdfPRow>();
            foreach (XElement rowEle in tableEle.Elements())
            {
                List<PdfPCell> cellList = new List<iTextSharp.text.pdf.PdfPCell>();
                foreach (XElement cellEle in rowEle.Elements())
                {
                    string label = cellEle.Attribute("label").Value;
                    string value = cellEle.Attribute("value").Value;

                    iTextSharp.text.pdf.PdfPCell cellLabel = new iTextSharp.text.pdf.PdfPCell(new Phrase(label, fontLabel));
                    cellLabel.FixedHeight = 24;
                    cellLabel.Padding = 4;
                    cellLabel.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                    cellLabel.VerticalAlignment = PdfPCell.ALIGN_CENTER;

                    PdfPCell cellContent = new iTextSharp.text.pdf.PdfPCell(new Phrase(value, fontContent));
                    cellContent.FixedHeight = 24;
                    cellContent.PaddingTop = 4;
                    cellContent.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    XAttribute colSpanAtt = cellEle.Attribute("colspan");
                    if (colSpanAtt != null)
                    {
                        string colspan = colSpanAtt.Value;
                        if (colspan != "")
                        {
                            cellContent.Colspan = int.Parse(colspan);
                        }
                    }
                    cellList.Add(cellLabel);
                    cellList.Add(cellContent);
                }
                PdfPRow row = new iTextSharp.text.pdf.PdfPRow(cellList.ToArray<PdfPCell>());
                rowList.Add(row);
            }
            table.Rows.AddRange(rowList);
            table.KeepTogether = true;
            table.SpacingBefore = 10;
            table.TotalWidth = 750;
            table.LockedWidth = true;
            Paragraph pTable = new Paragraph();
            pTable.Add(table);
            pdfDoc.Add(pTable);
        }

        /// <summary>
        /// 创建参数表格
        /// </summary>
        /// <param name="tableEle"></param>
        /// <param name="columnCount"></param>
        protected void CreateCompareTable(XElement tableEle, int columnCount)
        {
            string remark = tableEle.Attribute("remark").Value;
            Paragraph parTableRemark = new Paragraph(remark, fontTableRemark);
            parTableRemark.IndentationLeft = 24;
            parTableRemark.SpacingBefore = 20;
            pdfDoc.Add(parTableRemark);

            PdfPTable table = new iTextSharp.text.pdf.PdfPTable(new float[] { 350, 100, 100, 100, 100 });
            List<PdfPRow> rowList = new List<iTextSharp.text.pdf.PdfPRow>();
            for (int i = 0; i < tableEle.Elements().Count(); i++)
            {
                XElement rowEle = tableEle.Elements().ElementAt(i);
                List<PdfPCell> cellList = new List<iTextSharp.text.pdf.PdfPCell>();
                foreach (XElement cellEle in rowEle.Elements())
                {
                    string label = cellEle.Attribute("label").Value;

                    iTextSharp.text.pdf.PdfPCell cellLabel = null;
                    if (i < 2)
                    {
                        cellLabel = new iTextSharp.text.pdf.PdfPCell(new Phrase(label, fontLabel));
                        cellLabel.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    }
                    else
                    {
                        cellLabel = new iTextSharp.text.pdf.PdfPCell(new Phrase(label, fontContent));
                        cellLabel.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    }
                    cellLabel.FixedHeight = 24;
                    cellLabel.Padding = 4;

                    cellLabel.VerticalAlignment = PdfPCell.ALIGN_CENTER;

                    XAttribute colSpanAtt = cellEle.Attribute("colspan");
                    if (colSpanAtt != null)
                    {
                        string colspan = colSpanAtt.Value;
                        if (colspan != "")
                        {
                            cellLabel.Colspan = int.Parse(colspan);
                        }
                    }
                    XAttribute rowSpanAtt = cellEle.Attribute("rowspan");
                    if (rowSpanAtt != null)
                    {
                        string rowspan = rowSpanAtt.Value;
                        if (rowspan != "")
                        {
                            cellLabel.Rowspan = int.Parse(rowspan);
                        }
                    }


                    cellList.Add(cellLabel);
                }
                PdfPRow row = new iTextSharp.text.pdf.PdfPRow(cellList.ToArray<PdfPCell>());
                rowList.Add(row);
            }
            table.Rows.AddRange(rowList);
            //table.KeepTogether = true;
            table.SpacingBefore = 10;
            table.TotalWidth = 750;

            table.LockedWidth = true;
            Paragraph pTable = new Paragraph();
            pTable.Add(table);
            pdfDoc.Add(pTable);
        }

        /// <summary>
        /// 创建参数表格
        /// </summary>
        /// <param name="tableEle"></param>
        /// <param name="columnCount"></param>
        protected void CreateCompareTable2(XElement tableEle, int columnCount)
        {
            string remark = tableEle.Attribute("remark").Value;
            Paragraph parTableRemark = new Paragraph(remark, fontTableRemark);
            parTableRemark.IndentationLeft = 24;
            parTableRemark.SpacingBefore = 20;
            pdfDoc.Add(parTableRemark);

            PdfPTable table = new iTextSharp.text.pdf.PdfPTable(new float[] { 270, 80, 80, 80, 80, 80, 80 });
            List<PdfPRow> rowList = new List<iTextSharp.text.pdf.PdfPRow>();
            for (int i = 0; i < tableEle.Elements().Count(); i++)
            {
                XElement rowEle = tableEle.Elements().ElementAt(i);
                List<PdfPCell> cellList = new List<iTextSharp.text.pdf.PdfPCell>();
                foreach (XElement cellEle in rowEle.Elements())
                {
                    string label = cellEle.Attribute("label").Value;

                    iTextSharp.text.pdf.PdfPCell cellLabel = null;
                    if (i < 1)
                    {
                        cellLabel = new iTextSharp.text.pdf.PdfPCell(new Phrase(label, fontLabel));
                        cellLabel.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    }
                    else
                    {
                        cellLabel = new iTextSharp.text.pdf.PdfPCell(new Phrase(label, fontContent));
                        cellLabel.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    }
                    cellLabel.FixedHeight = 24;
                    cellLabel.Padding = 4;

                    cellLabel.VerticalAlignment = PdfPCell.ALIGN_CENTER;

                    XAttribute colSpanAtt = cellEle.Attribute("colspan");
                    if (colSpanAtt != null)
                    {
                        string colspan = colSpanAtt.Value;
                        if (colspan != "")
                        {
                            cellLabel.Colspan = int.Parse(colspan);
                        }
                    }
                    XAttribute rowSpanAtt = cellEle.Attribute("rowspan");
                    if (rowSpanAtt != null)
                    {
                        string rowspan = rowSpanAtt.Value;
                        if (rowspan != "")
                        {
                            cellLabel.Rowspan = int.Parse(rowspan);
                        }
                    }


                    cellList.Add(cellLabel);
                }
                PdfPRow row = new iTextSharp.text.pdf.PdfPRow(cellList.ToArray<PdfPCell>());
                rowList.Add(row);
            }
            table.Rows.AddRange(rowList);
            //table.KeepTogether = true;
            table.SpacingBefore = 10;
            table.TotalWidth = 750;

            table.LockedWidth = true;
            Paragraph pTable = new Paragraph();
            pTable.Add(table);
            pdfDoc.Add(pTable);
        }
    }
}
