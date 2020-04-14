namespace DBConnectionDesTools
{
    partial class NETDB
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblConfig = new System.Windows.Forms.Label();
            this.txtConfigFile = new System.Windows.Forms.TextBox();
            this.btnCreateKeyContainer = new System.Windows.Forms.Button();
            this.btnEncrypt = new System.Windows.Forms.Button();
            this.txtToolsFile = new System.Windows.Forms.TextBox();
            this.lblTools = new System.Windows.Forms.Label();
            this.btnExportKeyContainer = new System.Windows.Forms.Button();
            this.btnDeleteKeyContainer = new System.Windows.Forms.Button();
            this.btnImportKeyContainer = new System.Windows.Forms.Button();
            this.txtKeyName = new System.Windows.Forms.TextBox();
            this.lblKeyName = new System.Windows.Forms.Label();
            this.gvOperate = new System.Windows.Forms.GroupBox();
            this.gvLog = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.btnDecrypt = new System.Windows.Forms.Button();
            this.gvOperate.SuspendLayout();
            this.gvLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblConfig
            // 
            this.lblConfig.AutoSize = true;
            this.lblConfig.Location = new System.Drawing.Point(18, 35);
            this.lblConfig.Name = "lblConfig";
            this.lblConfig.Size = new System.Drawing.Size(77, 12);
            this.lblConfig.TabIndex = 1;
            this.lblConfig.Text = "配置文件目录";
            // 
            // txtConfigFile
            // 
            this.txtConfigFile.Location = new System.Drawing.Point(20, 63);
            this.txtConfigFile.Name = "txtConfigFile";
            this.txtConfigFile.Size = new System.Drawing.Size(712, 21);
            this.txtConfigFile.TabIndex = 2;
            this.txtConfigFile.Text = "E:/ServicePush";
            // 
            // btnCreateKeyContainer
            // 
            this.btnCreateKeyContainer.Location = new System.Drawing.Point(21, 244);
            this.btnCreateKeyContainer.Name = "btnCreateKeyContainer";
            this.btnCreateKeyContainer.Size = new System.Drawing.Size(110, 23);
            this.btnCreateKeyContainer.TabIndex = 3;
            this.btnCreateKeyContainer.Text = "创建秘钥容器";
            this.btnCreateKeyContainer.UseVisualStyleBackColor = true;
            this.btnCreateKeyContainer.Click += new System.EventHandler(this.btnCreateKeyContainer_Click);
            // 
            // btnEncrypt
            // 
            this.btnEncrypt.Location = new System.Drawing.Point(137, 244);
            this.btnEncrypt.Name = "btnEncrypt";
            this.btnEncrypt.Size = new System.Drawing.Size(110, 23);
            this.btnEncrypt.TabIndex = 4;
            this.btnEncrypt.Text = "加密连接字符串";
            this.btnEncrypt.UseVisualStyleBackColor = true;
            this.btnEncrypt.Click += new System.EventHandler(this.btnEncrypt_Click);
            // 
            // txtToolsFile
            // 
            this.txtToolsFile.Location = new System.Drawing.Point(20, 127);
            this.txtToolsFile.Name = "txtToolsFile";
            this.txtToolsFile.Size = new System.Drawing.Size(712, 21);
            this.txtToolsFile.TabIndex = 6;
            this.txtToolsFile.Text = "%comspec% /k \"\"C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\Common7\\Tools\\" +
    "VsDevCmd.bat\"\"";
            // 
            // lblTools
            // 
            this.lblTools.AutoSize = true;
            this.lblTools.Location = new System.Drawing.Point(18, 99);
            this.lblTools.Name = "lblTools";
            this.lblTools.Size = new System.Drawing.Size(161, 12);
            this.lblTools.TabIndex = 5;
            this.lblTools.Text = "VS开发人员命令提示工具目录";
            // 
            // btnExportKeyContainer
            // 
            this.btnExportKeyContainer.Location = new System.Drawing.Point(376, 244);
            this.btnExportKeyContainer.Name = "btnExportKeyContainer";
            this.btnExportKeyContainer.Size = new System.Drawing.Size(110, 23);
            this.btnExportKeyContainer.TabIndex = 7;
            this.btnExportKeyContainer.Text = "导出秘钥容器";
            this.btnExportKeyContainer.UseVisualStyleBackColor = true;
            this.btnExportKeyContainer.Click += new System.EventHandler(this.btnExportKeyContainer_Click);
            // 
            // btnDeleteKeyContainer
            // 
            this.btnDeleteKeyContainer.Location = new System.Drawing.Point(492, 244);
            this.btnDeleteKeyContainer.Name = "btnDeleteKeyContainer";
            this.btnDeleteKeyContainer.Size = new System.Drawing.Size(110, 23);
            this.btnDeleteKeyContainer.TabIndex = 8;
            this.btnDeleteKeyContainer.Text = "删除秘钥容器";
            this.btnDeleteKeyContainer.UseVisualStyleBackColor = true;
            this.btnDeleteKeyContainer.Click += new System.EventHandler(this.btnDeleteKeyContainer_Click);
            // 
            // btnImportKeyContainer
            // 
            this.btnImportKeyContainer.Location = new System.Drawing.Point(608, 244);
            this.btnImportKeyContainer.Name = "btnImportKeyContainer";
            this.btnImportKeyContainer.Size = new System.Drawing.Size(110, 23);
            this.btnImportKeyContainer.TabIndex = 9;
            this.btnImportKeyContainer.Text = "导入秘钥容器";
            this.btnImportKeyContainer.UseVisualStyleBackColor = true;
            this.btnImportKeyContainer.Click += new System.EventHandler(this.btnImportKeyContainer_Click);
            // 
            // txtKeyName
            // 
            this.txtKeyName.Location = new System.Drawing.Point(20, 193);
            this.txtKeyName.Name = "txtKeyName";
            this.txtKeyName.Size = new System.Drawing.Size(712, 21);
            this.txtKeyName.TabIndex = 11;
            this.txtKeyName.Text = "ConnStrKeyContainer";
            // 
            // lblKeyName
            // 
            this.lblKeyName.AutoSize = true;
            this.lblKeyName.Location = new System.Drawing.Point(18, 165);
            this.lblKeyName.Name = "lblKeyName";
            this.lblKeyName.Size = new System.Drawing.Size(77, 12);
            this.lblKeyName.TabIndex = 10;
            this.lblKeyName.Text = "秘钥容器名称";
            // 
            // gvOperate
            // 
            this.gvOperate.Controls.Add(this.btnDecrypt);
            this.gvOperate.Controls.Add(this.txtKeyName);
            this.gvOperate.Controls.Add(this.lblConfig);
            this.gvOperate.Controls.Add(this.lblKeyName);
            this.gvOperate.Controls.Add(this.txtConfigFile);
            this.gvOperate.Controls.Add(this.btnImportKeyContainer);
            this.gvOperate.Controls.Add(this.btnCreateKeyContainer);
            this.gvOperate.Controls.Add(this.btnDeleteKeyContainer);
            this.gvOperate.Controls.Add(this.btnEncrypt);
            this.gvOperate.Controls.Add(this.btnExportKeyContainer);
            this.gvOperate.Controls.Add(this.lblTools);
            this.gvOperate.Controls.Add(this.txtToolsFile);
            this.gvOperate.Location = new System.Drawing.Point(38, 12);
            this.gvOperate.Name = "gvOperate";
            this.gvOperate.Size = new System.Drawing.Size(738, 304);
            this.gvOperate.TabIndex = 12;
            this.gvOperate.TabStop = false;
            this.gvOperate.Text = "操作区";
            // 
            // gvLog
            // 
            this.gvLog.Controls.Add(this.txtLog);
            this.gvLog.Location = new System.Drawing.Point(38, 343);
            this.gvLog.Name = "gvLog";
            this.gvLog.Size = new System.Drawing.Size(738, 257);
            this.gvLog.TabIndex = 13;
            this.gvLog.TabStop = false;
            this.gvLog.Text = "日志";
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(20, 20);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(712, 231);
            this.txtLog.TabIndex = 0;
            // 
            // btnDecrypt
            // 
            this.btnDecrypt.Location = new System.Drawing.Point(253, 244);
            this.btnDecrypt.Name = "btnDecrypt";
            this.btnDecrypt.Size = new System.Drawing.Size(110, 23);
            this.btnDecrypt.TabIndex = 12;
            this.btnDecrypt.Text = "解密字符串";
            this.btnDecrypt.UseVisualStyleBackColor = true;
            this.btnDecrypt.Click += new System.EventHandler(this.btnDecrypt_Click);
            // 
            // NETDB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(854, 633);
            this.Controls.Add(this.gvLog);
            this.Controls.Add(this.gvOperate);
            this.MaximizeBox = false;
            this.Name = "NETDB";
            this.Text = "NETDB";
            this.Load += new System.EventHandler(this.NETDB_Load);
            this.gvOperate.ResumeLayout(false);
            this.gvOperate.PerformLayout();
            this.gvLog.ResumeLayout(false);
            this.gvLog.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblConfig;
        private System.Windows.Forms.TextBox txtConfigFile;
        private System.Windows.Forms.Button btnCreateKeyContainer;
        private System.Windows.Forms.Button btnEncrypt;
        private System.Windows.Forms.TextBox txtToolsFile;
        private System.Windows.Forms.Label lblTools;
        private System.Windows.Forms.Button btnExportKeyContainer;
        private System.Windows.Forms.Button btnDeleteKeyContainer;
        private System.Windows.Forms.Button btnImportKeyContainer;
        private System.Windows.Forms.TextBox txtKeyName;
        private System.Windows.Forms.Label lblKeyName;
        private System.Windows.Forms.GroupBox gvOperate;
        private System.Windows.Forms.GroupBox gvLog;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnDecrypt;
    }
}