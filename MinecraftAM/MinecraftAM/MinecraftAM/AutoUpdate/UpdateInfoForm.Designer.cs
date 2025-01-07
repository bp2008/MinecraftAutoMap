namespace MinecraftAM.MinecraftAM.AutoUpdate
{
	partial class UpdateInfoForm
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
			this.txtUpdateInfo = new System.Windows.Forms.TextBox();
			this.lblVersionCurrent = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.lblVersionAvailable = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.txtWebsite = new System.Windows.Forms.TextBox();
			this.btnNotThisUpdate = new System.Windows.Forms.Button();
			this.btnNeverCheck = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.label4 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// txtUpdateInfo
			// 
			this.txtUpdateInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtUpdateInfo.BackColor = System.Drawing.SystemColors.Window;
			this.txtUpdateInfo.Location = new System.Drawing.Point(12, 75);
			this.txtUpdateInfo.Multiline = true;
			this.txtUpdateInfo.Name = "txtUpdateInfo";
			this.txtUpdateInfo.ReadOnly = true;
			this.txtUpdateInfo.Size = new System.Drawing.Size(440, 222);
			this.txtUpdateInfo.TabIndex = 1;
			// 
			// lblVersionCurrent
			// 
			this.lblVersionCurrent.AutoSize = true;
			this.lblVersionCurrent.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblVersionCurrent.Location = new System.Drawing.Point(154, 7);
			this.lblVersionCurrent.Name = "lblVersionCurrent";
			this.lblVersionCurrent.Size = new System.Drawing.Size(63, 16);
			this.lblVersionCurrent.TabIndex = 1;
			this.lblVersionCurrent.Text = "Unknown";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(33, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(115, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Your AutoMap version:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 27);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(136, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Available AutoMap version:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// lblVersionAvailable
			// 
			this.lblVersionAvailable.AutoSize = true;
			this.lblVersionAvailable.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblVersionAvailable.Location = new System.Drawing.Point(154, 25);
			this.lblVersionAvailable.Name = "lblVersionAvailable";
			this.lblVersionAvailable.Size = new System.Drawing.Size(70, 16);
			this.lblVersionAvailable.TabIndex = 4;
			this.lblVersionAvailable.Text = "Unknown";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(58, 50);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(90, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Website Address:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// txtWebsite
			// 
			this.txtWebsite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtWebsite.BackColor = System.Drawing.SystemColors.Window;
			this.txtWebsite.Location = new System.Drawing.Point(154, 47);
			this.txtWebsite.Name = "txtWebsite";
			this.txtWebsite.ReadOnly = true;
			this.txtWebsite.Size = new System.Drawing.Size(298, 20);
			this.txtWebsite.TabIndex = 0;
			// 
			// btnNotThisUpdate
			// 
			this.btnNotThisUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnNotThisUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnNotThisUpdate.Location = new System.Drawing.Point(12, 303);
			this.btnNotThisUpdate.Name = "btnNotThisUpdate";
			this.btnNotThisUpdate.Size = new System.Drawing.Size(271, 28);
			this.btnNotThisUpdate.TabIndex = 6;
			this.btnNotThisUpdate.Text = "Do not tell me about this update again.";
			this.toolTip1.SetToolTip(this.btnNotThisUpdate, "If you want to skip this update but still receive notifications of future updates" +
					", click this button.");
			this.btnNotThisUpdate.UseVisualStyleBackColor = true;
			this.btnNotThisUpdate.Click += new System.EventHandler(this.btnNotThisUpdate_Click);
			// 
			// btnNeverCheck
			// 
			this.btnNeverCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnNeverCheck.Location = new System.Drawing.Point(289, 303);
			this.btnNeverCheck.Name = "btnNeverCheck";
			this.btnNeverCheck.Size = new System.Drawing.Size(163, 28);
			this.btnNeverCheck.TabIndex = 7;
			this.btnNeverCheck.Text = "Never check for updates.";
			this.btnNeverCheck.UseVisualStyleBackColor = true;
			this.btnNeverCheck.Click += new System.EventHandler(this.btnNeverCheck_Click);
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 339);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(343, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = "Note: AutoMap updates must still be downloaded and applied manually.";
			// 
			// UpdateInfoForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(464, 361);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.btnNeverCheck);
			this.Controls.Add(this.btnNotThisUpdate);
			this.Controls.Add(this.txtWebsite);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.lblVersionAvailable);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lblVersionCurrent);
			this.Controls.Add(this.txtUpdateInfo);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "UpdateInfoForm";
			this.Text = "An AutoMap update is available online.";
			this.Load += new System.EventHandler(this.UpdateInfoForm_Load);
			this.VisibleChanged += new System.EventHandler(this.UpdateInfoForm_VisibleChanged);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtUpdateInfo;
		private System.Windows.Forms.Label lblVersionCurrent;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label lblVersionAvailable;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtWebsite;
		private System.Windows.Forms.Button btnNotThisUpdate;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button btnNeverCheck;
		private System.Windows.Forms.Label label4;
	}
}