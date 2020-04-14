using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreateModelTools
{
    public partial class ShowText : Form
    {

        public ShowText()
        {
            InitializeComponent();
            //设定按字体来缩放控件
            this.AutoScaleMode = AutoScaleMode.Font;
            //设定字体大小为12px
            this.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Pixel, 134);

        }
        public void SetTextContent(string content)
        {
            tbContent.Text = content;
        }
    }
}
