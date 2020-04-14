namespace CreateModelTools
{
    partial class AppendColumn
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
            this.btnArea = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnInit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnArea
            // 
            this.btnArea.Location = new System.Drawing.Point(36, 32);
            this.btnArea.Name = "btnArea";
            this.btnArea.Size = new System.Drawing.Size(123, 23);
            this.btnArea.TabIndex = 2;
            this.btnArea.Text = "初始化行政区域";
            this.btnArea.UseVisualStyleBackColor = true;
            this.btnArea.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(205, 32);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(258, 23);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Text = "清理数据库(即清除IsDelete=true的数据)";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnInit
            // 
            this.btnInit.Location = new System.Drawing.Point(36, 94);
            this.btnInit.Name = "btnInit";
            this.btnInit.Size = new System.Drawing.Size(232, 23);
            this.btnInit.TabIndex = 4;
            this.btnInit.Text = "初始化系统(即只保留系统初始数据)";
            this.btnInit.UseVisualStyleBackColor = true;
            this.btnInit.Click += new System.EventHandler(this.btnInit_Click);
            // 
            // AppendColumn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(559, 327);
            this.Controls.Add(this.btnInit);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnArea);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AppendColumn";
            this.Text = "电招平台数据库初始化工具";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnArea;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnInit;
    }
}