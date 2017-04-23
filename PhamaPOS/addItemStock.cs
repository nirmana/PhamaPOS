using PhamaPOS.common;
using PhamaPOS_Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhamaPOS
{
    public partial class addItemStock : Form
    {
        private int itemCode;
        public dbConnector _dbConnector = new dbConnector();
        public PhamaPOSEntities _PhamaPOSEntities;
        private user _user;
        public addItemStock()
        {
            InitializeComponent();
        }

        public addItemStock(int itemCode, user user)
        {
            this._user = user;
           this.itemCode = itemCode;
           InitializeComponent();
        }

        public addItemStock(user user)
        {
            // TODO: Complete member initialization
            this._user = user;
            InitializeComponent();
        }

        private void addItemStock_Load(object sender, EventArgs e)
        {
            if (itemCode > 0)
            {
                txtItemCode.Text = itemCode.ToString().PadLeft(4, '0');
                txtItemCode.Enabled = false;
                //txtAddingDate.Enabled = false;
                //txtEDate.Enabled = false;
                //txtMDate.Enabled = false;
                //txtSellingPrice.Enabled = false;
                //txtUnitPrice.Enabled = false;
                GetExistingItemBatchs();
                FillDetailsByBatchId();

            }
            else
            {

            }
            
        }

        private void FillDetailsByBatchId()
        {
           
            if (radioUpdate.Checked)
            {
                int batchId;
                bool result = int.TryParse(drpBatch.SelectedValue.ToString(), out batchId);
                if (result)
                {
                    _PhamaPOSEntities = _dbConnector.getConn();
                    var batchData = _PhamaPOSEntities.stocks.Where(a => a.stockId == batchId).FirstOrDefault();
                    txtQty.Text = batchData.runningQuantity.ToString();
                    txtUnitPrice.Text = batchData.itemUnitPriceBuying.ToString();
                    txtSellingPrice.Text = batchData.itemUnitPriceSelling.ToString();
                    txtAddingDate.Value = batchData.stockReceivedDate;
                    txtEDate.Value = batchData.stockExpDate;
                    txtMDate.Value = (DateTime)batchData.stockMfdDate;
                }
                else
                {
                    MessageBox.Show("Invalid Selection..");
                }
            }
        }

        private void radioAddNew_CheckedChanged(object sender, EventArgs e)
        {
            if (radioUpdate.Checked)
            {
                radioUpdate.Checked = false;
                radioAddNew.Checked = true;
                GetExistingItemBatchs();
                
            }
        }

        private void radioUpdate_CheckedChanged(object sender, EventArgs e)
        {
            if (radioAddNew.Checked)
            {
                radioAddNew.Checked = false;
                radioUpdate.Checked = true;
                GetExistingItemBatchs();
                
            }
        }

        private void GetExistingItemBatchs()
        {
            int iCode;
            bool result = int.TryParse(txtItemCode.Text, out iCode);
            if(result){
                _PhamaPOSEntities = _dbConnector.getConn();
                var Ds =_PhamaPOSEntities.items.Where(a => a.itemCode == iCode);
                var itemCount = Ds.Count();
                if (itemCount > 0)
                {
                    lblItemName.Text = Ds.FirstOrDefault().itemDescription;
                    var Ds1 = Ds
                        .FirstOrDefault().stocks
                        .Where(a => a.isAvailable == true);
                    drpBatch.DataSource = null;
                    drpBatch.Items.Clear();
                    if (Ds1.Count() > 0)
                    {
                        if (!radioAddNew.Checked)
                        {
                           
                            drpBatch.DataSource = Ds1
                                .Reverse()
                                .Select(a => new
                                {
                                    a.stockId,
                                    a.stockReceivedDate
                                }).ToList();

                            drpBatch.ValueMember = "stockId";
                            drpBatch.DisplayMember = "stockReceivedDate";
                            FillDetailsByBatchId();
                        }
                        else
                        {
                            Dictionary<string, string> comboSource = new Dictionary<string, string>();
                            comboSource.Add("0", "New Batch");
                            drpBatch.DataSource = new BindingSource(comboSource, null);
                            drpBatch.DisplayMember = "Value";
                            drpBatch.ValueMember = "Key";
                        }
                    }
                    else
                    {
                        Dictionary<string, string> comboSource = new Dictionary<string, string>();
                        comboSource.Add("0", "New Batch");
                        drpBatch.DataSource = new BindingSource(comboSource, null);
                        drpBatch.DisplayMember = "Value";
                        drpBatch.ValueMember = "Key";
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Item Code...");
                    txtItemCode.Text = "";
                }
            }
            else
            {
                MessageBox.Show("Invalid Item Code...");
                txtItemCode.Text = "";
            }
        }

        private void txtQty_Leave(object sender, EventArgs e)
        {
            string Quantity = txtQty.Text;
            int iQuantity;
            bool result = int.TryParse(Quantity, out iQuantity);
            if (result)
            {

            }
            else
            {
                MessageBox.Show("Invalid Item Quantity...");
                txtQty.Text = "";
            }
        }

        private void txtUnitPrice_Leave(object sender, EventArgs e)
        {
            string UnitPrice = txtUnitPrice.Text;
            float iUnitPrice;
            bool result = float.TryParse(UnitPrice, out iUnitPrice);
            if (result)
            {
                txtUnitPrice.Text = iUnitPrice.ToString("0.00");
            }
            else
            {
                MessageBox.Show("Invalid Unit Price...");
                txtUnitPrice.Text="";
            }
        }

        private void txtItemCode_Leave(object sender, EventArgs e)
        {
            txtItemCode.Text = txtItemCode.Text.PadLeft(4, '0');
            txtItemCode.Enabled = true;
            GetExistingItemBatchs();
        }

        private void txtSellingPrice_Leave(object sender, EventArgs e)
        {
            string SellingPrice = txtSellingPrice.Text;
            float iSellingPrice;
            bool result = float.TryParse(SellingPrice, out iSellingPrice);
            if (result)
            {
                txtSellingPrice.Text = iSellingPrice.ToString("0.00");
            }
            else
            {
                MessageBox.Show("Invalid Selling Price...");
                txtSellingPrice.Text = "";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (radioAddNew.Checked)
                {
                    string code = txtItemCode.Text;
                    string Quantity = txtQty.Text;
                    string UnitPrice = txtUnitPrice.Text;
                    string SellingPrice = txtSellingPrice.Text;
                    DateTime addedDate = txtAddingDate.Value.Date;
                    DateTime expDate = txtEDate.Value.Date;
                    DateTime mfDate = txtMDate.Value.Date;

                    int icode = Convert.ToInt32(code);
                    if (code != "" && Quantity != "" && UnitPrice != "" && SellingPrice != "")
                    {
                        _PhamaPOSEntities = _dbConnector.getConn();
                        int itemId = _PhamaPOSEntities.items.Where(a => a.itemCode == icode).FirstOrDefault().itemId;
                        stock _stock = new stock();
                        _stock.itemId = itemId;
                        _stock.isAvailable = true;
                        _stock.entryDate = DateTime.Now;
                        _stock.entryBy = _user.userName;
                        _stock.itemUnitPriceBuying = Convert.ToDecimal(UnitPrice);
                        _stock.itemUnitPriceSelling = Convert.ToDecimal(SellingPrice);
                        _stock.runningQuantity = Convert.ToInt32(Quantity);
                        _stock.stockExpDate = expDate;
                        _stock.stockMfdDate = mfDate;
                        _stock.StockQuantity = Convert.ToInt32(Quantity);
                        _stock.stockReceivedDate = addedDate;

                        _PhamaPOSEntities.stocks.Add(_stock);
                        _PhamaPOSEntities.SaveChanges();
                        ClearFields();
                        MessageBox.Show("Stock Successfully added..");
                        if (itemCode != null)
                        {
                            this.Dispose();
                            addItem i = new addItem(_user);
                            i.Show();
                        }
                        else
                        {
                            this.Dispose();

                        }
                    }
                    else
                    {
                        MessageBox.Show("Please Fill All the mandetary fields..");
                    }
                }
                else
                {
                    string code = txtItemCode.Text;
                    string Quantity = txtQty.Text;
                    string UnitPrice = txtUnitPrice.Text;
                    string SellingPrice = txtSellingPrice.Text;
                    DateTime addedDate = txtAddingDate.Value.Date;
                    DateTime expDate = txtEDate.Value.Date;
                    DateTime mfDate = txtMDate.Value.Date;
                    int batchId = Convert.ToInt32(drpBatch.SelectedValue);
                    int icode = Convert.ToInt32(code);
                    if (code != "" && Quantity != "" && UnitPrice != "" && SellingPrice != "")
                    {

                        _PhamaPOSEntities = _dbConnector.getConn();
                        var _stock = _PhamaPOSEntities.stocks.Where(a => a.stockId == batchId).FirstOrDefault();

                        _stock.stockExpDate = expDate;
                        _stock.runningQuantity = Convert.ToInt32(Quantity);
                        _stock.itemUnitPriceBuying = Convert.ToDecimal(UnitPrice);
                        _stock.itemUnitPriceSelling = Convert.ToDecimal(SellingPrice);
                        _stock.stockMfdDate = mfDate;
                        _stock.StockQuantity = Convert.ToInt32(Quantity);

                        _PhamaPOSEntities.stocks.Attach(_stock);
                        var entry = _PhamaPOSEntities.Entry(_stock);

                        entry.Property(a => a.runningQuantity).IsModified = true;
                        entry.Property(a => a.itemUnitPriceBuying).IsModified = true;
                        entry.Property(a => a.itemUnitPriceSelling).IsModified = true;
                        entry.Property(a => a.stockExpDate).IsModified = true;
                        entry.Property(a => a.stockMfdDate).IsModified = true;
                        entry.Property(a => a.StockQuantity).IsModified = true;
                        _PhamaPOSEntities.SaveChanges();
                        MessageBox.Show("Stock Successfully updated..");
                        ClearFields();
                        this.Hide();


                    }
                    else
                    {
                        MessageBox.Show("Please Fill All the mandetary fields..");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Occured.. Please Retry.. " + ex.Message);
            }
            
        }

        private void ClearFields()
        {
            if (radioAddNew.Checked)
            {
                //txtItemCode.Text = ""; 
                txtQty.Text = ""; 
                txtUnitPrice.Text = ""; 
                txtSellingPrice.Text = ""; 
                txtAddingDate.Value = DateTime.Now;
                txtEDate.Value = DateTime.Now;
                txtMDate.Value = DateTime.Now;
                drpBatch.Refresh();
            }
            else
            {
                txtQty.Text = "";
                txtUnitPrice.Text = "";
                txtSellingPrice.Text = "";
                txtAddingDate.Value = DateTime.Now;
                txtEDate.Value = DateTime.Now;
                txtMDate.Value = DateTime.Now;
                drpBatch.Refresh();
                
            }

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void drpBatch_SelectedIndexChanged(object sender, EventArgs e)
        {
           
          // FillDetailsByBatchId();
        }

        private void drpBatch_SelectedValueChanged(object sender, EventArgs e)
        {
            //FillDetailsByBatchId();
        }

        private void drpBatch_ValueMemberChanged(object sender, EventArgs e)
        {
            //FillDetailsByBatchId();
        }

        private void drpBatch_SelectionChangeCommitted(object sender, EventArgs e)
        {
            FillDetailsByBatchId();
        }

        private void addItemStock_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (itemCode > 0) {
                addItem myform = new addItem(_user);
                myform.Show();
            }
        }

       
    }
}
