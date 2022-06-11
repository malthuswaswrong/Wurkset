namespace WeightTracker;

partial class frmMain
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
            this.lvWeightData = new System.Windows.Forms.ListView();
            this.colDay = new System.Windows.Forms.ColumnHeader();
            this.colWeight = new System.Windows.Forms.ColumnHeader();
            this.colBF = new System.Windows.Forms.ColumnHeader();
            this.colOverweight = new System.Windows.Forms.ColumnHeader();
            this.colExcessCalories = new System.Windows.Forms.ColumnHeader();
            this.label1 = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtWeight = new System.Windows.Forms.TextBox();
            this.txtBFPercent = new System.Windows.Forms.TextBox();
            this.cmdSave = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.cmdAddBefore = new System.Windows.Forms.Button();
            this.cmdAddAfter = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvWeightData
            // 
            this.lvWeightData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvWeightData.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colDay,
            this.colWeight,
            this.colBF,
            this.colOverweight,
            this.colExcessCalories});
            this.lvWeightData.FullRowSelect = true;
            this.lvWeightData.Location = new System.Drawing.Point(2, 2);
            this.lvWeightData.MultiSelect = false;
            this.lvWeightData.Name = "lvWeightData";
            this.lvWeightData.Size = new System.Drawing.Size(498, 287);
            this.lvWeightData.TabIndex = 0;
            this.lvWeightData.UseCompatibleStateImageBehavior = false;
            this.lvWeightData.View = System.Windows.Forms.View.Details;
            this.lvWeightData.SelectedIndexChanged += new System.EventHandler(this.lvWeightData_SelectedIndexChanged);
            // 
            // colDay
            // 
            this.colDay.Text = "Day";
            this.colDay.Width = 120;
            // 
            // colWeight
            // 
            this.colWeight.Text = "Weight";
            this.colWeight.Width = 70;
            // 
            // colBF
            // 
            this.colBF.Text = "BF%";
            this.colBF.Width = 70;
            // 
            // colOverweight
            // 
            this.colOverweight.Text = "+Weight";
            this.colOverweight.Width = 90;
            // 
            // colExcessCalories
            // 
            this.colExcessCalories.Text = "Excess Calories";
            this.colExcessCalories.Width = 120;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 309);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Date :";
            // 
            // lblDate
            // 
            this.lblDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblDate.Location = new System.Drawing.Point(78, 309);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(92, 22);
            this.lblDate.TabIndex = 2;
            this.lblDate.Text = "not selected";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 344);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Weight :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 381);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "BF% :";
            // 
            // txtWeight
            // 
            this.txtWeight.Location = new System.Drawing.Point(78, 348);
            this.txtWeight.Name = "txtWeight";
            this.txtWeight.Size = new System.Drawing.Size(92, 27);
            this.txtWeight.TabIndex = 5;
            // 
            // txtBFPercent
            // 
            this.txtBFPercent.Location = new System.Drawing.Point(78, 381);
            this.txtBFPercent.Name = "txtBFPercent";
            this.txtBFPercent.Size = new System.Drawing.Size(92, 27);
            this.txtBFPercent.TabIndex = 6;
            // 
            // cmdSave
            // 
            this.cmdSave.Location = new System.Drawing.Point(12, 424);
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.Size = new System.Drawing.Size(158, 26);
            this.cmdSave.TabIndex = 7;
            this.cmdSave.Text = "Save";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 461);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(503, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 16);
            // 
            // cmdAddBefore
            // 
            this.cmdAddBefore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdAddBefore.Location = new System.Drawing.Point(255, 306);
            this.cmdAddBefore.Name = "cmdAddBefore";
            this.cmdAddBefore.Size = new System.Drawing.Size(236, 29);
            this.cmdAddBefore.TabIndex = 9;
            this.cmdAddBefore.Text = "Add Day Before Selected";
            this.cmdAddBefore.UseVisualStyleBackColor = true;
            this.cmdAddBefore.Click += new System.EventHandler(this.cmdAddBefore_Click);
            // 
            // cmdAddAfter
            // 
            this.cmdAddAfter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdAddAfter.Location = new System.Drawing.Point(255, 348);
            this.cmdAddAfter.Name = "cmdAddAfter";
            this.cmdAddAfter.Size = new System.Drawing.Size(236, 29);
            this.cmdAddAfter.TabIndex = 10;
            this.cmdAddAfter.Text = "Add Day After Selected";
            this.cmdAddAfter.UseVisualStyleBackColor = true;
            this.cmdAddAfter.Click += new System.EventHandler(this.cmdAddAfter_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(503, 483);
            this.Controls.Add(this.cmdAddAfter);
            this.Controls.Add(this.cmdAddBefore);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.cmdSave);
            this.Controls.Add(this.txtBFPercent);
            this.Controls.Add(this.txtWeight);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lvWeightData);
            this.Name = "frmMain";
            this.Text = "Weight Tracker";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private ListView lvWeightData;
    private ColumnHeader colDay;
    private ColumnHeader colWeight;
    private ColumnHeader colBF;
    private ColumnHeader colOverweight;
    private ColumnHeader colExcessCalories;
    private Label label1;
    private Label lblDate;
    private Label label2;
    private Label label3;
    private TextBox txtWeight;
    private TextBox txtBFPercent;
    private Button cmdSave;
    private StatusStrip statusStrip1;
    private ToolStripStatusLabel toolStripStatusLabel1;
    private Button cmdAddBefore;
    private Button cmdAddAfter;
}