using HumanResourceMachineCodeGen.Compile;
using HumanResourceMachineCodeGen.Optimize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace HumanResourceMachineCodeGen
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void Generate()
        {
            try
            {
                HRMProgram pro = ConstructSyntaxTree(codeBox.Text);
                List<HRMCode> ass = Assembly(pro);
                List<HRMCode> opt = Optimize(ass);
                genBox.Text = WriteCode(opt);
            }
            catch (CompileException ce)
            {
                MessageBox.Show(ce.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetLabel(int x)
        {
            StringBuilder sb = new StringBuilder();
            do
            {
                sb.Append((char)('a' + x % 26));
                x /= 26;
            } while (x != 0);
            return sb.ToString();
        }

        private string WriteCode(List<HRMCode> opt)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("-- HUMAN RESOURCE MACHINE PROGRAM --");
            foreach (var code in opt)
            {
                string dest = code.Param > 0 ? $"{code.Param - 1}" : $"[{-code.Param - 1}]";
                string lab = GetLabel(code.Param);
                const int codePadding = -9;
                switch (code.Instruction)
                {
                    case HRMInstr.In:
                        sb.AppendLine($"    {"INBOX",codePadding}");
                        break;
                    case HRMInstr.Out:
                        sb.AppendLine($"    {"OUTBOX",codePadding}");
                        break;
                    case HRMInstr.Cpyf:
                        sb.AppendLine($"    {"COPYFROM",codePadding}{dest}");
                        break;
                    case HRMInstr.Cpyt:
                        sb.AppendLine($"    {"COPYTO",codePadding}{dest}");
                        break;
                    case HRMInstr.Inc:
                        sb.AppendLine($"    {"BUMPUP",codePadding}{dest}");
                        break;
                    case HRMInstr.Dec:
                        sb.AppendLine($"    {"BUMPDN",codePadding}{dest}");
                        break;
                    case HRMInstr.Add:
                        sb.AppendLine($"    {"ADD",codePadding}{dest}");
                        break;
                    case HRMInstr.Sub:
                        sb.AppendLine($"    {"SUB",codePadding}{dest}");
                        break;
                    case HRMInstr.Jmp:
                        sb.AppendLine($"    {"JUMP",codePadding}{lab}");
                        break;
                    case HRMInstr.Jz:
                        sb.AppendLine($"    {"JUMPZ",codePadding}{lab}");
                        break;
                    case HRMInstr.Jn:
                        sb.AppendLine($"    {"JUMPN",codePadding}{lab}");
                        break;
                    case HRMInstr.Lab:
                        sb.AppendLine($"{lab}:");
                        break;
                    default:
                        break;
                }
            }
            return sb.ToString();
        }

        private List<HRMCode> Optimize(List<HRMCode> ass)
        {
            List<HRMCode> opt;
            DefaultOptimizer.ForceOptimize(ass, out opt);
            return opt;
        }

        private List<HRMCode> Assembly(HRMProgram pro)
        {
            List<HRMCode> codes = pro.Emit();
            return codes;
        }

        private HRMProgram ConstructSyntaxTree(string code)
        {
            return new HRMProgram(code);
        }

        private void generateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Generate();
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
                switch (e.KeyCode)
                {
                    case Keys.A:
                        (sender as TextBox)?.SelectAll();
                        break;
                    case Keys.S:
                        SaveFile();
                        break;
                    case Keys.O:
                        OpenFile();
                        break;
                    case Keys.N:
                        codeBox.Text = "";
                        break;
                }
        }

        private void SaveFile()
        {
            SaveFileDialog diag = new SaveFileDialog();
            if (diag.ShowDialog() == DialogResult.OK)
                using (StreamWriter sw = new StreamWriter(diag.FileName))
                    sw.Write(codeBox.Text);
        }

        private void OpenFile()
        {
            OpenFileDialog diag = new OpenFileDialog();
            if (diag.ShowDialog() == DialogResult.OK)
                using (StreamReader sr = new StreamReader(diag.FileName))
                    codeBox.Text = sr.ReadToEnd();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e) => OpenFile();

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) => SaveFile();
    }
}
