using PhamaPOS.common;
using PhamaPOS_Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PhamaPOS
{
    public partial class checkOut : Form
    {
        private DataTable _tblItems;
        private string _subTotal;
        private string _txtDiscAmt;
        private string _grandTotal;
        private user _user;
        public dbConnector _dbConnector = new dbConnector();
        public PhamaPOSEntities _PhamaPOSEntities;

        public checkOut()
        {
            InitializeComponent();
        }

        public checkOut(DataTable tblItems, string subTotal, string txtDiscAmt, string grandTotal, user user)
        {
            InitializeComponent();
            this._tblItems = tblItems;
            this._subTotal = subTotal;
            this._txtDiscAmt = txtDiscAmt;
            this._grandTotal = grandTotal;
            this._user = user;

            txtTotalAmt.Text = grandTotal;
        }

        private void button2_Click(object sender, EventArgs e)
        {
          
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            using (var contex = new PhamaPOSEntities() )
            {
                
                using (var dbContextTransaction = contex.Database.BeginTransaction())
                {
                    try
                    {
                        saleBatch sb = new saleBatch();
                        sb.entryBy = _user.userName;
                        sb.entryDate = DateTime.Now;
                        sb.saleDate = DateTime.Now;
                        sb.discountedAmount = Convert.ToDecimal(_txtDiscAmt);
                        sb.grandTotal = Convert.ToDecimal(_grandTotal);
                        sb.subTotal = Convert.ToDecimal(_subTotal);
                        contex.saleBatches.Add(sb);
                        contex.SaveChanges();

                        foreach (DataRow dr in _tblItems.Rows)
                        {
                            Int64 batchID = Convert.ToInt64(dr["Item Sock"]);
                            int quantity = Convert.ToInt32(dr["Quantity"]);
                            //update stock
                            stock s = new stock();
                            s = contex.stocks.Where(a => a.stockId == batchID).FirstOrDefault();
                            if (s.runningQuantity == quantity)
                            {

                                s.runningQuantity = 0;
                                s.isAvailable = false;
                                s.stockClearedDate = DateTime.Now;

                                contex.stocks.Attach(s);
                                var entry = contex.Entry(s);
                                entry.Property(a => a.runningQuantity).IsModified = true;
                                entry.Property(a => a.isAvailable).IsModified = true;
                                entry.Property(a => a.stockClearedDate).IsModified = true;
                                contex.SaveChanges();

                            }
                            else
                            {
                                s.runningQuantity = s.runningQuantity - quantity;
                                contex.stocks.Attach(s);
                                var entry = contex.Entry(s);
                                entry.Property(a => a.runningQuantity).IsModified = true;
                                contex.SaveChanges();
                            }
                            //update sale 
                            sale sale = new sale();
                            sale.amount = Convert.ToDecimal(dr["Total"]);
                            sale.itemStockId = Convert.ToInt64(dr["Item Sock"]);
                            sale.saleBatchId = sb.saleBatchId;
                            sale.soldQuantity = Convert.ToInt64(dr["Quantity"]);
                            sale.unitPrice = Convert.ToDecimal(dr["Unit Price"]);
                            contex.sales.Add(sale);
                            contex.SaveChanges();
                            
                            //form.Show();
                            

                        }
                        dbContextTransaction.Commit();
                        MessageBox.Show("Payment Proceed Successfull..");
                        home form = new home(_user);
                        form.Show();
                        this.Close();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Error Occured.. Please Retry..");
                        dbContextTransaction.Rollback();
                    }
                }
                
            } 
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text.Length>0)
            {
                textBox2.Text = (Convert.ToDecimal(textBox1.Text) - Convert.ToDecimal(txtTotalAmt.Text)).ToString();
            }
           
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
        (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
    }
}
