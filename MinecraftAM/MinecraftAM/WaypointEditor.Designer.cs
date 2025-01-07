namespace MinecraftAM
{
	partial class WaypointEditor
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
			this.lbWaypoints = new System.Windows.Forms.ListBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.gbCoordinates = new System.Windows.Forms.GroupBox();
			this.btnTeleportMeToWaypoint = new System.Windows.Forms.Button();
			this.btnMoveToMe = new System.Windows.Forms.Button();
			this.nudZ = new System.Windows.Forms.NumericUpDown();
			this.nudY = new System.Windows.Forms.NumericUpDown();
			this.nudX = new System.Windows.Forms.NumericUpDown();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.txtName = new System.Windows.Forms.TextBox();
			this.txtColor = new System.Windows.Forms.TextBox();
			this.pWaypointColor = new System.Windows.Forms.Panel();
			this.lblCurrentWorld = new System.Windows.Forms.Label();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnRemoveThis = new System.Windows.Forms.Button();
			this.gbCoordinates.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudZ)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudY)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudX)).BeginInit();
			this.SuspendLayout();
			// 
			// lbWaypoints
			// 
			this.lbWaypoints.FormattingEnabled = true;
			this.lbWaypoints.Location = new System.Drawing.Point(3, 31);
			this.lbWaypoints.Name = "lbWaypoints";
			this.lbWaypoints.Size = new System.Drawing.Size(220, 329);
			this.lbWaypoints.TabIndex = 0;
			this.lbWaypoints.SelectedIndexChanged += new System.EventHandler(this.lbWaypoints_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(232, 60);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(83, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Waypoint Name";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(232, 104);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(117, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Waypoint Color (R,G,B)";
			// 
			// gbCoordinates
			// 
			this.gbCoordinates.Controls.Add(this.btnTeleportMeToWaypoint);
			this.gbCoordinates.Controls.Add(this.btnMoveToMe);
			this.gbCoordinates.Controls.Add(this.nudZ);
			this.gbCoordinates.Controls.Add(this.nudY);
			this.gbCoordinates.Controls.Add(this.nudX);
			this.gbCoordinates.Controls.Add(this.label6);
			this.gbCoordinates.Controls.Add(this.label5);
			this.gbCoordinates.Controls.Add(this.label4);
			this.gbCoordinates.Location = new System.Drawing.Point(235, 146);
			this.gbCoordinates.Name = "gbCoordinates";
			this.gbCoordinates.Size = new System.Drawing.Size(347, 126);
			this.gbCoordinates.TabIndex = 5;
			this.gbCoordinates.TabStop = false;
			this.gbCoordinates.Text = "Waypoint Coordinates";
			// 
			// btnTeleportMeToWaypoint
			// 
			this.btnTeleportMeToWaypoint.Location = new System.Drawing.Point(176, 96);
			this.btnTeleportMeToWaypoint.Name = "btnTeleportMeToWaypoint";
			this.btnTeleportMeToWaypoint.Size = new System.Drawing.Size(165, 23);
			this.btnTeleportMeToWaypoint.TabIndex = 17;
			this.btnTeleportMeToWaypoint.Text = "Teleport Me to Waypoint";
			this.btnTeleportMeToWaypoint.UseVisualStyleBackColor = true;
			this.btnTeleportMeToWaypoint.Click += new System.EventHandler(this.btnTeleportMeToWaypoint_Click);
			// 
			// btnMoveToMe
			// 
			this.btnMoveToMe.Location = new System.Drawing.Point(6, 96);
			this.btnMoveToMe.Name = "btnMoveToMe";
			this.btnMoveToMe.Size = new System.Drawing.Size(165, 23);
			this.btnMoveToMe.TabIndex = 16;
			this.btnMoveToMe.Text = "Move Waypoint To Me";
			this.btnMoveToMe.UseVisualStyleBackColor = true;
			this.btnMoveToMe.Click += new System.EventHandler(this.btnMoveToMe_Click);
			// 
			// nudZ
			// 
			this.nudZ.DecimalPlaces = 9;
			this.nudZ.Location = new System.Drawing.Point(30, 70);
			this.nudZ.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
			this.nudZ.Minimum = new decimal(new int[] {
            999999999,
            0,
            0,
            -2147483648});
			this.nudZ.Name = "nudZ";
			this.nudZ.Size = new System.Drawing.Size(311, 20);
			this.nudZ.TabIndex = 15;
			// 
			// nudY
			// 
			this.nudY.DecimalPlaces = 9;
			this.nudY.Location = new System.Drawing.Point(30, 44);
			this.nudY.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
			this.nudY.Minimum = new decimal(new int[] {
            999999999,
            0,
            0,
            -2147483648});
			this.nudY.Name = "nudY";
			this.nudY.Size = new System.Drawing.Size(311, 20);
			this.nudY.TabIndex = 14;
			// 
			// nudX
			// 
			this.nudX.DecimalPlaces = 9;
			this.nudX.Location = new System.Drawing.Point(30, 18);
			this.nudX.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
			this.nudX.Minimum = new decimal(new int[] {
            999999999,
            0,
            0,
            -2147483648});
			this.nudX.Name = "nudX";
			this.nudX.Size = new System.Drawing.Size(311, 20);
			this.nudX.TabIndex = 13;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(7, 72);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(17, 13);
			this.label6.TabIndex = 12;
			this.label6.Text = "Z:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(7, 46);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(17, 13);
			this.label5.TabIndex = 10;
			this.label5.Text = "Y:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(7, 20);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(17, 13);
			this.label4.TabIndex = 0;
			this.label4.Text = "X:";
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(235, 77);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(347, 20);
			this.txtName.TabIndex = 6;
			// 
			// txtColor
			// 
			this.txtColor.Location = new System.Drawing.Point(358, 120);
			this.txtColor.Name = "txtColor";
			this.txtColor.Size = new System.Drawing.Size(224, 20);
			this.txtColor.TabIndex = 7;
			// 
			// pWaypointColor
			// 
			this.pWaypointColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
			this.pWaypointColor.Location = new System.Drawing.Point(235, 120);
			this.pWaypointColor.Name = "pWaypointColor";
			this.pWaypointColor.Size = new System.Drawing.Size(117, 20);
			this.pWaypointColor.TabIndex = 8;
			// 
			// lblCurrentWorld
			// 
			this.lblCurrentWorld.AutoSize = true;
			this.lblCurrentWorld.Location = new System.Drawing.Point(12, 9);
			this.lblCurrentWorld.Name = "lblCurrentWorld";
			this.lblCurrentWorld.Size = new System.Drawing.Size(124, 13);
			this.lblCurrentWorld.TabIndex = 11;
			this.lblCurrentWorld.Text = "Currently Loaded World: ";
			// 
			// btnAdd
			// 
			this.btnAdd.Location = new System.Drawing.Point(235, 33);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(116, 23);
			this.btnAdd.TabIndex = 12;
			this.btnAdd.Text = "Add Waypoint";
			this.btnAdd.UseVisualStyleBackColor = true;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnRemoveThis
			// 
			this.btnRemoveThis.Location = new System.Drawing.Point(358, 33);
			this.btnRemoveThis.Name = "btnRemoveThis";
			this.btnRemoveThis.Size = new System.Drawing.Size(158, 23);
			this.btnRemoveThis.TabIndex = 13;
			this.btnRemoveThis.Text = "Remove This Waypoint";
			this.btnRemoveThis.UseVisualStyleBackColor = true;
			this.btnRemoveThis.Click += new System.EventHandler(this.btnRemoveThis_Click);
			// 
			// WaypointEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(594, 364);
			this.Controls.Add(this.btnRemoveThis);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.lblCurrentWorld);
			this.Controls.Add(this.pWaypointColor);
			this.Controls.Add(this.txtColor);
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.gbCoordinates);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.lbWaypoints);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "WaypointEditor";
			this.Text = "Waypoint Editor - Minecraft AutoMap";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WaypointEditor_FormClosing);
			this.Load += new System.EventHandler(this.WaypointEditor_Load);
			this.gbCoordinates.ResumeLayout(false);
			this.gbCoordinates.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudZ)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudY)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudX)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox lbWaypoints;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox gbCoordinates;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.TextBox txtColor;
		private System.Windows.Forms.Panel pWaypointColor;
		private System.Windows.Forms.NumericUpDown nudX;
		private System.Windows.Forms.Label lblCurrentWorld;
		private System.Windows.Forms.NumericUpDown nudZ;
		private System.Windows.Forms.NumericUpDown nudY;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnRemoveThis;
		private System.Windows.Forms.Button btnMoveToMe;
		private System.Windows.Forms.Button btnTeleportMeToWaypoint;
	}
}