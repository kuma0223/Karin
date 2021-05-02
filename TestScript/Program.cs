using Karin;
using Kourin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestScript
{
    class Program
    {
        static string MyPath;

        static void Main(string[] args) {

            var code = @"
1+2+3
$var = 4 + 5 + 6;
$$str = ""abcd""

$x = add[123, sub[456, 441*2]]

funcs {
 $a = 123
 func2{
   sub+1
 }
}

$a = 1

if[$a==1, {
    $a=2
    $b=3
}]

$x
";
            try {
                var ana = new Karin.TextAnalyzer(code, "script root");
                ana.Analyze();
                var tokens = ana.Tokens;
                tokens = TokenUtility.ToRPN(tokens);

                foreach (var t in tokens) {
                    Console.WriteLine(t.ToString());
                }

                Console.WriteLine(new KarinEngine().Execute(code));

            } catch(KarinException ex) {
                Console.WriteLine($"{ex.Message}{Environment.NewLine}{ex.ScriptStackTrace}");
            }

            //MyPath = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
            //MyPath += "/../..";

            //All();
        }
        /*
        static void All() {
            foreach (var i in Directory.GetFiles($"{MyPath}/in")) {
                One(Path.GetFileNameWithoutExtension(i));
                Console.WriteLine();
            }
        }

        static void One(string name) {
            var inp = $"{MyPath}/in/{name}.txt";
            var outp = $"{MyPath}/out/{name}.txt";

            if (File.Exists(outp)) {
                Exec(Read(inp), Read(outp));
            } else {
                ExecNg(Read(inp));
            }
            Console.WriteLine($"[{name}]");
        }

        static string Read(string path) {
            using (var ins = new StreamReader(path)) {
                return ins.ReadToEnd();
            }
        }

        static void Exec(string code, string ret) {
            KourinEngine eng = new KourinEngine();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            string ans;
            bool ok;

            try {
                ans = "" + eng.execute(code);
                ok = ans == ret;
            }catch(Exception ex) {
                ans = ex.Message;
                if(ex is KourinException) {
                    ans += "\r\n" + (ex as KourinException).ScriptStackTrace;
                }
                ok = false;
            }

            sw.Stop();
            Console.WriteLine($"{(ok?"OK":"NG")} : {ans} : {sw.ElapsedMilliseconds}ms");
        }

        static void ExecNg(string code) {
            KourinEngine eng = new KourinEngine();
            
            string ans = "";
            bool ok = false;

            try {
                eng.execute(code);
            } catch(KourinException ex) {
                ans = ex.Message;
                ok = true;
            } catch (Exception ex) {
                ans = ex.Message;
            }

            Console.WriteLine($"{(ok ? "OK" : "NG")} : {ans}");
        }
        */
    }
}
