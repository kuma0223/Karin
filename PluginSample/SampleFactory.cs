using System;
using System.Collections.Generic;
using Karin;

namespace PluginSample
{
    public class SampleFactory : IPluginFunctionFactory
    {
        public string KeyName {
            get { return "SampleFunctions"; }
        }

        public IEnumerable<IKarinFunction> GetPluginFunctions() {
            yield return new SampleFunction();
        }
    }
}
