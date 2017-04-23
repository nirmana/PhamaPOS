using PhamaPOS.common;
using PhamaPOS_Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhamaPOS
{
    public partial class itemInformationManagement : Form
    {
        public dbConnector _dbConnector = new dbConnector();
        public PhamaPOSEntities _PhamaPOSEntities;
        public DataTable tblItems;
        public itemInformationManagement()
        {
            InitializeComponent();
            tblItems = new DataTable();
           
            tblItems.Columns.Add("Item Code");
            tblItems.Columns.Add("Item Name");
            tblItems.Columns.Add("Item Description");
            tblItems.Columns.Add("Item Unit");
            tblItems.Columns.Add("Item Status");
            tblItems.Columns.Add("Stock Available", typeof(int));
            tblItems.Columns.Add("Stock Value", typeof(decimal));
            tblItems.Columns.Add("Stock Cost", typeof(decimal));
            tblItems.Columns.Add("Expiry Date", typeof(DateTime));
            getAllItems();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string searchString = textBox1.Text;
            checkBox1.Checked = false;
            checkBox2.Checked = false;
            checkBox3.Checked = false;
          
            _PhamaPOSEntities = _dbConnector.getConn();
            tblItems.Rows.Clear();
            List<item> _itemList = new List<item>();
            grdItem.DataSource = tblItems;
            if (searchString.Length>0)
            {
                _itemList = _PhamaPOSEntities.items.Where(a => a.itemDescription.ToLower().Contains(searchString.ToLower())).ToList();
            }
            else
            {
                _itemList = _PhamaPOSEntities.items.ToList();
               
            }
            foreach (item i in _itemList)
            {
                DataRow dr = tblItems.NewRow();
                dr["Item Code"] = i.itemCode.ToString().PadLeft(4, '0');
                dr["Item Name"] = i.itemDescription;
                dr["Item Description"] = i.itemCallingName;
                dr["Item Unit"] = i.unit.unitCode;
                dr["Item Status"] = i.itemStatus == true ? "Available" : "Not Available";
                dr["Stock Available"] = i.stocks.Sum(a => a.runningQuantity);
                //dr["Stock Value"] = i.stocks.Count != 0 ? ((int)(i.stocks.Average(a => a.itemUnitPriceSelling) * i.stocks.Sum(a => a.runningQuantity))) : 0.00;
                dr["Stock Value"] = i.stocks.Count != 0 ? (
                    i.stocks.Select(a => new { 
                        value = a.itemUnitPriceSelling*a.runningQuantity
                    }).Sum(a=>a.value)
                    ) : (decimal)0.00;
               
                dr["Stock Cost"] = i.stocks.Count != 0 ? ((int)(i.stocks.Average(a => a.itemUnitPriceBuying) * i.stocks.Sum(a => a.runningQuantity))) : 0.00;
                dr["Expiry Date"] = i.stocks.Count != 0 ? i.stocks.OrderByDescending(a => a.stockExpDate).FirstOrDefault().stockExpDate : DateTime.MinValue;


                tblItems.Rows.Add(dr);
            }
            grdItem.Refresh();
        }
        private void getAllItems() {
            _PhamaPOSEntities = _dbConnector.getConn();
            List<item> _itemList = new List<item>();
            tblItems.Rows.Clear();
            grdItem.DataSource = tblItems;
            _itemList = _PhamaPOSEntities.items.ToList();
            foreach (item i in _itemList)
            {
                DataRow dr = tblItems.NewRow();
                dr["Item Code"] = i.itemCode.ToString().PadLeft(4, '0');
                dr["Item Name"] = i.itemDescription;
                dr["Item Description"] = i.itemCallingName;
                dr["Item Unit"] = i.unit.unitCode;
                dr["Item Status"] = i.itemStatus == true ? "Available" : "Not Available";
                dr["Stock Available"] = i.stocks.Sum(a => a.runningQuantity);
              //  dr["Stock Value"] =i.stocks.Count!=0? ((int)(i.stocks.Average(a => a.itemUnitPriceSelling) * i.stocks.Sum(a => a.runningQuantity))):0.00;
                dr["Stock Value"] = i.stocks.Count != 0 ? (
                   i.stocks.Select(a => new
                   {
                       value = a.itemUnitPriceSelling * a.runningQuantity
                   }).Sum(a => a.value)
                   ) : (decimal)0.00;
                dr["Stock Cost"] = i.stocks.Count != 0 ? ((int)(i.stocks.Average(a => a.itemUnitPriceBuying) * i.stocks.Sum(a => a.runningQuantity))) : 0.00;
               dr["Expiry Date"]  = i.stocks.Count != 0 ? i.stocks.OrderByDescending(a => a.stockExpDate).FirstOrDefault().stockExpDate:DateTime.MinValue;
                tblItems.Rows.Add(dr);
            }

            LoadAllStockValue();
            grdItem.Refresh();
        
        }

        private void LoadAllStockValue()
        {
            _PhamaPOSEntities = _dbConnector.getConn();
            decimal ttlVal =
                   _PhamaPOSEntities.stocks.Select(a => new
                   {
                       value = a.itemUnitPriceSelling * a.runningQuantity
                   }).Sum(a => a.value);
            
            label2.Text = "Total Stock Value : " + ttlVal.ToString("#,##0");       
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
           
            if (checkBox1.Checked) {
                textBox1.Text = "";
                checkBox2.Checked = false;
                checkBox3.Checked = false;
               

                _PhamaPOSEntities = _dbConnector.getConn();
                List<item> _itemList = new List<item>();
                tblItems.Rows.Clear();
                grdItem.DataSource = tblItems;
                _itemList = _PhamaPOSEntities.items
                    .Where(a=>a.stocks.Count>0 && a.stocks.OrderByDescending(b=>b.stockExpDate).FirstOrDefault().stockExpDate < EntityFunctions.AddMonths(DateTime.Now,6) && a.itemStatus==true)
                    .OrderBy(a=>a.stocks.OrderBy(b=>b.stockId).FirstOrDefault().stockExpDate)
                    .ToList();
                foreach (item i in _itemList)
                {
                    DataRow dr = tblItems.NewRow();
                    dr["Item Code"] = i.itemCode.ToString().PadLeft(4, '0');
                    dr["Item Name"] = i.itemDescription;
                    dr["Item Description"] = i.itemCallingName;
                    dr["Item Unit"] = i.unit.unitCode;
                    dr["Item Status"] = i.itemStatus == true ? "Available" : "Not Available";
                    dr["Stock Available"] = i.stocks.Sum(a => a.runningQuantity);
                   // dr["Stock Value"] = i.stocks.Count != 0 ? ((int)(i.stocks.Average(a => a.itemUnitPriceSelling) * i.stocks.Sum(a => a.runningQuantity))) : 0.00;
                    dr["Stock Value"] = i.stocks.Count != 0 ? (
                   i.stocks.Select(a => new
                   {
                       value = a.itemUnitPriceSelling * a.runningQuantity
                   }).Sum(a => a.value)
                   ) : (decimal)0.00;
                    dr["Stock Cost"] = i.stocks.Count != 0 ? ((int)(i.stocks.Average(a => a.itemUnitPriceBuying) * i.stocks.Sum(a => a.runningQuantity))) : 0.00;
                    dr["Expiry Date"] = i.stocks.Count != 0 ? i.stocks.OrderByDescending(a => a.stockExpDate).FirstOrDefault().stockExpDate : DateTime.MinValue;
                    tblItems.Rows.Add(dr);
                }
                grdItem.Sort(this.grdItem.Columns["Expiry Date"], ListSortDirection.Ascending);
                grdItem.Refresh();

            }
            else
            {
                getAllItems();
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                textBox1.Text = "";
                checkBox1.Checked = false;
                checkBox3.Checked = false;
                
                _PhamaPOSEntities = _dbConnector.getConn();
                List<item> _itemList = new List<item>();
                tblItems.Rows.Clear();
                grdItem.DataSource = tblItems;
                _itemList = _PhamaPOSEntities.items
                    .Where(a => a.stocks.Count > 0  && a.itemStatus == true)
                    .OrderBy(a=>a.stocks.Sum(c=>c.runningQuantity))
                    .ToList();
                foreach (item i in _itemList)
                {
                    DataRow dr = tblItems.NewRow();
                    dr["Item Code"] = i.itemCode.ToString().PadLeft(4, '0');
                    dr["Item Name"] = i.itemDescription;
                    dr["Item Description"] = i.itemCallingName;
                    dr["Item Unit"] = i.unit.unitCode;
                    dr["Item Status"] = i.itemStatus == true ? "Available" : "Not Available";
                    dr["Stock Available"] = i.stocks.Sum(a => a.runningQuantity);
                   // dr["Stock Value"] = i.stocks.Count != 0 ? ((int)(i.stocks.Average(a => a.itemUnitPriceSelling) * i.stocks.Sum(a => a.runningQuantity))) : 0.00;
                    dr["Stock Value"] = i.stocks.Count != 0 ? (
                   i.stocks.Select(a => new
                   {
                       value = a.itemUnitPriceSelling * a.runningQuantity
                   }).Sum(a => a.value)
                   ) : (decimal)0.00;
                    dr["Stock Cost"] = i.stocks.Count != 0 ? ((int)(i.stocks.Average(a => a.itemUnitPriceBuying) * i.stocks.Sum(a => a.runningQuantity))) : 0.00;
                    dr["Expiry Date"] = i.stocks.Count != 0 ? i.stocks.OrderByDescending(a => a.stockExpDate).FirstOrDefault().stockExpDate:DateTime.MinValue;
                    tblItems.Rows.Add(dr);
                }
                grdItem.Sort(this.grdItem.Columns["Stock Available"], ListSortDirection.Ascending);
                grdItem.Refresh();
            }
            else
            {
                getAllItems();
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            
            if (checkBox3.Checked)
            {
                textBox1.Text = "";
                checkBox2.Checked = false;
                checkBox1.Checked = false;
              

                List<item> _itemList = new List<item>();
                tblItems.Rows.Clear();
                grdItem.DataSource = tblItems;
                _itemList = _PhamaPOSEntities.items
                    .Where(a => a.stocks.Count > 0 && a.itemStatus == false)
                    .ToList();
                foreach (item i in _itemList)
                {
                    DataRow dr = tblItems.NewRow();
                    dr["Item Code"] = i.itemCode.ToString().PadLeft(4, '0');
                    dr["Item Name"] = i.itemDescription;
                    dr["Item Description"] = i.itemCallingName;
                    dr["Item Unit"] = i.unit.unitCode;
                    dr["Item Status"] = i.itemStatus == true ? "Available" : "Not Available";
                    dr["Stock Available"] = i.stocks.Sum(a => a.runningQuantity);
                   // dr["Stock Value"] = i.stocks.Count != 0 ? ((int)(i.stocks.Average(a => a.itemUnitPriceSelling) * i.stocks.Sum(a => a.runningQuantity))) : 0.00;
                    dr["Stock Value"] = i.stocks.Count != 0 ? (
                   i.stocks.Select(a => new
                   {
                       value = a.itemUnitPriceSelling * a.runningQuantity
                   }).Sum(a => a.value)
                   ) : (decimal)0.00;
                    dr["Stock Cost"] = i.stocks.Count != 0 ? ((int)(i.stocks.Average(a => a.itemUnitPriceBuying) * i.stocks.Sum(a => a.runningQuantity))) : 0.00;
                    dr["Expiry Date"] = i.stocks.Count != 0 ? i.stocks.OrderByDescending(a => a.stockExpDate).FirstOrDefault().stockExpDate : DateTime.MinValue;

                    tblItems.Rows.Add(dr);
                }
                grdItem.Refresh();
            }
            else
            {
                getAllItems();

            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

      
    }
}
