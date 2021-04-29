using System;
using System.Collections.Generic;
using System.Text;

namespace Karin.Preset
{
    class PresetFunctions
    {
        public static IEnumerable<IKarinFunction> Create() {
            //new KFunc_EXECUTIONALL();
            yield return new KFunc_ADD();
            yield return new KFunc_SUB();
            yield return new KFunc_MUL();
            yield return new KFunc_DIV();
            yield return new KFunc_MOD();
            yield return new KFunc_POW();
            yield return new KFunc_EQUAL();
            yield return new KFunc_NEQUAL();
            yield return new KFunc_OVER();
            yield return new KFunc_EOVER();
            yield return new KFunc_UNDER();
            yield return new KFunc_EUNDER();
            yield return new KFunc_AND();
            yield return new KFunc_OR();
            yield return new KFunc_NOT();
            yield return new KFunc_TRUE();
            yield return new KFunc_FALSE();
            yield return new KFunc_NULL();
            yield return new KFunc_CAT();
            yield return new KFunc_FORMAT();
            yield return new KFunc_DATE();
            yield return new KFunc_INT();
            yield return new KFunc_DOUBLE();
            yield return new KFunc_METHOD();
            yield return new KFunc_PROPERTY();

            yield return new KFunc_IF();
            yield return new KFunc_REPEAT();
            yield return new KFunc_WHILE();
            yield return new KFunc_RETURN();
            yield return new KFunc_TOSCRIPT();
        }
    }
}
