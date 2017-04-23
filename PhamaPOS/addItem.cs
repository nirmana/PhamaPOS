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
    public partial class addItem : Form
    {
        public dbConnector _dbConnector = new dbConnector();
        public PhamaPOSEntities _PhamaPOSEntities;
        private user _user;
        public addItem()
        {
            InitializeComponent();
            LoadAvailability();
            LoadUnits();
        }

        public addItem(user user)
        {
            // TODO: Complete member initialization
            this._user = user;
            InitializeComponent();
            LoadAvailability();
            LoadUnits();
        }

        private void LoadAvailability()
        {
            drpAvailability.Items.Add("Available");
            drpAvailability.Items.Add("Not Avaliable");
            drpAvailability.SelectedItem = "Available";

        }

        private void LoadUnits()
        {
            _PhamaPOSEntities = _dbConnector.getConn();
            drpUnitOfMesure.DataSource = _PhamaPOSEntities.units
                .Select(a => new
                {
                    a.unitId,
                    a.unitCode

                }).ToList();
            drpUnitOfMesure.ValueMember = "unitId";
            drpUnitOfMesure.DisplayMember = "unitCode";


        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (radioAddNew.Checked)
                {
                    int itemCode = Convert.ToInt32(txtItemCode.Text);
                    var itemDesc = txtItemName.Text;
                    var itemCallingname = txtItemCallingName.Text;
                    var unitId = (int)drpUnitOfMesure.SelectedValue;
                    var itemStatus = drpAvailability.SelectedItem;
                    if (itemCode > 0)
                    {
                        item i = new item();
                        i.entryBy = _user.userName;
                        i.entryDate = DateTime.Now;
                        i.itemCallingName = itemCallingname;
                        i.itemCode = itemCode;
                        i.itemDescription = itemDesc;
                        i.itemStatus = itemStatus == "Available" ? true : false;
                        i.itemUnitId = unitId;

                        _PhamaPOSEntities.items.Add(i);
                        _PhamaPOSEntities.SaveChanges();

                        DialogResult result1 = MessageBox.Show("Item Added Sucessfully. Do you want to add stock also for saved item ?",
                       "Important Question",
                       MessageBoxButtons.YesNo);
                        if (result1 == DialogResult.Yes)
                        {
                            this.Dispose();
                            addItemStock form = new addItemStock(itemCode, _user);
                            form.Show();

                        }
                        else
                        {
                            clearForm();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid Item Code..!");
                    }

                }
                else
                {

                    int itemCode = Convert.ToInt32(txtItemCode.Text);
                    var itemDesc = txtItemName.Text;
                    var itemCallingname = txtItemCallingName.Text;
                    var unitId = (int)drpUnitOfMesure.SelectedValue;
                    var itemStatus = drpAvailability.SelectedItem;
                    if (itemCode > 0)
                    {
                        _PhamaPOSEntities = _dbConnector.getConn();
                        var selectedItem = _PhamaPOSEntities.items.Where(a => a.itemCode == itemCode);
                        if (selectedItem.Count() > 0)
                        {
                            item i = selectedItem.FirstOrDefault();


                            i.itemCallingName = itemCallingname;
                            i.itemDescription = itemDesc;
                            i.itemStatus = itemStatus == "Available" ? true : false;
                            i.itemUnitId = unitId;

                            _PhamaPOSEntities.items.Attach(i);
                            var entry = _PhamaPOSEntities.Entry(i);

                            entry.Property(a => a.itemCallingName).IsModified = true;
                            entry.Property(a => a.itemDescription).IsModified = true;
                            entry.Property(a => a.itemStatus).IsModified = true;
                            entry.Property(a => a.itemUnitId).IsModified = true;

                            _PhamaPOSEntities.SaveChanges();

                            MessageBox.Show("Successfully Updated..");
                            clearForm();
                        }
                        else
                        {
                            MessageBox.Show("Invalid Item Code..");

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Occured.. Please Retry.. "+ex.Message);
            }
           
        }

        private void txtItemCode_Leave(object sender, EventArgs e)
        {
            int itemCode = 0;
            bool result = int.TryParse(txtItemCode.Text, out itemCode);
            if (itemCode == 0)
            {
                MessageBox.Show("Invalid Item Code...!");
                txtItemCode.Text = "";
                //txtItemCode.Focus();
            }
            else
            {
                if (result)
                {
                    if (radioAddNew.Checked)
                    {
                        _PhamaPOSEntities = _dbConnector.getConn();
                        int count = _PhamaPOSEntities.items.Where(a => a.itemCode == itemCode).Count();
                        if (count > 0)
                        {
                            MessageBox.Show("Item Code Already Exist In The Database...!");
                            txtItemCode.Text = "";
                            //txtItemCode.Focus();
                        }
                        else
                        {
                            txtItemCode.Text = itemCode.ToString().PadLeft(4, '0');
                        }
                        

                    }
                    else
                    {
                        _PhamaPOSEntities = _dbConnector.getConn();
                        int count = _PhamaPOSEntities.items.Where(a => a.itemCode == itemCode).Count();
                        if (count == 0)
                        {
                            MessageBox.Show("Item Code Already Not Exist In The Database...!");
                            txtItemCode.Text = "";
                            //txtItemCode.Focus();
                        }
                        else
                        {
                            txtItemCode.Text = itemCode.ToString().PadLeft(4, '0');
                            var item = _PhamaPOSEntities.items.Where(a => a.itemCode == itemCode).FirstOrDefault();
                            txtItemName.Text = item.itemDescription;
                            txtItemCallingName.Text = item.itemCallingName;
                            drpUnitOfMesure.SelectedValue = item.itemUnitId;
                            drpAvailability.SelectedItem = item.itemStatus == true ? "Available" : "Not Avaliable";

                        }
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Item Code...!");
                    txtItemCode.Text = "";
                    //txtItemCode.Focus();
                }
            }
        }

        private void radioAddNew_CheckedChanged(object sender, EventArgs e)
        {
            if (radioUpdate.Checked)
            {
                radioUpdate.Checked = false;
                radioAddNew.Checked = true;
            }
        }

        private void radioUpdate_CheckedChanged(object sender, EventArgs e)
        {
            if (radioAddNew.Checked)
            {
                radioAddNew.Checked = false;
                radioUpdate.Checked = true;

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtItemCode.Text = "";
            txtItemName.Text = "";
            txtItemCallingName.Text="";
        }
        private void clearForm() {
            txtItemCode.Text = "";
            txtItemName.Text = "";
            txtItemCallingName.Text = "";
        }

        private void txtItemCode_TextChanged(object sender, EventArgs e)
        {

        }

        //private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        //{

        //}

        //private void dateTimePicker_OnTextChange(object sender, EventArgs e)
        //{
        //    dataGridView1.CurrentCell.Value = oDateTimePicker.Text.ToString();  
        //}

        //private void oDateTimePicker_CloseUp(object sender, EventArgs e)
        //{
        //    oDateTimePicker.Visible = false;
        //}

        //private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (e.ColumnIndex == 5 && e.RowIndex!=-1)
        //    {
        //        //Initialized a new DateTimePicker Control  
        //        oDateTimePicker = new DateTimePicker();

        //        //Adding DateTimePicker control into DataGridView   
        //        dataGridView1.Controls.Add(oDateTimePicker);

        //        // Setting the format (i.e. 2014-10-10)  
        //        oDateTimePicker.Format = DateTimePickerFormat.Short;

        //        // It returns the retangular area that represents the Display area for a cell  
        //        Rectangle oRectangle = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);

        //        //Setting area for DateTimePicker Control  
        //        oDateTimePicker.Size = new Size(oRectangle.Width, oRectangle.Height);

        //        // Setting Location  
        //        oDateTimePicker.Location = new Point(oRectangle.X, oRectangle.Y);

        //        // An event attached to dateTimePicker Control which is fired when DateTimeControl is closed  
        //        oDateTimePicker.CloseUp += new EventHandler(oDateTimePicker_CloseUp);

        //        // An event attached to dateTimePicker Control which is fired when any date is selected  
        //        oDateTimePicker.TextChanged += new EventHandler(dateTimePicker_OnTextChange);

        //        // Now make it visible  
        //        oDateTimePicker.Visible = true;
        //    }  
        //}

        //private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        //{
        // if (e.ColumnIndex == 5 && e.RowIndex!=-1)
        //    {
        //        //Initialized a new DateTimePicker Control  
        //        oDateTimePicker = new DateTimePicker();

        //        //Adding DateTimePicker control into DataGridView   
        //        dataGridView1.Controls.Add(oDateTimePicker);

        //        // Setting the format (i.e. 2014-10-10)  
        //        oDateTimePicker.Format = DateTimePickerFormat.Short;

        //        // It returns the retangular area that represents the Display area for a cell  
        //        Rectangle oRectangle = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);

        //        //Setting area for DateTimePicker Control  
        //        oDateTimePicker.Size = new Size(oRectangle.Width, oRectangle.Height);

        //        // Setting Location  
        //        oDateTimePicker.Location = new Point(oRectangle.X, oRectangle.Y);

        //        // An event attached to dateTimePicker Control which is fired when DateTimeControl is closed  
        //        oDateTimePicker.CloseUp += new EventHandler(oDateTimePicker_CloseUp);

        //        // An event attached to dateTimePicker Control which is fired when any date is selected  
        //        oDateTimePicker.TextChanged += new EventHandler(dateTimePicker_OnTextChange);

        //        // Now make it visible  
        //        oDateTimePicker.Visible = true;
        //        oDateTimePicker.AllowDrop = true;

        //    }  
        //}

        //private void dataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
        //{
        //    //if (e.RowIndex != -1)
        //    //{
        //    //    if (e.ColumnIndex == 2) 
        //    //    {

        //    //        DataGridViewCell MyCell = dataGridView1[e.ColumnIndex, e.RowIndex];

        //    //        if (MyCell != null)
        //    //        {
        //    //            if (MyCell.Value != null)
        //    //            {
        //    //                int Quantity;
        //    //                var res = int.TryParse(MyCell.Value.ToString(),out Quantity);
        //    //                if (res)
        //    //                {

        //    //                }else
        //    //                {

        //    //                }
        //    //            }
        //    //            else
        //    //            {
        //    //                MessageBox.Show("Invalid Quantity..");
        //    //            }
        //    //        }
        //    //        else
        //    //        {
        //    //            MessageBox.Show("Invalid Quantity..");
        //    //        }
        //    //        //var Quantity = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();  
        //    //        //int quantity;
        //    //        //var result = int.TryParse(Quantity.ToString(), out quantity);
        //    //        //if (!result) {
        //    //        //    dataGridView1.Rows[e.RowIndex].Cells["col_stockQuantity"].Value = null;
        //    //        //    MessageBox.Show("Invalid Quantity..");
        //    //        //}
        //    //    }
        //    //}
        //}

        //private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        //{
        //    if (e.RowIndex != -1)
        //    {
        //        for (int i = 2; i < 7; i++)
        //        {
        //            DataGridViewCell MyCell = dataGridView1[i, e.RowIndex];

        //        }

        //    }
        //}
    }
}
