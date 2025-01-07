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
	public partial class Options : Form
	{
		public Options()
		{
			InitializeComponent();
		}

		private void Options_Load(object sender, EventArgs e)
		{
			ApplyToUI();
			timerHalfSecond_Tick(null, null);
		}

		public void ApplyToUI()
		{
			nudProcessPriority.Value = (int)AMSettings.ProcessPriority;
			if (AMSettings.iDynamicMapMaxX > nudDynamicMapMaxWidth.Maximum)
				AMSettings.iDynamicMapMaxX = (int)nudDynamicMapMaxWidth.Maximum;
			if (AMSettings.iDynamicMapMinX < nudDynamicMapMinWidth.Minimum)
				AMSettings.iDynamicMapMinX = (int)nudDynamicMapMinWidth.Minimum;
			nudDynamicMapMaxWidth.Value = (int)AMSettings.iDynamicMapMaxX;
			nudDynamicMapMinWidth.Value = (int)AMSettings.iDynamicMapMinX;
			cbLava.Checked = AMSettings.bLavaDetection;
			cbWater.Checked = AMSettings.bWaterDetection;
			cbOreDetection.Checked = AMSettings.bOreDetection;
			cbStatusText.Checked = AMSettings.bShowStatusText;
			cbHotkeys.Checked = AMSettings.bShowHotkeys;
			cbShowRealtimeMap.Checked = AMSettings.bDynamicMapEnabled;
			cbHideMapZoomed.Checked = AMSettings.bDynamicMapHiddenZoomedOut;
			txtLava.Text = AMSettings.sLavaDetection;
			txtWater.Text = AMSettings.sWaterDetection;
			txtOre.Text = AMSettings.sOreDetection;
			txtStatusText.Text = AMSettings.sShowStatusText;
			txtHotkeys.Text = AMSettings.sShowHotkeys;
			txtShowRealtimeMap.Text = AMSettings.sDynamicMapEnabled;
			txtShowOptions.Text = AMSettings.sShowOptions;
			txtCI.Text = AMSettings.sCI;
			txtCD.Text = AMSettings.sCD;
			txtCR.Text = AMSettings.sCR;
			txtRotate.Text = AMSettings.sRotate;
			txtShowCompass.Text = AMSettings.sCompass;
			txtStick.Text = AMSettings.sStick;
			nudIDD.Value = AMSettings.iIndoorDepth;
			nudOCH.Value = AMSettings.iOutdoorCeiling;
			nudMaxCeilingHeight.Value = AMSettings.iMaxCeilingHeight;
			nudODD.Value = AMSettings.iOutdoorDepth;
			cbRotate.Checked = AMSettings.bRotate;
			cbShowCompass.Checked = AMSettings.bCompass;
			cbGlowingPath.Checked = AMSettings.bGlowingPath;
			txtGlowingPath.Text = AMSettings.sGlowingPath;
			nudGlowingIntensity.Value = AMSettings.iGlowingIntensity;
			txtClearGlowingPath.Text = AMSettings.sClearGlowingPath;
			txtResetRealtimeMap.Text = AMSettings.sResetRealtimeMap;
			txtCBG.Text = AMSettings.FromColor(AMSettings.cBackgroundColor);
			txtCGC.Text = AMSettings.FromColor(AMSettings.cNotDrawnBlockColor);
			nudDegMapRot.Value = AMSettings.iDefaultMapRotation;
			cbShowHostileNPCs.Checked = AMSettings.bShowHostileNPCs;
			txtShowHostileNPCs.Text = AMSettings.sShowHostileNPCs;
			cbShowPassiveNPCs.Checked = AMSettings.bShowPassiveNPCs;
			txtShowPassiveNPCs.Text = AMSettings.sShowPassiveNPCs;
			nudCreeperProximityAlert.Value = AMSettings.iCreeperProximityAlert;
			cbHideDistantNPCs.Checked = AMSettings.bHideDistantNPCs;
			cbShowWaypoints.Checked = AMSettings.bShowWaypoints;
			txtShowWaypoints.Text = AMSettings.sShowWaypoints;
			txtNewWaypoint.Text = AMSettings.sNewWaypoint;
			cbDrawWaypointLines.Checked = AMSettings.bDrawWaypointLines;
			txtDrawWaypointLines.Text = AMSettings.sDrawWaypointLines;
			txtNPCFilter.Text = AMSettings.sNPCFilter;
			txtItemFilter.Text = AMSettings.sItemFilter;
			nudDroppedItemVisibility.Value = AMSettings.iDroppedItemVisibility;
			nudProjectileVisibility.Value = AMSettings.iProjectileVisibility;
			nudVehicleVisibility.Value = AMSettings.iVehicleVisibility;
			nudStaticMapUpdateInterval.Value = AMSettings.iStaticMapUpdateInterval;
			cbDisableStreamingOnStaticMap.Checked = AMSettings.bDisableStaticMapStreaming;
			cbDisableStaticMap.Checked = AMSettings.bDisableStaticMap;
			cbShowPaintings.Checked = AMSettings.bDrawPaintings;
			cbShowOreHeight.Checked = AMSettings.bShowOreHeightRelative;
			txtShowOreHeight.Text = AMSettings.sShowOreHeightRelative;
			cbAutoUpdateCheck.Checked = !AMSettings.bDisableAutomaticUpdateChecking;
			cbShowIngameLighting.Checked = AMSettings.bShowIngameLighting;
			nudWarningLight.Value = AMSettings.iWarningLightLevel;
			txtOreList.Text = AMSettings.sOresList;
			txtMarkBlocks.Text = AMSettings.sMarkBlocks;
			if (AMSettings.iStaticMapBitsPerPixel == 16)
				rb16bpp.Checked = true;
			else if (AMSettings.iStaticMapBitsPerPixel == 24)
				rb24bpp.Checked = true;
			else if (AMSettings.iStaticMapBitsPerPixel == 32)
				rb32bpp.Checked = true;
			cbTeleportProtection.Checked = AMSettings.bSMPTeleportProtection;
			cbPowerToysCombo.Items.Clear();
			cbPowerToysCombo.Items.AddRange(Enum.GetNames(typeof(MiddleClickOptions)));
			for (int i = 0; i < cbPowerToysCombo.Items.Count; i++)
			{
				if ((string)cbPowerToysCombo.Items[i] == Enum.GetName(typeof(MiddleClickOptions), AMSettings.MiddleClickFunction))
				{
					cbPowerToysCombo.SelectedIndex = i;
					break;
				}
			}
			nudExplosionPower.Value = (decimal)AMSettings.fExplosionPower;
			HandleWarnings();
			cbMinecraftCoordinateMode.Checked = AMSettings.bMinecraftCoordinateDisplay;
			if (AMSettings.iStaticMapGeneration == 2)
				rbSM2G.Checked = true;
			else
				rbSM3G.Checked = true;
			txtMultiplayerFeaturesDisabled.Text = DataSource.PermissionManager.GetDisabledFeaturesString();
			nudMaxTransparentLiquids.Value = AMSettings.iTransparentLiquidMax;
			nudNetherStaticMapHeight.Value = AMSettings.iNetherStaticMapHeightStart;
		}

		private void btnDone_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void nudProcessPriority_ValueChanged(object sender, EventArgs e)
		{
			AMSettings.ProcessPriority = (AMSettings.Priority)((int)nudProcessPriority.Value);
			Globals.SetPriority();
		}

		private void nudDynamicMapMaxWidth_ValueChanged(object sender, EventArgs e)
		{
			AMSettings.iDynamicMapMaxX = (int)nudDynamicMapMaxWidth.Value;
			nudDynamicMapMinWidth.Maximum = AMSettings.iDynamicMapMaxX;
		}

		private void nudDynamicMapMinWidth_ValueChanged(object sender, EventArgs e)
		{
			AMSettings.iDynamicMapMinX = (int)nudDynamicMapMinWidth.Value;
			nudDynamicMapMaxWidth.Minimum = AMSettings.iDynamicMapMinX;
		}

		private void cbLava_CheckedChanged(object sender, EventArgs e)
		{
			AMSettings.bLavaDetection = cbLava.Checked;
		}

		private void cbWater_CheckedChanged(object sender, EventArgs e)
		{
			AMSettings.bWaterDetection = cbWater.Checked;
		}

		private void cbOreDetection_CheckedChanged(object sender, EventArgs e)
		{
			AMSettings.bOreDetection = cbOreDetection.Checked;
		}

		private void cbStatusText_CheckedChanged(object sender, EventArgs e)
		{
			AMSettings.bShowStatusText = cbStatusText.Checked;
		}

		private void cbHotkeys_CheckedChanged(object sender, EventArgs e)
		{
			AMSettings.bShowHotkeys = cbHotkeys.Checked;
		}

		private void cbShowRealtimeMap_CheckedChanged(object sender, EventArgs e)
		{
			AMSettings.bDynamicMapEnabled = cbShowRealtimeMap.Checked;
		}

		private void txtLava_TextChanged(object sender, EventArgs e)
		{
			AMSettings.sLavaDetection = txtLava.Text.ToUpper();
		}

		private void txtWater_TextChanged(object sender, EventArgs e)
		{
			AMSettings.sWaterDetection = txtWater.Text.ToUpper();
		}

		private void txtOre_TextChanged(object sender, EventArgs e)
		{
			AMSettings.sOreDetection = txtOre.Text.ToUpper();
		}

		private void txtStatusText_TextChanged(object sender, EventArgs e)
		{
			AMSettings.sShowStatusText = txtStatusText.Text.ToUpper();
		}

		private void txtHotkeys_TextChanged(object sender, EventArgs e)
		{
			AMSettings.sShowHotkeys = txtHotkeys.Text.ToUpper();
		}

		private void txtShowRealtimeMap_TextChanged(object sender, EventArgs e)
		{
			AMSettings.sDynamicMapEnabled = txtShowRealtimeMap.Text.ToUpper();
		}

		private void txtShowOptions_TextChanged(object sender, EventArgs e)
		{
			AMSettings.sShowOptions = txtShowOptions.Text.ToUpper();
		}

		private void txtCI_TextChanged(object sender, EventArgs e)
		{
			AMSettings.sCI = txtCI.Text.ToUpper();
		}

		private void txtCD_TextChanged(object sender, EventArgs e)
		{
			AMSettings.sCD = txtCD.Text.ToUpper();
		}

		private void txtCR_TextChanged(object sender, EventArgs e)
		{
			AMSettings.sCR = txtCR.Text.ToUpper();
		}

		private void txtStick_TextChanged(object sender, EventArgs e)
		{
			AMSettings.sStick = txtStick.Text.ToUpper();
		}

		private void cbRotate_CheckedChanged(object sender, EventArgs e)
		{
			AMSettings.bRotate = cbRotate.Checked;
		}

		private void cbShowCompass_CheckedChanged(object sender, EventArgs e)
		{
			AMSettings.bCompass = cbShowCompass.Checked;
		}

		private void txtRotate_TextChanged(object sender, EventArgs e)
		{
			AMSettings.sRotate = txtRotate.Text.ToUpper();
		}

		private void txtShowCompass_TextChanged(object sender, EventArgs e)
		{
			AMSettings.sCompass = txtShowCompass.Text.ToUpper();
		}

		private void nudOCH_ValueChanged(object sender, EventArgs e)
		{
			AMSettings.iOutdoorCeiling = (int)nudOCH.Value;
		}

		private void nudODD_ValueChanged(object sender, EventArgs e)
		{
			AMSettings.iOutdoorDepth = (int)nudODD.Value;
		}

		private void nudIDD_ValueChanged(object sender, EventArgs e)
		{
			AMSettings.iIndoorDepth = (int)nudIDD.Value;
		}

		private void cbGlowingPath_CheckedChanged(object sender, EventArgs e)
		{
			AMSettings.bGlowingPath = cbGlowingPath.Checked;
		}

		private void txtGlowingPath_TextChanged(object sender, EventArgs e)
		{
			AMSettings.sGlowingPath = txtGlowingPath.Text.ToUpper();
		}

		private void nudGlowingIntensity_ValueChanged(object sender, EventArgs e)
		{
			AMSettings.iGlowingIntensity = (int)nudGlowingIntensity.Value;
		}

		private void cbHideMapZoomed_CheckedChanged(object sender, EventArgs e)
		{
			AMSettings.bDynamicMapHiddenZoomedOut = cbHideMapZoomed.Checked;
		}

		private void txtClearGlowingPath_TextChanged(object sender, EventArgs e)
		{
			AMSettings.sClearGlowingPath = txtClearGlowingPath.Text.ToUpper();
		}

		private void txtResetRealtimeMap_TextChanged(object sender, EventArgs e)
		{
			AMSettings.sResetRealtimeMap = txtResetRealtimeMap.Text;
		}

		private void Options_FormClosed(object sender, FormClosedEventArgs e)
		{
			Globals.optionsOpen = false;
			AMSettings.Save();
		}

		private void txtCBG_TextChanged(object sender, EventArgs e)
		{
			AMSettings.cBackgroundColor = AMSettings.GetColor(txtCBG.Text, AMSettings.cBackgroundColor);
		}

		private void txtCGC_TextChanged(object sender, EventArgs e)
		{
			AMSettings.cNotDrawnBlockColor = AMSettings.GetColor(txtCGC.Text, AMSettings.cNotDrawnBlockColor);
		}

		private void nudDegMapRot_ValueChanged(object sender, EventArgs e)
		{
			AMSettings.iDefaultMapRotation = (int)nudDegMapRot.Value;
		}

		private void cbShowHostileNPCs_CheckedChanged(object sender, EventArgs e)
		{
			AMSettings.bShowHostileNPCs = cbShowHostileNPCs.Checked;
		}

		private void txtShowHostileNPCs_TextChanged(object sender, EventArgs e)
		{
			AMSettings.sShowHostileNPCs = txtShowHostileNPCs.Text.ToUpper();
		}

		private void cbShowPassiveNPCs_CheckedChanged(object sender, EventArgs e)
		{
			AMSettings.bShowPassiveNPCs = cbShowPassiveNPCs.Checked;
		}

		private void txtShowPassiveNPCs_TextChanged(object sender, EventArgs e)
		{
			AMSettings.sShowPassiveNPCs = txtShowPassiveNPCs.Text.ToUpper();
		}

		private void cbHideDistantNPCs_CheckedChanged(object sender, EventArgs e)
		{
			AMSettings.bHideDistantNPCs = cbHideDistantNPCs.Checked;
		}

		private void nudCreeperProximityAlert_ValueChanged(object sender, EventArgs e)
		{
			AMSettings.iCreeperProximityAlert = (int)nudCreeperProximityAlert.Value;
		}

		private void cbShowWaypoints_CheckedChanged(object sender, EventArgs e)
		{
			AMSettings.bShowWaypoints = cbShowWaypoints.Checked;
		}

		private void txtShowWaypoints_TextChanged(object sender, EventArgs e)
		{
			AMSettings.sShowWaypoints = txtShowWaypoints.Text.ToUpper();
		}

		private void txtNewWaypoint_TextChanged(object sender, EventArgs e)
		{
			AMSettings.sNewWaypoint = txtNewWaypoint.Text.ToUpper();
		}

		private void btnWaypointEditor_Click(object sender, EventArgs e)
		{
			if (Globals.waypointEditor == null || Globals.waypointEditor.IsDisposed)
				Globals.waypointEditor = new WaypointEditor();
			if (Globals.waypointEditor.Visible)
			{
				Globals.waypointEditor.BringToFront();
				Globals.waypointEditor.Focus();
			}
			else
				Globals.waypointEditor.Show();
		}

		private void cbDrawWaypointLines_CheckedChanged(object sender, EventArgs e)
		{

			AMSettings.bDrawWaypointLines = cbDrawWaypointLines.Checked;
		}

		private void txtDrawWaypointLines_TextChanged(object sender, EventArgs e)
		{
			AMSettings.sDrawWaypointLines = txtDrawWaypointLines.Text.ToUpper();
		}

		private void txtNPCFilter_TextChanged(object sender, EventArgs e)
		{
			AMSettings.sNPCFilter = txtNPCFilter.Text;
		}

		private void nudDroppedItemVisibility_ValueChanged(object sender, EventArgs e)
		{
			AMSettings.iDroppedItemVisibility = (int)nudDroppedItemVisibility.Value;
		}

		private void nudProjectileVisibility_ValueChanged(object sender, EventArgs e)
		{
			AMSettings.iProjectileVisibility = (int)nudProjectileVisibility.Value;
		}

		private void nudVehicleVisibility_ValueChanged(object sender, EventArgs e)
		{
			AMSettings.iVehicleVisibility = (int)nudVehicleVisibility.Value;
		}

		private void btnReparseWorldMap_Click(object sender, EventArgs e)
		{
			if (Globals.mcrLoader.isLoading)
			{
				MessageBox.Show("This operation is already being performed.");
				return;
			}
			else if (AMSettings.bDisableStaticMap)
			{
				MessageBox.Show("You must enable the Static Map before you can refresh it from the world save files.");
				return;
			}
			else if (AMSettings.bDisableStaticMapStreaming)
			{
				MessageBox.Show("You must enable Static Map Streaming before you can refresh it from the world save files.\r\nYou can always disable it again once the static map is the way you like it.");
				return;
			}
			Globals.ParseWorldMap(Globals.worldPath);
		}

		private void btnMoreInformationMultiplayerData_Click(object sender, EventArgs e)
		{
			MoreInformationMultiplayerData frm = new MoreInformationMultiplayerData();
			frm.StartPosition = FormStartPosition.Manual;
			frm.Location = new Point(this.Left, this.Top);
			frm.Show();
		}

		private void cbDisableStreamingOnStaticMap_CheckedChanged(object sender, EventArgs e)
		{
			AMSettings.bDisableStaticMapStreaming = cbDisableStreamingOnStaticMap.Checked;
		}

		private void cbDisableStaticMap_CheckedChanged(object sender, EventArgs e)
		{
			AMSettings.bDisableStaticMap = cbDisableStaticMap.Checked;
		}

		private void cbShowPaintings_CheckedChanged(object sender, EventArgs e)
		{
			AMSettings.bDrawPaintings = cbShowPaintings.Checked;
		}

		private void txtItemFilter_TextChanged(object sender, EventArgs e)
		{
			AMSettings.sItemFilter = txtItemFilter.Text;
		}

		private void btnParseCustomWorldMap_Click(object sender, EventArgs e)
		{
			if (Globals.mcrLoader.isLoading)
			{
				MessageBox.Show("A Static Mmap creation operation is already being performed.");
				return;
			}
			else if (AMSettings.bDisableStaticMap)
			{
				MessageBox.Show("You must enable the Static Map before you can generate one from world save files.");
				return;
			}
			else if (AMSettings.bDisableStaticMapStreaming)
			{
				MessageBox.Show("You must enable Static Map Streaming before you can generate one from world save files.\r\nYou can always disable streaming again once the static map is the way you like it.");
				return;
			}
			MessageBox.Show("Please select the region folder for the world you are CURRENTLY PLAYING IN." + Environment.NewLine + Environment.NewLine + "NOTE: The Nether's region folder is located inside the DIM-1 folder.");
			DialogResult dr = folderBrowserDialog1.ShowDialog();
			if (dr == System.Windows.Forms.DialogResult.OK && !string.IsNullOrEmpty(folderBrowserDialog1.SelectedPath) && System.IO.Directory.Exists(folderBrowserDialog1.SelectedPath))
				Globals.ParseWorldMap(folderBrowserDialog1.SelectedPath, true);
		}

		private void cbShowOreHeight_CheckedChanged(object sender, EventArgs e)
		{
			AMSettings.bShowOreHeightRelative = cbShowOreHeight.Checked;
		}

		private void txtShowOreHeight_TextChanged(object sender, EventArgs e)
		{
			AMSettings.sShowOreHeightRelative = txtShowOreHeight.Text.ToUpper();
		}

		private void cbAutoUpdateCheck_CheckedChanged(object sender, EventArgs e)
		{
			AMSettings.bDisableAutomaticUpdateChecking = !cbAutoUpdateCheck.Checked;
		}

		private void cbShowIngameLighting_CheckedChanged(object sender, EventArgs e)
		{
			AMSettings.bShowIngameLighting = cbShowIngameLighting.Checked;
		}

		private void nudWarningLight_ValueChanged(object sender, EventArgs e)
		{
			AMSettings.iWarningLightLevel = (int)nudWarningLight.Value;
		}

		private void txtOreList_TextChanged(object sender, EventArgs e)
		{
			AMSettings.sOresList = txtOreList.Text;
		}

		private void txtMarkBlocks_TextChanged(object sender, EventArgs e)
		{
			AMSettings.sMarkBlocks = txtMarkBlocks.Text;
		}

		private void nudMaxCeilingHeight_ValueChanged(object sender, EventArgs e)
		{
			AMSettings.iMaxCeilingHeight = (int)nudMaxCeilingHeight.Value;
		}

		private void rb16bpp_CheckedChanged(object sender, EventArgs e)
		{
			SetStaticMapBitsPerPixel();
		}

		private void rb24bpp_CheckedChanged(object sender, EventArgs e)
		{
			SetStaticMapBitsPerPixel();
		}

		private void rb32bpp_CheckedChanged(object sender, EventArgs e)
		{
			SetStaticMapBitsPerPixel();
		}

		private void SetStaticMapBitsPerPixel()
		{
			if (rb16bpp.Checked)
				AMSettings.iStaticMapBitsPerPixel = 16;
			else if (rb24bpp.Checked)
				AMSettings.iStaticMapBitsPerPixel = 24;
			else if (rb32bpp.Checked)
				AMSettings.iStaticMapBitsPerPixel = 32;
		}

		private void timerHalfSecond_Tick(object sender, EventArgs e)
		{
			if (Globals.chunks != null && Globals.chunks.collector != null)
				lblSecondsSinceStaticMapUpdate.Text = (int)((DateTime.Now - Globals.staticMapLastLoaded).TotalSeconds) + " seconds since last update.";
		}

		private void nudStaticMapUpdateInterval_ValueChanged(object sender, EventArgs e)
		{
			AMSettings.iStaticMapUpdateInterval = (int)nudStaticMapUpdateInterval.Value;
		}

		private void cbTeleportProtection_CheckedChanged(object sender, EventArgs e)
		{
			AMSettings.bSMPTeleportProtection = cbTeleportProtection.Checked;
		}

		private void cbPowerToysCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
			AMSettings.MiddleClickFunction = (MiddleClickOptions)Enum.Parse(typeof(MiddleClickOptions), cbPowerToysCombo.SelectedItem.ToString());
			switch (AMSettings.MiddleClickFunction)
			{
				case MiddleClickOptions.Teleport:
					txtPowerToyDescription.Text = "Bends the fabric of space and time to teleport you to the location you clicked.  If this location is outside the dynamic map, you will be teleported, and your elevation will be increased by 1.  Chances are, you will appear in midair or inside something solid, so be careful!\r\n\r\nIn Multiplayer, this feature may be strictly forbidden and/or restricted.  It is solely your responsibility to follow the rules when playing on someone else's Minecraft server!";
					break;
				case MiddleClickOptions.Explode:
					txtPowerToyDescription.Text = "Don't like the way that mountain is looking at you?  Want to do something about it?  Crank up the explosion power and do it justice.\r\n\r\nFor comparison:\r\n1.0 will take out about 4-6 blocks.\r\n3.0 is equivalent to a Creeper.\r\n4.0 is TNT\r\n10.0 will put a Creeper to shame and completely level a modest-sized house.\r\n\r\nHigher than 10 gets messy and will not form a decent crater.  Raise the explosion power high enough and you will just corrupt your map and/or crash the game. :(";
					break;
				case MiddleClickOptions.None:
				default:
					txtPowerToyDescription.Text = "Better safe than sorry!";
					break;
			}
		}

		private void nudExplosionPower_ValueChanged(object sender, EventArgs e)
		{
			AMSettings.fExplosionPower = (float)nudExplosionPower.Value;
			HandleWarnings();
		}

		/// <summary>
		/// Handles the display of warning labels, such as ! Bad Idea ! when high explosion power is selected.
		/// </summary>
		private void HandleWarnings()
		{
			if (AMSettings.fExplosionPower >= 20)
				lblBadIdea.Visible = true;
			else
				lblBadIdea.Visible = false;
		}

		private void cbMinecraftCoordinateMode_CheckedChanged(object sender, EventArgs e)
		{
			AMSettings.bMinecraftCoordinateDisplay = cbMinecraftCoordinateMode.Checked;
		}

		private void rbSM2G_CheckedChanged(object sender, EventArgs e)
		{
			SetStaticMapMode();
		}

		private void rbSM3G_CheckedChanged(object sender, EventArgs e)
		{
			SetStaticMapMode();
		}

		private void SetStaticMapMode()
		{
			if (rbSM2G.Checked)
				AMSettings.iStaticMapGeneration = 2;
			else if (rbSM3G.Checked)
				AMSettings.iStaticMapGeneration = 3;
		}

		private void nudMaxTransparentLiquids_ValueChanged(object sender, EventArgs e)
		{
			AMSettings.iTransparentLiquidMax = (int)nudMaxTransparentLiquids.Value;
		}

		private void btnGarbageCollect_Click(object sender, EventArgs e)
		{
			btnGarbageCollect.Text = "Collecting...";
			Application.DoEvents();
			System.GC.Collect();
			btnGarbageCollect.Text = "Garbage Collect";
		}

		private void nudNetherStaticMapHeight_ValueChanged(object sender, EventArgs e)
		{
			AMSettings.iNetherStaticMapHeightStart = (int)nudNetherStaticMapHeight.Value;
		}
	}
}
