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
using System.Diagnostics;

namespace MinecraftAM
{
	public static class Globals
	{
		public const int worldHeight = 256;
		public const int worldHeightM1 = 255;
		public static OreHeight oreHeight = new OreHeight();
		public static Rectangle SourceRect0x0x8x8 = new Rectangle(0, 0, 8, 8);
		public static string errMsg = null;
		public static float camX = 1;
		public static float camY = 1;
		public static float panX = 1;
		public static float panY = 1;
		private static float camZoomInternal = 1;
		public static float camZoom
		{
			get
			{
				return camZoomInternal;
			}
			set
			{
				camZoomInternal = AMSettings.fZoomLevel = value;
			}
		}
		public static float camRotation = 0f;
		public static int screenWidth = 0;
		public static int screenHeight = 0;
		public static float halfScreenWidth = 0;
		public static float halfScreenHeight = 0;
		public static float Degrees360InRadians = DegreeToRadian(360);
		public static float Degrees45InRadians = DegreeToRadian(45);
		public static float Degrees90InRadians = DegreeToRadian(90);
		public static float Degrees180InRadians = DegreeToRadian(180);
		public static float Degrees270InRadians = DegreeToRadian(270);
		public static int TileWidth = 16;
		public static int TileHeight = 16;
		public static int textureSize = 16;
		public static int itemTextureSize = 16;
		public static Vector2 TileOrigin = new Vector2(TileWidth / 2, TileHeight / 2);
		public static int MapWidthPx = 0;
		public static int MapHeightPx = 0;
		public static SortedList<string, int> GraphicsIndices;
		public static ChunkManager chunks = new ChunkManager();
		public static MapFlattener flattener = new MapFlattener();
		public static IntVector3 vLastPlayerLoc = new IntVector3(0, 0, 0);
		public static Vector3 fvLastPlayerLoc = Vector3.Zero;
		//public static List<LitObject> LitObjects;
		//public static LightingTable lTable;
		public static Matrix WorldToScreenMatrix;
		//public static double[] playerLocation = new double[0];
		public static bool lockToPlayer = true;
		public static short depthOffset = 0;
		public static int depthOffMax = 60;
		public static int badpacketsRcvd = 0;
		public static int renderProgress = 0;
		//public static bool rendering = false;
		public static IntVector2 locationTopLeft;
		public static IntVector2 locationBottomRight;
		//public static FullMapRender render = new FullMapRender();

		public static string worldID = null;
		public static string worldPath = null;
		public static int worldMapOriginX = 0;
		public static int worldMapOriginY = 0;
		public static int worldMapWidth = 0;
		public static int worldMapHeight = 0;
		public static bool refreshWorldMap = false;
		public static int flattens = 0;
		public static bool multiplayer = false;
		public static List<DataSource.Entity> players = new List<DataSource.Entity>();
		public static DataSource.Entity userPlayer = null;

		public static int cacheCollisions = 0;
		public static int cacheMiss = 0;
		public static int cacheHit = 0;
		public static Trail currentTrail = new Trail();
		public static bool loadingWorld = false;
		public static bool zoomedOut = false;
		public static bool optionsOpen = false;
		public static Options optWin = new Options();
		public static WaypointEditor waypointEditor = new WaypointEditor();

		public static Texture2D pixel;
		public static Texture2D experience_orb;

		public static string worldMapImageExtension = ".tga";
		public static bool bCanUpdateBlocks = true;
		public static DataSource.Entity lastUserPlayer;
		public static int worldDimension;
		public static string worldName;
		public static McrLoader mcrLoader = new McrLoader();
		internal static MAM2DLayer mapLayer;
		public static bool loadStaticMapImmediately = false;

		public static HolidayManager holidayManager = new HolidayManager();
		public static DateTime staticMapLastLoaded = DateTime.Now;
		public static volatile bool staticMapLoading = false;
		public static DateTime canUpdateBlocks = DateTime.Now;

		public static System.Threading.Thread staticMapLoadingThread = null;

		public static SpriteFont fontDebug;
		public static Rectangle chestSrcRect = new Rectangle(0, 0, 16, 16);

		// Animation Helpers
		public static ulong slowerAnimationFrame = 0;
		public static ulong slowAnimationFrame = 0;
		public static ulong mediumAnimationFrame = 0;
		public static ulong fastAnimationFrame = 0;
		public static ulong fasterAnimationFrame = 0;
		private static GameTime gameTimeInternal = new GameTime();
		public static GameTime gameTime
		{
			get
			{
				return gameTimeInternal;
			}
			set
			{
				gameTimeInternal = value;
				ulong totalMS = (ulong)gameTimeInternal.TotalGameTime.TotalMilliseconds;
				slowerAnimationFrame = totalMS / 1000;
				slowAnimationFrame = totalMS / 500;
				mediumAnimationFrame = totalMS / 200;
				fastAnimationFrame = totalMS / 100;
				fasterAnimationFrame = totalMS / 50;
			}
		}

		public static void Initialize(ContentManager content)
		{
		}

		public static void SetPriority()
		{
			Process me = Process.GetCurrentProcess();
			if (AMSettings.ProcessPriority == AMSettings.Priority.BelowNormal)
				me.PriorityClass = ProcessPriorityClass.BelowNormal;
			else if (AMSettings.ProcessPriority == AMSettings.Priority.Low)
				me.PriorityClass = ProcessPriorityClass.Idle;
			else
				me.PriorityClass = ProcessPriorityClass.Normal;
		}
		/// <summary>
		/// Converts block location to pixel location.
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public static int BTP(int x)
		{ // BlockToPixel
			return x * Globals.TileWidth;
		}
		/// <summary>
		/// Converts pixel location to block location.
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public static int PTB(int x)
		{ // BlockToPixel
			return (int)(x / Globals.TileWidth);
		}
		public static float PTB(float x)
		{ // BlockToPixel
			return (x / Globals.TileWidth);
		}
		/// <summary>
		/// Returns the player's location in world coordinates.
		/// </summary>
		/// <returns></returns>
		public static void getPlayerWorldLocation(ref Rectangle rect, DataSource.Entity player, float scale = 1.0f, float HeightToWidthRatio = 1.0f)
		{

			if (player == null)
			{
				rect.X = 0;
				rect.Y = 0;
				rect.Width = TileWidth;
				rect.Height = TileHeight;
			}
			else
			{
				rect.X = (int)player.pixelx;
				rect.Y = (int)(player.pixely);
				rect.Width = (int)((float)TileWidth * (Globals.camZoom < 1 ? (1f / Globals.camZoom) * scale : scale));
				rect.Height = (int)((float)TileHeight * HeightToWidthRatio * (Globals.camZoom < 1 ? (1f / Globals.camZoom) * scale : scale));
			}
		}
		public static int round(double valueToRound)
		{
			return (int)Math.Floor(valueToRound);
		}
		public static float DegreeToRadian(double angle)
		{
			return (float)(Math.PI * angle / 180.0);
		}
		public static float DistBetween(Vector2 Location1, Vector2 Location2)
		{
			float dx = Location1.X - Location2.X;
			float dy = Location1.Y - Location2.Y;
			return (float)Math.Sqrt(dx * dx + dy * dy);
		}

		internal static void ShowOptions()
		{
			if (optionsOpen)
				return;
			optionsOpen = true;
			optWin = new Options();
			optWin.Show();
		}
		public static void ResetMapCollector(GraphicsDevice gd, string worldPath = "")
		{
			if (Globals.chunks.collector != null)
				Globals.chunks.collector.Stop(true);

			if (!AMSettings.bDisableStaticMap)
			{
				Globals.chunks.collector = new MapCollector();
				Globals.chunks.collector.gd = gd;
				FileInfo fiMap = new FileInfo(Globals.getStaticMapName());

				//// This feature is unavailable because the map format was changed.
				//if (!fiMap.Exists && !string.IsNullOrEmpty(worldPath) && !Globals.multiplayer)
				//    ParseWorldMap(worldPath);
				//else
				//    Globals.mcrLoader.bAbort = true; // This stops any parsing that may have still be going on from a previous world.
			}
		}

		public static void ParseWorldMap(string worldPath, bool overrideMultiplayerLockout = false)
		{
			// This feature is unavailable because the map format was changed.
			return;
			//if ((Globals.multiplayer && !overrideMultiplayerLockout) || AMSettings.bDisableStaticMap)
			//    return;
			//Globals.mcrLoader.bAbort = true;
			//Globals.mcrLoader = new McrLoader();
			//Globals.mcrLoader.LoadWorld(Globals.chunks.collector, worldPath);
		}

		public static string getStaticMapName(bool oldFormat = false)
		{
			return (oldFormat ? "" : "Map Data/") + worldID + (AMSettings.iInternalStaticMapGeneration == 3 ? "_amsm_v3" : "") + worldMapImageExtension;
		}
	}
}
