using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
//using System.Windows.Forms;

namespace MinecraftAM
{
	public class InputManager
	{
		protected MouseState msPrevious;
		protected bool bMsPreviousSet;
		protected bool bLeftDown = false;
		protected bool bRightDown = false;
		protected bool bUpDown = false;
		protected bool bDownDown = false;
		protected long lMaxCameraMovement = 50;
		public SortedList<string, bool> keyStates;
		public HashSet<string> keysDetected;
		private DateTime lastDepthChange = DateTime.Now;
		private TimeSpan depthChangeInterval = new TimeSpan(0, 0, 0, 0, 150);
		public Point mousePosition = new Point(0, 0);

		public InputManager()
		{
			keyStates = new SortedList<string, bool>();
			keysDetected = new HashSet<string>();
			msPrevious = new MouseState();
			bMsPreviousSet = false;
			EnforceCameraBounds();
		}
		private void keyIsDown(string key)
		{
			if (keyStates.ContainsKey(key))
			{
				if (!keyStates[key])
					KeyDown(key);
				keyStates[key] = true;
			}
			else
			{
				keyStates.Add(key, true);
				KeyDown(key);
			}
			if (!keysDetected.Contains(key))
				keysDetected.Add(key);
		}
		private void InputComplete()
		{
			for (int i = 0; i < keyStates.Count; i++)
			{
				string key = keyStates.Keys[i];
				if (!keysDetected.Contains(key))
				{
					keyStates[key] = false;
					KeyUp(key);
				}
			}
			keysDetected.Clear();
		}
		public bool InputAvailableForHotkeys
		{
			get
			{
				return (Globals.optWin == null || !Globals.optWin.Visible) && InputBox.InputWindowsOpen == 0 && (Globals.waypointEditor == null || !Globals.waypointEditor.IsDisposed || !Globals.waypointEditor.Visible);
			}
		}
		private void KeyDown(string key)
		{
			if (InputAvailableForHotkeys)
			{
				key = key.ToUpper();
				if (key == AMSettings.sRotate)
					AMSettings.bRotate = !AMSettings.bRotate;
				else if (key == AMSettings.sOreDetection)
					AMSettings.bOreDetection = !AMSettings.bOreDetection;
				else if (key == AMSettings.sShowHotkeys)
					AMSettings.bShowHotkeys = !AMSettings.bShowHotkeys;
				//else if (key == AMSettings.sUpdateStaticTerrain)
				//{
				//    Globals.ParseWorldMap(Globals.worldPath);
				//}
				else if (key == AMSettings.sCI) // Ceiling increase
				{
					//lastDepthChange = lastDepthChange - depthChangeInterval;
				}
				else if (key == AMSettings.sCD) // Ceiling decrease
				{
					//lastDepthChange = lastDepthChange - depthChangeInterval;
				}
				else if (key == AMSettings.sCR)
					Globals.depthOffset = 0;
				else if (key == AMSettings.sStick)
				{
					Globals.panX = 0;
					Globals.panY = 0;
					Globals.lockToPlayer = true;
				}
				else if (key == AMSettings.sShowOptions)
					Globals.ShowOptions();
				else if (key == AMSettings.sShowStatusText)
					AMSettings.bShowStatusText = !AMSettings.bShowStatusText;
				else if (key == AMSettings.sLavaDetection)
					AMSettings.bLavaDetection = !AMSettings.bLavaDetection;
				else if (key == AMSettings.sWaterDetection)
					AMSettings.bWaterDetection = !AMSettings.bWaterDetection;
				else if (key == AMSettings.sDynamicMapEnabled)
					AMSettings.bDynamicMapEnabled = !AMSettings.bDynamicMapEnabled;
				else if (key == AMSettings.sCompass)
					AMSettings.bCompass = !AMSettings.bCompass;
				else if (key == AMSettings.sGlowingPath)
					AMSettings.bGlowingPath = !AMSettings.bGlowingPath;
				else if (key == AMSettings.sClearGlowingPath)
				{
					Globals.currentTrail.active = false;
					Globals.currentTrail = new Trail();
				}
				else if (key == AMSettings.sResetRealtimeMap)
				{
					Globals.chunks.ClearCache();
				}
				else if (key == AMSettings.sShowHostileNPCs)
				{
					AMSettings.bShowHostileNPCs = !AMSettings.bShowHostileNPCs;
				}
				else if (key == AMSettings.sShowPassiveNPCs)
				{
					AMSettings.bShowPassiveNPCs = !AMSettings.bShowPassiveNPCs;
				}
				else if (key == AMSettings.sShowWaypoints)
				{
					AMSettings.bShowWaypoints = !AMSettings.bShowWaypoints;
				}
				else if (key == AMSettings.sNewWaypoint)
				{
					WaypointManager.NewWaypoint();
				}
				else if (key == AMSettings.sDrawWaypointLines)
				{
					AMSettings.bDrawWaypointLines = !AMSettings.bDrawWaypointLines;
				}
				else if (key == AMSettings.sShowOreHeightRelative)
				{
					AMSettings.bShowOreHeightRelative = !AMSettings.bShowOreHeightRelative;
				}
			}
			try
			{
				if (Globals.optWin.Visible)
					Globals.optWin.ApplyToUI();
			}
			catch (Exception)
			{
				Globals.optWin = null;
			}
		}
		private void KeyUp(string key)
		{
		}
		public void HandleInput(Game game, GameTime time)
		{
			HandleKeys(game, time);
			HandleMouse(game, time);
			InputComplete();
		}


		#region Input Handling
		protected void HandleKeys(Game game, GameTime time)
		{
			KeyboardState kbs = Keyboard.GetState();
			Keys[] keys = kbs.GetPressedKeys();
			foreach (Keys key in keys)
			{
				string sKey = GetKeyString(key);
				if (sKey.Length == 1)
					keyIsDown(sKey);

			}
			if (InputAvailableForHotkeys)
			{
				if (keyStates.ContainsKey(AMSettings.sCI) && keyStates[AMSettings.sCI])
				{
					if (DateTime.Now - depthChangeInterval > lastDepthChange)
					{
						Globals.depthOffset++;
						if (Math.Abs(Globals.depthOffset) > Globals.depthOffMax)
							Globals.depthOffset--;
						lastDepthChange = DateTime.Now;
					}
				}
				if (keyStates.ContainsKey(AMSettings.sCD) && keyStates[AMSettings.sCD])
				{
					if (DateTime.Now - depthChangeInterval > lastDepthChange)
					{
						Globals.depthOffset--;
						if (Math.Abs(Globals.depthOffset) > Globals.depthOffMax)
							Globals.depthOffset++;
						lastDepthChange = DateTime.Now;
					}
				}
				HandleArrowKeys(game, time, kbs);
			}
		}

		private string GetKeyString(Keys key)
		{
			if (key == Keys.D0)
				return "0";
			if (key == Keys.D1)
				return "1";
			if (key == Keys.D2)
				return "2";
			if (key == Keys.D3)
				return "3";
			if (key == Keys.D4)
				return "4";
			if (key == Keys.D5)
				return "5";
			if (key == Keys.D6)
				return "6";
			if (key == Keys.D7)
				return "7";
			if (key == Keys.D8)
				return "8";
			if (key == Keys.D9)
				return "9";
			if (key == Keys.OemPlus)
				return "=";
			if (key == Keys.OemMinus)
				return "-";
			if (key == Keys.OemBackslash)
				return "\\";
			if (key == Keys.OemPipe)
				return "|";
			if (key == Keys.OemPeriod)
				return ".";
			if (key == Keys.OemCloseBrackets)
				return "]";
			if (key == Keys.OemOpenBrackets)
				return "[";
			if (key == Keys.OemComma)
				return ",";
			if (key == Keys.OemQuestion)
				return "/";
			if (key == Keys.OemSemicolon)
				return ";";
			if (key == Keys.OemTilde)
				return "~";
			if (key == Keys.OemQuotes)
				return "\'";
			try
			{
				return Enum.GetName(typeof(Keys), key);
			}
			catch (Exception ex)
			{
				ex.GetType();
				return "";
			}
		}

		private void HandleArrowKeys(Game game, GameTime time, KeyboardState kbs)
		{
			HandleDirectionKey(Keys.Up, ref bUpDown, time, kbs);
			HandleDirectionKey(Keys.Down, ref bDownDown, time, kbs);
			HandleDirectionKey(Keys.Left, ref bLeftDown, time, kbs);
			HandleDirectionKey(Keys.Right, ref bRightDown, time, kbs);
		}

		private void HandleDirectionKey(Keys key, ref bool bKeyDown, GameTime time, KeyboardState kbs)
		{
			if (kbs.IsKeyDown(key))
			{
				if (Globals.lockToPlayer)
					Globals.panX = Globals.panY = 0;
				Globals.lockToPlayer = false;
				if (bKeyDown)
					ScrollScreen(key, time);
				else
					bKeyDown = true;
			}
			else
				bKeyDown = false;
		}

		private void ScrollScreen(Keys key, GameTime time)
		{
			switch (key)
			{
				case Keys.Up:
					Globals.panY -= Math.Min(lMaxCameraMovement, time.ElapsedGameTime.Milliseconds * (1 / Globals.camZoom));
					break;
				case Keys.Down:
					Globals.panY += Math.Min(lMaxCameraMovement, time.ElapsedGameTime.Milliseconds * (1 / Globals.camZoom));
					break;
				case Keys.Left:
					Globals.panX -= Math.Min(lMaxCameraMovement, time.ElapsedGameTime.Milliseconds * (1 / Globals.camZoom));
					break;
				case Keys.Right:
					Globals.panX += Math.Min(lMaxCameraMovement, time.ElapsedGameTime.Milliseconds * (1 / Globals.camZoom));
					break;
			}
		}

		private void EnforceCameraBounds()
		{
			Matrix m = Matrix.CreateTranslation(Globals.screenWidth / 2 - Globals.panX, Globals.screenHeight / 2 - Globals.panY, 0);
			m = Matrix.Multiply(m, Matrix.CreateScale(Globals.camZoom));
			m = Matrix.Multiply(m, Matrix.CreateRotationZ(Globals.camRotation));
			Vector2 cam = new Vector2(Globals.panX, Globals.panY);
			cam = Vector2.Transform(cam, m);

			if (Globals.panX / Globals.camZoom < Globals.halfScreenWidth / Globals.camZoom)
				Globals.panX = Globals.halfScreenWidth;
			if (Globals.panY / Globals.camZoom < Globals.halfScreenHeight / Globals.camZoom)
				Globals.panY = Globals.halfScreenHeight;
		}

		protected void ZoomIn(int amount)
		{
			Globals.camZoom += Math.Min(1.5f, Globals.camZoom / 4);
			//if (Globals.camZoom > 3f)
			//    Globals.camZoom += 1f * amount;
			//else if (Globals.camZoom > 1f)
			//    Globals.camZoom += 0.5f * amount;
			//else
			//    for (int i = 0; i < amount; i++)
			//        Globals.camZoom /= 0.875f;
		}
		protected void ZoomOut(int amount)
		{
			Globals.camZoom -= Math.Min(1.5f, Globals.camZoom / 4);
			if (Globals.camZoom < .0001f)
				Globals.camZoom = .0001f;
			//if(Globals.camZoom > 3f)
			//    Globals.camZoom -= 1f * amount;
			//else if (Globals.camZoom > 1f)
			//    Globals.camZoom -= 0.5f * amount;
			//else
			//    for (int i = 0; i < amount; i++)
			//        Globals.camZoom *= 0.875f;
		}


		bool rbIsDown = false;
		bool bLeftMBIsDown = false;
		int wdx, wdy;
		protected void HandleMouse(Game game, GameTime time)
		{
			// Handle Teleport timing for teleport detection protection.
			bool rbDetected = false;
			if (!bMsPreviousSet)
			{
				msPrevious = Mouse.GetState();
				bMsPreviousSet = true;
			}
			MouseState ms = Mouse.GetState();
			mousePosition = new Point(ms.X, ms.Y);
			int dw = ms.ScrollWheelValue - msPrevious.ScrollWheelValue;
			int dx = ms.X - msPrevious.X;
			int dy = ms.Y - msPrevious.Y;

			if (LeftButtonPressed(ms))
			{
				System.Windows.Forms.Form frm = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(game.Window.Handle);

				System.Drawing.Point screenPnt = frm.PointToScreen(new System.Drawing.Point(ms.X, ms.Y));
				bLeftMBIsDown = true;
				wdx = screenPnt.X - frm.Left;
				wdy = screenPnt.Y - frm.Top;
			}
			else if (LeftButtonReleased(ms))
			{
				bLeftMBIsDown = false;
			}
			if (ms.LeftButton == ButtonState.Pressed)
			{
				// Left button down
			}
			if (MiddleButtonPressed(ms))
			{
				if (DataSource.PermissionManager.allowPowerToys && Globals.userPlayer != null && ms.X >= 0 && ms.Y >= 0 && ms.X < game.GraphicsDevice.Viewport.Width && ms.Y < game.GraphicsDevice.Viewport.Height && AMSettings.MiddleClickFunction != MiddleClickOptions.None)
				{
					// Middle button clicked
					PowerToys.HandleMiddleClick(ms);
				}
			}
			else if (MiddleButtonReleased(ms))
			{
				// Middle button released
			}
			if (ms.MiddleButton == ButtonState.Pressed)
			{
				// Middle button down
			}
			if (RightButtonPressed(ms))
			{
				rbDetected = true;
				if (!rbIsDown)
				{

					// Right button clicked
					ToggleMinimapMode(game);
				}
			}
			else if (RightButtonReleased(ms))
			{
				// Right button released
			}
			if (ms.RightButton == ButtonState.Pressed)
			{
				// Right button down
				//Globals.camRotation += Globals.DegreeToRadian(dx);
			}
			if (rbIsDown && !rbDetected)
				rbIsDown = false;
			if (dw > 0)
				ZoomIn(dw / 100);
			else if (dw < 0)
				ZoomOut(Math.Abs(dw / 100));
			//if(M
			msPrevious = ms;

			if (bLeftMBIsDown)
			{
				System.Windows.Forms.Form frm = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(game.Window.Handle);
				if (frm.Focused)
				{
					System.Drawing.Point screenPoint = frm.PointToScreen(new System.Drawing.Point(ms.X, ms.Y));
					frm.Left = AMSettings.iWndX = screenPoint.X - wdx;
					frm.Top = AMSettings.iWndY = screenPoint.Y - wdy;
				}
				else
					bLeftMBIsDown = false;
			}
		}


		private static void ToggleMinimapMode(Game game)
		{
			System.Windows.Forms.Form frm = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(game.Window.Handle);
			if (frm.Focused)
			{
				AMSettings.bBorderlessMode = !AMSettings.bBorderlessMode;
				SetMinimapMode(game, AMSettings.bBorderlessMode);
			}
		}
		public static void SetMinimapMode(Game game, bool borderless)
		{
			System.Windows.Forms.Form frm = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(game.Window.Handle);
			if (borderless)
			{
				frm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
				frm.TopMost = AMSettings.bBorderlessMode = true;
			}
			else
			{
				frm.TopMost = AMSettings.bBorderlessMode = false;
				frm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
			}
		}
		protected bool LeftButtonPressed(MouseState ms)
		{
			return ms.LeftButton == ButtonState.Pressed && msPrevious.LeftButton == ButtonState.Released;
		}
		protected bool MiddleButtonPressed(MouseState ms)
		{
			return ms.MiddleButton == ButtonState.Pressed && msPrevious.MiddleButton == ButtonState.Released;
		}
		protected bool RightButtonPressed(MouseState ms)
		{
			return ms.RightButton == ButtonState.Pressed && msPrevious.RightButton == ButtonState.Released;
		}
		protected bool LeftButtonReleased(MouseState ms)
		{
			return ms.LeftButton == ButtonState.Released && msPrevious.LeftButton == ButtonState.Pressed;
		}
		protected bool MiddleButtonReleased(MouseState ms)
		{
			return ms.MiddleButton == ButtonState.Released && msPrevious.MiddleButton == ButtonState.Pressed;
		}
		protected bool RightButtonReleased(MouseState ms)
		{
			return ms.RightButton == ButtonState.Released && msPrevious.RightButton == ButtonState.Pressed;
		}
		#endregion
	}
}
