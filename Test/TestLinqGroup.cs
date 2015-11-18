using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{
    class TestLinqGroup
    {
        public static void TestGroup() {
            List<Model> modelList = new List<Model>() { 
                new Model(){ ID=1,Name="张三",Value=20},
                new Model(){ID=2,Name="李四", Value=30},
                new Model(){ID=1,Name="王五",Value=10},
                new Model(){ID=2,Name="赵六",Value=15}
            };

            var groupList = modelList.GroupBy(x => x.ID);

            foreach (var item in groupList) {
                Console.WriteLine("group");
                foreach (var item2 in item) {
                    Console.WriteLine("id :"+item2.ID+" name is:"+item2.Name);
                }
            }
        }

    }

    class Model {
        public int ID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public double Value
        {
            get;
            set;
        }
    }
}
