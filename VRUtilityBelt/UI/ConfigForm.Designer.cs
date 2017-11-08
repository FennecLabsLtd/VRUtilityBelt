namespace VRUB.UI
{
    partial class ConfigForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("VRUB Settings");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Addon: Test Suite");
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("Desktop View Settings");
            this.configGrid = new PropertyGridEx.PropertyGridEx();
            this.configSplitContainer = new System.Windows.Forms.SplitContainer();
            this.configList = new System.Windows.Forms.ListView();
            this.configTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnApply = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.configSplitContainer)).BeginInit();
            this.configSplitContainer.Panel1.SuspendLayout();
            this.configSplitContainer.Panel2.SuspendLayout();
            this.configSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // configGrid
            // 
            this.configGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.configGrid.DocCommentDescription.AccessibleName = "";
            this.configGrid.DocCommentDescription.Location = new System.Drawing.Point(3, 18);
            this.configGrid.DocCommentDescription.Name = "";
            this.configGrid.DocCommentDescription.Size = new System.Drawing.Size(422, 37);
            this.configGrid.DocCommentDescription.TabIndex = 1;
            this.configGrid.DocCommentImage = null;
            // 
            // 
            // 
            this.configGrid.DocCommentTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.configGrid.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
            this.configGrid.DocCommentTitle.Name = "";
            this.configGrid.DocCommentTitle.Size = new System.Drawing.Size(422, 15);
            this.configGrid.DocCommentTitle.TabIndex = 0;
            this.configGrid.DocCommentTitle.Text = "Property 1";
            this.configGrid.Item.Add(((PropertyGridEx.CustomProperty)(resources.GetObject("configGrid.Item"))));
            this.configGrid.Item.Add(((PropertyGridEx.CustomProperty)(resources.GetObject("configGrid.Item1"))));
            this.configGrid.Location = new System.Drawing.Point(3, 3);
            this.configGrid.Name = "configGrid";
            this.configGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.configGrid.SelectedObject = ((object)(resources.GetObject("configGrid.SelectedObject")));
            this.configGrid.ShowCustomProperties = true;
            this.configGrid.Size = new System.Drawing.Size(428, 468);
            this.configGrid.TabIndex = 0;
            this.configGrid.ToolbarVisible = false;
            // 
            // 
            // 
            this.configGrid.ToolStrip.Location = new System.Drawing.Point(0, 0);
            this.configGrid.ToolStrip.Name = "";
            this.configGrid.ToolStrip.Size = new System.Drawing.Size(428, 25);
            this.configGrid.ToolStrip.TabIndex = 1;
            this.configGrid.ToolStrip.Visible = false;
            // 
            // configSplitContainer
            // 
            this.configSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.configSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.configSplitContainer.Location = new System.Drawing.Point(12, 12);
            this.configSplitContainer.Name = "configSplitContainer";
            // 
            // configSplitContainer.Panel1
            // 
            this.configSplitContainer.Panel1.Controls.Add(this.configList);
            // 
            // configSplitContainer.Panel2
            // 
            this.configSplitContainer.Panel2.Controls.Add(this.configGrid);
            this.configSplitContainer.Size = new System.Drawing.Size(735, 477);
            this.configSplitContainer.SplitterDistance = 297;
            this.configSplitContainer.TabIndex = 1;
            // 
            // configList
            // 
            this.configList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.configTitle});
            this.configList.FullRowSelect = true;
            this.configList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.configList.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem4,
            listViewItem5,
            listViewItem6});
            this.configList.LabelWrap = false;
            this.configList.Location = new System.Drawing.Point(3, 3);
            this.configList.MultiSelect = false;
            this.configList.Name = "configList";
            this.configList.ShowGroups = false;
            this.configList.Size = new System.Drawing.Size(291, 468);
            this.configList.TabIndex = 0;
            this.configList.UseCompatibleStateImageBehavior = false;
            this.configList.View = System.Windows.Forms.View.Details;
            this.configList.SelectedIndexChanged += new System.EventHandler(this.configList_SelectedIndexChanged);
            // 
            // configTitle
            // 
            this.configTitle.Width = 280;
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(623, 495);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(121, 30);
            this.btnApply.TabIndex = 1;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(496, 495);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(121, 30);
            this.btnReset.TabIndex = 2;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(759, 534);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.configSplitContainer);
            this.Name = "ConfigForm";
            this.Text = "VRUB Settings";
            this.configSplitContainer.Panel1.ResumeLayout(false);
            this.configSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.configSplitContainer)).EndInit();
            this.configSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private PropertyGridEx.PropertyGridEx configGrid;
        private System.Windows.Forms.SplitContainer configSplitContainer;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.ListView configList;
        private System.Windows.Forms.ColumnHeader configTitle;
    }
}