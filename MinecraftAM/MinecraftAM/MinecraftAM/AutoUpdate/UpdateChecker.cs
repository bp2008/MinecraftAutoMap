using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;

namespace MinecraftAM.MinecraftAM.AutoUpdate
{
	public static class UpdateChecker
	{
		private static object checkLock = new object();
		private static bool haveCheckedSinceStartup;
		private static Thread UpdateCheckThread;
		public static void CheckForUpdates()
		{
			lock (checkLock)
			{
				if (haveCheckedSinceStartup)
					return;
				haveCheckedSinceStartup = true;
			}
			if (AMSettings.bDisableAutomaticUpdateChecking) return;
			UpdateCheckThread = new Thread(UpdateCheck);
			UpdateCheckThread.Name = "Update Check Thread";
			UpdateCheckThread.Start();
		}
		private static void UpdateCheck()
		{
			try
			{
				WebClient client = new WebClient();
				client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
				client.DownloadStringAsync(new Uri(AMSettings.sAutoUpdateCheckUrl));
			}
			catch (Exception) { }
		}

		private static void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
		{
			try
			{
				ParseUpdateString(e.Result);
			}
			catch (Exception) { }
		}

		private static void ParseUpdateString(string str)
		{
			try
			{
				if (string.IsNullOrEmpty(str)) return;
				int idxEndVersion1 = str.IndexOf('\r');
				int idxEndVersion2 = str.IndexOf('\n');
				if (idxEndVersion1 == -1)
					idxEndVersion1 = idxEndVersion2;
				if (idxEndVersion1 == -1)
					return;
				int idxEndVersion = Math.Min(idxEndVersion1, idxEndVersion2);
				int idxStartDescription = Math.Max(idxEndVersion1, idxEndVersion2) + 1;
				string version = "Unknown";
				string description = "No update description.";
				if (str.Length >= idxEndVersion)
					version = str.Substring(0, idxEndVersion).Trim();
				if (str.Length >= idxStartDescription)
					description = str.Substring(idxStartDescription).Trim();
				ShowUpdateAvailable(version, description);
			}
			catch (Exception) { }
		}
		private static void ShowUpdateAvailable(string sVersionAvailable, string sUpdateInfo)
		{
			if (AMSettings.bDisableAutomaticUpdateChecking || AMSettings.sVersionToHide.CompareTo(sVersionAvailable) >= 0 || AMSettings.sVersionCurrent.CompareTo(sVersionAvailable) >= 0)
				return;
			try
			{
				UpdateInfoForm uif = new UpdateInfoForm(AMSettings.sVersionCurrent, sVersionAvailable, AMSettings.sWebsiteAddress, sUpdateInfo);
				uif.ShowDialog();
			}
			catch (Exception) { }
		}
	}
}
