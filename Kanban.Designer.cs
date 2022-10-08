
namespace Kanban
{
    partial class Kanban
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Kanban));
            this.ScanDataGrid = new System.Windows.Forms.DataGridView();
            this.PartNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.QuantityIssued = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AddToExcel = new System.Windows.Forms.DataGridViewButtonColumn();
            this.lblinfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ScanDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // ScanDataGrid
            // 
            this.ScanDataGrid.AllowUserToResizeColumns = false;
            this.ScanDataGrid.AllowUserToResizeRows = false;
            this.ScanDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.ScanDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ScanDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PartNumber,
            this.Date,
            this.QuantityIssued,
            this.AddToExcel});
            this.ScanDataGrid.Location = new System.Drawing.Point(-1, 1);
            this.ScanDataGrid.Name = "ScanDataGrid";
            this.ScanDataGrid.RowHeadersWidth = 51;
            this.ScanDataGrid.Size = new System.Drawing.Size(409, 392);
            this.ScanDataGrid.TabIndex = 0;
            this.ScanDataGrid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ScanDataGrid_CellClick);
            // 
            // PartNumber
            // 
            this.PartNumber.HeaderText = "Part Number";
            this.PartNumber.MinimumWidth = 6;
            this.PartNumber.Name = "PartNumber";
            this.PartNumber.Width = 84;
            // 
            // Date
            // 
            this.Date.HeaderText = "Date";
            this.Date.MinimumWidth = 6;
            this.Date.Name = "Date";
            this.Date.ReadOnly = true;
            this.Date.Width = 55;
            // 
            // QuantityIssued
            // 
            this.QuantityIssued.HeaderText = "Quantity Issued";
            this.QuantityIssued.MinimumWidth = 6;
            this.QuantityIssued.Name = "QuantityIssued";
            this.QuantityIssued.Width = 96;
            // 
            // AddToExcel
            // 
            this.AddToExcel.HeaderText = "Insert into spreadsheet";
            this.AddToExcel.Name = "AddToExcel";
            this.AddToExcel.Text = "Add to last row";
            this.AddToExcel.UseColumnTextForButtonValue = true;
            this.AddToExcel.Width = 108;
            // 
            // lblinfo
            // 
            this.lblinfo.AutoSize = true;
            this.lblinfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblinfo.Location = new System.Drawing.Point(25, 410);
            this.lblinfo.Name = "lblinfo";
            this.lblinfo.Size = new System.Drawing.Size(119, 25);
            this.lblinfo.TabIndex = 1;
            this.lblinfo.Text = "Information";
            this.lblinfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Kanban
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(407, 455);
            this.Controls.Add(this.lblinfo);
            this.Controls.Add(this.ScanDataGrid);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Kanban";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Kanban";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Kanban_FormClosing);
            this.Load += new System.EventHandler(this.Kanban_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ScanDataGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView ScanDataGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn PartNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn QuantityIssued;
        private System.Windows.Forms.DataGridViewButtonColumn AddToExcel;
        private System.Windows.Forms.Label lblinfo;
    }
}

