using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace eta_4all
{
    public partial class config_form : Form
    {
        public config_form()
        {
            InitializeComponent();
        }

        private void label4_Click(object sender, EventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {

            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);

            config.AppSettings.Settings["client_id"].Value =txt_client_id.Text;
            config.AppSettings.Settings["client_secrit"].Value = txt_client_secrit.Text;
            config.AppSettings.Settings["token_type"].Value = cmb_tokin_type.Text;
            config.AppSettings.Settings["token_pin"].Value = txt_token_pin.Text;
            config.AppSettings.Settings["folder_path"].Value = lb_folder.Text;

            config.Save(System.Configuration.ConfigurationSaveMode.Modified);
            System.Configuration.ConfigurationManager.RefreshSection("appSettings");

            Application.Restart();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            lb_folder.Text = folderBrowserDialog1.SelectedPath;
        }
    }
}
