using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Ionic.Zip;

namespace MinecraftAM
{
	public partial class PatchDialog : Form
	{
		public bool WasSuccessful = false;
		public PatchDialog()
		{
			InitializeComponent();
			DirectoryInfo diAppData = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
			txtMinecraftJar.Text = diAppData.FullName + "\\.minecraft\\versions\\";
		}

		private void btnDoPatch_Click(object sender, EventArgs e)
		{
			FileInfo fiJar = new FileInfo(txtMinecraftJar.Text);
			if (!fiJar.Exists)
			{
				AppendLine("EXTRACT FAILED: Could not find the specified jar file.");
				return;
			}
			DirectoryInfo diAppData = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
			FileInfo exe = new FileInfo(Application.ExecutablePath);
			// Step 1.  Copy AutoMap java classes.
			//DirectoryInfo mod_automap = new DirectoryInfo(exe.Directory.FullName + "\\mod_automap");
			//Util.DirectoryCopy(mod_automap.FullName, fiJar.Directory.FullName + "\\mods\\", false);
			//AppendLine("Copied AutoMap Java classes.");

			// Step 2.  Extract Textures
			if (!Directory.Exists("tex/blocks"))
				Directory.CreateDirectory("tex/blocks");
			if (!Directory.Exists("tex/items"))
				Directory.CreateDirectory("tex/items");
			ZipFile zFile = ZipFile.Read(fiJar.FullName);
			string outputFileName = "";
			int count = 0;
			foreach (ZipEntry entry in zFile.Entries)
			{
				if (entry.FileName.StartsWith("assets/minecraft/textures/blocks/"))
					outputFileName = "tex/blocks/" + entry.FileName.Substring("assets/minecraft/textures/blocks/".Length);
				else if (entry.FileName.StartsWith("assets/minecraft/textures/items"))
					outputFileName = "tex/items/" + entry.FileName.Substring("assets/minecraft/textures/items/".Length);
				else
					continue;
				using (FileStream fs = new FileStream(outputFileName, FileMode.Create, FileAccess.ReadWrite))
				{
					entry.Extract(fs);
				}
				count++;
			}
			AppendLine("Extracted " + count + " texture files.");
			if (count < 1)
			{
				AppendLine("EXTRACT FAILED: Could not extract the texture files. Did you select a minecraft jar file?");
				return;
			}
			AMSettings.sMinecraftJar = fiJar.FullName;
			AppendLine("Extract successful.  You may now close this window.");
			WasSuccessful = true;
		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			DirectoryInfo diAppData = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
			openFileDialog1.InitialDirectory = diAppData.FullName + "\\.minecraft\\versions\\";
			if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				txtMinecraftJar.Text = openFileDialog1.FileName;
		}

		private void AppendLine(string line)
		{
			AppendText(line + Environment.NewLine);
		}
		private void AppendText(string text)
		{
			txtOut.AppendText(text);
			txtOut.SelectionStart = txtOut.TextLength - 1;
			txtOut.SelectionLength = 0;
		}

		private void PatchDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (WasSuccessful)
				AMSettings.Save();
		}
	}
}
