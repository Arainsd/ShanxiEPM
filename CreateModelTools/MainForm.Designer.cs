namespace CreateModelTools
{
    partial class MainForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.gbDataBase = new System.Windows.Forms.GroupBox();
            this.btnCreateInterface = new System.Windows.Forms.Button();
            this.tbDataOperate = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCreateService = new System.Windows.Forms.Button();
            this.btnCreateServerType = new System.Windows.Forms.Button();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnCreateDBSet = new System.Windows.Forms.Button();
            this.btnCreate = new System.Windows.Forms.Button();
            this.tbConnectionString = new System.Windows.Forms.TextBox();
            this.tbParentClass = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.lblParentClass = new System.Windows.Forms.Label();
            this.gvTables = new System.Windows.Forms.DataGridView();
            this.gvColumnMap = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.gvColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tbNamespace = new System.Windows.Forms.TextBox();
            this.tbSavePath = new System.Windows.Forms.TextBox();
            this.lblSavePath = new System.Windows.Forms.Label();
            this.lblNamespace = new System.Windows.Forms.Label();
            this.gbDataBase.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvTables)).BeginInit();
            this.SuspendLayout();
            // 
            // gbDataBase
            // 
            this.gbDataBase.Controls.Add(this.btnCreateInterface);
            this.gbDataBase.Controls.Add(this.tbDataOperate);
            this.gbDataBase.Controls.Add(this.label1);
            this.gbDataBase.Controls.Add(this.btnCreateService);
            this.gbDataBase.Controls.Add(this.btnCreateServerType);
            this.gbDataBase.Controls.Add(this.btnSelect);
            this.gbDataBase.Controls.Add(this.btnCreateDBSet);
            this.gbDataBase.Controls.Add(this.btnCreate);
            this.gbDataBase.Controls.Add(this.tbConnectionString);
            this.gbDataBase.Controls.Add(this.tbParentClass);
            this.gbDataBase.Controls.Add(this.btnConnect);
            this.gbDataBase.Controls.Add(this.lblParentClass);
            this.gbDataBase.Controls.Add(this.gvTables);
            this.gbDataBase.Controls.Add(this.tbNamespace);
            this.gbDataBase.Controls.Add(this.tbSavePath);
            this.gbDataBase.Controls.Add(this.lblSavePath);
            this.gbDataBase.Controls.Add(this.lblNamespace);
            this.gbDataBase.Location = new System.Drawing.Point(12, 12);
            this.gbDataBase.Name = "gbDataBase";
            this.gbDataBase.Size = new System.Drawing.Size(664, 507);
            this.gbDataBase.TabIndex = 0;
            this.gbDataBase.TabStop = false;
            this.gbDataBase.Text = "DataBase";
            // 
            // btnCreateInterface
            // 
            this.btnCreateInterface.Location = new System.Drawing.Point(394, 465);
            this.btnCreateInterface.Name = "btnCreateInterface";
            this.btnCreateInterface.Size = new System.Drawing.Size(112, 21);
            this.btnCreateInterface.TabIndex = 17;
            this.btnCreateInterface.Text = "CreateInterface";
            this.btnCreateInterface.UseVisualStyleBackColor = true;
            this.btnCreateInterface.Click += new System.EventHandler(this.btnCreateInterface_Click);
            // 
            // tbDataOperate
            // 
            this.tbDataOperate.Location = new System.Drawing.Point(398, 438);
            this.tbDataOperate.Name = "tbDataOperate";
            this.tbDataOperate.Size = new System.Drawing.Size(227, 21);
            this.tbDataOperate.TabIndex = 16;
            this.tbDataOperate.Text = "DataOperateCore";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(398, 414);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 15;
            this.label1.Text = "DataOperate";
            // 
            // btnCreateService
            // 
            this.btnCreateService.Location = new System.Drawing.Point(515, 465);
            this.btnCreateService.Name = "btnCreateService";
            this.btnCreateService.Size = new System.Drawing.Size(112, 21);
            this.btnCreateService.TabIndex = 14;
            this.btnCreateService.Text = "CreateService";
            this.btnCreateService.UseVisualStyleBackColor = true;
            this.btnCreateService.Click += new System.EventHandler(this.btnCreateService_Click);
            // 
            // btnCreateServerType
            // 
            this.btnCreateServerType.Location = new System.Drawing.Point(526, 323);
            this.btnCreateServerType.Name = "btnCreateServerType";
            this.btnCreateServerType.Size = new System.Drawing.Size(112, 21);
            this.btnCreateServerType.TabIndex = 13;
            this.btnCreateServerType.Text = "CreateServerType";
            this.btnCreateServerType.UseVisualStyleBackColor = true;
            this.btnCreateServerType.Click += new System.EventHandler(this.btnCreateServerType_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(6, 43);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 12;
            this.btnSelect.Text = "None";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnCreateDBSet
            // 
            this.btnCreateDBSet.Location = new System.Drawing.Point(400, 374);
            this.btnCreateDBSet.Name = "btnCreateDBSet";
            this.btnCreateDBSet.Size = new System.Drawing.Size(81, 21);
            this.btnCreateDBSet.TabIndex = 11;
            this.btnCreateDBSet.Text = "CreateDBSet";
            this.btnCreateDBSet.UseVisualStyleBackColor = true;
            this.btnCreateDBSet.Click += new System.EventHandler(this.btnCreateDBSet_Click);
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(400, 323);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(72, 21);
            this.btnCreate.TabIndex = 10;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // tbConnectionString
            // 
            this.tbConnectionString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbConnectionString.Location = new System.Drawing.Point(6, 20);
            this.tbConnectionString.Name = "tbConnectionString";
            this.tbConnectionString.Size = new System.Drawing.Size(591, 21);
            this.tbConnectionString.TabIndex = 5;
            this.tbConnectionString.Text = "server=192.168.1.239;uid=sa;pwd=123456;database=EPM_Business";
            // 
            // tbParentClass
            // 
            this.tbParentClass.Location = new System.Drawing.Point(400, 155);
            this.tbParentClass.Name = "tbParentClass";
            this.tbParentClass.Size = new System.Drawing.Size(227, 21);
            this.tbParentClass.TabIndex = 9;
            this.tbParentClass.Text = "BaseModel";
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.Location = new System.Drawing.Point(603, 20);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(55, 21);
            this.btnConnect.TabIndex = 6;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // lblParentClass
            // 
            this.lblParentClass.AutoSize = true;
            this.lblParentClass.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblParentClass.Location = new System.Drawing.Point(394, 125);
            this.lblParentClass.Name = "lblParentClass";
            this.lblParentClass.Size = new System.Drawing.Size(71, 12);
            this.lblParentClass.TabIndex = 8;
            this.lblParentClass.Text = "ParentClass";
            // 
            // gvTables
            // 
            this.gvTables.AllowUserToAddRows = false;
            this.gvTables.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gvTables.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gvTables.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.gvColumnMap,
            this.gvColumnName});
            this.gvTables.Location = new System.Drawing.Point(6, 72);
            this.gvTables.Name = "gvTables";
            this.gvTables.RowHeadersVisible = false;
            this.gvTables.Size = new System.Drawing.Size(349, 435);
            this.gvTables.TabIndex = 8;
            // 
            // gvColumnMap
            // 
            this.gvColumnMap.DataPropertyName = "IsMap";
            this.gvColumnMap.HeaderText = "IsCreate";
            this.gvColumnMap.Name = "gvColumnMap";
            // 
            // gvColumnName
            // 
            this.gvColumnName.DataPropertyName = "Name";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.gvColumnName.DefaultCellStyle = dataGridViewCellStyle2;
            this.gvColumnName.HeaderText = "Name";
            this.gvColumnName.Name = "gvColumnName";
            this.gvColumnName.Width = 240;
            // 
            // tbNamespace
            // 
            this.tbNamespace.Location = new System.Drawing.Point(398, 88);
            this.tbNamespace.Name = "tbNamespace";
            this.tbNamespace.Size = new System.Drawing.Size(227, 21);
            this.tbNamespace.TabIndex = 6;
            this.tbNamespace.Text = "hc.epm.DataModel.Basic";
            // 
            // tbSavePath
            // 
            this.tbSavePath.Location = new System.Drawing.Point(400, 234);
            this.tbSavePath.Name = "tbSavePath";
            this.tbSavePath.Size = new System.Drawing.Size(227, 21);
            this.tbSavePath.TabIndex = 7;
            this.tbSavePath.Text = "D:\\\\";
            // 
            // lblSavePath
            // 
            this.lblSavePath.AutoSize = true;
            this.lblSavePath.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblSavePath.Location = new System.Drawing.Point(398, 206);
            this.lblSavePath.Name = "lblSavePath";
            this.lblSavePath.Size = new System.Drawing.Size(53, 12);
            this.lblSavePath.TabIndex = 4;
            this.lblSavePath.Text = "SavePath";
            // 
            // lblNamespace
            // 
            this.lblNamespace.AutoSize = true;
            this.lblNamespace.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblNamespace.Location = new System.Drawing.Point(392, 58);
            this.lblNamespace.Name = "lblNamespace";
            this.lblNamespace.Size = new System.Drawing.Size(59, 12);
            this.lblNamespace.TabIndex = 5;
            this.lblNamespace.Text = "Namespace";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(717, 548);
            this.Controls.Add(this.gbDataBase);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.gbDataBase.ResumeLayout(false);
            this.gbDataBase.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvTables)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbDataBase;
        private System.Windows.Forms.TextBox tbConnectionString;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.DataGridView gvTables;
        private System.Windows.Forms.TextBox tbNamespace;
        private System.Windows.Forms.Label lblSavePath;
        private System.Windows.Forms.Label lblNamespace;
        private System.Windows.Forms.TextBox tbSavePath;
        private System.Windows.Forms.TextBox tbParentClass;
        private System.Windows.Forms.Label lblParentClass;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnCreateDBSet;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnCreateServerType;
        private System.Windows.Forms.DataGridViewCheckBoxColumn gvColumnMap;
        private System.Windows.Forms.DataGridViewTextBoxColumn gvColumnName;
        private System.Windows.Forms.Button btnCreateService;
        private System.Windows.Forms.TextBox tbDataOperate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCreateInterface;
    }
}