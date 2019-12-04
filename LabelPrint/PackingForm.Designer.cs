namespace LabelPrint
{
    partial class PackingForm
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
            this.btPrintLabel = new System.Windows.Forms.Button();
            this.dtPackDate = new System.Windows.Forms.DateTimePicker();
            this.lbPackDate = new System.Windows.Forms.Label();
            this.lstItems = new System.Windows.Forms.ListView();
            this.colNo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPacktime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.txtQty = new System.Windows.Forms.TextBox();
            this.txtCNo = new System.Windows.Forms.TextBox();
            this.rdRepacking = new System.Windows.Forms.RadioButton();
            this.rdPacking = new System.Windows.Forms.RadioButton();
            this.btApply = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.lbQty = new System.Windows.Forms.Label();
            this.lbCNo = new System.Windows.Forms.Label();
            this.lbPNo = new System.Windows.Forms.Label();
            this.txtPNo = new System.Windows.Forms.TextBox();
            this.btPrintLabelNew = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btPrintLabel
            // 
            this.btPrintLabel.Location = new System.Drawing.Point(861, 539);
            this.btPrintLabel.Name = "btPrintLabel";
            this.btPrintLabel.Size = new System.Drawing.Size(116, 40);
            this.btPrintLabel.TabIndex = 27;
            this.btPrintLabel.Text = "打印标签";
            this.btPrintLabel.UseVisualStyleBackColor = true;
            this.btPrintLabel.Click += new System.EventHandler(this.btPrintLabel_Click);
            // 
            // dtPackDate
            // 
            this.dtPackDate.CalendarFont = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dtPackDate.CalendarForeColor = System.Drawing.Color.Blue;
            this.dtPackDate.Location = new System.Drawing.Point(824, 3);
            this.dtPackDate.Name = "dtPackDate";
            this.dtPackDate.Size = new System.Drawing.Size(168, 33);
            this.dtPackDate.TabIndex = 26;
            // 
            // lbPackDate
            // 
            this.lbPackDate.AutoSize = true;
            this.lbPackDate.Location = new System.Drawing.Point(752, 8);
            this.lbPackDate.Name = "lbPackDate";
            this.lbPackDate.Size = new System.Drawing.Size(74, 25);
            this.lbPackDate.TabIndex = 25;
            this.lbPackDate.Text = "捆包日:";
            // 
            // lstItems
            // 
            this.lstItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colNo,
            this.colCode,
            this.colPacktime});
            this.lstItems.FullRowSelect = true;
            this.lstItems.GridLines = true;
            this.lstItems.Location = new System.Drawing.Point(12, 107);
            this.lstItems.Name = "lstItems";
            this.lstItems.Size = new System.Drawing.Size(980, 419);
            this.lstItems.TabIndex = 24;
            this.lstItems.UseCompatibleStateImageBehavior = false;
            this.lstItems.View = System.Windows.Forms.View.Details;
            this.lstItems.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.lstItems_KeyPress);
            // 
            // colNo
            // 
            this.colNo.Text = "序号";
            // 
            // colCode
            // 
            this.colCode.Text = "编码";
            this.colCode.Width = 400;
            // 
            // colPacktime
            // 
            this.colPacktime.Text = "打包时间";
            this.colPacktime.Width = 200;
            // 
            // txtTotal
            // 
            this.txtTotal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.txtTotal.ForeColor = System.Drawing.Color.DarkOrange;
            this.txtTotal.Location = new System.Drawing.Point(681, 55);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.Size = new System.Drawing.Size(77, 33);
            this.txtTotal.TabIndex = 19;
            this.txtTotal.Leave += new System.EventHandler(this.txtTotal_Leave);
            // 
            // txtQty
            // 
            this.txtQty.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.txtQty.ForeColor = System.Drawing.Color.Red;
            this.txtQty.Location = new System.Drawing.Point(572, 55);
            this.txtQty.Name = "txtQty";
            this.txtQty.ReadOnly = true;
            this.txtQty.Size = new System.Drawing.Size(77, 33);
            this.txtQty.TabIndex = 20;
            // 
            // txtCNo
            // 
            this.txtCNo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.txtCNo.ForeColor = System.Drawing.Color.Blue;
            this.txtCNo.Location = new System.Drawing.Point(143, 55);
            this.txtCNo.Name = "txtCNo";
            this.txtCNo.Size = new System.Drawing.Size(312, 33);
            this.txtCNo.TabIndex = 18;
            this.txtCNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCNo_KeyDown);
            // 
            // rdRepacking
            // 
            this.rdRepacking.AutoSize = true;
            this.rdRepacking.Location = new System.Drawing.Point(664, 6);
            this.rdRepacking.Name = "rdRepacking";
            this.rdRepacking.Size = new System.Drawing.Size(68, 29);
            this.rdRepacking.TabIndex = 22;
            this.rdRepacking.Text = "重装";
            this.rdRepacking.UseVisualStyleBackColor = true;
            this.rdRepacking.Click += new System.EventHandler(this.rdRepacking_Click);
            // 
            // rdPacking
            // 
            this.rdPacking.AutoSize = true;
            this.rdPacking.Checked = true;
            this.rdPacking.Location = new System.Drawing.Point(599, 6);
            this.rdPacking.Name = "rdPacking";
            this.rdPacking.Size = new System.Drawing.Size(68, 29);
            this.rdPacking.TabIndex = 21;
            this.rdPacking.TabStop = true;
            this.rdPacking.Text = "新装";
            this.rdPacking.UseVisualStyleBackColor = true;
            this.rdPacking.Click += new System.EventHandler(this.rdPacking_Click);
            // 
            // btApply
            // 
            this.btApply.Location = new System.Drawing.Point(469, 1);
            this.btApply.Name = "btApply";
            this.btApply.Size = new System.Drawing.Size(116, 40);
            this.btApply.TabIndex = 23;
            this.btApply.Text = "预约";
            this.btApply.UseVisualStyleBackColor = true;
            this.btApply.Click += new System.EventHandler(this.btApply_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(655, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 25);
            this.label4.TabIndex = 13;
            this.label4.Text = "/";
            // 
            // lbQty
            // 
            this.lbQty.AutoSize = true;
            this.lbQty.Location = new System.Drawing.Point(464, 58);
            this.lbQty.Name = "lbQty";
            this.lbQty.Size = new System.Drawing.Size(107, 25);
            this.lbQty.TabIndex = 14;
            this.lbQty.Text = "打包数量：";
            // 
            // lbCNo
            // 
            this.lbCNo.AutoSize = true;
            this.lbCNo.Location = new System.Drawing.Point(6, 58);
            this.lbCNo.Name = "lbCNo";
            this.lbCNo.Size = new System.Drawing.Size(89, 25);
            this.lbCNo.TabIndex = 15;
            this.lbCNo.Text = "Tray No.";
            // 
            // lbPNo
            // 
            this.lbPNo.AutoSize = true;
            this.lbPNo.Location = new System.Drawing.Point(6, 8);
            this.lbPNo.Name = "lbPNo";
            this.lbPNo.Size = new System.Drawing.Size(93, 25);
            this.lbPNo.TabIndex = 16;
            this.lbPNo.Text = "Pack No.";
            // 
            // txtPNo
            // 
            this.txtPNo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.txtPNo.ForeColor = System.Drawing.Color.Blue;
            this.txtPNo.Location = new System.Drawing.Point(143, 5);
            this.txtPNo.Name = "txtPNo";
            this.txtPNo.Size = new System.Drawing.Size(312, 33);
            this.txtPNo.TabIndex = 17;
            this.txtPNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPNo_KeyDown);
            // 
            // btPrintLabelNew
            // 
            this.btPrintLabelNew.Location = new System.Drawing.Point(739, 539);
            this.btPrintLabelNew.Name = "btPrintLabelNew";
            this.btPrintLabelNew.Size = new System.Drawing.Size(116, 40);
            this.btPrintLabelNew.TabIndex = 28;
            this.btPrintLabelNew.Text = "打印标签2";
            this.btPrintLabelNew.UseVisualStyleBackColor = true;
            this.btPrintLabelNew.Click += new System.EventHandler(this.btPrintLabelNew_Click);
            // 
            // PackingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1003, 592);
            this.Controls.Add(this.btPrintLabelNew);
            this.Controls.Add(this.btPrintLabel);
            this.Controls.Add(this.dtPackDate);
            this.Controls.Add(this.lbPackDate);
            this.Controls.Add(this.lstItems);
            this.Controls.Add(this.txtTotal);
            this.Controls.Add(this.txtQty);
            this.Controls.Add(this.txtCNo);
            this.Controls.Add(this.rdRepacking);
            this.Controls.Add(this.rdPacking);
            this.Controls.Add(this.btApply);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lbQty);
            this.Controls.Add(this.lbCNo);
            this.Controls.Add(this.lbPNo);
            this.Controls.Add(this.txtPNo);
            this.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PackingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PackingForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btPrintLabel;
        private System.Windows.Forms.DateTimePicker dtPackDate;
        private System.Windows.Forms.Label lbPackDate;
        private System.Windows.Forms.ListView lstItems;
        private System.Windows.Forms.ColumnHeader colNo;
        private System.Windows.Forms.ColumnHeader colCode;
        private System.Windows.Forms.ColumnHeader colPacktime;
        private System.Windows.Forms.TextBox txtTotal;
        private System.Windows.Forms.TextBox txtQty;
        private System.Windows.Forms.TextBox txtCNo;
        private System.Windows.Forms.RadioButton rdRepacking;
        private System.Windows.Forms.RadioButton rdPacking;
        private System.Windows.Forms.Button btApply;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbQty;
        private System.Windows.Forms.Label lbCNo;
        private System.Windows.Forms.Label lbPNo;
        private System.Windows.Forms.TextBox txtPNo;
        private System.Windows.Forms.Button btPrintLabelNew;

    }
}