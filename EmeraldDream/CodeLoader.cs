using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;

namespace EmeraldDream
{
    class CodeLoader
    {
        CSharpCodeProvider csc;
        CompilerParameters cp = new CompilerParameters();

        public CodeLoader()
        {

            Dictionary<string, string> providerOptions = new Dictionary<string, string>();
            providerOptions.Add("CompilerVersion", "v3.5");
            csc = new CSharpCodeProvider(providerOptions);

            cp.CompilerOptions = "";
            cp.ReferencedAssemblies.Add("EmeraldLibrary.dll");
            cp.ReferencedAssemblies.Add("EmeraldDream.exe");
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("System.Core.dll");
            cp.ReferencedAssemblies.Add("System.Data.dll");
            cp.ReferencedAssemblies.Add("System.Drawing.dll");
            cp.ReferencedAssemblies.Add("System.Windows.Forms.dll");

            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;
            cp.IncludeDebugInformation = true;
            cp.WarningLevel = 4;
        }

        public Assembly Compile(string name, string code)
        {
            cp.MainClass = name;
            cp.OutputAssembly = name;
#if DEBUG
            cp.IncludeDebugInformation = true;
            cp.TempFiles = new TempFileCollection(".", true);
#endif

            CompilerResults cr = csc.CompileAssemblyFromSource(cp, code);

            return cr.CompiledAssembly;
        }
    }
}
