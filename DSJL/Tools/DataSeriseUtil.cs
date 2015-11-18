using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Visifire.Charts;

namespace DSJL.Tools
{
    class DataSeriseUtil
    {
        public static void InitLineDataSerise(DataSeries ds, bool showInLengend, string lengendText, Brush brush)
        {
            if (ds == null)
            {
                ds = new DataSeries();
            }
            ds.ShowInLegend = showInLengend;
            ds.RenderAs = RenderAs.QuickLine;
            if (showInLengend)
            {
                if (brush != null)
                {
                    ds.Color = brush;
                }

                ds.LegendText = lengendText;
            }
        }

        public static void InitDataSerise(DataSeries ds, bool showInLengend, string lengendText, Brush brush,RenderAs render)
        {
            if (ds == null)
            {
                ds = new DataSeries();
            }
            ds.ShowInLegend = showInLengend;
            ds.RenderAs = render;
            if (brush != null)
            {
                ds.Color = brush;
            }
            if (showInLengend)
            {
                ds.LegendText = lengendText;
            }
        }

        public static void AddDataToDataSerise(DataSeries ds,List<double> list) {
            double allCount = 100;
            double value = allCount / list.Count;
            for (int i = 0; i < list.Count; i++)
            {
                DataPoint dp = new DataPoint() { YValue = System.Math.Abs(list[i]) * 0.1, XValue = i * value };
                ds.DataPoints.Add(dp);
            }
        }
        public static DataPointCollection GetDataPointCollection( List<double> list)
        {
            DataPointCollection dpc = new DataPointCollection();
            double allCount = 100;
            double value = allCount / list.Count;
            for (int i = 0; i < list.Count; i++)
            {
                DataPoint dp = new DataPoint() { YValue = System.Math.Abs(list[i]) * 0.1, XValue = i * value };
                dpc.Add(dp);
            }
            return dpc;
        }

    }
}
