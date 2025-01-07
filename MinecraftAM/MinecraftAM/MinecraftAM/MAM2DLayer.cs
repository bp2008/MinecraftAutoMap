using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Threading;
using System.Collections.Concurrent;

namespace MinecraftAM
{
	class MAM2DLayer : DataSource.DSHandlerPlayerLocation, DataSource.DSHandlerWorldPath, DataSource.DSHandlerTextureName
	{
		DateTime startedAt = DateTime.Now;
		DateTime nextUpdate = DateTime.Now + new TimeSpan(0, 0, 0, 0, 100);
		DateTime lastRedraw = DateTime.Now;
		DateTime lastChunkUpdate = DateTime.Now + new TimeSpan(0, 0, 10);
		TimeSpan timeTillUpdateBlocks = new TimeSpan(0, 0, 0, 20, 0);
		TimeSpan timeTillUpdate = new TimeSpan(0, 0, 0, 0, 100);
		int msBetweenPositionUpdates = 1000 / 30;
		int msBetweenChunkUpdates = 2000;
		TimeSpan timeTillForcedRedraw = new TimeSpan(0, 0, 0, 1, 0);
		TimeSpan timeTillStop = new TimeSpan(0, 0, 0, 10, 0);
		IntVector3 vLastPlayerLoc = null;
		ConcurrentQueue<object[]> playerPositionPackets = new ConcurrentQueue<object[]>();

		private bool bThreadAbort = false;

		SpriteBatch spriteBatch;
		GraphicsDevice gd;
		Texture2D blockBlack;
		//Texture2D fire;
		Texture2D arrowPlayer;
		Texture2D arrowOtherPlayer;
		Texture2D arrowHostile;
		Texture2D arrowPassive;
		//Texture2D blockTiles;
		Texture2D chestTiles;
		Texture2D chestTrappedTiles;
		Texture2D chestEnderTiles;
		//Texture2D itemTiles;
		Texture2D mergedTexture;
		Texture2D worldMap = null;
		public Texture2D WorldMap
		{
			get { return worldMap; }
			set { worldMap = value; }
		}
		SoundEffect sndRattle;
		SpriteFont toolTipFont;

		Thread positionRequesterThread = null;
		Thread chunkRequesterThread = null;

		MapLabel playerLabel = new MapLabel();

		private Thread gameUpdateThread;

		public MAM2DLayer(GraphicsDevice gDevice)
		{
			DataSource.DS.InitializePlayerLocation(this, Globals.TileWidth, Globals.TileHeight, Chunk.chunkSize);
			DataSource.DS.InitializeWorldPath(this);
			DataSource.DS.InitializeTextureNameCallback(this);
			gd = gDevice;
			spriteBatch = new SpriteBatch(gd);
			positionRequesterThread = new Thread(PositionRequester);
			positionRequesterThread.Name = "Position Update Requester";
			positionRequesterThread.Start();
			chunkRequesterThread = new Thread(ChunkUpdateRequester);
			chunkRequesterThread.Name = "Chunk Update Requester";
			chunkRequesterThread.Start();
		}
		public void LoadContent(ContentManager Content)
		{
			//fire = Content.Load<Texture2D>("Graphics\\MCBlocks\\Fire");
			blockBlack = Content.Load<Texture2D>("Graphics\\MCBlocks\\BlockBlack");

			// Load Texture files into large spritesheet
			string texName = TextureBuilder.BuildTextures(gd, new DirectoryInfo("tex/blocks"), new DirectoryInfo("tex/items"));
			FileStream fiMergedTexture = new FileStream(texName, FileMode.Open, FileAccess.Read);
			mergedTexture = Texture2D.FromStream(gd, fiMergedTexture);
			fiMergedTexture.Close();
			FileStream fiChest = new FileStream("chest.png", FileMode.Open, FileAccess.Read);
			chestTiles = Texture2D.FromStream(gd, fiChest);
			fiChest.Close();
			fiChest = new FileStream("chest_trapped.png", FileMode.Open, FileAccess.Read);
			chestTrappedTiles = Texture2D.FromStream(gd, fiChest);
			fiChest.Close();
			fiChest = new FileStream("chest_ender.png", FileMode.Open, FileAccess.Read);
			chestEnderTiles = Texture2D.FromStream(gd, fiChest);
			fiChest.Close();
			arrowPlayer = Content.Load<Texture2D>("PlayerArrow");
			arrowOtherPlayer = Content.Load<Texture2D>("OtherPlayerArrow");
			arrowHostile = Content.Load<Texture2D>("HostileArrow");
			arrowPassive = Content.Load<Texture2D>("PassiveArrow");
			sndRattle = Content.Load<SoundEffect>("Sounds/ModifiedRattler");
			toolTipFont = Content.Load<SpriteFont>("tooltip");
			Globals.pixel = Content.Load<Texture2D>("pixel");
			Globals.oreHeight.LoadContent(Content);
			WaypointManager.LoadContent(Content);
			//PresentationParameters pp = gd.PresentationParameters;
			//renderTarget = new RenderTarget2D(gd, pp.BackBufferWidth, pp.BackBufferHeight);

		}
		public void WorldPathReceived(string worldPath, int dimension, string worldName, int spawnX, int spawnY, int spawnZ)
		{
			// As of August 2014 the worldpath is not easily available anymore so features that required it will no longer be available, and it will always be assumed that the world is multiplayer.
			if (Globals.chunks.collector != null)
				Globals.chunks.collector.Stop(true);
			if (!string.IsNullOrEmpty(Globals.worldID))
				WaypointManager.Save();
			//if (worldPath == "Unknown")
			//{
			//    // Exit ASAP to prevent corrupting the stored info specific to the world that was just loaded.
			//    File.AppendAllText("errordump.txt", "\r\nMAM2DLayer.WorldPathReceived received Unknown world string.\r\n");
			//    System.Windows.Forms.Application.Exit();
			//    return;
			//}
			//if (worldPath == "multiplayer")
			//{
			Globals.worldPath = SanitizeFilename(worldName) + (dimension == 0 ? "" : "DIM" + dimension);
			Globals.worldID = SanitizeFilename(worldName) + (dimension == 0 ? "" : "DIM" + dimension);
			Globals.multiplayer = true;
			//}
			//else
			//{
			//    DirectoryInfo worldDirectory = new DirectoryInfo(worldPath);
			//    if (!worldDirectory.Exists)
			//    {
			//        // Exit ASAP to prevent corrupting the stored info specific to the world that was just loaded.
			//        File.AppendAllText("errordump.txt", "\r\nMAM2DLayer.WorldPathReceived received a world path, but the directory does not exist:\r\n" + worldPath + "\r\n");
			//        System.Windows.Forms.Application.Exit();
			//        return;
			//    }
			//    Globals.worldPath = Path.Combine(worldDirectory.FullName, (dimension == 0 ? "" : "DIM" + dimension));
			//    DirectoryInfo fullWorldDirectory = new DirectoryInfo(Globals.worldPath);
			//    if (!worldDirectory.Exists)
			//    {
			//        // Exit ASAP to prevent corrupting the stored info specific to the world that was just loaded.
			//        File.AppendAllText("errordump.txt", "\r\nMAM2DLayer.WorldPathReceived received a world path, but the directory does not exist when the dimension is considered:\r\n" + Globals.worldPath + "\r\n");
			//        System.Windows.Forms.Application.Exit();
			//        return;
			//    }
			//    Globals.worldID = worldDirectory.Name + (dimension == 0 ? "" : "DIM" + dimension);
			//    Globals.multiplayer = false;
			//}
			if (File.Exists(Globals.getStaticMapName(true)))
			{
				if (File.Exists(Globals.getStaticMapName()))
					File.Delete(Globals.getStaticMapName());
				File.Move(Globals.getStaticMapName(true), Globals.getStaticMapName());
			}
			//if (worldPath != Globals.worldPath || worldPath == "multiplayer")
			//{
			worldMap = null;
			Globals.refreshWorldMap = true;
			Globals.chunks.ClearCache();
			Globals.loadingWorld = true;
			//}
			Globals.worldDimension = dimension;
			Globals.worldName = worldName;
			if (AMSettings.iInternalStaticMapGeneration == 2)
			{
				Globals.loadStaticMapImmediately = true;
				LoadWorldMap();
			}
			//BuildWorldMap();
			WaypointManager.Load();
			if (Globals.waypointEditor != null && !Globals.waypointEditor.IsDisposed)
				Globals.waypointEditor.ReloadListBox();
			Globals.ResetMapCollector(gd, Globals.worldPath);
		}

		private string SanitizeFilename(string worldName)
		{
			char[] str = worldName.ToCharArray();
			for (int i = 0; i < str.Length; i++)
			{
				if (InvalidCharacter(str[i]))
					str[i] = ' ';
			}
			return new string(str);
		}

		private bool InvalidCharacter(char c)
		{
			int intValue = (int)c;
			if (intValue < 32)
				return true;
			if (c == '<' || c == '>' || c == ':' || c == '"' || c == '/' || c == '\\' || c == '|' || c == '?' || c == '*')
				return true;
			return false;
		}

		//private void BuildWorldMap()
		//{
		//    if (Globals.multiplayer)
		//        return;
		//    lock (Globals.render)
		//    {
		//        if (worldMap == null && !File.Exists(Globals.worldName + Globals.worldMapImageExtension) && !Globals.rendering)
		//        {
		//            Globals.rendering = true;
		//            Globals.render.Render();
		//        }
		//    }
		//}
		private void LoadWorldMap()
		{
			if (!AMSettings.bDisableStaticMap && ((worldMap == null && Globals.refreshWorldMap) || Globals.loadStaticMapImmediately) && !Globals.staticMapLoading)
			{
				Globals.staticMapLoading = true;
				Globals.refreshWorldMap = false;
				Globals.loadStaticMapImmediately = false;
				FileInfo fiMap = new FileInfo(Globals.getStaticMapName());
				if (AMSettings.iInternalStaticMapGeneration == 2 && !fiMap.Exists)
				{
					Globals.staticMapLoading = false;
					return;
				}

				if (Globals.staticMapLoadingThread != null)
				{
					Globals.staticMapLoadingThread.Abort();
					Globals.staticMapLoadingThread.Join(50);
				}
				Globals.staticMapLoadingThread = new Thread(ThreadedLoadStaticMap);
				Globals.staticMapLoadingThread.Name = "Static Map Loading Thread";
				Globals.staticMapLoadingThread.Start();
			}
		}

		private void ThreadedLoadStaticMap()
		{
			if (AMSettings.iInternalStaticMapGeneration == 2)
			{
				if (TgaWriter.mapSync.Wait(0))
				{
					TgaReader tgaReader = null;
					try
					{
						tgaReader = new TgaReader(Globals.getStaticMapName());
						tgaReader.Start();
						tgaReader.ReadMetaData();
						Texture2D mapHolder = tgaReader.ReadImageIntoTexture(gd);
						tgaReader.Stop();
						Globals.worldMapOriginX = tgaReader.originX * -1;
						Globals.worldMapOriginY = tgaReader.originY * -1;
						Globals.worldMapWidth = tgaReader.width;
						Globals.worldMapHeight = tgaReader.height;
						worldMap = mapHolder;
						Globals.staticMapLastLoaded = DateTime.Now;
					}
					catch (ThreadAbortException)
					{
						if (tgaReader != null)
						{
							tgaReader.bReadThreadAbort = true;
							Thread.Sleep(10);
							tgaReader.Stop();
						}
						worldMap = null;
						Globals.worldMapWidth = Globals.worldMapHeight = Globals.worldMapOriginX = Globals.worldMapOriginY = 0;
					}
					catch (Exception)
					{
						if (tgaReader != null)
						{
							tgaReader.bReadThreadAbort = true;
							Thread.Sleep(10);
							tgaReader.Stop();
						}
						worldMap = null;
						Globals.worldMapWidth = Globals.worldMapHeight = Globals.worldMapOriginX = Globals.worldMapOriginY = 0;
					}
					TgaWriter.mapSync.Release();
				}
			}
			else if (AMSettings.iInternalStaticMapGeneration == 3)
			{
				FixedStaticMap.Update(gd, ref worldMap);
				Globals.staticMapLastLoaded = DateTime.Now;
			}
			Globals.staticMapLoading = false;
		}

		public void StopThreads()
		{
			bThreadAbort = true;
		}
		private static int playerLocationReceivedErrorCount = 0;
		public void PlayerLocationReceived(List<DataSource.Entity> players, DataSource.Entity userPlayer)
		{
			if (Shitlist.IsOnShitlist(userPlayer.name))
			{
				System.Windows.Forms.MessageBox.Show("You are on the AutoMap Shit List.  Find another way to cheat!");
				return;
			}
			if (Thread.CurrentThread != gameUpdateThread)
			{
				playerPositionPackets.Enqueue(new object[] { players, userPlayer });
				return;
			}
			Globals.players = players;
			if (userPlayer != null)
				Globals.userPlayer = userPlayer;
			else
			{
				Globals.userPlayer = null;
				Console.WriteLine("Player did not exist in player list");
				if (playerLocationReceivedErrorCount++ % 100 == 0)
					throw new Exception("The user's Player object did not exist!");
				return;
			}

			// Detect warp / portal / teleporter use and stop requesting blocks temporarily, giving Minecraft time to load.

			if (Globals.lastUserPlayer != null)
			{
				double changeAmount = Globals.userPlayer.ChangeAmount(Globals.lastUserPlayer);
				// if player moved a lot.
				if (changeAmount > 32)
				{
					Globals.bCanUpdateBlocks = false;
					Globals.canUpdateBlocks = DateTime.Now + timeTillUpdateBlocks;
					//Console.WriteLine(changeAmount);
				}
				// if player moved at all
				//else if (changeAmount > 0)
				//{
				//    Globals.bCanUpdateBlocks = true;
				//    Console.WriteLine(changeAmount);
				//}
			}
			Globals.lastUserPlayer = Globals.userPlayer;

			if (Globals.lockToPlayer)
			{
				Globals.camX = Globals.round(Globals.userPlayer.pixelx);
				Globals.camY = Globals.round(Globals.userPlayer.pixely);
			}
			Block b = Globals.chunks.GetBlock(Globals.userPlayer.ix + (Chunk.chunkSize - 1), Globals.userPlayer.iy, Globals.userPlayer.iz - 1);
			if (b != null && b.blockType == BlockType.Snow)
				b.trail = true;
			else
			{
				b = Globals.chunks.GetBlock(Globals.userPlayer.ix + (Chunk.chunkSize - 1), Globals.userPlayer.iy, Globals.userPlayer.iz - 2);
				if (b != null && !ItemDrawing.transparentBlocks[(int)b.blockType])
					b.trail = true;
			}
			// Call this function last if in PlayerLocationReceived
			DistanceCheck();
		}

		private void DistanceCheck()
		{
			//int width = Globals.flattener.GetCurrentWidth();
			//int height = Globals.flattener.GetCurrentHeight();
			int farDistance = 100;
			int farAltitude = 20;
			//TODO: Implement creeper proximity checks.
			for (int i = 0; i < Globals.players.Count; i++)
			{
				DataSource.Entity loopEntity = Globals.players[i];
				if (loopEntity != Globals.userPlayer)
				{
					double distance = Globals.userPlayer.distanceFrom(loopEntity);
					if (distance > farDistance || Math.Abs(loopEntity.z - Globals.userPlayer.z) > farAltitude)
						loopEntity.isFarFromMainPlayer = true;
					if (AMSettings.iCreeperProximityAlert != 0 && AMSettings.iCreeperProximityAlert >= distance && (loopEntity.name.StartsWith("Creeper") || loopEntity.name.EndsWith("Creeper")))
						PlayCreeperAlertSound();
				}
			}
		}
		DateTime CanPlayCreeperAlertAt = DateTime.Now;
		TimeSpan TimeBetweenCreeperAlerts = TimeSpan.FromSeconds(5);
		private void PlayCreeperAlertSound()
		{
			if (sndRattle != null && DateTime.Now >= CanPlayCreeperAlertAt)
			{
				CanPlayCreeperAlertAt = DateTime.Now + TimeBetweenCreeperAlerts;
				sndRattle.Play(1, 0, 0);
			}
		}

		private void PositionRequester()
		{
			while (!bThreadAbort)
			{
				try
				{
					DataSource.DS.GetPlayerLocation();
				}
				catch (Exception err)
				{
					Globals.errMsg = err.Message;
				}
				Thread.Sleep(msBetweenPositionUpdates);
			}
		}
		private void ChunkUpdateRequester()
		{
			while (!bThreadAbort)
			{
				try
				{
					IntVector3 vPlayerLoc = vLastPlayerLoc;
					if (vPlayerLoc != null)
						Globals.chunks.UpdateChunksAround(vPlayerLoc.X, vPlayerLoc.Y);
				}
				catch (Exception err)
				{
					Globals.errMsg = err.Message;
				}
				Thread.Sleep(msBetweenChunkUpdates);
			}
		}

		DateTime requestedWorldPathAt = DateTime.Now - new TimeSpan(24, 0, 0);
		TimeSpan retryWorldPathTimer = new TimeSpan(0, 0, 5);
		public void Update(GameTime gameTime)
		{
			if (gameUpdateThread == null)
				gameUpdateThread = Thread.CurrentThread;

			UpdatePlayerLocation();

			if (DateTime.Now >= Globals.canUpdateBlocks)
				Globals.bCanUpdateBlocks = true;
			if (!Globals.staticMapLoading && (DateTime.Now - Globals.staticMapLastLoaded).TotalSeconds > AMSettings.iStaticMapUpdateInterval)
				Globals.loadStaticMapImmediately = true;
			if (Globals.loadStaticMapImmediately && !Globals.staticMapLoading)
				LoadWorldMap();
			if (DateTime.Now > nextUpdate)
			{
				if (string.IsNullOrEmpty(Globals.worldPath) && DateTime.Now - retryWorldPathTimer > requestedWorldPathAt)
				{
					requestedWorldPathAt = DateTime.Now;
					DataSource.DS.GetWorldPath();
				}
				nextUpdate = DateTime.Now + timeTillUpdate;

				if (Globals.players != null && Globals.players.Count > 0 && Globals.userPlayer != null)
				{
					Globals.fvLastPlayerLoc.X = (float)Globals.userPlayer.x;
					Globals.fvLastPlayerLoc.Y = (float)Globals.userPlayer.y;
					Globals.fvLastPlayerLoc.Z = (float)Globals.userPlayer.z;
					IntVector3 vPlayerLoc = new IntVector3(Globals.round(Globals.userPlayer.x), Globals.round(Globals.userPlayer.y), Globals.round(Globals.userPlayer.z));
					if (DateTime.Now - timeTillForcedRedraw > lastRedraw
						|| (vLastPlayerLoc == null || vPlayerLoc.X != vLastPlayerLoc.X || vPlayerLoc.Y != vLastPlayerLoc.Y || vPlayerLoc.Z != vLastPlayerLoc.Z))
					{ // player moved, redraw the map    
						if (!string.IsNullOrEmpty(Globals.worldPath))
						{
							if (Globals.refreshWorldMap)
							{
								worldMap = null;
								//BuildWorldMap();
							}
							//LoadWorldMap(); // This is not necessary to call here since the static map is now created by automap.
						}

						IntVector2 topLeft, bottomRight;
						int halfWidth = (Globals.screenWidth / 2) / Globals.TileWidth;
						int halfHeight = (Globals.screenHeight / 2) / Globals.TileHeight;
						int tlx, tly, brx, bry;
						float scale = 1f / Globals.camZoom;
						float scaledHalfWidth = (halfWidth * scale);
						float scaledHalfHeight = (halfHeight * scale);
						if (AMSettings.bRotate || AMSettings.iDefaultMapRotation % 180 != 0)
						{
							float fullWidth = (scaledHalfWidth * 2);
							float fullHeight = (scaledHalfHeight * 2);
							float corner2corner = (float)Math.Sqrt((fullWidth * fullWidth) + (fullHeight * fullHeight));
							if (halfWidth > halfHeight)
							{
								scaledHalfWidth *= corner2corner / fullWidth;
								scaledHalfHeight = scaledHalfWidth;
							}
							else
							{ // I expect this code to never run... but hey, someone might have a rotatable monitor.
								scaledHalfHeight *= corner2corner / fullWidth;
								scaledHalfWidth = scaledHalfHeight;
							}
						}

						tlx = ((vPlayerLoc.X + Chunk.chunkSizeM1) - (int)scaledHalfWidth) - 6;
						tly = (vPlayerLoc.Y - (int)scaledHalfHeight) - 5;
						brx = ((vPlayerLoc.X + Chunk.chunkSizeM1) + (int)scaledHalfWidth) + 6;
						bry = (vPlayerLoc.Y + (int)scaledHalfHeight) + 6;
						if (brx - tlx > AMSettings.iDynamicMapMaxX || (Math.Abs(Globals.panX) > 2500) || (Math.Abs(Globals.panY) > 2500))
						{
							Globals.zoomedOut = true;

							Globals.locationTopLeft = new IntVector2(tlx, tly);
							Globals.locationBottomRight = new IntVector2(brx, bry);
							vLastPlayerLoc = vPlayerLoc;
							Globals.vLastPlayerLoc = vLastPlayerLoc;

							int overage = ((brx - tlx) - AMSettings.iDynamicMapMaxX) / 2;
							float overagePrcnt = ((float)overage * 2f) / (float)(brx - tlx);
							tlx += overage;
							brx -= overage;
							int yOverage = (int)(overagePrcnt * (float)(bry - tly)) / 2;
							tly += yOverage;
							bry -= yOverage;
						}
						else
						{
							Globals.zoomedOut = false;
							if (brx - tlx < AMSettings.iDynamicMapMinX)
							{
								int overage = (AMSettings.iDynamicMapMinX - (brx - tlx)) / 2;
								float overagePrcnt = ((float)overage * 2f) / (float)(brx - tlx);
								tlx -= overage;
								brx += overage;
								int yOverage = (int)(overagePrcnt * (float)(bry - tly)) / 2;
								tly -= yOverage;
								bry += yOverage;

							}
						}
						topLeft = new IntVector2(tlx, tly);
						bottomRight = new IntVector2(brx, bry);

						if (AMSettings.bDynamicMapEnabled && (!Globals.zoomedOut || !AMSettings.bDynamicMapHiddenZoomedOut) && Globals.bCanUpdateBlocks)
						{
							lock (Globals.flattener.syncLock)
							{
								Globals.flattener.nextJob = new MapFlattener.JobSpec();
								Globals.flattener.nextJob.topLeft = topLeft.Copy();
								Globals.flattener.nextJob.bottomRight = bottomRight.Copy();
								Globals.flattener.nextJob.player = vPlayerLoc.Copy();
								Globals.flattener.newData = true;
							}
							Globals.flattener.trigger.Set();
						}
						lastRedraw = DateTime.Now;
						vLastPlayerLoc = vPlayerLoc;
						Globals.vLastPlayerLoc = vLastPlayerLoc;
					}
					Globals.holidayManager.Update();
				}

			}
		}
		/// <summary>
		/// Dequeues all collected player position packets and handles only the most recent, if one exists.
		/// </summary>
		private void UpdatePlayerLocation()
		{
			object[] mostRecentPositionData = null;
			object[] playerPositionData = null;
			while (playerPositionPackets.TryDequeue(out playerPositionData))
			{
				mostRecentPositionData = playerPositionData;
			}
			if (mostRecentPositionData != null)
				PlayerLocationReceived((List<DataSource.Entity>)mostRecentPositionData[0], (DataSource.Entity)mostRecentPositionData[1]);
		}

		//RenderTarget2D renderTarget;
		//bool scenecached = false;
		public void Draw(GameTime gameTime)
		{
			Matrix m = CreateMatrix();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, m);
			tilebuffer tbuffer = Globals.flattener.frontBuffer;

			if (worldMap != null)
			{
				// world map mode
				DrawWorldMap();
			}
			// live mode
			if (tbuffer.buffer != null && AMSettings.bDynamicMapEnabled && (!Globals.zoomedOut || !AMSettings.bDynamicMapHiddenZoomedOut) && !Globals.loadingWorld)
			{
				//if (!scenecached)
				//{
				//    gd.SetRenderTarget(renderTarget);
				//    DrawRealtimeScene(tbuffer);
				//    gd.SetRenderTarget(null);
				//    scenecached = true;
				//}
				//DrawBufferedScene(renderTarget);
				DrawRealtimeScene(tbuffer);
			}
			float scale = (Globals.camZoom < 1 ? (1f / Globals.camZoom) : 1);
			DrawOtherEntities(scale);
			DrawPlayer(arrowPlayer, Globals.userPlayer);
			WaypointManager.Draw(spriteBatch, toolTipFont, scale);
			spriteBatch.End();
		}

		private void DrawOtherEntities(float scale)
		{
			foreach (DataSource.Entity pl in Globals.players)
				if (pl != Globals.userPlayer)
				{
					if (pl.entityType == DataSource.Entity.EntityType.Item)
						DrawItem(scale * 0.8f, pl);
					else if (pl.entityType == DataSource.Entity.EntityType.Painting)
						DrawPainting(scale, pl);
					else if (pl.entityType == DataSource.Entity.EntityType.Block || pl.entityType == DataSource.Entity.EntityType.PhysicsObj)
						DrawBlockItem(scale * 0.8f, pl);
					else if (pl.entityType == DataSource.Entity.EntityType.Arrow)
						DrawArrowProjectile(scale, pl);
					else if (pl.entityType == DataSource.Entity.EntityType.Snowball)
						DrawSnowballProjectile(scale, pl);
					else if (pl.entityType == DataSource.Entity.EntityType.Minecart || pl.entityType == DataSource.Entity.EntityType.Boat)
						DrawVehicle(scale, pl);
					else if (pl.isNPC)
						DrawNPC(scale, pl);
					else
						DrawOtherPlayer(scale, pl);
				}
			if (Globals.holidayManager.CurrentHoliday == Holiday.AprilFools && Globals.holidayManager.showHerobrine)
				DrawHerobrine(scale);
		}

		private void DrawHerobrine(float scale)
		{
			DataSource.Entity plHerobrine = Globals.holidayManager.Herobrine;
			Globals.getPlayerWorldLocation(ref reusableRectangle, plHerobrine, 1f);
			spriteBatch.Draw(arrowHostile, reusableRectangle, null, Color.White, (float)plHerobrine.rotation, Globals.TileOrigin, SpriteEffects.None, 0.0f);
			DrawLabel(scale, plHerobrine, MapLabel.Style.Red);
		}


		private void DrawPainting(float scale, DataSource.Entity pl)
		{
			if (!AMSettings.bDrawPaintings) return;
			Globals.getPlayerWorldLocation(ref reusableRectangle, pl, 1f);
			ItemDrawing.Draw(spriteBatch, mergedTexture, pl, reusableRectangle, (float)pl.rotation + MathHelper.Pi);
		}
		private void DrawVehicle(float scale, DataSource.Entity pl)
		{
			if (AMSettings.iVehicleVisibility == 0) return;
			Globals.getPlayerWorldLocation(ref reusableRectangle, pl, 1f);
			ItemDrawing.Draw(spriteBatch, mergedTexture, pl, reusableRectangle);
			if (AMSettings.iVehicleVisibility == 2)
				DrawLabel(scale, pl, MapLabel.Style.Yellow);
		}
		private void DrawArrowProjectile(float scale, DataSource.Entity pl)
		{
			if (AMSettings.iProjectileVisibility == 0) return;
			Globals.getPlayerWorldLocation(ref reusableRectangle, pl, 1f);
			ItemDrawing.Draw(spriteBatch, mergedTexture, pl, reusableRectangle, (float)pl.rotation - MathHelper.ToRadians(12));
			if (AMSettings.iProjectileVisibility == 2)
				DrawLabel(scale, pl, MapLabel.Style.Red);
		}
		private void DrawSnowballProjectile(float scale, DataSource.Entity pl)
		{
			if (AMSettings.iProjectileVisibility == 0) return;
			Globals.getPlayerWorldLocation(ref reusableRectangle, pl, 1f);
			ItemDrawing.Draw(spriteBatch, mergedTexture, pl, reusableRectangle);
			if (AMSettings.iProjectileVisibility == 2)
				DrawLabel(scale, pl, MapLabel.Style.Blue);
		}

		private void DrawBlockItem(float scale, DataSource.Entity pl)
		{
			if (AMSettings.iDroppedItemVisibility == 0)
				return;
			Globals.getPlayerWorldLocation(ref reusableRectangle, pl, 0.75f);
			if (pl.itemID == (int)BlockType.Chest || pl.itemID == (int)BlockType.Locked_Chest)
				spriteBatch.Draw(chestTiles, reusableRectangle, Globals.chestSrcRect, Color.White, 0, Globals.TileOrigin, SpriteEffects.None, 0.0f);
			else if (pl.itemID == (int)BlockType.Trapped_Chest)
				spriteBatch.Draw(chestTrappedTiles, reusableRectangle, Globals.chestSrcRect, Color.White, 0, Globals.TileOrigin, SpriteEffects.None, 0.0f);
			else if (pl.itemID == (int)BlockType.Ender_Chest)
				spriteBatch.Draw(chestEnderTiles, reusableRectangle, Globals.chestSrcRect, Color.White, 0, Globals.TileOrigin, SpriteEffects.None, 0.0f);
			else
				ItemDrawing.Draw(spriteBatch, mergedTexture, pl, reusableRectangle);
			//	spriteBatch.Draw(blockTiles, reusableRectangle, pl.sourceRect, Color.White, 0, Globals.TileOrigin, SpriteEffects.None, 0.0f);
			if (AMSettings.iDroppedItemVisibility == 1)
				return;
			DrawBlockOrItemLabel(scale, pl);
		}

		private void DrawItem(float scale, DataSource.Entity pl)
		{
			if (AMSettings.iDroppedItemVisibility == 0)
				return;
			Globals.getPlayerWorldLocation(ref reusableRectangle, pl, 0.75f);
			ItemDrawing.Draw(spriteBatch, mergedTexture, pl, reusableRectangle);
			if (AMSettings.iDroppedItemVisibility == 1)
				return;
			DrawBlockOrItemLabel(scale, pl);
		}

		private void DrawBlockOrItemLabel(float scale, DataSource.Entity pl)
		{
			if (AMSettings.ItemFilter.Count > 0 && AMSettings.ItemFilter.Contains(pl.name.ToLower()))
				return;
			string altitude = (Globals.userPlayer != null && Math.Abs(Globals.userPlayer.iz - pl.iz) > 2) ? " (" + pl.iz + ")" : "";
			string label;
			if (AMSettings.iDroppedItemVisibility == 3)
				label = pl.name + (pl.itemCount > 1 ? " x" + pl.itemCount : "") + altitude;
			else if (pl.itemCount > 1)
				label = "x" + pl.itemCount + altitude;
			else if (string.IsNullOrEmpty(altitude))
				label = ".";
			else
				label = altitude.Substring(1);
			DrawLabel(scale, pl, MapLabel.Style.Green, label);
		}
		private void DrawLabel(float scale, DataSource.Entity pl, MapLabel.Style style, string text = null, bool isOtherPlayer = false)
		{
			if (text == null)
			{
				if (pl.entityType == DataSource.Entity.EntityType.XPOrb)
				{
					text = (Globals.userPlayer != null && Math.Abs(Globals.userPlayer.iz - pl.iz) > 2) ? "(" + pl.iz + ")" : "";
				}
				else
				{
					text = pl.name + " (" + pl.iz + ")";
					if (Shitlist.IsOnShitlist(pl.name))
						text = "**Asshat** " + text + " **Asshat**";
				}
			}
			if (!string.IsNullOrWhiteSpace(text))
			{
				playerLabel.Draw(spriteBatch, text, (int)pl.pixelx, (int)pl.pixely, scale,
				AMSettings.bRotate ? (float)Globals.userPlayer.rotation : -Globals.DegreeToRadian(AMSettings.iDefaultMapRotation), toolTipFont, style);
			}
		}

		private void DrawOtherPlayer(float scale, DataSource.Entity pl)
		{
			DrawPlayer(arrowOtherPlayer, pl);
			DrawLabel(scale, pl, MapLabel.Style.Normal, isOtherPlayer: true);
		}

		private void DrawNPC(float scale, DataSource.Entity pl)
		{
			if (pl.isFarFromMainPlayer && AMSettings.bHideDistantNPCs)
				return;
			if (AMSettings.NPCFilter.Count > 0 && AMSettings.NPCFilter.Contains(pl.nameWithoutTags.ToLower()))
				return;
			if (pl.entityType == DataSource.Entity.EntityType.XPOrb)
			{
				if (AMSettings.iDroppedItemVisibility > 0)
				{
					XPOrb.Draw(spriteBatch, pl);
					if (AMSettings.iDroppedItemVisibility > 1)
						DrawLabel(scale, pl, MapLabel.Style.Pulsing);
				}
			}
			else if (pl.isPassive)
			{
				if (AMSettings.bShowPassiveNPCs)
				{
					DrawPlayer(arrowPassive, pl, 0.75f);
					if (pl.entityType == DataSource.Entity.EntityType.Horse)
						DrawLabel(scale, pl, MapLabel.Style.Yellow);
					else
						DrawLabel(scale, pl, MapLabel.Style.Blue);
				}
			}
			else if (AMSettings.bShowHostileNPCs)
			{
				DrawPlayer(arrowHostile, pl, 0.75f);
				DrawLabel(scale, pl, MapLabel.Style.Red);
			}
		}

		private void DrawBufferedScene(Texture2D renderTarget)
		{
			Matrix m = Matrix.Identity;

			spriteBatch.Draw(renderTarget, new Vector2(0, 0), Color.White);

		}

		private void DrawPlayer(Texture2D texture, DataSource.Entity player, float scale = 1.0f)
		{
			DrawPlayer(texture, player, Color.White, scale);
		}
		private void DrawPlayer(Texture2D texture, DataSource.Entity player, Color color, float scale = 1.0f)
		{
			if (Globals.userPlayer != null)
			{
				Globals.getPlayerWorldLocation(ref reusableRectangle, player, scale);
				spriteBatch.Draw(texture, reusableRectangle, null, Color.White, (float)player.rotation, Globals.TileOrigin, SpriteEffects.None, 0.0f);
			}
		}

		private void DrawWorldMap()
		{
			if (worldMap == null)
				return;
			float widthDif = (float)worldMap.Width / (float)Globals.worldMapWidth;
			float heightDif = (float)worldMap.Height / (float)Globals.worldMapHeight;
			float widthDifG = (float)Globals.worldMapWidth / (float)worldMap.Width;
			float heightDifG = (float)Globals.worldMapHeight / (float)worldMap.Height;
			float originX = ((float)Globals.worldMapOriginX * widthDif);
			float originY = ((float)Globals.worldMapOriginY * heightDif);

			float bpp = (worldMap.Width > worldMap.Height ? widthDifG : heightDifG);

			int blockOriginX = (int)(originX * bpp);
			int blockOriginY = (int)(originY * bpp);
			reusableRectangle.X = -(blockOriginX * Globals.TileWidth);
			reusableRectangle.Y = -(blockOriginY * Globals.TileHeight);
			reusableRectangle.Width = (int)(worldMap.Width * bpp * Globals.TileWidth);
			reusableRectangle.Height = (int)(worldMap.Height * bpp * Globals.TileWidth);
			spriteBatch.Draw(worldMap, reusableRectangle, Color.White);

		}
		private Rectangle reusableRectangle = new Rectangle();
		private void DrawRealtimeScene(tilebuffer tbuffer)
		{

			MCTile[,] buffer = tbuffer.buffer;
			IntVector2 locationTopLeft = tbuffer.topLeft;

			for (int x = 0; x < buffer.GetLength(0); x++)
				for (int y = 0; y < buffer.GetLength(1); y++)
				{
					MCTile theTile = buffer[x, y];

					while (theTile != null)
					{
						reusableRectangle.X = Globals.BTP(locationTopLeft.X + x);
						reusableRectangle.Y = Globals.BTP(locationTopLeft.Y + y);
						reusableRectangle.Width = Globals.TileWidth;
						reusableRectangle.Height = Globals.TileHeight;
						if (theTile.solidColor)
							spriteBatch.Draw(blockBlack, reusableRectangle, theTile.c);
						//else if (theTile.blockType == BlockType.Fire)
						//    spriteBatch.Draw(fire, reusableRectangle, LightingManager.GetBlockColor(theTile));
						else if (theTile.blockType == BlockType.Chest || theTile.blockType == BlockType.Locked_Chest)
						{
							spriteBatch.Draw(chestTiles, reusableRectangle, Globals.chestSrcRect, DetermineColor(theTile));
							if (AMSettings.bShowOreHeightRelative && Globals.userPlayer != null && Block.IsOre((int)theTile.blockType))
								spriteBatch.Draw(Globals.oreHeight.GetTexture(Globals.userPlayer.iz, theTile.height), reusableRectangle, Globals.oreHeight.SourceRect, LightingManager.GetBlockColor(theTile));
						}
						else if (theTile.blockType == BlockType.Trapped_Chest)
						{
							spriteBatch.Draw(chestTrappedTiles, reusableRectangle, Globals.chestSrcRect, DetermineColor(theTile));
							if (AMSettings.bShowOreHeightRelative && Globals.userPlayer != null && Block.IsOre((int)theTile.blockType))
								spriteBatch.Draw(Globals.oreHeight.GetTexture(Globals.userPlayer.iz, theTile.height), reusableRectangle, Globals.oreHeight.SourceRect, LightingManager.GetBlockColor(theTile));
						}
						else if (theTile.blockType == BlockType.Ender_Chest)
						{
							spriteBatch.Draw(chestEnderTiles, reusableRectangle, Globals.chestSrcRect, DetermineColor(theTile));
							if (AMSettings.bShowOreHeightRelative && Globals.userPlayer != null && Block.IsOre((int)theTile.blockType))
								spriteBatch.Draw(Globals.oreHeight.GetTexture(Globals.userPlayer.iz, theTile.height), reusableRectangle, Globals.oreHeight.SourceRect, LightingManager.GetBlockColor(theTile));
						}
						else
						{
							// Draw Block Texture
							ItemDrawing.Draw(spriteBatch, mergedTexture, theTile, reusableRectangle);
							//spriteBatch.Draw(blockTiles, reusableRectangle, theTile.sourceRect, DetermineColor(theTile));
							if (AMSettings.bShowOreHeightRelative && Globals.userPlayer != null && Block.IsOre((int)theTile.blockType))
								spriteBatch.Draw(Globals.oreHeight.GetTexture(Globals.userPlayer.iz, theTile.height), reusableRectangle, Globals.oreHeight.SourceRect, LightingManager.GetBlockColor(theTile));
						}
						theTile = theTile.nextTile;
					}
				}
		}

		private Color DetermineColor(MCTile theTile)
		{
			return LightingManager.GetBlockColor(theTile);
		}

		public static Matrix CreateMatrix()
		{
			Matrix m = Matrix.Identity;

			m = Matrix.Multiply(Matrix.CreateTranslation((Globals.screenWidth / 2), (Globals.screenHeight / 2), 0), m); // center on origin
			m = Matrix.Multiply(Matrix.CreateScale(Globals.camZoom), m); // zoom

			if (!Globals.lockToPlayer)
			{
				m = Matrix.Multiply(Matrix.CreateTranslation(-Globals.panX, -Globals.panY, 0), m); // pan
			}

			if (AMSettings.bRotate && Globals.userPlayer != null)
				m = Matrix.Multiply(Matrix.CreateRotationZ((float)-Globals.userPlayer.rotation), m); //rotate
			else
				m = Matrix.Multiply(Matrix.CreateRotationZ((float)0.00065 + Globals.DegreeToRadian(AMSettings.iDefaultMapRotation)), m);

			m = Matrix.Multiply(Matrix.CreateTranslation(-((Globals.screenWidth / 2)), -((Globals.screenHeight / 2)), 0), m); // uncenter on origin
			m = Matrix.Multiply(Matrix.CreateTranslation(Globals.screenWidth / 2 - Globals.camX, Globals.screenHeight / 2 - Globals.camY, 0), m); // move to camera
			return m;
		}

		public void TextureNameReceived(int id, string textureName)
		{
			ItemDrawing.ResolveTexture(id, textureName);
		}
	}
}
