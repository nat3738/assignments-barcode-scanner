using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Barcode_Assignment_Checker
{
    public partial class frmGenerator : Form
    {

        List<string> combo;
        Dictionary<string, string> map;
        public frmGenerator()
        {
            InitializeComponent();

            this.coursesTableAdapter.Fill(this.dataSet1.courses);

            combo = new List<string>();
            map = new Dictionary<string, string>();

            foreach (DataRow r in dataSet1.courses.Rows)
            {
                if (map.ContainsKey(r["course"].ToString()))
                {
                    map[r["course"].ToString()] = r["id"].ToString();
                }
                else
                {
                    combo.Add(r["course"].ToString());
                    map.Add(r["course"].ToString(), r["id"].ToString());
                }
            }

            this.comboBox1.DataSource = combo;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string s = "http://161.200.155.12/barcode.php?c=" + String.Format("{0:00000}", Convert.ToInt32(map[comboBox1.SelectedValue.ToString()])) + "&s=" + textBox1.Text.Replace("\r\n", ",");
            System.Diagnostics.Process.Start(s);
        }

        private void frmGenerator_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dataSet1.courses' table. You can move, or remove it, as needed.
            
        }
    }
}
