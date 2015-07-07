using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using iTextSharp.text.pdf;
using System.IO;

namespace DSJL.Export
{
    class ExportCompareReport:AbsExportPDF
    {
        private string avgImg = AppDomain.CurrentDomain.BaseDirectory + "avg.jpg";//平均曲线图名称

        public ExportCompareReport(XDocument doc) : base(doc) { }

        public override bool Export(string fileName)
        {
            bool reportResult = true;

            try
            {
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, new FileStream(fileName, FileMode.Create));
                pdfDoc.Open();

                pdfDoc.NewPage();

                AddTitle();

                IEnumerable<XElement> testinfotableEles = xdoc.Descendants("testinfotable").ElementAt<XElement>(0).Elements();
                int testInfoTableCount = testinfotableEles.Count();
                for (int i = 0; i < testInfoTableCount; i++)
                {
                    CreateTestInfoTable(testinfotableEles.ElementAt(i), 6);
                    if ((i + 1) % 6 == 0) {
                        pdfDoc.NewPage();
                    }
                }
                pdfDoc.NewPage();
                IEnumerable<XElement> comparetableEles = xdoc.Descendants("comparetable").ElementAt<XElement>(0).Elements();
                int compareTableCount = comparetableEles.Count();
                for (int i = 0; i < compareTableCount; i++)
                {
                    CreateCompareTable2(comparetableEles.ElementAt(i), 7);
                }
                pdfDoc.NewPage();
                AddImg("平均曲线图", avgImg);
            }
            catch (Exception ee)
            {
                reportResult = false;
                throw ee;
            }
            finally {
                pdfDoc.Close();
            }

            return reportResult;
        }
    }
}
