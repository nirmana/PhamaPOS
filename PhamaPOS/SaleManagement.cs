using PhamaPOS.common;
using PhamaPOS_Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhamaPOS
{
    public partial class SaleManagement : Form
    {
        public dbConnector _dbConnector = new dbConnector();
        public PhamaPOSEntities _PhamaPOSEntities;
        public DataTable tblMostSold;
        public DataTable tblTransactions;
        public DataTable tblTransactionsDetails;

        public SaleManagement()
        {
            InitializeComponent();
            tblMostSold = new DataTable();
            tblMostSold.Columns.Add("Item Name",typeof(string));
            tblMostSold.Columns.Add("Item Quantity",typeof(int));
            tblMostSold.Columns.Add("Value", typeof(decimal));

            tblTransactions = new DataTable();
            tblTransactions.Columns.Add("BatchId", typeof(int));
            tblTransactions.Columns.Add("Sale Date", typeof(DateTime));
            tblTransactions.Columns.Add("Gross Amount", typeof(decimal));
            tblTransactions.Columns.Add("Nett Amount", typeof(decimal));
            tblTransactions.Columns.Add("Discount", typeof(decimal));
            tblTransactions.Columns.Add("User", typeof(string));

            tblTransactionsDetails = new DataTable();
            tblTransactionsDetails.Columns.Add("Item Name", typeof(string));
            tblTransactionsDetails.Columns.Add("Item Code", typeof(int));

            tblTransactionsDetails.Columns.Add("Quantity", typeof(decimal));
            tblTransactionsDetails.Columns.Add("Unit Price", typeof(decimal));
            tblTransactionsDetails.Columns.Add("Amount", typeof(decimal));
          

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            //DateTime dateFrom = dateTimePicker1.Value;
            //DateTime dateTo = dateTimePicker2.Value;


            //if (dateFrom > dateTo)
            //{
            //   // MessageBox.Show("to date should be grater than from date ");
            //    getAllStat(dateTo, dateFrom);
            //    GetMostSoldItems(dateTo, dateFrom);
            //    GetAllTransactions(dateTo, dateFrom);
            //}
            //else
            //{
            //    getAllStat(dateFrom, dateTo);
            //    GetMostSoldItems(dateFrom, dateTo);
            //    GetAllTransactions(dateFrom, dateTo);
            //}

        }

        private void GetAllTransactions(DateTime dateFrom, DateTime dateTo)
        {
            _PhamaPOSEntities = new PhamaPOSEntities();
            _PhamaPOSEntities = _dbConnector.getConn();
            tblTransactions.Rows.Clear();
            grdAllTransactions.DataSource = null;
            tblTransactionsDetails.Rows.Clear();
            if (dateFrom == dateTo)
            {
                var sales = _PhamaPOSEntities.saleBatches
                     .Where(a => EntityFunctions.TruncateTime(a.entryDate) == EntityFunctions.TruncateTime(dateFrom))
                     .Select(a => new
                     {
                        a.saleBatchId, 
                        a.entryDate,
                        a.entryBy,
                        a.discountedAmount,
                        a.grandTotal,   
                        a.subTotal
                         
                     }).Distinct();
                foreach (var rowObj in sales)
                {
                    DataRow dr = tblTransactions.NewRow();
                    dr["BatchId"] = rowObj.saleBatchId;
                    dr["Sale Date"] = rowObj.entryDate;
                    dr["User"] = rowObj.entryBy;
                    dr["Discount"] = rowObj.discountedAmount;
                    dr["Gross Amount"] = rowObj.grandTotal;
                    dr["Nett Amount"] = rowObj.subTotal;

                    tblTransactions.Rows.Add(dr);
                }

                grdAllTransactions.DataSource = tblTransactions;
                grdAllTransactions.Columns[0].Visible = false;
                grdAllTransactions.RowHeadersVisible = false;
                grdAllTransactions.Sort(this.grdAllTransactions.Columns["Sale Date"], ListSortDirection.Descending);
                grdAllTransactions.Refresh();
            }
            else
            {
                var sales = _PhamaPOSEntities.saleBatches
                    .Where(a => EntityFunctions.TruncateTime(a.entryDate) <= EntityFunctions.TruncateTime(dateTo) && EntityFunctions.TruncateTime(a.entryDate) >= EntityFunctions.TruncateTime(dateFrom))

                       .Select(a => new
                       {
                           a.saleBatchId,
                           a.entryDate,
                           a.entryBy,
                           a.discountedAmount,
                           a.grandTotal,
                           a.subTotal

                       }).Distinct();
                foreach (var rowObj in sales)
                {
                    DataRow dr = tblTransactions.NewRow();
                    dr["BatchId"] = rowObj.saleBatchId;
                    dr["Sale Date"] = rowObj.entryDate;
                    dr["User"] = rowObj.entryBy;
                    dr["Discount"] = rowObj.discountedAmount;
                    dr["Gross Amount"] = rowObj.grandTotal;
                    dr["Nett Amount"] = rowObj.subTotal;

                    tblTransactions.Rows.Add(dr);
                }

                grdAllTransactions.DataSource = tblTransactions;
                grdAllTransactions.Columns[0].Visible = false;
                grdAllTransactions.RowHeadersVisible = false;

               grdAllTransactions.Sort(this.grdAllTransactions.Columns["Sale Date"], ListSortDirection.Descending);
                grdAllTransactions.Refresh();
            }
        }

        private void GetMostSoldItems( DateTime dateFrom,DateTime dateTo)
        {
            _PhamaPOSEntities = new PhamaPOSEntities();
            _PhamaPOSEntities = _dbConnector.getConn();
            tblMostSold.Rows.Clear();
            if (dateFrom == dateTo)
            {
               var sales = _PhamaPOSEntities.sales
                    .Where(a => EntityFunctions.TruncateTime(a.saleBatch.entryDate) == EntityFunctions.TruncateTime(dateTo))
                    .GroupBy(a => new { a.stock.item })
                    .OrderByDescending(a =>  a.Sum(b => b.soldQuantity))
                    .Take(100)
                    
                    .Select(a => new
                    {
                        a.FirstOrDefault().stock.item.itemDescription,
                        soldQuantity = a.Sum(b => b.soldQuantity),
                        soldValue = a.Sum(b => b.amount)
                    }).Distinct();
               foreach (var rowObj in sales)
               {
                   DataRow dr = tblMostSold.NewRow();
                   dr["Item Name"] = rowObj.itemDescription;
                   dr["Item Quantity"] = rowObj.soldQuantity;
                   dr["Value"] = rowObj.soldValue;
                   tblMostSold.Rows.Add(dr);
               }
               
               grdMostSold.DataSource = tblMostSold;
               grdMostSold.RowHeadersVisible = false;

               grdMostSold.Sort(this.grdMostSold.Columns["Item Quantity"], ListSortDirection.Descending);
               grdMostSold.Refresh();
            }
            else
            {
                var sales = _PhamaPOSEntities.sales
                    .Where(a => EntityFunctions.TruncateTime(a.saleBatch.entryDate) <= EntityFunctions.TruncateTime(dateTo) && EntityFunctions.TruncateTime(a.saleBatch.entryDate) >= EntityFunctions.TruncateTime(dateFrom))
                      .GroupBy(a => new { a.stock.item })
                     .OrderByDescending(a => a.Sum(b => b.soldQuantity))
                    .Take(100)
                  
                    .Select(a => new
                    {
                        a.FirstOrDefault().stock.item.itemDescription,
                        soldQuantity = a.Sum(b => b.soldQuantity),
                        soldValue = a.Sum(b => b.amount)
                    }).Distinct();
                foreach (var rowObj in sales)
                {
                    DataRow dr = tblMostSold.NewRow();
                    dr["Item Name"] = rowObj.itemDescription;
                    dr["Item Quantity"] = rowObj.soldQuantity;
                    dr["Value"] = rowObj.soldValue;
                    tblMostSold.Rows.Add(dr);
                }
                //grdMostSold.Rows.Clear();
                grdMostSold.DataSource = tblMostSold;
                grdMostSold.RowHeadersVisible = false;

                grdMostSold.Sort(this.grdMostSold.Columns["Item Quantity"], ListSortDirection.Descending);
                grdMostSold.Refresh();
            }

  

        }

        private void getAllStat(DateTime dateFrom, DateTime dateTo)
        {
            _PhamaPOSEntities = _dbConnector.getConn();
            List<saleBatch> allSaleBatches = new List<saleBatch>();
            if (dateFrom == dateTo)
            {
                 allSaleBatches = _PhamaPOSEntities.saleBatches.Where(a => EntityFunctions.TruncateTime(a.saleDate) == EntityFunctions.TruncateTime(dateFrom)).ToList();

            }
            else {
                allSaleBatches = _PhamaPOSEntities.saleBatches.Where(a => EntityFunctions.TruncateTime(a.saleDate) >= EntityFunctions.TruncateTime(dateFrom) && EntityFunctions.TruncateTime(a.saleDate) <= EntityFunctions.TruncateTime(dateTo)).ToList();
            
            }
            if (allSaleBatches.Count() >0) {
                lbltotalSales.Text = string.Format("{0:n0}", allSaleBatches.Sum(a => a.subTotal).ToString("#,##0"));
                lblNettSales.Text = string.Format("{0:n0}", allSaleBatches.Sum(a => a.grandTotal).ToString("#,##0"));
                lblProfit.Text = string.Format("{0:n0}", allSaleBatches.Sum(a => a.discountedAmount).ToString("#,##0"));
            }
            else
            {
                lbltotalSales.Text = "N/A";
                lblNettSales.Text = "N/A";
                lblProfit.Text = "N/A";
            }

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            //DateTime dateFrom = dateTimePicker1.Value;
            //DateTime dateTo = dateTimePicker2.Value;

            //if (dateFrom > dateTo)
            //{
            // //   MessageBox.Show("to date should be grater than from date ");
            //    getAllStat(dateTo, dateFrom);
            //    GetMostSoldItems(dateTo, dateFrom);
            //    GetAllTransactions(dateTo, dateFrom);
            //}
            //else
            //{
            //    getAllStat(dateFrom, dateTo);
            //    GetMostSoldItems(dateFrom, dateTo);
            //    GetAllTransactions(dateFrom, dateTo);
            //}
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void grdAllTransactions_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
         

        }

        private void grdAllTransactions_SelectionChanged(object sender, EventArgs e)
        {
           
        }

        private void grdAllTransactions_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void grdAllTransactions_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            object value = grdAllTransactions.Rows[e.RowIndex].Cells[0].Value;
           // MessageBox.Show(value.ToString());
            int saleBatchId = 0;
            var result = int.TryParse(value.ToString(),out saleBatchId);
            if (result)
            {
                GetAllSaleBatchDetailsBySaleBatchId(saleBatchId);

            }
        }

        private void GetAllSaleBatchDetailsBySaleBatchId(int saleBatchId)
        {
            this.grdAllTransactionsDetails.DataSource = null;
            grdAllTransactionsDetails.Rows.Clear();
            tblTransactionsDetails.Rows.Clear();
            var sales = _PhamaPOSEntities.sales
                     .Where(a => a.saleBatchId == saleBatchId)
                     .Select(a => new
                     {
                         a.saleBatchId,
                         a.amount,
                         a.stock.item.itemDescription,
                         a.stock.item.itemCode,
                         a.unitPrice,
                         a.soldQuantity
                     }).Distinct();
            foreach (var rowObj in sales)
            {
                DataRow dr = tblTransactionsDetails.NewRow();
                dr["Item Name"] = rowObj.itemDescription;
                dr["Item Code"] = rowObj.itemCode;
                dr["Quantity"] = rowObj.soldQuantity;
                dr["Unit Price"] = rowObj.unitPrice;
                dr["Amount"] = rowObj.amount;
                tblTransactionsDetails.Rows.Add(dr);
            }

            grdAllTransactionsDetails.DataSource = tblTransactionsDetails;
            grdAllTransactionsDetails.RowHeadersVisible = false;
            grdAllTransactionsDetails.Sort(this.grdAllTransactionsDetails.Columns["Item Code"], ListSortDirection.Ascending);
            grdAllTransactionsDetails.Refresh();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime dateFrom = dateTimePicker1.Value;
            DateTime dateTo = dateTimePicker2.Value;

            if (dateFrom > dateTo)
            {
                //   MessageBox.Show("to date should be grater than from date ");
                getAllStat(dateTo, dateFrom);
                GetMostSoldItems(dateTo, dateFrom);
                GetAllTransactions(dateTo, dateFrom);
            }
            else
            {
                getAllStat(dateFrom, dateTo);
                GetMostSoldItems(dateFrom, dateTo);
                GetAllTransactions(dateFrom, dateTo);
            }
        }
    }
}
