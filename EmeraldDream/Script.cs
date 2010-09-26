using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;

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
            MainProcedureLabel,
            ProcedureLabel,
            Make,
            Menu,
            For,    // Start Choice subroutine
            End,    // End choice subroutine
            Jump
        }

        enum OperandType
        {
            String,
            ObjectName,
            MapName,
            ProcName,
            ProcLabel,
            ImageName,
            Choicename,
        }

        class Instruction
        {
            public InstructionType type;
            public List<string> operand = new List<string>();
            public List<OperandType> operandtype = new List<OperandType>();
            public Action<StreamReader> operandConsumer = null;
            public List<Procedure> associatedProcs = new List<Procedure>();
        }

        class Procedure
        {
            public string name;
            public List<Instruction> instructions = new List<Instruction>();
            public int returnInstruction = 0;
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
            sb.Append("using System.Collections.Generic;\n");
            sb.Append("using System.Collections;\n");
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
            ForeachInstruction(sr,
                inst => instructions.Add(currentInstruction),
                inst => currentInstruction != null && currentInstruction.type == InstructionType.Noop && onlyOneProc);
        }

        private void ForeachInstruction(StreamReader sr, Action<Instruction> foreachInstruction, Predicate<Instruction> shouldStop)
        {
            GetNextInstruction(sr); // Grab the first instruction

            while (currentInstruction != null)
            {
                if (currentInstruction.operandConsumer != null)
                {
                    currentInstruction.operandConsumer(sr);
                }

                foreachInstruction(currentInstruction);

                GetNextInstruction(sr);

                if (shouldStop(currentInstruction))
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
                currentInstruction.type = InstructionType.MainProcedureLabel;
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
                else if (tokens[1] == ("image"))
                {
                    currentInstruction.operandtype.Add(OperandType.ImageName);
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
            else if (line.StartsWith("choice"))
            {
                currentInstruction.type = InstructionType.Menu;
                if (line.StartsWith("choicetitle"))
                {
                    if (line.Split(' ').Length > 1)
                    {
                        currentInstruction.operand.Add(line.Substring(12));
                    }
                    else
                    {
                        currentInstruction.operand.Add("");
                    }
                    currentInstruction.operandtype.Add(OperandType.String);
                }
                else
                {
                    ParseChoiceLine(line);
                }
                currentInstruction.operandConsumer = ConsumeMenuInstructions;
            }
            else if (line.StartsWith("end"))
            {
                currentInstruction.type = InstructionType.End;
            }
            else if (line.StartsWith("for"))
            {
                currentInstruction.type = InstructionType.For;
                currentInstruction.operand.Add(line.Substring(4));
                currentInstruction.operandtype.Add(OperandType.Choicename);
            }
            else if (line.StartsWith("goto"))
            {
                currentInstruction.type = InstructionType.Jump;
                currentInstruction.operand.Add(line.Substring(5));
                currentInstruction.operandtype.Add(OperandType.ProcLabel);
            }
            else if (proclabelmatch.Success)
            {
                currentInstruction.type = InstructionType.ProcedureLabel;
                currentInstruction.operand.Add(proclabelmatch.Groups["procname"].ToString());
                currentInstruction.operandtype.Add(OperandType.ProcName);
            }
            else
            {
                currentInstruction.type = InstructionType.Noop;
            }
        }

        void ConsumeMenuInstructions(StreamReader sr)
        {
            Instruction mainInstruction = currentInstruction;
            GetNextInstruction(sr);
            List<Procedure> choiceprocs = new List<Procedure>();

            int choices = 1;
            while (currentInstruction != null && currentInstruction.type == InstructionType.Menu)
            {
                mainInstruction.operand.AddRange(currentInstruction.operand);
                mainInstruction.operandtype.AddRange(currentInstruction.operandtype);
                GetNextInstruction(sr);
                choices++;
            }

            for (int i = 0; i < choices; i++)
            {
                if (currentInstruction.type != InstructionType.For)
                {
                    break;
                }
                Procedure p = new Procedure();
                p.name = currentInstruction.operand[0];

                ForeachInstruction(sr,
                    inst => p.instructions.Add(inst),
                    inst => inst.type == InstructionType.End);
                
                GetNextInstruction(sr);

                choiceprocs.Add(p);
            }

            mainInstruction.associatedProcs = choiceprocs;

            peekedInstruction = currentInstruction;
            currentInstruction = mainInstruction;
        }

        private void ParseChoiceLine(string line)
        {
            string[] tokens = line.Substring(7).Split(' ');
            currentInstruction.operand.Add(tokens[0]);
            currentInstruction.operandtype.Add(OperandType.Choicename);
            currentInstruction.operand.Add(tokens[1]);
            currentInstruction.operandtype.Add(OperandType.String);
        }

        int mainproc = 0;
        Dictionary<string, string> procnameToInstruction = new Dictionary<string, string>();

        void CompileInstructions(List<Instruction> instructions, StringBuilder sb) {
            CompileInstructions(instructions, sb, "");
        }



        void CompileInstructions(List<Instruction> instructions, StringBuilder sb, string suffix)
        {
            for (int n = 0; n < instructions.Count; n++)
            {
                Instruction i = instructions[n];
                List<Procedure> subprocs = new List<Procedure>();

                sb.Append("static void " + NameInstruction(n, suffix) + " (Story story)\n");
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
                        if (ThereIsANextInstruction(instructions, n))
                        {
                            sb.Append("story.narrationdialog.SetDoOnceOnClose(() => {" + NameInstruction(n + 1, suffix) + "(story); });\n");
                        }
                        sb.Append("story.narrationdialog.Open();\n");
                        break;
                    case InstructionType.MainProcedureLabel:
                        if (ThereIsANextInstruction(instructions, n))
                        {
                            procnameToInstruction["main"] = NameInstruction(n + 1, suffix);
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
                            case OperandType.ImageName:
                                sb.Append("story.SetImage(\"" + i.operand[0] + "\");\n");
                                break;
                        }
                        if (ThereIsANextInstruction(instructions, n))
                        {
                            sb.Append(NameInstruction(n + 1, suffix) + "(story);\n");
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
                            sb.Append(NameInstruction(n + 1, suffix) + "(story);\n");
                        }
                        break;
                    case InstructionType.ProcedureLabel:
                        if (ThereIsANextInstruction(instructions, n))
                        {
                            procnameToInstruction[i.operand[0]] = NameInstruction(n + 1, suffix);
                            sb.Append(NameInstruction(n + 1, suffix) + "(story);\n");
                        }
                        break;
                    case InstructionType.Menu:

                        Queue<string> operands = new Queue<string>();
                        i.operand.ForEach(x => operands.Enqueue(x));

                        sb.Append("story.menudialog.ClearMenuItems();\n");

                        if (i.operandtype[0] == OperandType.String)
                        {
                            // If there is a question set
                            sb.Append("story.menudialog.SetQuestion(\"" + operands.Dequeue() + "\");\n");
                        }
                        else
                        {
                            sb.Append("story.menudialog.SetQuestion(\"\");\n");
                        }

                        sb.Append("Dictionary<string, Action<Story>> choiceToActionMap = new Dictionary<string, Action<Story>>();\n");

                        while (operands.Count > 0)
                        {
                            // Consume the menu choices
                            string choicename = operands.Dequeue();
                            string choicetext = operands.Dequeue();
                            sb.Append("story.menudialog.AddMenuItem(\"" + choicename + "\", \"" + choicetext + "\");\n");
                        }

                        int count = 0;
                        i.associatedProcs.ForEach(p => {
                            sb.Append("choiceToActionMap[\"" + p.name + "\"] = " + NameInstruction(0, MakeSuffix(n, count)) + ";\n");
                            
                            // Make a return instruction
                            Instruction retinst = new Instruction();
                            retinst.type = InstructionType.Jump;
                            retinst.operand.Add(NameInstruction(n + 1, suffix));
                            retinst.operandtype.Add(OperandType.ProcName);

                            p.instructions.Add(retinst);

                            count++;
                        });
                        subprocs = i.associatedProcs;

                        sb.Append("story.menudialog.SetDoOnceOnClose(() => {choiceToActionMap[story.menudialog.SelectedItem](story); });\n");

                        sb.Append("story.menudialog.Open();\n");
                        break;
                    case InstructionType.Jump:
                        string instname = i.operand[0];

                        if (i.operandtype[0] == OperandType.ProcLabel)
                        {
                            instname = procnameToInstruction[i.operand[0]];
                        }

                        sb.Append(instname + "(story);\n");
                        break;
                }

                sb.Append("}\n");

                int c = 0;
                subprocs.ForEach(p => {
                    CompileInstructions(p.instructions, sb, MakeSuffix(n, c));
                    c++;
                });
            }
        }

        private static string MakeSuffix(int n, int c)
        {
            return "_i" + n + "_p" + c;
        }

        private static string NameInstruction(int n, string suffix)
        {
            return "Instruction" + n + suffix;
        }

        private static bool ThereIsANextInstruction(List<Instruction> instructions, int n)
        {
            return (n + 1) < instructions.Count && instructions[n + 1].type != InstructionType.ProcedureLabel && instructions[n + 1].type != InstructionType.MainProcedureLabel;
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
