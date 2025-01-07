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
	public partial class InputBox : Form
	{
		private static object InputWndCtrLock = new object();
		private static int InputWindowsOpenInternal = 0;
		public static int InputWindowsOpen
		{
			get
			{
				lock (InputWndCtrLock)
				{
					return InputWindowsOpenInternal;
				}
			}
		}
		private InputResult IR;
		#region InputBoxClosed Event
		public delegate void InputBoxClosedHandler(InputResult Result);
		public event InputBoxClosedHandler InputBoxClosed;
		private void RaiseClosed()
		{
			if (InputBoxClosed != null)
				InputBoxClosed(IR);
		}
		#endregion
		/// <summary>
		/// Opens an input box with the specified Name and Prompt Text.  When the user closes the box, the InputBoxClosed event is fired and an InputResult object is provided containing the string that was entered and a boolean value determining whether the input was accepted or cancelled.
		/// </summary>
		/// <param name="boxName">The name to put in the title bar of the box.  A small amount of text will fit here.</param>
		/// <param name="promptText">The string to place in the label above the input box.  A small amount of text will fit here.</param>
		public InputBox(string boxName, string promptText)
		{
			InitializeComponent();
			this.Text = boxName;
			lblPrompt.Text = promptText;
			IR = new InputResult();
			this.VisibleChanged += new EventHandler(InputBox_VisibleChanged);
			this.FormClosed += new FormClosedEventHandler(InputBox_FormClosed);
		}

		void InputBox_FormClosed(object sender, FormClosedEventArgs e)
		{
			lock (InputWndCtrLock)
			{
				InputWindowsOpenInternal--;
			}
		}

		void InputBox_VisibleChanged(object sender, EventArgs e)
		{
			lock (InputWndCtrLock)
			{
				if (this.Visible)
					InputWindowsOpenInternal++;
			}
		}
		public static void TestFn()
		{
		}
		/// <summary>
		/// Opens an input box with the specified Name and Prompt Text.  When the user closes the box, the specified InputBoxClosedHandler is called.
		/// </summary>
		/// <param name="boxName">The name to put in the title bar of the box.  A small amount of text will fit here.</param>
		/// <param name="promptText">The string to place in the label above the input box.  A small amount of text will fit here.</param>
		/// <param name="closeHandler">The InputBoxClosedHandler to call when the input box is closed.</param>
		public static void Show(string boxName, string promptText, InputBoxClosedHandler closeHandler)
		{
			InputBox ib = new InputBox(boxName, promptText);
			ib.InputBoxClosed += closeHandler;
			ib.Show();
		}
		private void btnOk_Click(object sender, EventArgs e)
		{
			IR.Input = txtInput.Text;
			RaiseClosed();
			this.Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			IR.WasCancelled = true;
			RaiseClosed();
			this.Close();
		}
	}
	public class InputResult
	{
		public string Input = "";
		public bool WasCancelled = false;
	}
}
