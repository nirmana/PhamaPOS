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
    public partial class login : Form
    {
        dbConnector _dbConnector;
        PhamaPOSEntities _phamaPOSEntities;
        public login()
        {
            InitializeComponent();
            
            _dbConnector = new dbConnector();
            _phamaPOSEntities = _dbConnector.getConn();

        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string userName = textBox1.Text;
            string password = textBox2.Text;
            if (userName.Length > 0 && password.Length > 0)
            {
                var user = _phamaPOSEntities.users
                    .Where(a => a.userName.ToLower() == userName.ToLower() && a.password == password);

                if (user.Count() > 0)
                {

                    home form = new home(user.FirstOrDefault());
                    form.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("User Name or Password you enterd is invalid.. :(");
                }
            }
            else
            {
                MessageBox.Show("Please Enter both User Name and Password.. :(");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        
    }
}
