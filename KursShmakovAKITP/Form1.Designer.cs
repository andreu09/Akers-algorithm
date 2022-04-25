namespace KursShmakovAKITP
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Field = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.Field)).BeginInit();
            this.SuspendLayout();
            // 
            // Field
            // 
            this.Field.AllowUserToAddRows = false;
            this.Field.AllowUserToDeleteRows = false;
            this.Field.AllowUserToResizeColumns = false;
            this.Field.AllowUserToResizeRows = false;
            this.Field.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Field.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.Field.ColumnHeadersHeight = 29;
            this.Field.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.Field.ColumnHeadersVisible = false;
            this.Field.EnableHeadersVisualStyles = false;
            this.Field.Location = new System.Drawing.Point(-1, -1);
            this.Field.MultiSelect = false;
            this.Field.Name = "Field";
            this.Field.ReadOnly = true;
            this.Field.RowHeadersVisible = false;
            this.Field.RowHeadersWidth = 51;
            this.Field.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.Field.RowTemplate.Height = 29;
            this.Field.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.Field.ShowCellErrors = false;
            this.Field.ShowCellToolTips = false;
            this.Field.ShowEditingIcon = false;
            this.Field.ShowRowErrors = false;
            this.Field.Size = new System.Drawing.Size(1104, 724);
            this.Field.TabIndex = 3;
            this.Field.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.MarkingField);
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1103, 712);
            this.Controls.Add(this.Field);
            this.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Курсовой проект АКиТП Метод Акерса Шмаков А.Ю. 846гр.";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.Form1_HelpButtonClicked);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Field)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        public DataGridView Field;
    }
}