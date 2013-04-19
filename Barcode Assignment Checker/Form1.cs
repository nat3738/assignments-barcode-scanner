using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace Barcode_Assignment_Checker
{
    public partial class Form1 : Form
    {
        private OleDbConnection conn = null;
        

        public Form1()
        {
            InitializeComponent();
            lblStatus.Text = "ไม่ได้เชื่อมฐานข้อมูล";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToString();
        }

        private void txtInput_Leave(object sender, EventArgs e)
        {
            txtInput.Focus();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dataSet11.courses' table. You can move, or remove it, as needed.
            this.coursesTableAdapter.Fill(this.dataSet11.courses);
            // TODO: This line of code loads data into the 'dataSet11.assignments' table. You can move, or remove it, as needed.
            this.assignmentsTableAdapter.Fill(this.dataSet11.assignments);
            txtInput.Focus();
            txtInput.Focus();
            txtInput.Focus();
            txtInput.Focus();
            lblStudentID.Text = "";
            lblCourse.Text = "";
        }

        private void handleInput()
        {
            // TODO parse barcode entry
            // BARCODE: ##### ##### STUDENT_ID COURSE_ID
            if (txtInput.Text.Length < 10)
                return;
            string sid = txtInput.Text.Substring(0, 5);
            string cid = txtInput.Text.Substring(5, 5);

            if (conn == null)
            {
                DialogResult r = MessageBox.Show("ยังไม่ได้เชื่อมต่อฐานข้อมูล ต้องการเชื่อมต่อหรือไม่?", "นั่นสิ", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.Yes)
                {
                    button1_Click(null, null);
                }
            }

            lblCourse.Text = cid;
            lblStudentID.Text = sid;

            DataSet1.assignmentsRow d = dataSet11.assignments.NewassignmentsRow();
            d.course_id = Convert.ToInt32(cid);
            d.student_id = Convert.ToInt32(sid);
            d.datetime = DateTime.Now;

            DataRow[] v = dataSet11.courses.Select("ID = " + (d.course_id.ToString()));

            if (v.Length == 0)
            {
                lblCourse.Text = "ไม่พบข้อมูล";
                lblCourse.ForeColor = Color.Red;
            }
            else
            {
                // Insert into local table
                dataSet11.assignments.AddassignmentsRow(d);
                assignmentsTableAdapter.Update(dataSet11);

                // Insert into connected table
                if (conn != null)
                {
                    string sql = "insert into barcode (StudentNumber, course_id) values (" + Convert.ToInt32(sid) + "," + Convert.ToInt32(cid) + ")";
                    (new OleDbCommand(sql, conn)).ExecuteNonQuery();
                }

                timer2.Enabled = true;

            }

            txtInput.Text = "";
        }

        private void txtInput_KeyUp(object sender, KeyEventArgs e)
        {
            
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Enabled = false;
            lblCourse.Text = "";
            lblStudentID.Text = "";
        }

        // Boolean flag used to determine when a character other than a number is entered. 
        private bool nonNumberEntered = false;

        // Handle the KeyDown event to determine the type of character entered into the control. 
        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            lblCourse.Text = "";
            lblStudentID.Text = "";
            lblCourse.ForeColor = Color.Black;

            // Initialize the flag to false.
            nonNumberEntered = false;

            // Determine whether the keystroke is a number from the top of the keyboard. 
            if (e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9)
            {
                // Determine whether the keystroke is a number from the keypad. 
                if (e.KeyCode < Keys.NumPad0 || e.KeyCode > Keys.NumPad9)
                {
                    // Determine whether the keystroke is a backspace. 
                    if (e.KeyCode != Keys.Back)
                    {
                        // A non-numerical keystroke was pressed. 
                        // Set the flag to true and evaluate in KeyPress event.
                        nonNumberEntered = true;
                    }
                }
            }
            //If shift key was pressed, it's not a number. 
            if (Control.ModifierKeys == Keys.Shift)
            {
                nonNumberEntered = true;
            }

            if (e.KeyCode == Keys.Enter)
                handleInput();
        }

        // This event occurs after the KeyDown event and can be used to prevent 
        // characters from entering the control. 
        private void txtInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for the flag being set in the KeyDown event. 
            if (nonNumberEntered == true)
            {
                // Stop the character from being entered into the control since it is non-numerical.
                e.Handled = true;
            }
        }

        public void updateCourses()
        {
            this.coursesTableAdapter.Fill(this.dataSet11.courses);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (conn != null)
                conn.Close();
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            frmGenerator c = new frmGenerator();
            c.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (conn != null)
            {
                DialogResult r = MessageBox.Show("เชื่อมต่อกับฐานข้อมูลแล้ว ต้องเปลี่ยนฐานข้อมูลหรือไม่?", "ยืนยัน", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.No)
                    return;
                else
                    conn.Close();
            }
            OpenFileDialog fileDialogue = new OpenFileDialog();
            fileDialogue.Filter = "ฐานข้อมูล Access (*.mdb)|*.mdb";
            if (fileDialogue.ShowDialog() == DialogResult.OK)
            {
                string fileName = fileDialogue.FileName;
                try
                {
                    string connStr = "Provider = Microsoft.Jet.OLEDB.4.0; Data Source = " + fileName;
                    conn = new OleDbConnection(connStr);
                    conn.Open();
                    importCourses();
                    lblStatus.Text = "เชื่อมต่อกับ " + fileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ไม่สามารถเชื่อมต่อฐานข้อมูลได้", "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    conn = null;
                    return;
                }
            }
        }

        private void importCourses()
        {
            dataSet11.courses.Clear();
            OleDbCommand com;
            OleDbDataReader dr; 
            com = new OleDbCommand("Select * from SubjectAll", conn);
            dr = com.ExecuteReader();

            while (dr.Read())
            {
                DataSet1.coursesRow r = dataSet11.courses.NewcoursesRow();
                r.ID = dr.GetInt32(0);
                r.course = dr.GetString(1);
                dataSet11.courses.AddcoursesRow(r);
            }

            coursesTableAdapter.Update(dataSet11);
        }

        private void exportBarcode()
        {
        }

    }
}
