using System;
using Karin;

namespace PluginSample
{
    public class SampleFunction : IKarinFunction
    {
        public string Name{
            get { return "sample"; }
        }

        public object Execute(object[] args) {
            var s = "call sample. args:[";
            foreach (var x in args) {
                s += x.ToString();
                s += ",";
            }
            s += "]";
            return s;
        }
    }
}
