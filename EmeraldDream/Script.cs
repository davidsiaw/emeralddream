using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;

namespace EmeraldDream
{
    // Describes a script that can be loaded and run

    // Scripts add a set of tasks to the story's tasklist for that script.
    // Once the tasks are done, they call ScriptTaskDone on the story, which
    // moves on and does the next script task

    class Script
    {
        enum InstructionType
        {
            Noop,
            Say,
            Set,
            MainProcedure,
            Procedure,
            Make,
        }

        enum OperandType
        {
            String,
            ObjectName,
            MapName,
            ProcName,
        }

        class Instruction
        {
            public InstructionType type;
            public List<string> operand = new List<string>();
            public List<OperandType> operandtype = new List<OperandType>();
            public Action<StreamReader> operandConsumer = null;
            public List<Instruction> instructionsInProcedure = new List<Instruction>();
        }

        CodeLoader cl = new CodeLoader();
        Assembly asm;
        string name;

        public Script(string name, StreamReader sr, bool onlyOneProc)
        {
            LoadScript(name, onlyOneProc, sr);
        }

        public Script(string name, string filename)
            : this(name, filename, false)
        { 

        }

        public Script(string name, string filename, bool onlyOneProc)
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                LoadScript(name, onlyOneProc, sr);
            }
        }

        private void LoadScript(string name, bool onlyOneProc, StreamReader sr)
        {

            StringBuilder sb = new StringBuilder();

            sb.Append("using System;\n");
            sb.Append("using EmeraldDream;\n");
            sb.Append("using EmeraldLibrary;\n");
            sb.Append("using System.Windows.Forms;\n");

            sb.Append("namespace EmeraldDream\n");
            sb.Append("{\n");
            sb.Append("    public class " + name + "\n");
            sb.Append("    {\n");

            List<Instruction> instructions = new List<Instruction>();

            ConsumeInstructions(sr, instructions, onlyOneProc);

            CompileInstructions(instructions, sb);

            sb.Append("     public static void Main (Story story)\n");
            sb.Append("     {\n");

            if (instructions.Count > 0)
            {
                sb.Append("     Instruction" + mainproc + "(story);\n");
            }

            sb.Append("     }\n");

            sb.Append("    }\n");
            sb.Append("}\n");

            asm = cl.Compile(name, sb.ToString());
            this.name = name;
        }

        Instruction currentInstruction = null;

        void ConsumeInstructions(StreamReader sr, List<Instruction> instructions, bool onlyOneProc)
        {
            GetNextInstruction(sr); // Grab the first instruction

            while (currentInstruction != null)
            {
                if (currentInstruction.operandConsumer != null)
                {
                    currentInstruction.operandConsumer(sr);
                }
                instructions.Add(currentInstruction);
                GetNextInstruction(sr);

                if (currentInstruction != null && currentInstruction.type == InstructionType.Noop && onlyOneProc)
                {
                    break;
                }
            }
        }

        void ConsumeSayInstructions(StreamReader sr)
        {
            Instruction mainInstruction = currentInstruction;
            GetNextInstruction(sr);
            while (currentInstruction != null && currentInstruction.type == InstructionType.Say)
            {
                mainInstruction.operand.AddRange(currentInstruction.operand);
                mainInstruction.operandtype.AddRange(currentInstruction.operandtype);
                GetNextInstruction(sr);
            }
            peekedInstruction = currentInstruction;
            currentInstruction = mainInstruction;
        }

        Instruction peekedInstruction = null;

        Regex proclabel = new Regex(@"^(?<procname>[a-z][a-z|0-9]*)\:", RegexOptions.Compiled);

        void GetNextInstruction(StreamReader sr)
        {
            if (peekedInstruction != null)
            {
                // Woops! we read too far. lets consume the next instruction
                currentInstruction = peekedInstruction;
                peekedInstruction = null;
                return;
            }

            if (sr.EndOfStream)
            {
                currentInstruction = null;
                return;
            }

            currentInstruction = new Instruction();
            currentInstruction.type = InstructionType.Noop;
            string line = sr.ReadLine();
            Match proclabelmatch = proclabel.Match(line);

            if (line.StartsWith("say"))
            {
                currentInstruction.type = InstructionType.Say;
                currentInstruction.operand.Add(line.Substring(4));
                currentInstruction.operandtype.Add(OperandType.String);
                currentInstruction.operandConsumer = ConsumeSayInstructions;
            }
            else if (line.StartsWith("start:"))
            {
                currentInstruction.type = InstructionType.MainProcedure;
            }
            else if (line.StartsWith("set"))
            {
                string[] tokens = line.Split(' ');
                currentInstruction.type = InstructionType.Set;
                currentInstruction.operand.Add(tokens[2]);
                if (tokens[1].StartsWith("map"))
                {
                    currentInstruction.operandtype.Add(OperandType.MapName);
                }
                else if (tokens[1].StartsWith("object"))
                {
                    currentInstruction.operandtype.Add(OperandType.ObjectName);
                }
                else
                {
                    throw new ScriptException("I don't know what a {0} is", tokens[1]);
                }
            }
            else if (line.StartsWith("make"))
            {
                currentInstruction.type = InstructionType.Make;
                string[] tokens = line.Split(' ');
                if (tokens[1].StartsWith("object"))
                {
                    currentInstruction.operand.Add(tokens[2]);
                    currentInstruction.operandtype.Add(OperandType.ObjectName);
                }
                else
                {
                    throw new ScriptException("A {0} can't be made", tokens[1]);
                }

                currentInstruction.operand.Add(tokens[3]);
                currentInstruction.operandtype.Add(OperandType.String);
            }
            else if (proclabelmatch.Success)
            {
                currentInstruction.type = InstructionType.Procedure;
                currentInstruction.operand.Add(proclabelmatch.Groups["procname"].ToString());
                currentInstruction.operandtype.Add(OperandType.ProcName);
            }
        }

        int mainproc = 0;
        Dictionary<string, string> procnameToInstruction = new Dictionary<string, string>();

        void CompileInstructions(List<Instruction> instructions, StringBuilder sb)
        {
            for (int n = 0; n < instructions.Count; n++)
            {
                Instruction i = instructions[n];

                sb.Append("static void Instruction" + n + " (Story story)\n");
                sb.Append("{\n");

                switch (i.type)
                {
                    case InstructionType.Say:
                        sb.Append("string[] narration = new string[] {\n");

                        i.operand.ForEach(op =>
                        {
                             sb.Append("\"" + op + "\",");
                        });

                        sb.Append("};\n");
                        sb.Append("story.narrationdialog.SetNarration(narration);\n");
                        sb.Append("story.narrationdialog.Open();\n");
                        if (ThereIsANextInstruction(instructions, n))
                        {
                            sb.Append("story.narrationdialog.SetDoOnceOnClose(() => {Instruction" + (n + 1).ToString() + "(story); });\n");
                        }
                        break;
                    case InstructionType.MainProcedure:
                        if (ThereIsANextInstruction(instructions, n))
                        {
                            procnameToInstruction["main"] = "Instruction" + n + 1;
                            mainproc = n + 1;
                        }
                        break;
                    case InstructionType.Set:
                        switch (i.operandtype[0])
                        {
                            case OperandType.MapName:
                                sb.Append("story.ChangeMap(\"" + i.operand[0] + "\");\n");
                                break;
                            case OperandType.ObjectName:
                                sb.Append("story.ChangeCharacter(\"" + i.operand[0] + "\");\n");
                                break;
                        }
                        if (ThereIsANextInstruction(instructions, n))
                        {
                            sb.Append("Instruction" + (n + 1).ToString() + "(story);\n");
                        }
                        break;
                    case InstructionType.Make:
                        switch (i.operandtype[0])
                        {
                            case OperandType.ObjectName:
                                sb.Append("story.CreateObjectInstance(\"" + i.operand[0] + "\", \"" + i.operand[1] + "\");\n");
                                break;
                        }
                        if (ThereIsANextInstruction(instructions, n))
                        {
                            sb.Append("Instruction" + (n + 1).ToString() + "(story);\n");
                        }
                        break;
                    case InstructionType.Procedure:
                        if (ThereIsANextInstruction(instructions, n))
                        {
                            procnameToInstruction[i.operand[0]] = "Instruction" + n + 1;
                            sb.Append("Instruction" + (n + 1).ToString() + "(story);\n");
                        }
                        break;
                }

                sb.Append("}\n");

            };
        }

        private static bool ThereIsANextInstruction(List<Instruction> instructions, int n)
        {
            return (n + 1) < instructions.Count && instructions[n + 1].type != InstructionType.Procedure && instructions[n + 1].type != InstructionType.MainProcedure;
        }

        public MethodInfo GetMethod(string method)
        {
            Type t = asm.GetType("EmeraldDream." + name);
            MethodInfo mi = t.GetMethod(procnameToInstruction[method]);
            return mi;
        }

        public void Execute(Story state)
        {
            Type t = asm.GetType("EmeraldDream." + name);
            MethodInfo mi = t.GetMethod("Main");
            mi.Invoke(null, new object[] { state });
        }
    }

    public class ScriptException : Exception{
        public ScriptException(string msg) : base(msg) { }
        public ScriptException(string msg, params object[] parms) : base(String.Format(msg, parms)) { }
    }
}
