namespace LabelPrint
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cboLanguage = new System.Windows.Forms.ComboBox();
            this.btConfig = new System.Windows.Forms.Button();
            this.btExit = new System.Windows.Forms.Button();
            this.btDataBrowse = new System.Windows.Forms.Button();
            this.btReprint = new System.Windows.Forms.Button();
            this.btUnpacking = new System.Windows.Forms.Button();
            this.tabWork = new System.Windows.Forms.TabControl();
            this.tbPacking = new System.Windows.Forms.TabPage();
            this.tbCarton = new System.Windows.Forms.TabPage();
            this.tbPallet = new System.Windows.Forms.TabPage();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.tabWork.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitMain.Location = new System.Drawing.Point(0, 0);
            this.splitMain.Name = "splitMain";
            this.splitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.button1);
            this.splitMain.Panel1.Controls.Add(this.label1);
            this.splitMain.Panel1.Controls.Add(this.cboLanguage);
            this.splitMain.Panel1.Controls.Add(this.btConfig);
            this.splitMain.Panel1.Controls.Add(this.btExit);
            this.splitMain.Panel1.Controls.Add(this.btDataBrowse);
            this.splitMain.Panel1.Controls.Add(this.btReprint);
            this.splitMain.Panel1.Controls.Add(this.btUnpacking);
            this.splitMain.Panel1MinSize = 90;
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.tabWork);
            this.splitMain.Size = new System.Drawing.Size(1008, 730);
            this.splitMain.SplitterDistance = 90;
            this.splitMain.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(367, 21);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(117, 48);
            this.button1.TabIndex = 23;
            this.button1.Text = "出货";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(726, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 21);
            this.label1.TabIndex = 22;
            this.label1.Text = "语言设置";
            this.label1.Visible = false;
            // 
            // cboLanguage
            // 
            this.cboLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLanguage.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cboLanguage.FormattingEnabled = true;
            this.cboLanguage.ItemHeight = 21;
            this.cboLanguage.Items.AddRange(new object[] {
            "简体中文",
            "日文",
            "English"});
            this.cboLanguage.Location = new System.Drawing.Point(806, 32);
            this.cboLanguage.Name = "cboLanguage";
            this.cboLanguage.Size = new System.Drawing.Size(92, 29);
            this.cboLanguage.TabIndex = 21;
            this.cboLanguage.Visible = false;
            this.cboLanguage.SelectedIndexChanged += new System.EventHandler(this.cboLanguage_SelectedIndexChanged);
            // 
            // btConfig
            // 
            this.btConfig.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btConfig.Location = new System.Drawing.Point(247, 21);
            this.btConfig.Name = "btConfig";
            this.btConfig.Size = new System.Drawing.Size(117, 48);
            this.btConfig.TabIndex = 3;
            this.btConfig.Text = "参数设定";
            this.btConfig.UseVisualStyleBackColor = true;
            this.btConfig.Click += new System.EventHandler(this.btConfig_Click);
            // 
            // btExit
            // 
            this.btExit.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btExit.Location = new System.Drawing.Point(904, 29);
            this.btExit.Margin = new System.Windows.Forms.Padding(3, 3, 1, 3);
            this.btExit.Name = "btExit";
            this.btExit.Size = new System.Drawing.Size(92, 32);
            this.btExit.TabIndex = 2;
            this.btExit.Text = "退出";
            this.btExit.UseVisualStyleBackColor = true;
            this.btExit.Click += new System.EventHandler(this.btExit_Click);
            // 
            // btDataBrowse
            // 
            this.btDataBrowse.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btDataBrowse.Location = new System.Drawing.Point(487, 21);
            this.btDataBrowse.Name = "btDataBrowse";
            this.btDataBrowse.Size = new System.Drawing.Size(117, 48);
            this.btDataBrowse.TabIndex = 2;
            this.btDataBrowse.Text = "数据检索";
            this.btDataBrowse.UseVisualStyleBackColor = true;
            this.btDataBrowse.Visible = false;
            // 
            // btReprint
            // 
            this.btReprint.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btReprint.Location = new System.Drawing.Point(127, 21);
            this.btReprint.Name = "btReprint";
            this.btReprint.Size = new System.Drawing.Size(117, 48);
            this.btReprint.TabIndex = 1;
            this.btReprint.Text = "重新打印";
            this.btReprint.UseVisualStyleBackColor = true;
            this.btReprint.Click += new System.EventHandler(this.btReprint_Click);
            // 
            // btUnpacking
            // 
            this.btUnpacking.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btUnpacking.Location = new System.Drawing.Point(7, 21);
            this.btUnpacking.Name = "btUnpacking";
            this.btUnpacking.Size = new System.Drawing.Size(117, 48);
            this.btUnpacking.TabIndex = 1;
            this.btUnpacking.Text = "拆包";
            this.btUnpacking.UseVisualStyleBackColor = true;
            this.btUnpacking.Click += new System.EventHandler(this.btUnpacking_Click);
            // 
            // tabWork
            // 
            this.tabWork.Controls.Add(this.tbPacking);
            this.tabWork.Controls.Add(this.tbCarton);
            this.tabWork.Controls.Add(this.tbPallet);
            this.tabWork.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabWork.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabWork.Location = new System.Drawing.Point(0, 0);
            this.tabWork.Name = "tabWork";
            this.tabWork.SelectedIndex = 0;
            this.tabWork.Size = new System.Drawing.Size(1008, 636);
            this.tabWork.TabIndex = 0;
            this.tabWork.SelectedIndexChanged += new System.EventHandler(this.tabWork_SelectedIndexChanged);
            // 
            // tbPacking
            // 
            this.tbPacking.Location = new System.Drawing.Point(4, 34);
            this.tbPacking.Name = "tbPacking";
            this.tbPacking.Size = new System.Drawing.Size(1000, 598);
            this.tbPacking.TabIndex = 0;
            this.tbPacking.Text = "打包";
            this.tbPacking.UseVisualStyleBackColor = true;
            // 
            // tbCarton
            // 
            this.tbCarton.Location = new System.Drawing.Point(4, 34);
            this.tbCarton.Name = "tbCarton";
            this.tbCarton.Padding = new System.Windows.Forms.Padding(3);
            this.tbCarton.Size = new System.Drawing.Size(1000, 598);
            this.tbCarton.TabIndex = 1;
            this.tbCarton.Text = "装箱";
            this.tbCarton.UseVisualStyleBackColor = true;
            // 
            // tbPallet
            // 
            this.tbPallet.Location = new System.Drawing.Point(4, 34);
            this.tbPallet.Name = "tbPallet";
            this.tbPallet.Padding = new System.Windows.Forms.Padding(3);
            this.tbPallet.Size = new System.Drawing.Size(1000, 598);
            this.tbPallet.TabIndex = 2;
            this.tbPallet.Text = "装盘";
            this.tbPallet.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 730);
            this.Controls.Add(this.splitMain);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(1024, 726);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Label Print";
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel1.PerformLayout();
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.tabWork.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.Button btConfig;
        private System.Windows.Forms.Button btDataBrowse;
        private System.Windows.Forms.Button btUnpacking;
        private System.Windows.Forms.TabControl tabWork;
        private System.Windows.Forms.TabPage tbPacking;
        private System.Windows.Forms.TabPage tbCarton;
        private System.Windows.Forms.TabPage tbPallet;
        private System.Windows.Forms.Button btExit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboLanguage;
        private System.Windows.Forms.Button btReprint;
		private System.Windows.Forms.Button button1;
    }
}

