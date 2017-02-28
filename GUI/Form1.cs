using FlowGraph;
using GCC_Optimizer;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    public partial class Form1 : MaterialForm
    {
        // GUI folder directory
        string project_dir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
        string file_1;
        string file_2;
        public Form1()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Green700, Primary.Green900, Primary.Green500, Accent.LightGreen400, TextShade.WHITE);
        }

        private void materialLabel3_Click(object sender, EventArgs e)
        {

        }

        private void materialLabel2_Click(object sender, EventArgs e)
        {

        }

        private void materialLabel5_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            string upload_filename = "";
            string upload_path = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                upload_filename = Path.GetFileName(openFileDialog1.FileName);
                upload_path = Path.GetDirectoryName(openFileDialog1.FileName);
            }
            //if (!File.Exists(openFileDialog1.FileName))
            {
                File.Copy(Path.Combine(upload_path, upload_filename), Path.Combine(project_dir, upload_filename));
            }
            file_1 = Path.Combine(project_dir, upload_filename);
            materialLabel3.Text = upload_filename;
        }

        private void materialRaisedButton2_Click(object sender, EventArgs e)
        {
            string upload_filename = "";
            string upload_path = "";
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                upload_filename = Path.GetFileName(openFileDialog2.FileName);
                upload_path = Path.GetDirectoryName(openFileDialog2.FileName);
            }
           // if (!File.Exists(openFileDialog2.FileName))
            {
                File.Copy(Path.Combine(upload_path, upload_filename), Path.Combine(project_dir, upload_filename));
            }
            file_2 = Path.Combine(project_dir, upload_filename);
            materialLabel4.Text = upload_filename;
        }
        public static GFunction getGFunction(string fileName)
        {
            var optimizer = new Optimizer(fileName);
            var gimple = optimizer.GIMPLE;

            GFunction gFunction = new GFunction(gimple);

            return gFunction;
        }
        private void materialRaisedButton3_Click(object sender, EventArgs e)
        {
            if (file_1 == null || file_2 == null)
            {
                MessageBox.Show("Please Upload File", "Error");
                return;
            }
            GFunction gFunc1 = getGFunction(file_1);
            GFunction gFunc2 = getGFunction(file_2);
            decimal result = gFunc1.Compare(gFunc2);
            label1.Text = result.ToString();

            File.Delete(Path.Combine(project_dir, file_1));
            File.Delete(Path.Combine(project_dir, file_2));
        }


    }
}
