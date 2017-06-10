namespace SEVSTools.OptionsPage
{
	partial class GeneralOptions
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.buildPathButton = new System.Windows.Forms.Button();
			this.gamePathButton = new System.Windows.Forms.Button();
			this.gamePathLabel = new System.Windows.Forms.Label();
			this.buildPathLabel = new System.Windows.Forms.Label();
			this.gamePathTextBox = new System.Windows.Forms.TextBox();
			this.buildPathTextBox = new System.Windows.Forms.TextBox();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.ColumnCount = 3;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tableLayoutPanel1.Controls.Add(this.buildPathButton, 2, 1);
			this.tableLayoutPanel1.Controls.Add(this.gamePathButton, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this.gamePathLabel, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.buildPathLabel, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.gamePathTextBox, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.buildPathTextBox, 1, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(553, 58);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// buildPathButton
			// 
			this.buildPathButton.Location = new System.Drawing.Point(526, 32);
			this.buildPathButton.Name = "buildPathButton";
			this.buildPathButton.Size = new System.Drawing.Size(24, 23);
			this.buildPathButton.TabIndex = 5;
			this.buildPathButton.Text = "...";
			this.buildPathButton.UseVisualStyleBackColor = true;
			this.buildPathButton.Click += new System.EventHandler(this.buildPathButton_Click);
			// 
			// gamePathButton
			// 
			this.gamePathButton.Location = new System.Drawing.Point(526, 3);
			this.gamePathButton.Name = "gamePathButton";
			this.gamePathButton.Size = new System.Drawing.Size(24, 23);
			this.gamePathButton.TabIndex = 0;
			this.gamePathButton.Text = "...";
			this.gamePathButton.UseVisualStyleBackColor = true;
			this.gamePathButton.Click += new System.EventHandler(this.gamePathButton_Click);
			// 
			// gamePathLabel
			// 
			this.gamePathLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.gamePathLabel.AutoSize = true;
			this.gamePathLabel.Location = new System.Drawing.Point(3, 8);
			this.gamePathLabel.Name = "gamePathLabel";
			this.gamePathLabel.Size = new System.Drawing.Size(65, 13);
			this.gamePathLabel.TabIndex = 1;
			this.gamePathLabel.Text = "Game path: ";
			// 
			// buildPathLabel
			// 
			this.buildPathLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.buildPathLabel.AutoSize = true;
			this.buildPathLabel.Location = new System.Drawing.Point(3, 37);
			this.buildPathLabel.Name = "buildPathLabel";
			this.buildPathLabel.Size = new System.Drawing.Size(57, 13);
			this.buildPathLabel.TabIndex = 2;
			this.buildPathLabel.Text = "Build path:";
			// 
			// gamePathTextBox
			// 
			this.gamePathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gamePathTextBox.Location = new System.Drawing.Point(103, 5);
			this.gamePathTextBox.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
			this.gamePathTextBox.Name = "gamePathTextBox";
			this.gamePathTextBox.Size = new System.Drawing.Size(417, 20);
			this.gamePathTextBox.TabIndex = 3;
			// 
			// buildPathTextBox
			// 
			this.buildPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.buildPathTextBox.Location = new System.Drawing.Point(103, 32);
			this.buildPathTextBox.Name = "buildPathTextBox";
			this.buildPathTextBox.Size = new System.Drawing.Size(417, 20);
			this.buildPathTextBox.TabIndex = 4;
			// 
			// folderBrowserDialog
			// 
			this.folderBrowserDialog.HelpRequest += new System.EventHandler(this.folderBrowserDialog_HelpRequest);
			// 
			// GeneralOptions
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "GeneralOptions";
			this.Size = new System.Drawing.Size(553, 150);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Button gamePathButton;
		private System.Windows.Forms.Label gamePathLabel;
		private System.Windows.Forms.Label buildPathLabel;
		private System.Windows.Forms.TextBox gamePathTextBox;
		private System.Windows.Forms.TextBox buildPathTextBox;
		private System.Windows.Forms.Button buildPathButton;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
	}
}
