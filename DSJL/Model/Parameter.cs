using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSJL.Model
{
    public class Parameter : BaseModel
    {
        private static List<Parameter> paramList = new List<Parameter>() { 
                new Parameter(){ParamName="峰值力矩(N·m)",IsChecked=true,Index=0},
                new Parameter(){ParamName="相对峰力矩(Nm/Kg)",IsChecked=true,Index=2},
                new Parameter(){ParamName="做功(J)",IsChecked=true,Index=3},
                new Parameter(){ParamName="相对做功(J/Kg)",IsChecked=true,Index=4},
                new Parameter(){ParamName="峰值功率(W)",IsChecked=true,Index=5},
                new Parameter(){ParamName="峰值相对功率(W/Kg)",IsChecked=true,Index=6},

                new Parameter(){ParamName="峰力矩时关节角(°)",IsChecked=false,Index=1},
                new Parameter(){ParamName="终点角度(°)",IsChecked=false,Index=7},
                new Parameter(){ParamName="慢速测试前50ms力矩(N·m)",IsChecked=false,Index=8},
                new Parameter(){ParamName="慢速测试前100ms力矩(N·m)",IsChecked=false,Index=9},
                new Parameter(){ParamName="50-100ms力矩斜率(N*m/ms)",IsChecked=false,Index=10},
                new Parameter(){ParamName="达到最大力矩20%所用时间(s)",IsChecked=false,Index=11},
                new Parameter(){ParamName="达到最大力矩70%所用时间(s)",IsChecked=false,Index=12},

                new Parameter(){ParamName="20-70%最大力矩的斜率(N*m/ms)",IsChecked=false,Index=13},
                new Parameter(){ParamName="疲劳下降率1",IsChecked=false,Index=14},
                new Parameter(){ParamName="疲劳下降率2",IsChecked=false,Index=15},

                new Parameter(){ParamName="峰值力矩比值",IsChecked=true,Index=16},
                new Parameter(){ParamName="做功比值",IsChecked=false,Index=17},
                new Parameter(){ParamName="功率比值",IsChecked=false,Index=18}
        };

        public static void CheckAll() {
            foreach (Parameter p in paramList) {
                p.IsChecked = true;
            }
        }

        public static void CheckDefault()
        {
            paramList = new List<Parameter>() 
            { 
                new Parameter(){ParamName="峰值力矩(N·m)",IsChecked=true,Index=0},
                new Parameter(){ParamName="峰力矩时关节角(°)",IsChecked=false,Index=1},
                new Parameter(){ParamName="相对峰力矩(Nm/Kg)",IsChecked=true,Index=2},
                new Parameter(){ParamName="做功(J)",IsChecked=true,Index=3},
                new Parameter(){ParamName="相对做功(J/Kg)",IsChecked=true,Index=4},
                new Parameter(){ParamName="峰值功率(W)",IsChecked=true,Index=5},
                new Parameter(){ParamName="峰值相对功率(W/Kg)",IsChecked=true,Index=6},

               
                new Parameter(){ParamName="终点角度(°)",IsChecked=false,Index=7},
                new Parameter(){ParamName="慢速测试前50ms力矩(N·m)",IsChecked=false,Index=8},
                new Parameter(){ParamName="慢速测试前100ms力矩(N·m)",IsChecked=false,Index=9},
                new Parameter(){ParamName="50-100ms力矩斜率(N*m/ms)",IsChecked=false,Index=10},
                new Parameter(){ParamName="达到最大力矩20%所用时间(s)",IsChecked=false,Index=11},
                new Parameter(){ParamName="达到最大力矩70%所用时间(s)",IsChecked=false,Index=12},

                new Parameter(){ParamName="20-70%最大力矩的斜率(N*m/ms)",IsChecked=false,Index=13},
                new Parameter(){ParamName="疲劳下降率1",IsChecked=false,Index=14},
                new Parameter(){ParamName="疲劳下降率2",IsChecked=false,Index=15},

                new Parameter(){ParamName="峰值力矩比值",IsChecked=true,Index=16},
                new Parameter(){ParamName="做功比值",IsChecked=false,Index=17},
                new Parameter(){ParamName="功率比值",IsChecked=false,Index=18}
            };
        }

        public string ParamName
        {
            get;
            set;
        }

        public static List<Parameter> GetAllParams() {
            return paramList;
        }
    }
}
