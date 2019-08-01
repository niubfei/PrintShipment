namespace LabelPrint
{
    partial class UnpackForm
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
            this.lbUnpackCode = new System.Windows.Forms.Label();
            this.txtUnpackCode = new System.Windows.Forms.TextBox();
            this.lstUnpackItems = new System.Windows.Forms.ListView();
            this.colNo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lbUnpackItemCode = new System.Windows.Forms.Label();
            this.txtUnpackItemCode = new System.Windows.Forms.TextBox();
            this.btAll = new System.Windows.Forms.Button();
            this.btOK = new System.Windows.Forms.Button();
            this.btClose = new System.Windows.Forms.Button();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.txtQty = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lbQty = new System.Windows.Forms.Label();
            this.rdoSingle = new System.Windows.Forms.RadioButton();
            this.rdoDeepCancel = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // lbUnpackCode
            // 
            this.lbUnpackCode.AutoSize = true;
            this.lbUnpackCode.Location = new System.Drawing.Point(12, 8);
            this.lbUnpackCode.Name = "lbUnpackCode";
            this.lbUnpackCode.Size = new System.Drawing.Size(65, 25);
            this.lbUnpackCode.TabIndex = 1;
            this.lbUnpackCode.Text = "Code:";
            // 
            // txtUnpackCode
            // 
            this.txtUnpackCode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.txtUnpackCode.ForeColor = System.Drawing.Color.Blue;
            this.txtUnpackCode.Location = new System.Drawing.Point(131, 5);
            this.txtUnpackCode.Name = "txtUnpackCode";
            this.txtUnpackCode.Size = new System.Drawing.Size(292, 33);
            this.txtUnpackCode.TabIndex = 1;
            this.txtUnpackCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtUnpackCode_KeyDown);
            // 
            // lstUnpackItems
            // 
            this.lstUnpackItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colNo,
            this.colCode,
            this.colDate,
            this.colStatus});
            this.lstUnpackItems.FullRowSelect = true;
            this.lstUnpackItems.GridLines = true;
            this.lstUnpackItems.Location = new System.Drawing.Point(6, 83);
            this.lstUnpackItems.Name = "lstUnpackItems";
            this.lstUnpackItems.Size = new System.Drawing.Size(831, 407);
            this.lstUnpackItems.TabIndex = 3;
            this.lstUnpackItems.UseCompatibleStateImageBehavior = false;
            this.lstUnpackItems.View = System.Windows.Forms.View.Details;
            this.lstUnpackItems.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.lstUnpackItems_KeyPress);
            // 
            // colNo
            // 
            this.colNo.Text = "No.";
            // 
            // colCode
            // 
            this.colCode.Text = "Code";
            this.colCode.Width = 342;
            // 
            // colDate
            // 
            this.colDate.Text = "Pack Date";
            this.colDate.Width = 216;
            // 
            // colStatus
            // 
            this.colStatus.Text = "Status";
            this.colStatus.Width = 162;
            // 
            // lbUnpackItemCode
            // 
            this.lbUnpackItemCode.AutoSize = true;
            this.lbUnpackItemCode.Location = new System.Drawing.Point(12, 47);
            this.lbUnpackItemCode.Name = "lbUnpackItemCode";
            this.lbUnpackItemCode.Size = new System.Drawing.Size(113, 25);
            this.lbUnpackItemCode.TabIndex = 1;
            this.lbUnpackItemCode.Text = "Item Code:";
            // 
            // txtUnpackItemCode
            // 
            this.txtUnpackItemCode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.txtUnpackItemCode.ForeColor = System.Drawing.Color.Blue;
            this.txtUnpackItemCode.Location = new System.Drawing.Point(131, 44);
            this.txtUnpackItemCode.Name = "txtUnpackItemCode";
            this.txtUnpackItemCode.Size = new System.Drawing.Size(292, 33);
            this.txtUnpackItemCode.TabIndex = 2;
            this.txtUnpackItemCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtUnpackItemCode_KeyDown);
            // 
            // btAll
            // 
            this.btAll.Enabled = false;
            this.btAll.Location = new System.Drawing.Point(52, 513);
            this.btAll.Name = "btAll";
            this.btAll.Size = new System.Drawing.Size(127, 41);
            this.btAll.TabIndex = 4;
            this.btAll.Text = "全部取消";
            this.btAll.UseVisualStyleBackColor = true;
            this.btAll.Click += new System.EventHandler(this.btAll_Click);
            // 
            // btOK
            // 
            this.btOK.Location = new System.Drawing.Point(353, 513);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(127, 41);
            this.btOK.TabIndex = 5;
            this.btOK.Text = "确定";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // btClose
            // 
            this.btClose.Location = new System.Drawing.Point(654, 513);
            this.btClose.Name = "btClose";
            this.btClose.Size = new System.Drawing.Size(127, 41);
            this.btClose.TabIndex = 6;
            this.btClose.Text = "关闭";
            this.btClose.UseVisualStyleBackColor = true;
            this.btClose.Click += new System.EventHandler(this.btClose_Click);
            // 
            // txtTotal
            // 
            this.txtTotal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.txtTotal.ForeColor = System.Drawing.Color.DarkOrange;
            this.txtTotal.Location = new System.Drawing.Point(760, 39);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.ReadOnly = true;
            this.txtTotal.Size = new System.Drawing.Size(77, 33);
            this.txtTotal.TabIndex = 23;
            // 
            // txtQty
            // 
            this.txtQty.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.txtQty.ForeColor = System.Drawing.Color.Red;
            this.txtQty.Location = new System.Drawing.Point(651, 39);
            this.txtQty.Name = "txtQty";
            this.txtQty.ReadOnly = true;
            this.txtQty.Size = new System.Drawing.Size(77, 33);
            this.txtQty.TabIndex = 24;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(734, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 25);
            this.label4.TabIndex = 21;
            this.label4.Text = "/";
            // 
            // lbQty
            // 
            this.lbQty.AutoSize = true;
            this.lbQty.Location = new System.Drawing.Point(543, 42);
            this.lbQty.Name = "lbQty";
            this.lbQty.Size = new System.Drawing.Size(107, 25);
            this.lbQty.TabIndex = 22;
            this.lbQty.Text = "打包数量：";
            // 
            // rdoSingle
            // 
            this.rdoSingle.AutoSize = true;
            this.rdoSingle.Checked = true;
            this.rdoSingle.Location = new System.Drawing.Point(548, 6);
            this.rdoSingle.Name = "rdoSingle";
            this.rdoSingle.Size = new System.Drawing.Size(106, 29);
            this.rdoSingle.TabIndex = 25;
            this.rdoSingle.TabStop = true;
            this.rdoSingle.Text = "单层拆包";
            this.rdoSingle.UseVisualStyleBackColor = true;
            // 
            // rdoDeepCancel
            // 
            this.rdoDeepCancel.AutoSize = true;
            this.rdoDeepCancel.Location = new System.Drawing.Point(651, 6);
            this.rdoDeepCancel.Name = "rdoDeepCancel";
            this.rdoDeepCancel.Size = new System.Drawing.Size(106, 29);
            this.rdoDeepCancel.TabIndex = 25;
            this.rdoDeepCancel.Text = "深度拆包";
            this.rdoDeepCancel.UseVisualStyleBackColor = true;
            // 
            // UnpackForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(841, 576);
            this.Controls.Add(this.rdoDeepCancel);
            this.Controls.Add(this.rdoSingle);
            this.Controls.Add(this.txtTotal);
            this.Controls.Add(this.txtQty);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lbQty);
            this.Controls.Add(this.btClose);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.btAll);
            this.Controls.Add(this.lstUnpackItems);
            this.Controls.Add(this.txtUnpackItemCode);
            this.Controls.Add(this.lbUnpackItemCode);
            this.Controls.Add(this.txtUnpackCode);
            this.Controls.Add(this.lbUnpackCode);
            this.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "UnpackForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "拆包";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbUnpackCode;
        private System.Windows.Forms.TextBox txtUnpackCode;
        private System.Windows.Forms.ListView lstUnpackItems;
        private System.Windows.Forms.ColumnHeader colNo;
        private System.Windows.Forms.ColumnHeader colCode;
        private System.Windows.Forms.ColumnHeader colDate;
        private System.Windows.Forms.ColumnHeader colStatus;
        private System.Windows.Forms.Label lbUnpackItemCode;
        private System.Windows.Forms.TextBox txtUnpackItemCode;
        private System.Windows.Forms.Button btAll;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Button btClose;
        private System.Windows.Forms.TextBox txtTotal;
        private System.Windows.Forms.TextBox txtQty;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbQty;
        private System.Windows.Forms.RadioButton rdoSingle;
        private System.Windows.Forms.RadioButton rdoDeepCancel;
    }
}