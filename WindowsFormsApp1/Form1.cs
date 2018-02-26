using System;
using System.Data;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Configuration;
using NLog;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form

 {
        
        BindingSource bindSourse = new BindingSource();
        Logger logger = LogManager.GetCurrentClassLogger();

        public static string connectionStringGl;

        public Form1()
        {
            InitializeComponent();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            
            logger.Trace("trace message");
            logger.Debug("debug message");
            logger.Info("info message");
            logger.Warn("warn message");
            logger.Error("error message");
            logger.Fatal("fatal message");

        }

        public void GetConnectionStrings()
        {


            var appSettings =  ConfigurationSettings.AppSettings;
            if (appSettings.Count == 0)
            {
                Console.WriteLine("AppSettings is empty.");
            }
            else
            {
                foreach (var key in appSettings.AllKeys)
                {
                    Console.WriteLine("Key: {0} Value: {1}", key, appSettings[key]);
                }

                connectionStringGl = appSettings["DataSourse"] + appSettings["Password"] + pass.Text  + appSettings["UserId"] + login.Text + appSettings["DB"];
               
            }

            logger.Debug("Строка соединения настроена");
            logger.Debug(connectionStringGl);

        }

        public DataSet ds = new DataSet();
        public string lastUseSql = "";
        public string conn = "";


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                GetConnectionStrings();
                string connectionString = @"Provider=SQLOLEDB;" + connectionStringGl;
                string sqlQuery = "SELECT name, id, xtype FROM  dbo.sysobjects where xtype = 'u'";
                OleDbDataAdapter dataAdapter = new OleDbDataAdapter(sqlQuery, connectionString);
                // создаем объект DataSet
                DataSet ds = new DataSet();
                ds.Tables.Add("[sysobjects]");

                // заполняем таблицу sysobjects  
                // данными из базы данных
                dataAdapter.Fill(ds, "[sysobjects]");




                comboBox1.Items.Clear();


                comboBox1.DisplayMember = "Text";
                comboBox1.ValueMember = "Value";

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //combo1.Items.Add(new ListItem("Text", "Value"));
                    comboBox1.Items.Add(new { Text = ds.Tables[0].Rows[i]["name"], Value = ds.Tables[0].Rows[i]["id"] });


                    //  comboBox1.ValueMember = ds.Tables[0].Rows[i]["id"];

                }

                dataAdapter.Dispose();
                logger.Trace("Database connected");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Проверьте логин и пароль");
                logger.Error("Connect filed. Try config and NetWork");
                logger.Error(ex.ToString());
            }

        }

        private void Login_TextChanged(object sender, EventArgs e)
        {

        }

        private void pass_TextChanged(object sender, EventArgs e)
        {

        }
      

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                
                string sql = @"SELECT * FROM " + comboBox1.Text;
                string connectionString = @"Provider=SQLOLEDB; " + connectionStringGl;


                OleDbDataAdapter dataAdapter = new OleDbDataAdapter(sql, connectionString);
                // создаем объект DataSet
                DataSet ds = new DataSet();
                ds.Tables.Add("["+comboBox1.Text+"]");
                this.bindSourse.DataSource = ds.Tables[comboBox1.Text];
                // заполняем таблицу sysobjects  
                // данными из базы данных
                dataAdapter.Fill(ds, "[" + comboBox1.Text + "]");


                //добавление таблицы в грид
                dataGridView1.DataSource = ds.Tables[0].DefaultView;
                logger.Trace("Table selected and loaded successfully");
            }
            catch (Exception ex)
            {
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                dataGridView1.Refresh();


                MessageBox.Show("Ошибка таблицы базы данных!!!");

                logger.Error("Database table do not loaded");
                logger.Error(ex.ToString());
            }            

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {

            
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView1_CellBorderStyleChanged(object sender, EventArgs e)
        {
           
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void bn_RefreshItems(object sender, EventArgs e)
        {

        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                GetConnectionStrings();
                string sql = @"SELECT * FROM " + comboBox1.Text;
                string connectionString = @connectionStringGl;
                string tableName = comboBox1.Text;

                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql, connection);
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(sql, connectionString);
                System.Data.DataTable dt = new System.Data.DataTable();
                da.Fill(ds, tableName);
                ds.Tables.Add(dt);
                DataView view = new DataView(ds.Tables[tableName]);
                SqlCommandBuilder cmdbl = new SqlCommandBuilder(da);
                da.Update(ds, tableName);
          

            
                connection.Open();
                dataGridView1.DataSource = view;
                connection.Close();
                logger.Trace("Update table successfull");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Выберите таблицу");
                logger.Error("Update table exseption");
                logger.Error(ex.ToString());
            }

        }

        private void delBtn_Click(object sender, EventArgs e)
        {
            try
            {
                GetConnectionStrings();
                SqlCommandBuilder cmdBuilder;
                SqlConnection cn = new SqlConnection();
                DataSet CustomersDataSet = new DataSet();
                SqlDataAdapter da;
                SqlCommand DADeleteCmd;


                string sql = @"SELECT * FROM " + comboBox1.Text;
                string connectionString = @connectionStringGl;
                cn.ConnectionString = connectionString;

                cn.Open();

                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {

                    string nopr = dataGridView1["id", dataGridView1.CurrentRow.Index].Value.ToString();
                    string sqlDelete = @"DELETE FROM  [dbo].[" + comboBox1.Text + "]  WHERE [id] = '" + nopr + "'";
                  
                    da = new SqlDataAdapter(sql, cn);
                    da = new SqlDataAdapter(sql, cn);

                    DADeleteCmd = new SqlCommand(sqlDelete, da.SelectCommand.Connection);

                    da.DeleteCommand = DADeleteCmd;
                    cmdBuilder = new SqlCommandBuilder(da);

                    da.Fill(CustomersDataSet, comboBox1.Text);
                    da.DeleteCommand = new SqlCommand(sqlDelete, cn);

                    da.DeleteCommand.ExecuteNonQuery();



                    dataGridView1.Rows.Remove(row);
                }
                cn.Close();
                logger.Trace("Row delete successfull");

            }
            catch(Exception ex)
            {

                dataGridView1.Rows.Clear();
                dataGridView1.Refresh();
                logger.Error("Failed to delete row");
                logger.Error(ex.ToString());


                MessageBox.Show(ex.ToString());
            }
            
        }

        private void bn_RefreshItems_1(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                SqlCommandBuilder cmdBuilder;
                SqlConnection cn = new SqlConnection();
                DataSet CustomersDataSet = new DataSet();
                SqlDataAdapter da;
                SqlCommand DAUpdateCmd;

                string col = dataGridView1.Columns[e.ColumnIndex].DataPropertyName;
                string dataChange = dataGridView1.EditingControl.Text;
                string anyColValue = Convert.ToString(dataGridView1.CurrentRow.Cells["id"].Value);

                string sql = @"SELECT * FROM " + comboBox1.Text;
                string connectionString = @connectionStringGl;
                string sqlUpdate = @"UPDATE [dbo].[" + comboBox1.Text + "]  SET [" + col + "] = '" + dataChange + "'  WHERE [id] = '" + anyColValue + "'";
                cn.ConnectionString = connectionString;

                cn.Open();

                da = new SqlDataAdapter(sql, cn);

                DAUpdateCmd = new SqlCommand(sqlUpdate, da.SelectCommand.Connection);

                da.UpdateCommand = DAUpdateCmd;
                cmdBuilder = new SqlCommandBuilder(da);

                da.Fill(CustomersDataSet, comboBox1.Text);
                da.UpdateCommand = new SqlCommand(sqlUpdate, cn);

                da.UpdateCommand.ExecuteNonQuery();
                cn.Close();

                logger.Trace("Cell update successful");

            }
            catch (Exception ex)
            {
                logger.Error("Cell update error");
                logger.Error(ex.ToString());
            }
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {

        }

        private void addBtn_Click(object sender, EventArgs e)
        {

            try
            {

                if ((dataGridView1.Rows.Count - 1) < 0)
                {

                    dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];

                }
                else
                {
                    dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0];

                }
                string sqlInsert = "INSERT INTO [dbo].[" + comboBox1.Text + "] (";


                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    if (i == dataGridView1.Columns.Count - 1)
                    {
                        sqlInsert += "[" + dataGridView1.Columns[i].Name + "] ";
                    }
                    else
                    {
                        sqlInsert += "[" + dataGridView1.Columns[i].Name + "], ";

                    }

                }

                sqlInsert += ") VALUES (";

                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    if (i == dataGridView1.Columns.Count - 1)
                    {
                        sqlInsert += "'" + (String)dataGridView1[dataGridView1.Columns[i].Name.ToString(), dataGridView1.Rows.Count - 2].Value + "' ";

                    }
                    else
                    {

                        sqlInsert += "'" + dataGridView1[dataGridView1.Columns[i].Name.ToString(), dataGridView1.Rows.Count - 2].Value + "', ";


                    }

                }
                sqlInsert += ")";




                SqlCommandBuilder cmdBuilder;
                SqlConnection cn = new SqlConnection();
                DataSet CustomersDataSet = new DataSet();
                SqlDataAdapter da;
                SqlCommand DAUpdateCmd;


                string sql = @"SELECT * FROM " + comboBox1.Text;
                string connectionString = @connectionStringGl;
                cn.ConnectionString = connectionString;

                cn.Open();

                da = new SqlDataAdapter(sql, cn);

                DAUpdateCmd = new SqlCommand(sqlInsert, da.SelectCommand.Connection);

                da.UpdateCommand = DAUpdateCmd;
                cmdBuilder = new SqlCommandBuilder(da);

                da.Fill(CustomersDataSet, comboBox1.Text);
                da.UpdateCommand = new SqlCommand(sqlInsert, cn);

                da.UpdateCommand.ExecuteNonQuery();
                cn.Close();

                logger.Trace("Insert new row successfull");

            }
            catch (Exception ex)
            {
                logger.Error("Insert filed");
                logger.Error(ex.ToString());
            }
        }
    }
}
