namespace LabelPrint
{
    partial class ReprintForm
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
            this.lbScan = new System.Windows.Forms.Label();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.lbDate = new System.Windows.Forms.Label();
            this.dtPicker = new System.Windows.Forms.DateTimePicker();
            this.lstItems = new System.Windows.Forms.ListView();
            this.colNo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.txtQty = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lbQty = new System.Windows.Forms.Label();
            this.btClose = new System.Windows.Forms.Button();
            this.btReprintNew = new System.Windows.Forms.Button();
            this.btReprint3 = new System.Windows.Forms.Button();
            this.btReprint = new System.Windows.Forms.Button();
            this.txtChangePackID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbScan
            // 
            this.lbScan.AutoSize = true;
            this.lbScan.Location = new System.Drawing.Point(7, 40);
            this.lbScan.Name = "lbScan";
            this.lbScan.Size = new System.Drawing.Size(90, 21);
            this.lbScan.TabIndex = 0;
            this.lbScan.Text = "标签条码：";
            // 
            // txtCode
            // 
            this.txtCode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.txtCode.ForeColor = System.Drawing.Color.Blue;
            this.txtCode.Location = new System.Drawing.Point(103, 37);
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(355, 29);
            this.txtCode.TabIndex = 1;
            this.txtCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCode_KeyDown);
            // 
            // lbDate
            // 
            this.lbDate.AutoSize = true;
            this.lbDate.Location = new System.Drawing.Point(504, 5);
            this.lbDate.Name = "lbDate";
            this.lbDate.Size = new System.Drawing.Size(74, 21);
            this.lbDate.TabIndex = 0;
            this.lbDate.Text = "捆包日：";
            // 
            // dtPicker
            // 
            this.dtPicker.Location = new System.Drawing.Point(596, 2);
            this.dtPicker.Name = "dtPicker";
            this.dtPicker.Size = new System.Drawing.Size(186, 29);
            this.dtPicker.TabIndex = 2;
            // 
            // lstItems
            // 
            this.lstItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colNo,
            this.colCode,
            this.colDate});
            this.lstItems.FullRowSelect = true;
            this.lstItems.GridLines = true;
            this.lstItems.HideSelection = false;
            this.lstItems.Location = new System.Drawing.Point(4, 73);
            this.lstItems.Name = "lstItems";
            this.lstItems.Size = new System.Drawing.Size(778, 362);
            this.lstItems.TabIndex = 3;
            this.lstItems.UseCompatibleStateImageBehavior = false;
            this.lstItems.View = System.Windows.Forms.View.Details;
            this.lstItems.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.lstItems_KeyPress);
            // 
            // colNo
            // 
            this.colNo.Text = "No.";
            this.colNo.Width = 40;
            // 
            // colCode
            // 
            this.colCode.Text = "Code";
            this.colCode.Width = 300;
            // 
            // colDate
            // 
            this.colDate.Text = "Date";
            this.colDate.Width = 200;
            // 
            // txtTotal
            // 
            this.txtTotal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.txtTotal.ForeColor = System.Drawing.Color.DarkOrange;
            this.txtTotal.Location = new System.Drawing.Point(705, 37);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.ReadOnly = true;
            this.txtTotal.Size = new System.Drawing.Size(77, 29);
            this.txtTotal.TabIndex = 23;
            // 
            // txtQty
            // 
            this.txtQty.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.txtQty.ForeColor = System.Drawing.Color.Red;
            this.txtQty.Location = new System.Drawing.Point(596, 37);
            this.txtQty.Name = "txtQty";
            this.txtQty.ReadOnly = true;
            this.txtQty.Size = new System.Drawing.Size(77, 29);
            this.txtQty.TabIndex = 24;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(679, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 21);
            this.label4.TabIndex = 21;
            this.label4.Text = "/";
            // 
            // lbQty
            // 
            this.lbQty.AutoSize = true;
            this.lbQty.Location = new System.Drawing.Point(488, 40);
            this.lbQty.Name = "lbQty";
            this.lbQty.Size = new System.Drawing.Size(90, 21);
            this.lbQty.TabIndex = 22;
            this.lbQty.Text = "打包数量：";
            // 
            // btClose
            // 
            this.btClose.Location = new System.Drawing.Point(527, 458);
            this.btClose.Name = "btClose";
            this.btClose.Size = new System.Drawing.Size(126, 41);
            this.btClose.TabIndex = 25;
            this.btClose.Text = "关闭";
            this.btClose.UseVisualStyleBackColor = true;
            this.btClose.Click += new System.EventHandler(this.btClose_Click);
            // 
            // btReprintNew
            // 
            this.btReprintNew.Location = new System.Drawing.Point(137, 458);
            this.btReprintNew.Name = "btReprintNew";
            this.btReprintNew.Size = new System.Drawing.Size(126, 41);
            this.btReprintNew.TabIndex = 25;
            this.btReprintNew.Text = "打印2";
            this.btReprintNew.UseVisualStyleBackColor = true;
            this.btReprintNew.Click += new System.EventHandler(this.btReprintNew_Click);
            // 
            // btReprint3
            // 
            this.btReprint3.Location = new System.Drawing.Point(5, 458);
            this.btReprint3.Name = "btReprint3";
            this.btReprint3.Size = new System.Drawing.Size(126, 41);
            this.btReprint3.TabIndex = 26;
            this.btReprint3.Text = "打印3";
            this.btReprint3.UseVisualStyleBackColor = true;
            this.btReprint3.Click += new System.EventHandler(this.BtReprint3_Click);
            // 
            // btReprint
            // 
            this.btReprint.Location = new System.Drawing.Point(269, 458);
            this.btReprint.Name = "btReprint";
            this.btReprint.Size = new System.Drawing.Size(126, 41);
            this.btReprint.TabIndex = 25;
            this.btReprint.Text = "打印";
            this.btReprint.UseVisualStyleBackColor = true;
            this.btReprint.Click += new System.EventHandler(this.btReprint_Click);
            // 
            // txtChangePackID
            // 
            this.txtChangePackID.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.txtChangePackID.ForeColor = System.Drawing.Color.Blue;
            this.txtChangePackID.Location = new System.Drawing.Point(103, 5);
            this.txtChangePackID.Name = "txtChangePackID";
            this.txtChangePackID.Size = new System.Drawing.Size(355, 29);
            this.txtChangePackID.TabIndex = 27;
            this.txtChangePackID.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtChangePackID_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 21);
            this.label1.TabIndex = 28;
            this.label1.Text = "包号转换：";
            // 
            // ReprintForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(788, 526);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtChangePackID);
            this.Controls.Add(this.btReprint3);
            this.Controls.Add(this.btClose);
            this.Controls.Add(this.btReprintNew);
            this.Controls.Add(this.btReprint);
            this.Controls.Add(this.txtTotal);
            this.Controls.Add(this.txtQty);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lbQty);
            this.Controls.Add(this.lstItems);
            this.Controls.Add(this.dtPicker);
            this.Controls.Add(this.txtCode);
            this.Controls.Add(this.lbDate);
            this.Controls.Add(this.lbScan);
            this.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "ReprintForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "重新打印";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbScan;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Label lbDate;
        private System.Windows.Forms.DateTimePicker dtPicker;
        private System.Windows.Forms.ListView lstItems;
        private System.Windows.Forms.ColumnHeader colNo;
        private System.Windows.Forms.ColumnHeader colCode;
        private System.Windows.Forms.ColumnHeader colDate;
        private System.Windows.Forms.TextBox txtTotal;
        private System.Windows.Forms.TextBox txtQty;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbQty;
        private System.Windows.Forms.Button btClose;
        private System.Windows.Forms.Button btReprintNew;
        private System.Windows.Forms.Button btReprint3;
        private System.Windows.Forms.Button btReprint;
        private System.Windows.Forms.TextBox txtChangePackID;
        private System.Windows.Forms.Label label1;
    }
}