namespace VRUtilityBelt.UI.Workshop
{
    partial class WorkshopUploader
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
            this.cmbAddons = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblAddonIgnore = new System.Windows.Forms.Label();
            this.lblIgnore = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblAddonTitle = new System.Windows.Forms.Label();
            this.lblAddonType = new System.Windows.Forms.Label();
            this.lblAddonTags = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.txtUpdateNotes = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbAddons
            // 
            this.cmbAddons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbAddons.FormattingEnabled = true;
            this.cmbAddons.Location = new System.Drawing.Point(12, 12);
            this.cmbAddons.Name = "cmbAddons";
            this.cmbAddons.Size = new System.Drawing.Size(390, 21);
            this.cmbAddons.TabIndex = 0;
            this.cmbAddons.Text = "Select an Addon";
            this.cmbAddons.SelectedIndexChanged += new System.EventHandler(this.cmbAddons_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Location = new System.Drawing.Point(13, 51);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(10);
            this.groupBox1.Size = new System.Drawing.Size(389, 136);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Selected Addon";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lblAddonIgnore, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblIgnore, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblAddonTitle, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblAddonType, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblAddonTags, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(13, 26);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(363, 95);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // lblAddonIgnore
            // 
            this.lblAddonIgnore.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAddonIgnore.AutoSize = true;
            this.lblAddonIgnore.Location = new System.Drawing.Point(86, 74);
            this.lblAddonIgnore.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lblAddonIgnore.Name = "lblAddonIgnore";
            this.lblAddonIgnore.Size = new System.Drawing.Size(274, 16);
            this.lblAddonIgnore.TabIndex = 7;
            // 
            // lblIgnore
            // 
            this.lblIgnore.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblIgnore.AutoSize = true;
            this.lblIgnore.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIgnore.Location = new System.Drawing.Point(3, 74);
            this.lblIgnore.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lblIgnore.Name = "lblIgnore";
            this.lblIgnore.Size = new System.Drawing.Size(77, 16);
            this.lblIgnore.TabIndex = 6;
            this.lblIgnore.Text = "Ignore Files:";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Title:";
            // 
            // lblAddonTitle
            // 
            this.lblAddonTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAddonTitle.AutoSize = true;
            this.lblAddonTitle.Location = new System.Drawing.Point(86, 5);
            this.lblAddonTitle.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lblAddonTitle.Name = "lblAddonTitle";
            this.lblAddonTitle.Size = new System.Drawing.Size(274, 13);
            this.lblAddonTitle.TabIndex = 1;
            // 
            // lblAddonType
            // 
            this.lblAddonType.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAddonType.AutoSize = true;
            this.lblAddonType.Location = new System.Drawing.Point(86, 28);
            this.lblAddonType.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lblAddonType.Name = "lblAddonType";
            this.lblAddonType.Size = new System.Drawing.Size(274, 13);
            this.lblAddonType.TabIndex = 3;
            // 
            // lblAddonTags
            // 
            this.lblAddonTags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAddonTags.AutoSize = true;
            this.lblAddonTags.Location = new System.Drawing.Point(86, 51);
            this.lblAddonTags.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.lblAddonTags.Name = "lblAddonTags";
            this.lblAddonTags.Size = new System.Drawing.Size(274, 13);
            this.lblAddonTags.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 28);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Type:";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(3, 51);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Tags:";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Enabled = false;
            this.btnSubmit.Location = new System.Drawing.Point(12, 255);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(389, 54);
            this.btnSubmit.TabIndex = 2;
            this.btnSubmit.Text = "Create Workshop Item";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // txtUpdateNotes
            // 
            this.txtUpdateNotes.Location = new System.Drawing.Point(12, 219);
            this.txtUpdateNotes.Name = "txtUpdateNotes";
            this.txtUpdateNotes.Size = new System.Drawing.Size(389, 20);
            this.txtUpdateNotes.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(9, 203);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Update Notes";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 323);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(193, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "By submitting this item, you agree to the";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(209, 323);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(139, 13);
            this.linkLabel1.TabIndex = 6;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Workshop Terms of Service";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // WorkshopUploader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(414, 349);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtUpdateNotes);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cmbAddons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "WorkshopUploader";
            this.Text = " ";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbAddons;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblAddonTitle;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblAddonTags;
        private System.Windows.Forms.Label lblAddonType;
        private System.Windows.Forms.Label lblAddonIgnore;
        private System.Windows.Forms.Label lblIgnore;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.TextBox txtUpdateNotes;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}