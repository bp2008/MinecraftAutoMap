using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MinecraftAM.MinecraftAM.AutoUpdate
{
	public partial class UpdateInfoForm : Form
	{
		public string currentVersion = "Unknown", availableVersion = "Unknown", website = "Unknown", updateInfo = "Unknown";
		public UpdateInfoForm(string currentVersion, string availableVersion, string website, string updateInfo)
		{
			this.currentVersion = currentVersion;
			this.availableVersion = availableVersion;
			this.website = website;
			this.updateInfo = updateInfo;
			InitializeComponent();
		}

		private void RefreshContent()
		{
			lblVersionCurrent.Text = currentVersion;
			lblVersionAvailable.Text = availableVersion;
			txtWebsite.Text = website;
			txtUpdateInfo.Text = updateInfo;
		}
		private void UpdateInfoForm_Load(object sender, EventArgs e)
		{
			RefreshContent();
		}
		private void UpdateInfoForm_VisibleChanged(object sender, EventArgs e)
		{
			RefreshContent();
		}

		private void btnNotThisUpdate_Click(object sender, EventArgs e)
		{
			AMSettings.sVersionToHide = availableVersion;
			this.Close();
		}

		private void btnNeverCheck_Click(object sender, EventArgs e)
		{
			AMSettings.bDisableAutomaticUpdateChecking = true;
			this.Close();
		}
	}
}
