using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace DSJL.Model.TestData
{
    /// <summary>
    /// 测试数据model
    /// </summary>
    public class TestData
    {
        public XDocument XDoc {
            get;
            set;
        }

        public XElement GetOddAvgElement() {
            if (XDoc==null)
            {
                return null;
            }
            else
            {
                return XDoc.Root.Element("avgcurve").Element("oddavg");
            }
        }

        public XElement GetEvenAvgElement()
        {
            if (XDoc == null)
            {
                return null;
            }
            else
            {
                return XDoc.Root.Element("avgcurve").Element("evenavg");
            }
        }
    }
}
