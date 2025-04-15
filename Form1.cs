using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;

namespace Lab1
{
    public partial class Adaposturi_Animale : Form
    {
        string conn;
        SqlConnection cs;
        SqlDataAdapter da;
        DataSet ds;

        string ID = string.Empty;
        string IDVoluntar = string.Empty;

        public Adaposturi_Animale()
        {
            InitializeComponent();

            string conn = ConfigurationManager.ConnectionStrings["cn"].ConnectionString;
            cs = new SqlConnection(conn);
            

            string ParentTable = ConfigurationManager.AppSettings["ParentTableName"];
            string query = "SELECT * FROM " + ParentTable;
            SqlDataAdapter da = new SqlDataAdapter(query, cs);
            DataSet ds = new DataSet();

            cs.Open();
            da.Fill(ds);
            dataGridView1.DefaultCellStyle.Font = new Font("Arial", 8);
            dataGridView1.DataSource = ds.Tables[0];

            string[] ColumnNamesList = ConfigurationManager.AppSettings["ChildColumnNames"].Split(',');
            int y = 20;

            foreach (string column in ColumnNamesList)
            {
                Label lbl = new Label();
                lbl.Text = column;
                lbl.Location = new Point(10, y);
                lbl.Width = 100;
                panel1.Controls.Add(lbl);

                TextBox txt = new TextBox();
                txt.Name = column;
                txt.Location = new Point(120, y);
                txt.Width = 200;
                panel1.Controls.Add(txt);

                y += 30;
            }


            string[] ColumnUpdateNamesList = ConfigurationManager.AppSettings["ChildUpddateColumnNames"].Split(',');
            y = 20;

            foreach (string column in ColumnUpdateNamesList)
            {
                Label lbl1 = new Label();
                lbl1.Text = column;
                lbl1.Location = new Point(10, y);
                lbl1.Width = 100;
                panel2.Controls.Add(lbl1);

                TextBox txt1 = new TextBox();
                txt1.Name = column;
                txt1.Location = new Point(120, y);
                txt1.Width = 200;
                panel2.Controls.Add(txt1);

                y += 30;
            }


        }
        private void update() {

            string ChildTable = ConfigurationManager.AppSettings["ChildTableName"];
            string ParentId = ConfigurationManager.AppSettings["ChildColumnSelect"];
            string query = "SELECT * FROM " + ChildTable + " WHERE " + ParentId + " = @id";

            SqlDataAdapter da3 = new SqlDataAdapter(query, cs);
            da3.SelectCommand.Parameters.AddWithValue("@id", ID);

            DataSet ds3 = new DataSet();
            da3.Fill(ds3);
            dataGridView2.DataSource = ds3.Tables[0];
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            string id = row.Cells[0].Value.ToString();
            ID = id;

            string ChildTable = ConfigurationManager.AppSettings["ChildTableName"];
            string ParentId = ConfigurationManager.AppSettings["ChildColumnSelect"];
            string query = "SELECT * FROM " + ChildTable + " WHERE " + ParentId + " = @id";

            SqlDataAdapter da2 = new SqlDataAdapter(query, cs);
            da2.SelectCommand.Parameters.AddWithValue("@id", id);

            DataSet ds2 = new DataSet();
            da2.Fill(ds2);
            dataGridView2.DataSource = ds2.Tables[0];
        }

        private void StergeClick(object sender, EventArgs e)
        {
            if (IDVoluntar != string.Empty)
            {

                if (cs.State != ConnectionState.Open)
                {
                    cs.Open();
                }

                string ChildTable = ConfigurationManager.AppSettings["ChildTableName"];
                string CNP = ConfigurationManager.AppSettings["cnp"];
                string query = "DELETE FROM " + ChildTable + " WHERE " + CNP + " = @id";

                SqlCommand cmd = new SqlCommand(query, cs);
                cmd.Parameters.AddWithValue("@id", IDVoluntar);
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Stergerea a avut succes!");
                }

                cs.Close();
                update();
            }
            else
            {
                MessageBox.Show("ID-ul nu este valid!");
            }

        }

        private void ActualizeazaClick(object sender, EventArgs e)
        {
            if (IDVoluntar != string.Empty)
            {
                    try
                    {
                        if (cs.State != ConnectionState.Open)
                        {
                            cs.Open();
                        }

                    string ChildTableName = ConfigurationManager.AppSettings["ChildTableName"];
                    string id1 = ConfigurationManager.AppSettings["cnp"];

                    string[] ColumnNamesList = ConfigurationManager.AppSettings["ChildUpddateColumnNames"].Split(',');
                    string[] ColumnInsertParams = ConfigurationManager.AppSettings["ColumnUpdateNamesInsertParameters"].Split(',');

                    string setClause = string.Join(",", ColumnNamesList.Select(c => c + " = @" + c));
                    Console.WriteLine(setClause);

                    string sql = $"UPDATE {ChildTableName} SET {setClause} WHERE {id1} = @ID";

                    SqlCommand cmd = new SqlCommand(sql, cs);
                    cmd.Parameters.AddWithValue("@ID", IDVoluntar);

                    foreach (string column in ColumnNamesList)
                    {
                        TextBox textBox = (TextBox)panel2.Controls[column];
                        cmd.Parameters.AddWithValue("@" + column, textBox.Text);
                    }


                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Row updated successfully!");

                    update();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("A apărut o eroare la actualizarea datelor: " + ex.Message);
                    }
                }
            else
            {
                MessageBox.Show("Nu ai selectat un voluntar pentru actualizare!");
            }
        }

        private void AdaugaClick(object sender, EventArgs e)
        {
            if (ID != string.Empty)
            {
                
                try
                    {
                        if (cs.State != ConnectionState.Open)
                        {
                            cs.Open();
                        }

                        string ChildTableName = ConfigurationManager.AppSettings["ChildTableName"];
                        string id1 = ConfigurationManager.AppSettings["ChildColumnSelect"];

                        string ChildColumnNames = ConfigurationManager.AppSettings["ChildColumnNames"];
                        string ColumnNamesInsertParameters = ConfigurationManager.AppSettings["ColumnNamesInsertParameters"];

                        List<string> ColumnNamesList = new List<string>(ConfigurationManager.AppSettings["ChildColumnNames"].Split(','));
                        Console.WriteLine(ColumnNamesList);
                        SqlCommand cmd = new SqlCommand("INSERT INTO " + ChildTableName + " (" + ChildColumnNames + "," + id1 + ") VALUES(" + ColumnNamesInsertParameters + ", @ID)", cs);
                        cmd.Parameters.AddWithValue("@ID", ID);

                        foreach (string column in ColumnNamesList)
                        {
                            TextBox textBox = (TextBox)panel1.Controls[column];
                            cmd.Parameters.AddWithValue("@" + column, textBox.Text);
                        }

                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Adaugare cu succes!");

                        cs.Close();

                        update();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("A apărut o eroare la adăugare: " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Selecteaza!");
                }
            }

        private void VoluntariDoubleClicked(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dataGridView2.Rows[e.RowIndex];
            string id = row.Cells[0].Value.ToString();
            IDVoluntar = id;

            Console.WriteLine( IDVoluntar );
        }
    }
}
