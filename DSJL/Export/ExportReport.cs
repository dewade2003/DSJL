using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.xml;
using System.Xml.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace DSJL.Export
{
    class ExportReport
    {
        Document pdfDoc;

        private XDocument xdoc;

        private Font fontTitle = new Font();//表格头字体
        private Font fontContent = new Font();//内容字体
        private Font fontLabel = new Font();//测试名称字体
        private Font fontTableRemark = new Font();//表格备注字体

        private static readonly string progressImg = AppDomain.CurrentDomain.BaseDirectory + "progress.jpg";
        private static readonly string maxColumnImg = AppDomain.CurrentDomain.BaseDirectory + "maxcolumn.jpg";
        private static readonly string maxLineImg = AppDomain.CurrentDomain.BaseDirectory + "maxline.jpg";
        private static readonly string oddAvgImg = AppDomain.CurrentDomain.BaseDirectory + "oddavg.jpg";
        private static readonly string evenAvgImg = AppDomain.CurrentDomain.BaseDirectory + "evenavg.jpg";
        private static readonly string avgImg = AppDomain.CurrentDomain.BaseDirectory + "avg.jpg";

        #region 初始化
        public ExportReport() {
            xdoc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "\\AppTemplate\\report.xml");
            pdfDoc = new Document(PageSize.A4, 0, 0, 0, 0);
            Rectangle rect = new Rectangle(794, 1123);
            pdfDoc.SetPageSize(rect);
            pdfDoc.SetMargins(0, 0, 20, 0);

            BaseFont.AddToResourceSearch("iTextAsian.dll");
            BaseFont.AddToResourceSearch("iTextAsianCmaps.dll");
            BaseFont bsFont = BaseFont.CreateFont("STSong-Light", "UniGB-UCS2-H", BaseFont.NOT_EMBEDDED);
            fontTitle = new Font(bsFont, 20);
            fontContent = new Font(bsFont, 12);
            fontLabel = new Font(bsFont, 12);
            fontTableRemark = new Font(bsFont, 14);
            fontLabel.SetStyle(Font.BOLD);
            fontTitle.SetStyle(Font.BOLD);
            fontTableRemark.SetStyle(Font.BOLD);
        }

        public ExportReport(XDocument doc) {
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
        #endregion

        public bool Export(string fileName) {
            bool result = false;
            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, new FileStream(fileName, FileMode.Create));
            //writer.SetEncryption(PdfWriter.STRENGTH40BITS, null, null, PdfWriter.ALLOW_PRINTING);//指定只能打印而不能复制或修改
            pdfDoc.Open();

            pdfDoc.NewPage();

            AddTitle();

            IEnumerable<XElement> tableEles = xdoc.Descendants("table");
            CreateTable1(tableEles.ElementAt(0), 6);
            CreateTable1(tableEles.ElementAt(1), 6);
            CreateTable2(tableEles.ElementAt(2), 5);

            pdfDoc.NewPage();

            AddImg("测试过程图", progressImg);
            AddImg("最大力矩柱状图",maxColumnImg);
            AddImg("最大力矩折线图",maxLineImg);
            AddImg("平均曲线图",avgImg);

            pdfDoc.Close(); 
            return result;
        }

        /// <summary>
        /// 添加标题
        /// </summary>
        protected void AddTitle() {
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
        private void AddImg(string parName ,string imgName) {
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
        private void CreateTable1(XElement tableEle, int columnCount) {
            string remark = tableEle.Attribute("remark").Value;
            Paragraph parTableRemark = new Paragraph(remark, fontTableRemark);
            parTableRemark.IndentationLeft = 24;
            parTableRemark.SpacingBefore = 20;
            pdfDoc.Add(parTableRemark);

            PdfPTable table = new iTextSharp.text.pdf.PdfPTable(columnCount);
            List<PdfPRow> rowList = new List<iTextSharp.text.pdf.PdfPRow>();
            foreach (XElement rowEle in tableEle.Elements()) {
                List<PdfPCell> cellList = new List<iTextSharp.text.pdf.PdfPCell>();
                foreach (XElement cellEle in rowEle.Elements()) {
                    string label = cellEle.Attribute("label").Value;
                    if (label.Trim().Equals(""))
                    {
                        label = "/";
                    }
                    string value = cellEle.Attribute("value").Value.Trim();
                    if (value.Trim().Equals(""))
                    {
                        value = "/";
                    }

                    iTextSharp.text.pdf.PdfPCell cellLabel = new iTextSharp.text.pdf.PdfPCell(new Phrase(label, fontLabel));
                    cellLabel.FixedHeight = 26;
                    cellLabel.Padding = 6;
                    cellLabel.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                    cellLabel.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    //cellLabel.AddElement(new Phrase(label, fontLabel));

                    PdfPCell cellContent = new iTextSharp.text.pdf.PdfPCell(new Phrase(value, fontContent));
                    cellContent.FixedHeight = 26;
                    cellContent.PaddingTop = 6;
                    cellContent.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                    XAttribute colSpanAtt = cellEle.Attribute("colspan");
                    if (colSpanAtt != null) {
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
        private void CreateTable2(XElement tableEle, int columnCount) {
            string remark = tableEle.Attribute("remark").Value;
            Paragraph parTableRemark = new Paragraph(remark, fontTableRemark);
            parTableRemark.IndentationLeft = 24;
            parTableRemark.SpacingBefore = 20;
            pdfDoc.Add(parTableRemark);

            PdfPTable table = new iTextSharp.text.pdf.PdfPTable(new float[] { 350, 100, 100, 100, 100 });
            //List<PdfPRow> rowList = new List<iTextSharp.text.pdf.PdfPRow>();
            for (int i = 0; i < tableEle.Elements().Count(); i++) {
                XElement rowEle = tableEle.Elements().ElementAt(i);
                //List<PdfPCell> cellList = new List<iTextSharp.text.pdf.PdfPCell>();
                foreach (XElement cellEle in rowEle.Elements())
                {
                    string label = cellEle.Attribute("label").Value;
                    if (label.Trim().Equals(""))
                    {
                        label = "/";
                    }

                    iTextSharp.text.pdf.PdfPCell cellLabel = null;
                    if (i < 2)
                    {
                        cellLabel = new iTextSharp.text.pdf.PdfPCell(new Paragraph(label, fontLabel));
                        cellLabel.HorizontalAlignment = Element.ALIGN_CENTER;
                    }
                    else
                    {
                        cellLabel = new iTextSharp.text.pdf.PdfPCell(new Paragraph(label, fontContent));
                        cellLabel.HorizontalAlignment = Element.ALIGN_LEFT;
                    }
                    cellLabel.FixedHeight = 26;
                    cellLabel.Padding = 0;
                    cellLabel.PaddingLeft = cellLabel.PaddingRight = 6;

                    cellLabel.VerticalAlignment = Element.ALIGN_MIDDLE;

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

                    table.AddCell(cellLabel);
                    //cellList.Add(cellLabel);
                }
                //PdfPRow row = new iTextSharp.text.pdf.PdfPRow(cellList.ToArray<PdfPCell>());
                //rowList.Add(row);
            }
            
            //table.Rows.AddRange(rowList);
            table.KeepTogether = true;
            table.SpacingBefore = 10;
            table.TotalWidth = 750;
            
            table.LockedWidth = true;
            Paragraph pTable = new Paragraph();
            pTable.Add(table);
            pdfDoc.Add(pTable);
        }
    }
}
