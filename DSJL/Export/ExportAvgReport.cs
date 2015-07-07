using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using iTextSharp.text.pdf;
using System.IO;

namespace DSJL.Export
{
    /// <summary>
    /// 导出平均曲线报告
    /// </summary>
    class ExportAvgReport:AbsExportPDF
    {
        private string avgImg =  AppDomain.CurrentDomain.BaseDirectory + "avg.jpg";//平均曲线图名称

        public ExportAvgReport(XDocument doc) : base(doc) { 
            
        }

        public override bool Export(string fileName)
        {
            bool result = false;

            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, new FileStream(fileName, FileMode.Create));
            pdfDoc.Open();

            pdfDoc.NewPage();

            AddTitle();

            IEnumerable<XElement> tableEles = xdoc.Descendants("table");
            int testInfoTableCount = tableEles.Count() - 1;
            for (int i = 0; i < testInfoTableCount; i++) {
                CreateTestInfoTable(tableEles.ElementAt(i), 6);
            }

            CreateCompareTable(tableEles.Last(), 5);

            AddImg("平均曲线图", avgImg);

            pdfDoc.Close();
            return result;
        }
    }
}
