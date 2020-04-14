using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBConnectionDesTools
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            string source = txtSource.Text;
            string encrypt = "";
            //if (string.IsNullOrEmpty(source))
            //{
            //    MessageBox.Show("待加密连接字符串不能为空");
            //}
            //else
            //{
            //    DesTool._DESKey = DesKey;
            //    encrypt = DesTool.DesEncrypt(source);
            //}
            txtEncrypt.Text = encrypt;
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            //string source = "";
            //string encrypt = txtEncrypt.Text;
            //if (string.IsNullOrEmpty(encrypt))
            //{
            //    MessageBox.Show("待解密连接字符串不能为空");
            //}
            //else
            //{
            //    DesTool._DESKey = DesKey;
            //    source = DesTool.DesDecrypt(source);
            //}
            //txtSource.Text = source;
        }
        /// <summary>
        /// 加解密key
        /// </summary>
        public string DesKey
        {
            get
            {
                string key = txtKey.Text.Trim();
                //if (string.IsNullOrEmpty(key))
                //{
                //    return DesTool._DESKey;
                //}
                return key;
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            txtKey.Text = DesKey;
        }
    }
}
