using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataSource;

namespace MinecraftAM
{
	public partial class WaypointEditor : Form
	{
		private bool hasListeners = false;
		private int currentIndex = -1;
		private WaypointManager.WaypointAddedEventHandler waeh;
		private EventHandler formValuesChangedHandler;
		private void ChangeListeners(bool AddListeners)
		{
			if (AddListeners)
			{
				if (!hasListeners)
				{
					this.nudZ.ValueChanged += formValuesChangedHandler;
					this.nudY.ValueChanged += formValuesChangedHandler;
					this.nudX.ValueChanged += formValuesChangedHandler;
					this.txtName.TextChanged += formValuesChangedHandler;
					this.txtColor.TextChanged += formValuesChangedHandler;
					hasListeners = true;
				}
			}
			else
			{
				if (hasListeners)
				{
					this.nudZ.ValueChanged -= formValuesChangedHandler;
					this.nudY.ValueChanged -= formValuesChangedHandler;
					this.nudX.ValueChanged -= formValuesChangedHandler;
					this.txtName.TextChanged -= formValuesChangedHandler;
					this.txtColor.TextChanged -= formValuesChangedHandler;
					hasListeners = false;
				}
			}
		}
		public WaypointEditor()
		{
			formValuesChangedHandler = new EventHandler(FormValuesChanged);
			waeh = new WaypointManager.WaypointAddedEventHandler(WaypointManager_WaypointAdded);
			InitializeComponent();
		}

		private void WaypointEditor_Load(object sender, EventArgs e)
		{
			ChangeListeners(true);
			WaypointManager.WaypointAdded += waeh;
			ReloadListBox();
		}

		private void WaypointManager_WaypointAdded(Entity waypoint)
		{
			ReloadListBox();
			lbWaypoints.SelectedIndex = WaypointManager.lWaypoints.IndexOf(waypoint);
		}

		public void ReloadListBox()
		{
			lblCurrentWorld.Text = "Currently Loaded World: " + Globals.worldID + " / " + Globals.worldName;
			lbWaypoints.Items.Clear();
			lbWaypoints.Items.AddRange(WaypointManager.lWaypoints.ToArray<object>());
			if (lbWaypoints.Items.Count > 0)
				lbWaypoints.SelectedIndex = 0;
		}

		private void lbWaypoints_SelectedIndexChanged(object sender, EventArgs e)
		{
			LoadFormValues(lbWaypoints.SelectedIndex);
		}

		private void SaveFormValues()
		{
			if (currentIndex < 0 || currentIndex >= WaypointManager.lWaypoints.Count)
				return;
			Entity wp = WaypointManager.lWaypoints[currentIndex];
			wp.name = txtName.Text;
			Microsoft.Xna.Framework.Vector3 coords = CoordinateTranslator.translateToAMCoordsAccordingToSettings(new Microsoft.Xna.Framework.Vector3((float)nudX.Value, (float)nudY.Value, (float)nudZ.Value));
			wp.x = (double)coords.X;
			wp.y = (double)coords.Y;
			wp.z = (double)coords.Z;
			wp.color = AMSettings.GetColor(txtColor.Text, Microsoft.Xna.Framework.Color.Orange);
			pWaypointColor.BackColor = XnaColorToDrawingColor(wp.color);
			wp.Calc();
		}

		private Color XnaColorToDrawingColor(Microsoft.Xna.Framework.Color color)
		{
			return Color.FromArgb(color.A, color.R, color.G, color.B);
		}

		private void LoadFormValues(int index)
		{
			currentIndex = index;
			if (index < 0 || index >= WaypointManager.lWaypoints.Count)
				return;
			Entity wp = WaypointManager.lWaypoints[index];
			ChangeListeners(false);
			txtName.Text = wp.name;
			txtColor.Text = AMSettings.FromColor(wp.color);
			pWaypointColor.BackColor = XnaColorToDrawingColor(wp.color);
			Microsoft.Xna.Framework.Vector3 coords = CoordinateTranslator.translateFromAMCoordsAccordingToSettings(new Microsoft.Xna.Framework.Vector3((float)wp.x, (float)wp.y, (float)wp.z));
			nudX.Value = (decimal)coords.X;
			nudY.Value = (decimal)coords.Y;
			nudZ.Value = (decimal)coords.Z;
			ChangeListeners(true);
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			WaypointManager.NewWaypoint();
		}

		private void btnRemoveThis_Click(object sender, EventArgs e)
		{
			if (currentIndex < 0 || currentIndex >= WaypointManager.lWaypoints.Count)
			{
				MessageBox.Show("Unable to remove waypoint!  Please try again.");
			}
			else
			{
				DialogResult dr = MessageBox.Show("Are you sure?", "Are you sure?", MessageBoxButtons.YesNo);
				if (dr == DialogResult.Yes)
				{
					WaypointManager.lWaypoints.RemoveAt(currentIndex);
				}
			}
			ReloadListBox();
		}

		private void WaypointEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			WaypointManager.WaypointAdded -= waeh;
			WaypointManager.Save();
		}

		private void FormValuesChanged(object sender, EventArgs e)
		{
			SaveFormValues();
			if (sender == txtName)
			{
				try
				{
					lbWaypoints.Items[currentIndex] = txtName.Text;
				}
				catch (Exception) { }
			}
		}

		private void btnMoveToMe_Click(object sender, EventArgs e)
		{
			if (currentIndex < 0 || currentIndex >= WaypointManager.lWaypoints.Count || Globals.userPlayer == null)
				return;
			Entity wp = WaypointManager.lWaypoints[currentIndex];
			ChangeListeners(false);
			Microsoft.Xna.Framework.Vector3 coords = CoordinateTranslator.translateFromAMCoordsAccordingToSettings(new Microsoft.Xna.Framework.Vector3((float)Globals.userPlayer.x, (float)Globals.userPlayer.y, (float)Globals.userPlayer.z));
			nudX.Value = (decimal)coords.X;
			nudY.Value = (decimal)coords.Y;
			nudZ.Value = (decimal)coords.Z;
			ChangeListeners(true);
			SaveFormValues();
		}

		private void btnTeleportMeToWaypoint_Click(object sender, EventArgs e)
		{
			if (currentIndex < 0 || currentIndex >= WaypointManager.lWaypoints.Count || Globals.userPlayer == null)
				return;
			Entity wp = WaypointManager.lWaypoints[currentIndex];
			PowerToys.TryTeleport((float)wp.x, (float)wp.y, (float)wp.z);
		}
	}
}
