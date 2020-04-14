namespace hc.epm.TimerManage
{
    partial class MessageForm
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.gbMessage = new System.Windows.Forms.GroupBox();
            this.btnOpenLog = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStopAll = new System.Windows.Forms.Button();
            this.gvTables = new System.Windows.Forms.DataGridView();
            this.gbLog = new System.Windows.Forms.GroupBox();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.btnCleanLog = new System.Windows.Forms.Button();
            this.gvColumnId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClassPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MethodName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.State = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TaskType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IntervalTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ExcuteTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gvColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbMessage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvTables)).BeginInit();
            this.gbLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbMessage
            // 
            this.gbMessage.Controls.Add(this.btnCleanLog);
            this.gbMessage.Controls.Add(this.btnOpenLog);
            this.gbMessage.Controls.Add(this.btnStart);
            this.gbMessage.Controls.Add(this.btnStopAll);
            this.gbMessage.Controls.Add(this.gvTables);
            this.gbMessage.Location = new System.Drawing.Point(22, 27);
            this.gbMessage.Name = "gbMessage";
            this.gbMessage.Size = new System.Drawing.Size(724, 435);
            this.gbMessage.TabIndex = 0;
            this.gbMessage.TabStop = false;
            this.gbMessage.Text = "当前任务";
            // 
            // btnOpenLog
            // 
            this.btnOpenLog.Location = new System.Drawing.Point(358, 20);
            this.btnOpenLog.Name = "btnOpenLog";
            this.btnOpenLog.Size = new System.Drawing.Size(122, 23);
            this.btnOpenLog.TabIndex = 15;
            this.btnOpenLog.Text = "打开日志文件夹";
            this.btnOpenLog.UseVisualStyleBackColor = true;
            this.btnOpenLog.Click += new System.EventHandler(this.btnOpenLog_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(143, 21);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(93, 23);
            this.btnStart.TabIndex = 13;
            this.btnStart.Text = "开始所有任务";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStopAll
            // 
            this.btnStopAll.Location = new System.Drawing.Point(23, 21);
            this.btnStopAll.Name = "btnStopAll";
            this.btnStopAll.Size = new System.Drawing.Size(100, 23);
            this.btnStopAll.TabIndex = 12;
            this.btnStopAll.Text = "停止所有任务";
            this.btnStopAll.UseVisualStyleBackColor = true;
            this.btnStopAll.Click += new System.EventHandler(this.btnStopAll_Click);
            // 
            // gvTables
            // 
            this.gvTables.AllowUserToAddRows = false;
            this.gvTables.AllowUserToDeleteRows = false;
            this.gvTables.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gvTables.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.gvTables.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.gvColumnId,
            this.ClassPath,
            this.MethodName,
            this.State,
            this.TaskType,
            this.IntervalTime,
            this.ExcuteTime,
            this.gvColumnName});
            this.gvTables.Location = new System.Drawing.Point(6, 55);
            this.gvTables.Name = "gvTables";
            this.gvTables.ReadOnly = true;
            this.gvTables.RowHeadersVisible = false;
            this.gvTables.Size = new System.Drawing.Size(690, 365);
            this.gvTables.TabIndex = 10;
            // 
            // gbLog
            // 
            this.gbLog.Controls.Add(this.tbLog);
            this.gbLog.Location = new System.Drawing.Point(772, 27);
            this.gbLog.Name = "gbLog";
            this.gbLog.Size = new System.Drawing.Size(333, 435);
            this.gbLog.TabIndex = 1;
            this.gbLog.TabStop = false;
            this.gbLog.Text = "日志";
            // 
            // tbLog
            // 
            this.tbLog.Location = new System.Drawing.Point(6, 20);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbLog.Size = new System.Drawing.Size(322, 400);
            this.tbLog.TabIndex = 0;
            // 
            // btnCleanLog
            // 
            this.btnCleanLog.Location = new System.Drawing.Point(511, 21);
            this.btnCleanLog.Name = "btnCleanLog";
            this.btnCleanLog.Size = new System.Drawing.Size(75, 23);
            this.btnCleanLog.TabIndex = 16;
            this.btnCleanLog.Text = "清空日志";
            this.btnCleanLog.UseVisualStyleBackColor = true;
            this.btnCleanLog.Click += new System.EventHandler(this.btnCleanLog_Click);
            // 
            // gvColumnId
            // 
            this.gvColumnId.DataPropertyName = "Id";
            this.gvColumnId.HeaderText = "ID";
            this.gvColumnId.Name = "gvColumnId";
            this.gvColumnId.ReadOnly = true;
            this.gvColumnId.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.gvColumnId.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.gvColumnId.Width = 50;
            // 
            // ClassPath
            // 
            this.ClassPath.DataPropertyName = "ClassPath";
            this.ClassPath.HeaderText = "ClassPath";
            this.ClassPath.Name = "ClassPath";
            this.ClassPath.ReadOnly = true;
            this.ClassPath.Width = 220;
            // 
            // MethodName
            // 
            this.MethodName.DataPropertyName = "MethodName";
            this.MethodName.HeaderText = "方法名称";
            this.MethodName.Name = "MethodName";
            this.MethodName.ReadOnly = true;
            // 
            // State
            // 
            this.State.DataPropertyName = "State";
            this.State.HeaderText = "状态";
            this.State.Name = "State";
            this.State.ReadOnly = true;
            // 
            // TaskType
            // 
            this.TaskType.DataPropertyName = "TaskType";
            this.TaskType.HeaderText = "任务类型";
            this.TaskType.Name = "TaskType";
            this.TaskType.ReadOnly = true;
            // 
            // IntervalTime
            // 
            this.IntervalTime.DataPropertyName = "IntervalTime";
            this.IntervalTime.HeaderText = "间隔时间";
            this.IntervalTime.Name = "IntervalTime";
            this.IntervalTime.ReadOnly = true;
            // 
            // ExcuteTime
            // 
            this.ExcuteTime.DataPropertyName = "ExcuteTime";
            this.ExcuteTime.HeaderText = "执行时间";
            this.ExcuteTime.Name = "ExcuteTime";
            this.ExcuteTime.ReadOnly = true;
            // 
            // gvColumnName
            // 
            this.gvColumnName.DataPropertyName = "AssemblyPath";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.gvColumnName.DefaultCellStyle = dataGridViewCellStyle4;
            this.gvColumnName.HeaderText = "AssemblyPath";
            this.gvColumnName.Name = "gvColumnName";
            this.gvColumnName.ReadOnly = true;
            // 
            // MessageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1123, 497);
            this.Controls.Add(this.gbLog);
            this.Controls.Add(this.gbMessage);
            this.MaximizeBox = false;
            this.Name = "MessageForm";
            this.Text = "消息服务管理";
            this.Load += new System.EventHandler(this.MessageForm_Load);
            this.gbMessage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvTables)).EndInit();
            this.gbLog.ResumeLayout(false);
            this.gbLog.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbMessage;
        private System.Windows.Forms.DataGridView gvTables;
        private System.Windows.Forms.GroupBox gbLog;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.Button btnStopAll;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnOpenLog;
        private System.Windows.Forms.Button btnCleanLog;
        private System.Windows.Forms.DataGridViewTextBoxColumn gvColumnId;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClassPath;
        private System.Windows.Forms.DataGridViewTextBoxColumn MethodName;
        private System.Windows.Forms.DataGridViewTextBoxColumn State;
        private System.Windows.Forms.DataGridViewTextBoxColumn TaskType;
        private System.Windows.Forms.DataGridViewTextBoxColumn IntervalTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn ExcuteTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn gvColumnName;
    }
}

