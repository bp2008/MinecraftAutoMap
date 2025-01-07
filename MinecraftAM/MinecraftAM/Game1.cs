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
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace MinecraftAM
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game, DataSource.DSHandlerConnectionStatusChanged
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		MAM2DLayer mapLayer;
		Compass compass;
		InputManager iManager;
		SpriteFont fontDebug;

		Texture2D pointerTNT;
		Texture2D pointerPortal;
		//ProtoUI.GUI gui;
		bool showingCursor = true;

		string connectionStatus = "Not Connected";
		public static string lastMiddleClick = "";
		public Game1(bool patch)
		{
			DataSource.DS.InitializeConnectionStatusChanged(this);
			AMSettings.Load();
			if (patch)
				AMSettings.sMinecraftJar = "";
			//MTS.ChunkCache.worldXBound = 250;
			//MTS.ChunkCache.worldYBound = 250;
			DirectoryInfo diAppData = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
			if (string.IsNullOrEmpty(AMSettings.sMinecraftJar))
			{ // Install
				PatchDialog pd = new PatchDialog();
				pd.ShowDialog();
				if (!pd.WasSuccessful)
					this.Exit();
			}
			if (!Directory.Exists("Map Data"))
				Directory.CreateDirectory("Map Data");
			graphics = new GraphicsDeviceManager(this);
			if (AMSettings.iWndW < 10 || AMSettings.iWndW > 5760)
				AMSettings.iWndW = 400;
			if (AMSettings.iWndH < 10 || AMSettings.iWndH > 3600)
				AMSettings.iWndH = 30;
			graphics.PreferredBackBufferWidth = AMSettings.iWndW;
			graphics.PreferredBackBufferHeight = AMSettings.iWndH;
			this.Window.AllowUserResizing = true;
			this.Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);
			this.Window.Title = "Minecraft AutoMap " + AMSettings.sVersionCurrent;

			InputManager.SetMinimapMode(this, AMSettings.bBorderlessMode);
			Globals.camZoom = AMSettings.fZoomLevel;

			Content.RootDirectory = "Content";
			this.IsMouseVisible = true;
			iManager = new InputManager();
			compass = new Compass();
			Globals.SetPriority();
		}

		void frm_Move(object sender, EventArgs e)
		{
			Form frm = (Form)Form.FromHandle(this.Window.Handle);
			AMSettings.iWndX = frm.Bounds.X;
			AMSettings.iWndY = frm.Bounds.Y;
			AMSettings.bMaximized = frm.WindowState == FormWindowState.Maximized;
		}


		void Window_ClientSizeChanged(object sender, EventArgs e)
		{
			Globals.screenWidth = GraphicsDevice.Viewport.Width;
			Globals.screenHeight = GraphicsDevice.Viewport.Height;
			Form frm = (Form)Form.FromHandle(this.Window.Handle);

			AMSettings.iWndW = frm.Bounds.Width;
			AMSettings.iWndH = frm.Bounds.Height;
			AMSettings.bMaximized = frm.WindowState == FormWindowState.Maximized;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			Globals.Initialize(Content);

			mapLayer = new MAM2DLayer(GraphicsDevice);
			Globals.mapLayer = mapLayer;
			DataSource.DS.Connect(AMSettings.sHostName, AMSettings.iPortTCP);

			MinecraftAM.AutoUpdate.UpdateChecker.CheckForUpdates();

			base.Initialize();

		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.b
		/// </summary>
		protected override void LoadContent()
		{
			fontDebug = Content.Load<SpriteFont>("DebugFont");
			Globals.fontDebug = fontDebug;
			pointerPortal = Content.Load<Texture2D>("Graphics/Portal");
			pointerTNT = Content.Load<Texture2D>("Graphics/TNT");
			Globals.experience_orb = Content.Load<Texture2D>("Graphics/experience_orb");

			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			compass.LoadContent(Content);
			mapLayer.LoadContent(Content);

			Globals.screenWidth = GraphicsDevice.Viewport.Width;
			Globals.screenHeight = GraphicsDevice.Viewport.Height;
			Globals.halfScreenWidth = Globals.screenWidth / 2;
			Globals.halfScreenHeight = Globals.screenHeight / 2;

			// Set window position
			Form frm = (Form)Form.FromHandle(this.Window.Handle);
			if (AMSettings.iWndX < -2700 || AMSettings.iWndX > 2700)
				AMSettings.iWndX = 0;
			if (AMSettings.iWndX > 2700)
				AMSettings.iWndX = 0;
			if (AMSettings.iWndY < -2700 || AMSettings.iWndY > 2700)
				AMSettings.iWndY = 0;
			frm.SetDesktopLocation(AMSettings.iWndX, AMSettings.iWndY);
			frm.Move += new EventHandler(frm_Move);
			if (AMSettings.bMaximized)
				frm.WindowState = FormWindowState.Maximized;

			//ProtoUI.Panel p = new ProtoUI.Panel(100, 100, 200, 300);
			//p.Title = "Options";
			//gui = new ProtoUI.GUI(GraphicsDevice);
			//gui.AddWidget(p);
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			mapLayer.StopThreads();
			Globals.chunks.StopLoaders();
			try
			{
				if (Globals.mcrLoader != null) Globals.mcrLoader.Abort();
			}
			catch (Exception) { }

			Globals.flattener.StopThreads();

			if (Globals.chunks.collector != null)
				Globals.chunks.collector.Stop(false);

			try
			{
				if (Globals.staticMapLoadingThread != null)
				{
					Globals.staticMapLoadingThread.Abort();
					Globals.staticMapLoadingThread.Join(50);
				}
			}
			catch (Exception) { }

			DataSource.DS.ShutDown();
			try
			{
				Form frm = (Form)Form.FromHandle(this.Window.Handle);
				AMSettings.bMaximized = frm.WindowState == FormWindowState.Maximized;
			}
			catch (Exception) { }
			AMSettings.Save();
		}
		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			Globals.gameTime = gameTime;
			MouseState ms = Mouse.GetState();
			Rectangle rect = new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
			//gui.Update(gameTime);
			if (AMSettings.MiddleClickFunction == MiddleClickOptions.None || !iManager.InputAvailableForHotkeys || !rect.Contains(ms.X, ms.Y) || !DataSource.PermissionManager.allowPowerToys)
			{
				if (!showingCursor)
				{
					showingCursor = true;
					Cursor.Show();
				}
			}
			else
			{
				if (showingCursor)
				{
					showingCursor = false;
					Cursor.Hide();
				}
			}
			mapLayer.Update(gameTime);
			iManager.HandleInput(this, gameTime);

			base.Update(gameTime);
		}


		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(AMSettings.cBackgroundColor);

			mapLayer.Draw(gameTime);

			string message = connectionStatus;
			string helpText = "";
			string warningLightText = "";
			if (AMSettings.bShowIngameLighting)
				warningLightText += "\r\nRealistic Lighting Mode Enabled";
			if (AMSettings.iWarningLightLevel > -1)
				warningLightText += "\r\nShowing blocks with light level <= " + AMSettings.iWarningLightLevel + " in red.";
			if (AMSettings.bShowHotkeys)
				helpText = "\r\nCommon Hotkey List\r\n" + AMSettings.sRotate + ": Rotate with player: " + (AMSettings.bRotate ? "on" : "off") + "\r\n"
					+ AMSettings.sStick + ": Stick to player (after panning with arrow keys)\r\n"
					+ AMSettings.sCD + "/" + AMSettings.sCI + "/" + AMSettings.sCR + ": Decrease/Increase/Reset rendering altitude.\r\n"
					+ AMSettings.sShowStatusText + ": Toggle status text.\r\n"
					+ AMSettings.sWaterDetection + ": Toggle water detection.\r\n"
					+ AMSettings.sLavaDetection + ": Toggle lava detection.\r\n"
					+ AMSettings.sOreDetection + ": Toggle mineral detection.\r\n"
					+ AMSettings.sDynamicMapEnabled + ": Toggle the realtime map.\r\n"
					+ AMSettings.sShowOptions + ": Show options window.\r\n"
					+ "Pan with arrows, zoom with mouse wheel.\r\n"
					+ "Right click for always-on-top minimap mode.\r\n";
			string timeTilDynMapRefresh = "";
			if (Globals.canUpdateBlocks > DateTime.Now)
				timeTilDynMapRefresh = "\r\n" + Math.Ceiling((Globals.canUpdateBlocks - DateTime.Now).TotalSeconds) + " seconds til dynamic map refresh.";
			if (!string.IsNullOrEmpty(Globals.errMsg))
				message += Globals.errMsg;
			else if (Globals.userPlayer != null && AMSettings.bShowStatusText)
			{
				Vector3 pc = CoordinateTranslator.translateFromAMCoordsAccordingToSettings(new Vector3(Globals.fvLastPlayerLoc.X, Globals.fvLastPlayerLoc.Y, Globals.fvLastPlayerLoc.Z));
				message += (int)Math.Floor(pc.X) + "," + (int)Math.Floor(pc.Y) + "," + (int)Math.Floor(pc.Z) + (Globals.depthOffset != 0 ? " Depth ('" + AMSettings.sCR + "' key resets): " + Globals.depthOffset : "") + " - " + AMSettings.sShowOptions + " shows Options." + warningLightText + helpText + timeTilDynMapRefresh;
			}
			else if (AMSettings.bShowHotkeys)
				message += helpText.Substring(2);
			bool bDrawDebugString = false;
			if (bDrawDebugString)
				message += "  --- Chunks loaded: " + Globals.chunks.chunksLoaded + " / " + Globals.chunks.chunkLoadTotal + "\r\n" + /* Send Attempts/Actual Sent/Receive Calls/Tot Msgs Received: " + DataSource.EventTcpClient.allRequestCnt + "/" + DataSource.EventTcpClient.allRequestSendCnt + "/" + DataSource.EventTcpClient.allRecptCnt + "/" + DataSource.EventTcpClient.allRecptDeliveredCnt + "\r\nBytes Sent/Bytes Recv: " + DataSource.EventTcpClient.sendChars + "/" + DataSource.EventTcpClient.recvChars + "\r\n255s/Errors: " + StringPacker.existing255s + "/" + StringPacker.parseerrors + "\r\n + */ "Bad Chunks RCVD:" + Globals.badpacketsRcvd + "  Flatten Count: " + Globals.flattens + "\r\nCache Hits:" + Globals.cacheHit + " / Cache Miss:" + Globals.cacheMiss + " Collisions:" + Globals.cacheCollisions + "\r\nLast Middle Click block:" + lastMiddleClick;
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone);
			if (Globals.mcrLoader.isLoading)
				DrawString((Globals.screenWidth / 2) - 153, Globals.screenHeight / 2, "Rendering Static World Map: " + Globals.mcrLoader.percentLoaded + "%");
			else if (Globals.loadingWorld)
				DrawString((Globals.screenWidth / 2) - 200, Globals.screenHeight / 2, "Giving Minecraft time to load the world completely.");
			else
			{
				DrawString(10, 10, message);
				if (AMSettings.bCompass && Globals.userPlayer != null)
					compass.Draw(gameTime, spriteBatch);
			}
			//gui.Draw(spriteBatch);
			DrawCursor();
			spriteBatch.End();
			base.Draw(gameTime);
		}

		private void DrawCursor()
		{
			if (DataSource.PermissionManager.allowPowerToys)
				switch (AMSettings.MiddleClickFunction)
				{
					case MiddleClickOptions.Teleport:
						MouseState ms = Mouse.GetState();
						spriteBatch.Draw(pointerPortal, new Vector2(ms.X, ms.Y), null, Color.White, 0, new Vector2(16, 16), 1f, SpriteEffects.None, 0.0f);
						break;
					case MiddleClickOptions.Explode:
						MouseState ms2 = Mouse.GetState();
						spriteBatch.Draw(pointerTNT, new Vector2(ms2.X, ms2.Y), null, Color.White, 0, new Vector2(16, 16), 0.25f, SpriteEffects.None, 0.0f);
						break;
					case MiddleClickOptions.None:
					default:
						break;
				}
		}

		private void DrawString(int x, int y, string message)
		{
			float scale = 1f;//Globals.screenWidth < 425 || Globals.screenHeight < 325 ? 1f : 1f;
			spriteBatch.DrawString(fontDebug, message, new Vector2(x - 1, y - 1), Color.Black, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
			spriteBatch.DrawString(fontDebug, message, new Vector2(x + 1, y + 1), Color.Black, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
			spriteBatch.DrawString(fontDebug, message, new Vector2(x - 1, y + 1), Color.Black, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
			spriteBatch.DrawString(fontDebug, message, new Vector2(x + 1, y - 1), Color.Black, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
			spriteBatch.DrawString(fontDebug, message, new Vector2(x, y), Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
		}

		#region DSHandlerConnectionStatusChanged Members

		public void ConnectionStatusChanged(DataSource.ConnectionStatus newStatus)
		{
			switch (newStatus)
			{
				case DataSource.ConnectionStatus.Idle:
					connectionStatus = "Not Connected\r\n";
					break;
				case DataSource.ConnectionStatus.Connecting:
					connectionStatus = "Connecting\r\n";
					break;
				case DataSource.ConnectionStatus.Connected:
					connectionStatus = "";
					break;
				case DataSource.ConnectionStatus.Refused:
					connectionStatus = "Connection refused.\r\n If Minecraft is already running and a\r\n world is loaded, you may have\r\n forgotten to install Risugami's ModLoader,\r\n or the Automap mod may not be installed\r\n correctly.  Trying connection again...\r\n";
					break;
				case DataSource.ConnectionStatus.Disconnected:
					Globals.loadingWorld = false;
					connectionStatus = "Disconnected.  Attempting reconnect...\r\n";
					break;
				default:
					connectionStatus = "Unknown Connection Status\r\n";
					break;
			}
		}

		#endregion
	}
}
