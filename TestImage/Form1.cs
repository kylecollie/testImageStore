using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

namespace TestImage
{
    public partial class frmTestImage : Form
    {
        string imageName;
        SqlDataAdapter da;
        DataSet ds;

        public frmTestImage()
        {
            InitializeComponent();
        }


        private void updateData()
        {
            // use filestream object to read the image
            // read to the full length of the image to a bte array
            // add this byte as a parameter and insert it into database

            try
            {
                if (imageName != "")
                {
                    FileStream fs = new FileStream(@imageName, FileMode.Open, FileAccess.Read);
                    byte[] picByte = new byte[fs.Length];
                    fs.Read(picByte, 0, System.Convert.ToInt32(fs.Length));
                    fs.Close();

                    string connStr = @"Server=.\SQLExpress;AttachDbFilename=c:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\TestImage .mdf;Database=TestImage;Trusted_Connection=Yes;User Instance=false;";
                    
                    SqlConnection conn = new SqlConnection(connStr);
                    conn.Open();

                    string query = "insert into test_table(id_image,pic) values('" +
                        textBox1.Text + "'," + " @pic)";
                    SqlParameter picParm = new SqlParameter("pic", SqlDbType.Image);
                    picParm.Value = picByte;
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add(picParm);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Image added", "Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cmd.Dispose();
                    conn.Close();
                    conn.Dispose();

                    Connection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Connection()
        {
            // connect to the database and table
            // select all columns
            //add the name column to the combobox

            try
            {
                string connStr = @"Server=.\SQLExpress;AttachDbFilename=c:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\TestImage .mdf;Database=TestImage;Trusted_Connection=Yes;User Instance=false;";
                SqlConnection conn = new SqlConnection(connStr);
                conn.Open();
                da = new SqlDataAdapter();
                da.SelectCommand = new SqlCommand("SELECT * FROM test_table", conn);
                ds = new DataSet("ds");
                da.Fill(ds);
                DataTable dt;
                dt = ds.Tables[0];
                comboBox1.Items.Clear();
                foreach (DataRow dr in dt.Rows)
                {
                    comboBox1.Items.Add(dr[0].ToString());
                    comboBox1.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                FileDialog fldlg = new OpenFileDialog();
                fldlg.InitialDirectory = @":C:\Users\Kyle\Pictures\";
                fldlg.Filter = "Image File (*.jpg;*.bmp;*.gif)|*.jpg;*.bmp;*.gif";

                if (fldlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    imageName = fldlg.FileName;
                    Bitmap newimg = new Bitmap(imageName);
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox1.Image = (Image)newimg;
                }
                fldlg = null;
            }
            catch (System.ArgumentException ae)
            {
                imageName = " ";
                MessageBox.Show(ae.Message.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnStore_Click(object sender, EventArgs e)
        {
            updateData();
        }

        private void btnRetrieve_Click(object sender, EventArgs e)
        {
            DataTable dt = ds.Tables[0];

            if (pictureBox2.Image != null)
            {
                pictureBox2.Image.Dispose();
            }

            FileStream FS1 = new FileStream("image.jpg", FileMode.Create);

            foreach (DataRow dr in dt.Rows)
            {
                if (dr[0].ToString() == comboBox1.SelectedItem.ToString())
                {
                    byte[] blob = (byte[])dr[1];
                    FS1.Write(blob, 0, blob.Length);
                    FS1.Close();
                    FS1 = null;
                    pictureBox2.Image = Image.FromFile("image.jpg");
                    pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox2.Refresh();
                }
            }
        }
    }
}
