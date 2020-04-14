using System; 
using System.Windows.Forms;

namespace hc.Plat.ServerManager
{
    public partial class ServerUserForm : Form
    {
        public ServerUserForm()
        {
            InitializeComponent();
        }

        public string UserName
        {
            get
            {
                return NameTextBox.Text.Trim();
            }
        }
        public string UserPassword
        {
            get { return PasswordTextBox.Text.Trim(); }
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}