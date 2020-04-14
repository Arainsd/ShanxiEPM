namespace hc.Plat.ServerManager
{
    partial class ManagerForm
    {
        /// <summary>
        /// 必需的设计器变量。

        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。

        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。

        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManagerForm));
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.paneltop = new System.Windows.Forms.Panel();
            this.labServiceName = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnImportKey = new System.Windows.Forms.Button();
            this.btnDBTest = new System.Windows.Forms.Button();
            this.cbAutoStart = new System.Windows.Forms.CheckBox();
            this.btnInstall = new System.Windows.Forms.Button();
            this.MainimageList = new System.Windows.Forms.ImageList(this.components);
            this.btnUnInstall = new System.Windows.Forms.Button();
            this.MainnotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.cmsMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmStart = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmStop = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmShow = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmHide = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmClose = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tbMsg = new System.Windows.Forms.ToolStripStatusLabel();
            this.StateProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.paneltop.SuspendLayout();
            this.panel1.SuspendLayout();
            this.cmsMain.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStop.Location = new System.Drawing.Point(355, 81);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(192, 62);
            this.btnStop.TabIndex = 6;
            this.btnStop.Text = "停止服务";
            this.btnStop.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Enabled = false;
            this.btnStart.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.Image = ((System.Drawing.Image)(resources.GetObject("btnStart.Image")));
            this.btnStart.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnStart.Location = new System.Drawing.Point(355, 6);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(192, 69);
            this.btnStart.TabIndex = 5;
            this.btnStart.Text = "启动服务";
            this.btnStart.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(83, 88);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 2000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // paneltop
            // 
            this.paneltop.Controls.Add(this.labServiceName);
            this.paneltop.Controls.Add(this.pictureBox1);
            this.paneltop.Dock = System.Windows.Forms.DockStyle.Top;
            this.paneltop.Location = new System.Drawing.Point(0, 0);
            this.paneltop.Name = "paneltop";
            this.paneltop.Size = new System.Drawing.Size(553, 88);
            this.paneltop.TabIndex = 5;
            // 
            // labServiceName
            // 
            this.labServiceName.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.labServiceName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labServiceName.Font = new System.Drawing.Font("黑体", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labServiceName.Location = new System.Drawing.Point(83, 0);
            this.labServiceName.Name = "labServiceName";
            this.labServiceName.Size = new System.Drawing.Size(470, 88);
            this.labServiceName.TabIndex = 5;
            this.labServiceName.Text = "应用服务管理器";
            this.labServiceName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnImportKey);
            this.panel1.Controls.Add(this.btnDBTest);
            this.panel1.Controls.Add(this.cbAutoStart);
            this.panel1.Controls.Add(this.btnInstall);
            this.panel1.Controls.Add(this.btnUnInstall);
            this.panel1.Controls.Add(this.btnStart);
            this.panel1.Controls.Add(this.btnStop);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 88);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(553, 206);
            this.panel1.TabIndex = 7;
            // 
            // btnImportKey
            // 
            this.btnImportKey.Enabled = false;
            this.btnImportKey.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnImportKey.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImportKey.Image = ((System.Drawing.Image)(resources.GetObject("btnImportKey.Image")));
            this.btnImportKey.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnImportKey.Location = new System.Drawing.Point(206, 6);
            this.btnImportKey.Name = "btnImportKey";
            this.btnImportKey.Size = new System.Drawing.Size(143, 137);
            this.btnImportKey.TabIndex = 3;
            this.btnImportKey.Text = "导入注册文件";
            this.btnImportKey.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btnImportKey.UseVisualStyleBackColor = false;
            this.btnImportKey.Click += new System.EventHandler(this.btnImportKey_Click);
            // 
            // btnDBTest
            // 
            this.btnDBTest.Enabled = false;
            this.btnDBTest.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnDBTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDBTest.Image = ((System.Drawing.Image)(resources.GetObject("btnDBTest.Image")));
            this.btnDBTest.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDBTest.Location = new System.Drawing.Point(6, 47);
            this.btnDBTest.Name = "btnDBTest";
            this.btnDBTest.Size = new System.Drawing.Size(194, 96);
            this.btnDBTest.TabIndex = 4;
            this.btnDBTest.Text = "测试数据库";
            this.btnDBTest.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btnDBTest.UseVisualStyleBackColor = false;
            // 
            // cbAutoStart
            // 
            this.cbAutoStart.AutoSize = true;
            this.cbAutoStart.Checked = true;
            this.cbAutoStart.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAutoStart.Enabled = false;
            this.cbAutoStart.Location = new System.Drawing.Point(6, 155);
            this.cbAutoStart.Name = "cbAutoStart";
            this.cbAutoStart.Size = new System.Drawing.Size(216, 16);
            this.cbAutoStart.TabIndex = 0;
            this.cbAutoStart.Text = "每次计算机启动后，自动启动此服务";
            this.cbAutoStart.UseVisualStyleBackColor = true;
            this.cbAutoStart.CheckedChanged += new System.EventHandler(this.cbAutoStart_CheckedChanged);
            // 
            // btnInstall
            // 
            this.btnInstall.Enabled = false;
            this.btnInstall.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnInstall.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInstall.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnInstall.ImageIndex = 0;
            this.btnInstall.ImageList = this.MainimageList;
            this.btnInstall.Location = new System.Drawing.Point(6, 6);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(96, 35);
            this.btnInstall.TabIndex = 1;
            this.btnInstall.Text = "安装服务";
            this.btnInstall.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnInstall.UseVisualStyleBackColor = false;
            this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
            // 
            // MainimageList
            // 
            this.MainimageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("MainimageList.ImageStream")));
            this.MainimageList.TransparentColor = System.Drawing.Color.Transparent;
            this.MainimageList.Images.SetKeyName(0, "install.png");
            this.MainimageList.Images.SetKeyName(1, "uninstall.png");
            // 
            // btnUnInstall
            // 
            this.btnUnInstall.Enabled = false;
            this.btnUnInstall.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnUnInstall.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUnInstall.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUnInstall.ImageIndex = 1;
            this.btnUnInstall.ImageList = this.MainimageList;
            this.btnUnInstall.Location = new System.Drawing.Point(105, 6);
            this.btnUnInstall.Name = "btnUnInstall";
            this.btnUnInstall.Size = new System.Drawing.Size(95, 35);
            this.btnUnInstall.TabIndex = 2;
            this.btnUnInstall.Text = "卸载服务";
            this.btnUnInstall.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnUnInstall.UseVisualStyleBackColor = false;
            this.btnUnInstall.Click += new System.EventHandler(this.btnUninstall_Click);
            // 
            // MainnotifyIcon
            // 
            this.MainnotifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.MainnotifyIcon.BalloonTipTitle = "应用服务管理器";
            this.MainnotifyIcon.ContextMenuStrip = this.cmsMain;
            this.MainnotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("MainnotifyIcon.Icon")));
            this.MainnotifyIcon.Text = "...";
            this.MainnotifyIcon.Visible = true;
            this.MainnotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.MainnotifyIcon_MouseDoubleClick);
            // 
            // cmsMain
            // 
            this.cmsMain.Enabled = false;
            this.cmsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmStart,
            this.tsmStop,
            this.toolStripMenuItem1,
            this.tsmShow,
            this.tsmHide,
            this.toolStripMenuItem2,
            this.tsmClose});
            this.cmsMain.Name = "cmsMain";
            this.cmsMain.Size = new System.Drawing.Size(149, 126);
            // 
            // tsmStart
            // 
            this.tsmStart.Enabled = false;
            this.tsmStart.Name = "tsmStart";
            this.tsmStart.Size = new System.Drawing.Size(148, 22);
            this.tsmStart.Text = "启动服务";
            this.tsmStart.Click += new System.EventHandler(this.tsmStart_Click);
            // 
            // tsmStop
            // 
            this.tsmStop.Enabled = false;
            this.tsmStop.Name = "tsmStop";
            this.tsmStop.Size = new System.Drawing.Size(148, 22);
            this.tsmStop.Text = "停止服务";
            this.tsmStop.Click += new System.EventHandler(this.tsmStop_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(145, 6);
            // 
            // tsmShow
            // 
            this.tsmShow.Enabled = false;
            this.tsmShow.Name = "tsmShow";
            this.tsmShow.Size = new System.Drawing.Size(148, 22);
            this.tsmShow.Text = "显示管理界面";
            this.tsmShow.Click += new System.EventHandler(this.tsmShow_Click);
            // 
            // tsmHide
            // 
            this.tsmHide.Enabled = false;
            this.tsmHide.Name = "tsmHide";
            this.tsmHide.Size = new System.Drawing.Size(148, 22);
            this.tsmHide.Text = "隐藏管理界面";
            this.tsmHide.Click += new System.EventHandler(this.tsmHide_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(145, 6);
            // 
            // tsmClose
            // 
            this.tsmClose.Enabled = false;
            this.tsmClose.Name = "tsmClose";
            this.tsmClose.Size = new System.Drawing.Size(148, 22);
            this.tsmClose.Text = "退出管理器";
            this.tsmClose.Click += new System.EventHandler(this.tsmClose_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbMsg,
            this.StateProgressBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 272);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.ShowItemToolTips = true;
            this.statusStrip1.Size = new System.Drawing.Size(553, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 8;
            // 
            // tbMsg
            // 
            this.tbMsg.AutoSize = false;
            this.tbMsg.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tbMsg.Name = "tbMsg";
            this.tbMsg.Size = new System.Drawing.Size(300, 17);
            this.tbMsg.Text = "正在刷新服务状态，请稍后......";
            this.tbMsg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // StateProgressBar
            // 
            this.StateProgressBar.Name = "StateProgressBar";
            this.StateProgressBar.Size = new System.Drawing.Size(245, 16);
            // 
            // ManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(553, 294);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.paneltop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ManagerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "应用服务管理器";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ManagerForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.paneltop.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.cmsMain.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Panel paneltop;
        private System.Windows.Forms.Label labServiceName;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnInstall;
        private System.Windows.Forms.Button btnUnInstall;
        private System.Windows.Forms.ImageList MainimageList;
        private System.Windows.Forms.NotifyIcon MainnotifyIcon;
        private System.Windows.Forms.ContextMenuStrip cmsMain;
        private System.Windows.Forms.ToolStripMenuItem tsmStart;
        private System.Windows.Forms.ToolStripMenuItem tsmStop;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem tsmClose;
        private System.Windows.Forms.ToolStripMenuItem tsmShow;
        private System.Windows.Forms.ToolStripMenuItem tsmHide;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.CheckBox cbAutoStart;
        private System.Windows.Forms.Button btnDBTest;
        private System.Windows.Forms.Button btnImportKey;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tbMsg;
        private System.Windows.Forms.ToolStripProgressBar StateProgressBar;
    }
}

