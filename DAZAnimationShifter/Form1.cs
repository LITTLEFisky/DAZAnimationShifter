using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace DAZAnimationShifter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private int cutString(string cut)
        {
            for(int i = 0; i < cut.Length; i++)
            {
                if ((cut[i] != '\t'))
                {
                    return i;
                }
            }
            return -1;
        }

        private bool seekFrame(int frame)
        {
            System.IO.StreamReader infile = new System.IO.StreamReader(openPZ2.FileName);
            string line = "";
            while (infile.ReadLine() != null)
            {
                line = infile.ReadLine();
                if (line != null)
                {
                    if (line.Contains("k " + Convert.ToString(frame)) == true)
                    {
                        Program.framesTot++;
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openPZ2.ShowDialog();
            Program.PZpath = openPZ2.FileName;
            Program.fixedPZpath = Program.PZpath.Replace(".pz2", "fixed.pz2");
            label1.Text = Program.PZpath;
            label1.Visible = true;
            label1.Update();

            System.IO.StreamReader infile = new System.IO.StreamReader(openPZ2.FileName);

            string line = "";
            while (infile.ReadLine() != null)
            {
                line = infile.ReadLine();
                if (line.Contains("k ") == true)
                {
                    line = line.Substring(cutString(line));
                    string[] sub = line.Split();
                    Program.frameFirst = Convert.ToInt32(sub[1]);
                    break;
                }
            }
            bool foundFrame = true;
            infile.DiscardBufferedData();
            infile.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
            int seekingFrame = Program.frameFirst+1;
            while(foundFrame == true)
            {
                if (seekFrame(seekingFrame) == false)
                {
                    foundFrame = false;
                    break;
                };
                seekingFrame++;
            }
            Program.frameLast = Program.framesTot + Program.frameFirst;
            label2.Text = $"Start frame: {Program.frameFirst}\nEnd frame: {Program.frameLast}\nTotal frames: {Program.framesTot}";
            label2.Update();
            label2.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string contents = System.IO.File.ReadAllText(Program.PZpath);
            for(int currFrame = Program.frameFirst, newFrame = 0 ; currFrame < Program.frameLast ; currFrame++, newFrame++)
            {
                string origin = "k " + currFrame;
                string replacement = "k " + newFrame;
                contents = contents.Replace(origin, replacement);
            }
            System.IO.File.WriteAllText(Program.fixedPZpath, contents);

            System.IO.StreamReader infile = new System.IO.StreamReader(Program.fixedPZpath);

            string line = "";
            while (infile.ReadLine() != null)
            {
                line = infile.ReadLine();
                if (line.Contains("k ") == true)
                {
                    line = line.Substring(cutString(line));
                    string[] sub = line.Split();
                    Program.frameFirst = Convert.ToInt32(sub[1]);
                    break;
                }
            }
            bool foundFrame = true;
            infile.DiscardBufferedData();
            infile.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
            int seekingFrame = Program.frameFirst + 1;
            while (foundFrame == true)
            {
                if (seekFrame(seekingFrame) == false)
                {
                    foundFrame = false;
                    break;
                };
                seekingFrame++;
            }
            Program.frameLast = Program.framesTot + Program.frameFirst;
            label3.Text = $"Start frame: {Program.frameFirst}\nEnd frame: {Program.frameLast}\nTotal frames: {Program.framesTot}";
            label3.Update();
            label3.Visible = true;
            label4.Text = Program.fixedPZpath;
            label4.Visible = true;
            label4.Update();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This is an addition for \"PoserFormatExporter\"\n" +
                            "script for DAZ Studio. If you are exporting ani-\n" +
                            "mated range with it - your animation will \n" +
                            "start from the selected in the script start frame.\n" +
                            "This program shifts all exported frames, so the\n" +
                            "animation starts from frame \"0\", and you can\n" +
                            "place it anywhere on the timeline", "Code (C)LITTLEFisky, 2023", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
