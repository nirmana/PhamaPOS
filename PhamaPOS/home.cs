using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PhamaPOS_Data;
using PhamaPOS.common;

namespace PhamaPOS
{
    public partial class home : Form
    {
        private user _user;
        public dbConnector _dbConnector = new dbConnector();
        public PhamaPOSEntities _PhamaPOSEntities;
        public DataTable tblItems;
        public home()
        {
            InitializeComponent();
            tblItems = new DataTable();
            tblItems.Columns.Add("item#");
            tblItems.Columns.Add("Item Code");
            tblItems.Columns.Add("Item Sock");
            tblItems.Columns.Add("Item Name");
            tblItems.Columns.Add("Unit Price");
            tblItems.Columns.Add("Quantity");
            tblItems.Columns.Add("Total");
        }

        public home(user user)
        {

            _user = user;
            InitializeComponent();
            tblItems = new DataTable();
            tblItems.Columns.Add("item#");
            tblItems.Columns.Add("Item Code");
            tblItems.Columns.Add("Item Sock");
            tblItems.Columns.Add("Item Name");
            tblItems.Columns.Add("Unit Price");
            tblItems.Columns.Add("Quantity");
            tblItems.Columns.Add("Total");

            if (_user.userType == "Admin")
            {
                adminToolStripMenuItem.Enabled = true;
            }
            else
            {
                adminToolStripMenuItem.Enabled = false;
            }
        }

        private void aaaToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
           
        }

        private void home_Load(object sender, EventArgs e)
        {
            //comboBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            //comboBox1.a
            // comboBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;
            //  AutoCompleteStringCollection combData = new AutoCompleteStringCollection();
            //getData(combData);
            // comboBox1.AutoCompleteCustomSource = combData;



            _PhamaPOSEntities = _dbConnector.getConn();
            var Ds = _PhamaPOSEntities.items;
                // .Where(a => a.itemCode.ToString().Contains(comboBox1.Text) || a.itemDescription.Contains(comboBox1.Text))

                
            var itemCount = Ds.Count();
            if (itemCount > 0)
            {


                comboBox1.DataSource = Ds
                           .Select(a => new
                           {
                               value = a.itemCode,
                               desc = a.itemDescription+"-->"+a.itemCode
                           }).ToList();

                comboBox1.ValueMember = "value";
                comboBox1.DisplayMember = "desc";
            }
        }

        private void getData(AutoCompleteStringCollection dataCollection)
        {

            _PhamaPOSEntities = _dbConnector.getConn();
             var Ds = _PhamaPOSEntities.items;
                var itemCount = Ds.Count();
                if (itemCount > 0)
                {
                    foreach (item item in Ds.ToList())
                    {
                        dataCollection.Add(item.itemCode+"-->"+item.itemDescription);
                    }
                }
            //string connetionString = null;
            //SqlConnection connection;
            //SqlCommand command;
            //SqlDataAdapter adapter = new SqlDataAdapter();
            //DataSet ds = new DataSet();
            //connetionString = "Data Source=.;Initial Catalog=pubs;User ID=sa;password=zen412";
            //string sql = "SELECT DISTINCT [fname] FROM [employee]";
            //connection = new SqlConnection(connetionString);
            //try
            //{
            //    connection.Open();
            //    command = new SqlCommand(sql, connection);
            //    adapter.SelectCommand = command;
            //    adapter.Fill(ds);
            //    adapter.Dispose();
            //    command.Dispose();
            //    connection.Close();
            //    foreach (DataRow row in ds.Tables[0].Rows)
            //    {
            //        dataCollection.Add(row[0].ToString());
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Can not open connection ! ");
            //}
        }
        private void button1_Click(object sender, EventArgs e)
        {
            addItem myForm = new addItem();
            //myForm.TopLevel = false;
            //myForm.WindowState = FormWindowState.Maximized;
            //panel1.Controls.Add(myForm);
            myForm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            addItemStock myForm = new addItemStock();
            myForm.Show();
        }

        private void addItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addItem myForm = new addItem(_user);
            myForm.Show();
        }

        private void stockManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addItemStock myForm = new addItemStock(_user);
            myForm.Show();
        }

        private void txtItem_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtItem_Leave(object sender, EventArgs e)
        {
            var  value = txtItem.Text;
            txtItem.Text = txtItem.Text.PadLeft(4, '0');
            
            GetExistingItemBatchs();
          //  comboBox1.SelectedValue = txtItem.Text;
        }

        private void GetExistingItemBatchs()
        {
            int iCode;
            bool result = int.TryParse(txtItem.Text, out iCode);
            if (result)
            {
                _PhamaPOSEntities = _dbConnector.getConn();
                var Ds = _PhamaPOSEntities.items.Where(a => a.itemCode == iCode);
                var itemCount = Ds.Count();
                if (itemCount > 0)
                {
                    txtItemName.Text = Ds.FirstOrDefault().itemDescription;
                    txtItemDetails.Text = Ds.FirstOrDefault().itemCallingName;
                   // comboBox1.SelectedText = "";
                    comboBox1.Text = Ds.FirstOrDefault().itemDescription+"-->"+Ds.FirstOrDefault().itemCode;

                    var Ds1 = Ds
                        .FirstOrDefault().stocks
                        .Where(a => a.isAvailable == true && a.runningQuantity>0)
                        .OrderBy(a=>a.entryDate);
                    drpBatch.DataSource = null;
                    drpBatch.Items.Clear();
                    if (Ds1.Count() > 0)
                    {
                        drpBatch.DataSource = Ds1
                            
                            .Select(a => new
                            {
                                a.stockId,
                                a.entryDate
                            }).ToList();

                        drpBatch.ValueMember = "stockId";
                        drpBatch.DisplayMember = "entryDate";
                        FillDetailsByBatchId();
                       
                    }
                    else
                    {
                        Dictionary<string, string> comboSource = new Dictionary<string, string>();
                        comboSource.Add("0", "No Stocks Found");
                        drpBatch.DataSource = new BindingSource(comboSource, null);
                        drpBatch.DisplayMember = "Value";
                        drpBatch.ValueMember = "Key";
                        Clearfields();
                      //  txtItem.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Item Code...");
                    txtItem.Text = "";
                    Clearfields();
                   // txtItem.Focus();
                }
            }
            else
            {
                MessageBox.Show("Invalid Item Code...");
                txtItem.Text = "";
               // txtItem.Focus();
            }

        }

        private void FillDetailsByBatchId()
        {
            int batchId;
            bool result = int.TryParse(drpBatch.SelectedValue.ToString(), out batchId);
            if (result)
            {
                _PhamaPOSEntities = _dbConnector.getConn();
                var batchData = _PhamaPOSEntities.stocks.Where(a => a.stockId == batchId).FirstOrDefault();
                if (batchData != null)
                {
                    txtQty.Text = batchData.runningQuantity.ToString();
                    txtUnitPrice.Text = batchData.itemUnitPriceSelling.ToString();
                    txtEDate.Value = batchData.stockExpDate;
                }
                else
                {
                    MessageBox.Show("No Stock found..");
                }
            }
            else
            {
                MessageBox.Show("Invalid Selection..");
            }
        }

        private void drpBatch_SelectionChangeCommitted(object sender, EventArgs e)
        {
            FillDetailsByBatchId();
        }

        private void Clearfields() {
            txtQty.Text = "";
            txtUnitPrice.Text = "";
            txtEDate.Value =DateTime.Now;
            txtItemName.Text = "";
        }

        private void btnAddToGrid_Click(object sender, EventArgs e)
        {

            int iCode;
            bool result = int.TryParse(txtItem.Text, out iCode);
            if (result)
            {
                _PhamaPOSEntities = _dbConnector.getConn();
                var Ds = _PhamaPOSEntities.items.Where(a => a.itemCode == iCode);
                var itemCount = Ds.Count();
                if (itemCount > 0)
                {
                    int batchId = Convert.ToInt32(drpBatch.SelectedValue);
                    if (batchId>0)
                    {
                        int iQuantity;
                        bool resultQty = int.TryParse(txtQuantity.Text, out iQuantity);
                        if (resultQty) {
                            if (iQuantity > 0)
                            {
                                long availableStock = Ds.FirstOrDefault().stocks.Where(a => a.stockId == batchId).FirstOrDefault().runningQuantity;
                                if (availableStock > 0) {
                                    if (iQuantity > availableStock) {
                                        MessageBox.Show("The Stock Quantity is Less Than Requested Quantity..");
                                    }
                                    else
                                    {
                                        bool isAvailable = false;
                                        foreach (DataRow _dr in tblItems.Rows)
                                        {
                                            if (Convert.ToInt32(_dr["Item Code"]) == iCode && batchId == Convert.ToInt32(_dr["Item Sock"]))
                                            {
                                                if (availableStock >= Convert.ToInt32(_dr["Quantity"]) + iQuantity) { 
                                                    int UpdatedQty = Convert.ToInt32(_dr["Quantity"]) + iQuantity;
                                                    _dr["Quantity"] = UpdatedQty;
                                                    _dr["Total"] = UpdatedQty * Ds.FirstOrDefault().stocks.Where(a => a.stockId == batchId).FirstOrDefault().itemUnitPriceSelling;

                                                }
                                                else
                                                {
                                                    MessageBox.Show("Quntity Exeed Available Stock..");
                                                }
                                                isAvailable = true;
                                                break;
                                            }
                                        }
                                        if (!isAvailable)
                                        {
                                            DataRow dr = tblItems.NewRow();
                                            dr["item#"] = tblItems.Rows.Count + 1;
                                            dr["Item Code"] = iCode;
                                            dr["Item Sock"] = batchId;
                                            dr["Item Name"] = Ds.FirstOrDefault().itemDescription;
                                            dr["Unit Price"] = Ds.FirstOrDefault().stocks.Where(a => a.stockId == batchId).FirstOrDefault().itemUnitPriceSelling;
                                            dr["Quantity"] = iQuantity;
                                            dr["Total"] = iQuantity * Ds.FirstOrDefault().stocks.Where(a => a.stockId == batchId).FirstOrDefault().itemUnitPriceSelling;
                                            tblItems.Rows.Add(dr);
                                            dataGridView1.DataSource = tblItems;
                                        }
                                        else
                                        {
                                            dataGridView1.Refresh();

                                        }
                                        updateSubTotal();
                                        txtItem.Focus();
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("No Stocks Found For Item..");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Invalid Quantity..");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Invalid Quantity..");
                        }
                      
                    }
                    else
                    {
                        MessageBox.Show("Invalid Stock for Item..");
                        txtItem.Text = "";
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Item Code...");
                    txtItem.Text = "";
                }
            }
            else
            {
                MessageBox.Show("Invalid Item Code...");
                txtItem.Text = "";
            }
        }

        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
           // MessageBox.Show(e.Row.Index.ToString());\
            if (tblItems.Rows.Count>1)
            {
                tblItems.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
                for (int i = dataGridView1.CurrentRow.Index; i < tblItems.Rows.Count; i++)
                {
                    tblItems.Rows[i]["item#"] = i + 1;
                }
            }
            else
            {
                tblItems.Rows.Clear();
            }
            dataGridView1.Refresh();
            updateSubTotal();
            e.Cancel = true;

        }

        public void updateSubTotal() {
            decimal subTotal=0;
            foreach (DataRow dr in tblItems.Rows)
            {
                subTotal += Convert.ToDecimal(dr["Total"]);
            }
            txtSubTotal.Text = subTotal.ToString();
            updateGrandTotal();
        }
        public void updateGrandTotal()
        {
            decimal subTotal = 0;
            foreach (DataRow dr in tblItems.Rows)
            {
                subTotal += Convert.ToDecimal(dr["Total"]);
            }
            decimal discRate =  Convert.ToDecimal(txtDisc.Text);
            txtGrandTotal.Text = (subTotal * (1 - (discRate / 100))).ToString("0.00");
            txtDiscAmt.Text = (subTotal * (discRate / 100)).ToString("0.00");
        }

        private void txtDisc_Leave(object sender, EventArgs e)
        {
             Decimal iQuantity;
            bool resultQty = Decimal.TryParse(txtDisc.Text, out iQuantity);
            if (resultQty && iQuantity<=100)
            {
                updateGrandTotal();
            }
            else
            {
                txtDisc.Text = "0.00";
                updateGrandTotal();
            }
           
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (tblItems.Rows.Count>0)
            {
                 DialogResult result1 = MessageBox.Show("Are you sure you want to checkout Added Items ?",
                   "Important Question",
                   MessageBoxButtons.YesNo);
                 if (result1 == DialogResult.Yes)
                 {
                     checkOut form = new checkOut(tblItems, txtSubTotal.Text, txtDiscAmt.Text, txtGrandTotal.Text, _user);
                     form.Show();
                     this.Hide();
                 }
            }
            else
            {
                MessageBox.Show("No Items in grid to Checkout...");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result1 = MessageBox.Show("Are You Sure you want to Clear Items ?",
                 "Important Question",
                 MessageBoxButtons.YesNo);
            if (result1 == DialogResult.Yes)
            {
                txtQty.Text = "";
                txtUnitPrice.Text = "";
                txtEDate.Value = DateTime.Now;
                txtItemName.Text = "";
                txtSubTotal.Text = "";
                txtDisc.Text = "0.00";
                txtGrandTotal.Text = "";
                txtDiscAmt.Text = "0.00";
                txtQuantity.Text = "";
                tblItems.Rows.Clear();
                dataGridView1.Refresh();
            }
        }

        private void itemInformationManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            itemInformationManagement iim = new itemInformationManagement();
            iim.Show();
        }

        private void home_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
           
           // comboBox1.Items.Clear();
            //if (comboBox1.Text.Length == 0)
            //{
            //  //  hideResults();
            //    return;
            //}

            //_PhamaPOSEntities = _dbConnector.getConn();
            //var Ds = _PhamaPOSEntities.items
            //    .Where(a => a.itemCode.ToString().Contains(comboBox1.Text) || a.itemDescription.Contains(comboBox1.Text))
                
            //    ;
            //var itemCount = Ds.Count();
            //if (itemCount > 0)
            //{


            //    comboBox1.DataSource = Ds
            //               .Select(a => new
            //               {
            //                   value = a.itemCode,
            //                   desc = a.itemCode.ToString()+"-->"+a.itemDescription
            //               }).ToList();

            //    comboBox1.ValueMember = "value";
            //    comboBox1.DisplayMember = "desc";
            //    //FillDetailsByBatchId();
            //}

            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           // DataRow selectedDataRow = ((DataRowView)comboBox1.SelectedItem).Row;

           // string selectedValue = (Dictionary<string, string>)comboBox1.SelectedItem;
            var selVal = comboBox1.SelectedValue;
           // MessageBox.Show(selVal.ToString());
            if (selVal != null)
            {
                int selectedId = 0;
                var result = int.TryParse(selVal.ToString(), out selectedId);
                if (result) {
                   // MessageBox.Show(selVal.ToString());
                    txtItem.Text = selVal.ToString().PadLeft(4, '0');
                    GetExistingItemBatchs();
                }

               
               
            }     

           // txtItem.Text = selectedValue.PadLeft(4, '0');
            //GetExistingItemBatchs();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void saleInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaleManagement sm = new SaleManagement();
            sm.Show();
        }

        private void comboBox1_Leave(object sender, EventArgs e)
        {
            drpBatch.Focus();
        }

      
    }
}
