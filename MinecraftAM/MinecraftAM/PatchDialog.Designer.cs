namespace MinecraftAM
{
	partial class PatchDialog
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
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.txtMinecraftJar = new System.Windows.Forms.TextBox();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.txtOut = new System.Windows.Forms.TextBox();
			this.btnDoPatch = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// txtMinecraftJar
			// 
			this.txtMinecraftJar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtMinecraftJar.Location = new System.Drawing.Point(12, 58);
			this.txtMinecraftJar.Name = "txtMinecraftJar";
			this.txtMinecraftJar.Size = new System.Drawing.Size(507, 20);
			this.txtMinecraftJar.TabIndex = 0;
			// 
			// btnBrowse
			// 
			this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowse.Location = new System.Drawing.Point(525, 58);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(75, 37);
			this.btnBrowse.TabIndex = 1;
			this.btnBrowse.Text = "Browse";
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(588, 46);
			this.label1.TabIndex = 2;
			this.label1.Text = "Please locate the minecraft JAR file you will be running (for example: 1.7.10-For" +
    "ge10.13.0.1180.jar).";
			// 
			// txtOut
			// 
			this.txtOut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtOut.Location = new System.Drawing.Point(12, 133);
			this.txtOut.Multiline = true;
			this.txtOut.Name = "txtOut";
			this.txtOut.ReadOnly = true;
			this.txtOut.Size = new System.Drawing.Size(588, 196);
			this.txtOut.TabIndex = 3;
			// 
			// btnDoPatch
			// 
			this.btnDoPatch.Location = new System.Drawing.Point(240, 84);
			this.btnDoPatch.Name = "btnDoPatch";
			this.btnDoPatch.Size = new System.Drawing.Size(134, 43);
			this.btnDoPatch.TabIndex = 4;
			this.btnDoPatch.Text = "Extract Textures";
			this.btnDoPatch.UseVisualStyleBackColor = true;
			this.btnDoPatch.Click += new System.EventHandler(this.btnDoPatch_Click);
			// 
			// PatchDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(612, 341);
			this.Controls.Add(this.btnDoPatch);
			this.Controls.Add(this.txtOut);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnBrowse);
			this.Controls.Add(this.txtMinecraftJar);
			this.Name = "PatchDialog";
			this.Text = "AutoMap Texture Extractor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PatchDialog_FormClosing);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.TextBox txtMinecraftJar;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtOut;
		private System.Windows.Forms.Button btnDoPatch;
	}
}