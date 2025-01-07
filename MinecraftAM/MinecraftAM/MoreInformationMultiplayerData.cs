using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MinecraftAM
{
	public partial class MoreInformationMultiplayerData : Form
	{
		private bool wasOpenedWhileAnotherWasOpen = false;
		public static bool isOpen = false;
		Point MouseOffset = new Point(0, 0);
		bool dragging = false;
		public MoreInformationMultiplayerData()
		{
			InitializeComponent();
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void MoreInformationMultiplayerData_MouseDown(object sender, MouseEventArgs e)
		{
			MouseOffset = e.Location;
			dragging = true;
		}

		private void MoreInformationMultiplayerData_MouseLeave(object sender, EventArgs e)
		{
			dragging = false;
		}

		private void MoreInformationMultiplayerData_MouseMove(object sender, MouseEventArgs e)
		{
			if (dragging)
			{
                this.Left += e.X - MouseOffset.X;
                this.Top += e.Y - MouseOffset.Y;
			}
		}

		private void MoreInformationMultiplayerData_MouseUp(object sender, MouseEventArgs e)
		{
			dragging = false;
		}

		private void MoreInformationMultiplayerData_Load(object sender, EventArgs e)
		{
			if (isOpen)
			{
				wasOpenedWhileAnotherWasOpen = true;
				this.Close();
				return;
			}
			isOpen = true;
		}

		private void MoreInformationMultiplayerData_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (!wasOpenedWhileAnotherWasOpen)
				isOpen = false;
		}
	}
}
