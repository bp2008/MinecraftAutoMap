namespace MinecraftAM
{
	partial class MoreInformationMultiplayerData
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MoreInformationMultiplayerData));
			this.label1 = new System.Windows.Forms.Label();
			this.btnClose = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(7, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(547, 295);
			this.label1.TabIndex = 0;
			this.label1.Text = resources.GetString("label1.Text");
			this.label1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MoreInformationMultiplayerData_MouseDown);
			this.label1.MouseLeave += new System.EventHandler(this.MoreInformationMultiplayerData_MouseLeave);
			this.label1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MoreInformationMultiplayerData_MouseMove);
			this.label1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MoreInformationMultiplayerData_MouseUp);
			// 
			// btnClose
			// 
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnClose.Location = new System.Drawing.Point(16, 311);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(533, 79);
			this.btnClose.TabIndex = 1;
			this.btnClose.Text = "I will not complain when I accidentally wipe out my multiplayer data.";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// MoreInformationMultiplayerData
			// 
			this.AcceptButton = this.btnClose;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.CancelButton = this.btnClose;
			this.ClientSize = new System.Drawing.Size(561, 402);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "MoreInformationMultiplayerData";
			this.Text = "MoreInformationMultiplayerData";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MoreInformationMultiplayerData_FormClosed);
			this.Load += new System.EventHandler(this.MoreInformationMultiplayerData_Load);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MoreInformationMultiplayerData_MouseDown);
			this.MouseLeave += new System.EventHandler(this.MoreInformationMultiplayerData_MouseLeave);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MoreInformationMultiplayerData_MouseMove);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MoreInformationMultiplayerData_MouseUp);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnClose;
	}
}