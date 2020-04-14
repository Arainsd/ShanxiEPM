using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace hc.Plat.ServerManager
{
    public partial class RegistryCodeForm : Form
    {
        public RegistryCodeForm()
        {
            InitializeComponent();
        }

        public static string GetCode(string Code)
        {
            RegistryCodeForm form = new RegistryCodeForm();
            form.tbCode.Text = Code;
            if (form.ShowDialog() == DialogResult.OK)
            {
                return form.tbCode.Text;
            }
            else
            {
                return "";
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
