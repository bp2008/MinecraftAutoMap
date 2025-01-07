using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace MinecraftAM
{
	public enum MiddleClickOptions { None, Teleport, Explode };
	public static class AMSettings
	{
		private static FileInfo fiSettings = new FileInfo("MinecraftAMSettings.txt");
		public enum Priority
		{
			Low, BelowNormal, Normal
		}
		// This should be used in ToString(AMSettings.AutoMapCulture) at any time when writing decimal numbers into a comma separated list!
		public static CultureInfo AutoMapCulture = new CultureInfo("en-US");
		private static string internal_sNPCFilter = "";
		private static HashSet<string> internal_NPCFilter = new HashSet<string>();
		public static HashSet<string> NPCFilter
		{
			get
			{
				if (internal_sNPCFilter != sNPCFilter)
				{
					internal_sNPCFilter = sNPCFilter;
					internal_NPCFilter = StringToHashSet(internal_sNPCFilter);
				}
				return internal_NPCFilter;
			}
		}
		private static string internal_sItemFilter = "";
		private static HashSet<string> internal_ItemFilter = new HashSet<string>();
		public static HashSet<string> ItemFilter
		{
			get
			{
				if (internal_sItemFilter != sItemFilter)
				{
					internal_sItemFilter = sItemFilter;
					internal_ItemFilter = StringToHashSet(internal_sItemFilter);
				}
				return internal_ItemFilter;
			}
		}
		private static string internal_sOresList = "";
		private static HashSet<int> internal_OresList = new HashSet<int>();
		public static HashSet<int> OresList
		{
			get
			{
				if (internal_sOresList != sOresList)
				{
					internal_sOresList = sOresList;
					internal_OresList = StringToHashSetInt(internal_sOresList);
				}
				return internal_OresList;
			}
		}
		private static string internal_sMarkBlocks = "";
		private static HashSet<int> internal_MarkBlocks = new HashSet<int>();
		public static HashSet<int> MarkBlocks
		{
			get
			{
				if (internal_sMarkBlocks != sMarkBlocks)
				{
					internal_sMarkBlocks = sMarkBlocks;
					internal_MarkBlocks = StringToHashSetInt(internal_sMarkBlocks);
				}
				return internal_MarkBlocks;
			}
		}
		// Configurable Settings
		public static int iDrawingOffsetZ = 0;
		public static Priority ProcessPriority = Priority.BelowNormal;
		public static bool bLavaDetection = true;
		public static bool bWaterDetection = true;
		public static bool bOreDetection = false;
		public static bool bShowStatusText = true;
		public static bool bShowHotkeys = false;
		public static bool bDynamicMapEnabled = true;
		public static bool bDynamicMapHiddenZoomedOut = false;
		public static string sLavaDetection = "";
		public static string sWaterDetection = "";
		public static string sOreDetection = "M";
		public static string sShowStatusText = "S";
		public static string sShowHotkeys = "H";
		public static string sDynamicMapEnabled = "";
		public static string sShowOptions = "O";
		public static string sCI = "=";
		public static string sCD = "-";
		public static string sCR = "0";
		public static string sRotate = "R";
		public static string sCompass = "C";
		public static string sStick = "F";
		private static int iDynamicMapMaxPrivateX = 254;
		private static int iDynamicMapMinPrivateX = 160;
		public static int iOutdoorCeiling = 40;
		public static int iMaxCeilingHeight = 10;
		public static int iIndoorDepth = 30;
		public static int iOutdoorDepth = 70;
		public static bool bRotate = false;
		public static bool bCompass = true;
		public static bool bGlowingPath = false;
		public static string sGlowingPath = "G";
		public static string sClearGlowingPath = "P";
		public static int iGlowingIntensity = 75; // amount of red color to add to visited squares.
		public static string sUpdateStaticTerrain = "";
		public static string sResetRealtimeMap = "X";
		public static Microsoft.Xna.Framework.Color cBackgroundColor = GetColor("79,79,79,255", Microsoft.Xna.Framework.Color.Gray);
		public static Microsoft.Xna.Framework.Color cNotDrawnBlockColor = GetColor("47,47,47,255", Microsoft.Xna.Framework.Color.Gray);
		public static int iDefaultMapRotation = 0;
		public static string sHostName = "localhost";
		public static bool bShowHostileNPCs = true;
		public static string sShowHostileNPCs = "B";
		public static bool bShowPassiveNPCs = true;
		public static string sShowPassiveNPCs = "N";
		public static int iCreeperProximityAlert = 10;
		public static bool bHideDistantNPCs = true;
		public static bool bShowWaypoints = true;
		public static string sShowWaypoints = "K";
		public static string sNewWaypoint = "J";
		public static float fZoomLevel = 1;
		public static int iWndX = 100;
		public static int iWndY = 100;
		public static int iWndW = 400;
		public static int iWndH = 300;
		public static bool bBorderlessMode = false;
		public static bool bDrawWaypointLines = true;
		public static string sDrawWaypointLines = "I";
		public static string sNPCFilter = "";
		public static string sItemFilter = "";
		public static string sOresList = "";
		public static string sMarkBlocks = "";
		public static ushort iPortTCP = 50191;
		public static int iDroppedItemVisibility = 3;
		public static int iProjectileVisibility = 1;
		public static int iVehicleVisibility = 2;
		public static int iStaticMapUpdateInterval = 60;
		public static bool bDisableStaticMapStreaming = false;
		public static bool bDisableStaticMap = false;
		public static bool bDrawPaintings = true;
		public static bool bShowOreHeightRelative = true;
		public static string sShowOreHeightRelative = ",";
		public static bool bShowIngameLighting = false;
		public static int iWarningLightLevel = -1;
		public static int iStaticMapBitsPerPixel = 24;
		public static bool bMaximized = false;
		public static bool bSMPTeleportProtection = true;
		public static MiddleClickOptions MiddleClickFunction = MiddleClickOptions.None;
		public static float fExplosionPower = 3.0f;
		public static bool bMinecraftCoordinateDisplay = true;
		public static int iTransparentLiquidMax = 0;
		public static int iNetherStaticMapHeightStart = -1;
		/// <summary>
		/// DO NOT USE THIS for static map logic.  Use the internal one instead which is only updated at app load.
		/// </summary>
		public static int iStaticMapGeneration = 3;
		public static int iInternalStaticMapGeneration = 3;
		public static string sMinecraftJar = "";
		/// <summary>
		/// Current Version
		/// </summary>
		public static string sVersionCurrent = "0.7.9.2";
		public static string sVersionToHide = "0.6.0.0"; // No need to update this.
		public static string sWebsiteAddress = "http://www.minecraftam.com/";
		public static string sAutoUpdateCheckUrl = "http://www.minecraftam.com/updates/currentam.txt";
		public static bool bDisableAutomaticUpdateChecking = false;
		public static int iDynamicMapMaxX
		{
			get
			{
				return iDynamicMapMaxPrivateX;
			}
			set
			{
				if (value > 254)
					iDynamicMapMaxPrivateX = 254;
				else if (value < 10)
					iDynamicMapMaxPrivateX = 10;
				else
					iDynamicMapMaxPrivateX = value;
			}
		}
		public static int iDynamicMapMinX
		{
			get
			{
				return iDynamicMapMinPrivateX;
			}
			set
			{
				if (value > 254)
					iDynamicMapMinPrivateX = 254;
				else if (value < 10)
					iDynamicMapMinPrivateX = 10;
				else
					iDynamicMapMinPrivateX = value;
			}
		}
		public static void Load()
		{
			StreamReader sr = null;
			try
			{
				if (fiSettings.Exists)
				{
					sr = new StreamReader(fiSettings.FullName);
					string sFull = sr.ReadToEnd();
					sr.Close();
					string[] parts = sFull.Split(new char[] { '\r', '\n' });
					for (int i = 0; i < parts.Length; i++)
					{
						try
						{
							String[] keyAndValue = parts[i].Split('=');
							if (keyAndValue.Length >= 2)
							{
								if (keyAndValue.Length > 2)
									if (keyAndValue[1].Length == 0 && keyAndValue[2].Length == 0)
										keyAndValue[1] = "=";
								if (keyAndValue[0] == "iDrawingOffsetZ")
									iDrawingOffsetZ = int.Parse(keyAndValue[1]);
								else if (keyAndValue[0] == "ProcessPriority")
									ProcessPriority = (Priority)int.Parse(keyAndValue[1]);
								else if (keyAndValue[0] == "bLavaDetection")
									bLavaDetection = ParseBool(keyAndValue[1]);
								else if (keyAndValue[0] == "bWaterDetection")
									bWaterDetection = ParseBool(keyAndValue[1]);
								else if (keyAndValue[0] == "bOreDetection")
									bOreDetection = ParseBool(keyAndValue[1]);
								else if (keyAndValue[0] == "bShowStatusText")
									bShowStatusText = ParseBool(keyAndValue[1]);
								else if (keyAndValue[0] == "bShowHotkeys")
									bShowHotkeys = ParseBool(keyAndValue[1]);
								else if (keyAndValue[0] == "bDynamicMapEnabled")
									bDynamicMapEnabled = ParseBool(keyAndValue[1]);
								else if (keyAndValue[0] == "bDynamicMapHiddenZoomedOut")
									bDynamicMapHiddenZoomedOut = ParseBool(keyAndValue[1]);
								else if (keyAndValue[0] == "iDynamicMapMaxX")
									iDynamicMapMaxX = int.Parse(keyAndValue[1]);
								else if (keyAndValue[0] == "iDynamicMapMinX")
									iDynamicMapMinX = int.Parse(keyAndValue[1]);
								else if (keyAndValue[0] == "sLavaDetection")
									sLavaDetection = keyAndValue[1];
								else if (keyAndValue[0] == "sWaterDetection")
									sWaterDetection = keyAndValue[1];
								else if (keyAndValue[0] == "sOreDetection")
									sOreDetection = keyAndValue[1];
								else if (keyAndValue[0] == "sShowStatusText")
									sShowStatusText = keyAndValue[1];
								else if (keyAndValue[0] == "sShowHotkeys")
									sShowHotkeys = keyAndValue[1];
								else if (keyAndValue[0] == "sDynamicMapEnabled")
									sDynamicMapEnabled = keyAndValue[1];
								else if (keyAndValue[0] == "sShowOptions")
									sShowOptions = keyAndValue[1];
								else if (keyAndValue[0] == "sCI")
									sCI = keyAndValue[1];
								else if (keyAndValue[0] == "sCD")
									sCD = keyAndValue[1];
								else if (keyAndValue[0] == "sCR")
									sCR = keyAndValue[1];
								else if (keyAndValue[0] == "sRotate")
									sRotate = keyAndValue[1];
								else if (keyAndValue[0] == "sCompass")
									sCompass = keyAndValue[1];
								else if (keyAndValue[0] == "sStick")
									sStick = keyAndValue[1];
								else if (keyAndValue[0] == "iIndoorDepth")
									iIndoorDepth = int.Parse(keyAndValue[1]);
								else if (keyAndValue[0] == "iOutdoorCeiling")
									iOutdoorCeiling = int.Parse(keyAndValue[1]);
								else if (keyAndValue[0] == "iMaxCeilingHeight")
									iMaxCeilingHeight = int.Parse(keyAndValue[1]);
								else if (keyAndValue[0] == "iOutdoorDepth")
									iOutdoorDepth = int.Parse(keyAndValue[1]);
								else if (keyAndValue[0] == "bRotate")
									bRotate = ParseBool(keyAndValue[1]);
								else if (keyAndValue[0] == "bCompass")
									bCompass = ParseBool(keyAndValue[1]);
								else if (keyAndValue[0] == "bGlowingPath")
									bGlowingPath = ParseBool(keyAndValue[1]);
								else if (keyAndValue[0] == "sGlowingPath")
									sGlowingPath = keyAndValue[1];
								else if (keyAndValue[0] == "sClearGlowingPath")
									sClearGlowingPath = keyAndValue[1];
								else if (keyAndValue[0] == "iGlowingIntensity")
									iGlowingIntensity = int.Parse(keyAndValue[1]);
								else if (keyAndValue[0] == "sUpdateStaticTerrain")
									sUpdateStaticTerrain = keyAndValue[1];
								else if (keyAndValue[0] == "sResetRealtimeMap")
									sResetRealtimeMap = keyAndValue[1];
								else if (keyAndValue[0] == "sHostName")
									sHostName = keyAndValue[1];
								else if (keyAndValue[0] == "cBackgroundColor")
									cBackgroundColor = GetColor(keyAndValue[1], cBackgroundColor);
								else if (keyAndValue[0] == "cNotDrawnBlockColor")
									cNotDrawnBlockColor = GetColor(keyAndValue[1], cNotDrawnBlockColor);
								else if (keyAndValue[0] == "iDefaultMapRotation")
									iDefaultMapRotation = int.Parse(keyAndValue[1]);
								else if (keyAndValue[0] == "bShowHostileNPCs")
									bShowHostileNPCs = ParseBool(keyAndValue[1]);
								else if (keyAndValue[0] == "sShowHostileNPCs")
									sShowHostileNPCs = keyAndValue[1];
								else if (keyAndValue[0] == "bShowPassiveNPCs")
									bShowPassiveNPCs = ParseBool(keyAndValue[1]);
								else if (keyAndValue[0] == "sShowPassiveNPCs")
									sShowPassiveNPCs = keyAndValue[1];
								else if (keyAndValue[0] == "iCreeperProximityAlert")
									iCreeperProximityAlert = int.Parse(keyAndValue[1]);
								else if (keyAndValue[0] == "bHideDistantNPCs")
									bHideDistantNPCs = ParseBool(keyAndValue[1]);
								else if (keyAndValue[0] == "bShowWaypoints")
									bShowWaypoints = ParseBool(keyAndValue[1]);
								else if (keyAndValue[0] == "sShowWaypoints")
									sShowWaypoints = keyAndValue[1];
								else if (keyAndValue[0] == "sNewWaypoint")
									sNewWaypoint = keyAndValue[1];
								else if (keyAndValue[0] == "fZoomLevel")
									fZoomLevel = float.Parse(keyAndValue[1], AutoMapCulture);
								else if (keyAndValue[0] == "iWndX")
									iWndX = int.Parse(keyAndValue[1]);
								else if (keyAndValue[0] == "iWndY")
									iWndY = int.Parse(keyAndValue[1]);
								else if (keyAndValue[0] == "iWndW")
									iWndW = int.Parse(keyAndValue[1]);
								else if (keyAndValue[0] == "iWndH")
									iWndH = int.Parse(keyAndValue[1]);
								else if (keyAndValue[0] == "bBorderlessMode")
									bBorderlessMode = ParseBool(keyAndValue[1]);
								else if (keyAndValue[0] == "bDrawWaypointLines")
									bDrawWaypointLines = ParseBool(keyAndValue[1]);
								else if (keyAndValue[0] == "sDrawWaypointLines")
									sDrawWaypointLines = keyAndValue[1];
								else if (keyAndValue[0] == "sNPCFilter")
									sNPCFilter = keyAndValue[1];
								else if (keyAndValue[0] == "sItemFilter")
									sItemFilter = keyAndValue[1];
								else if (keyAndValue[0] == "sOresList")
									sOresList = keyAndValue[1];
								else if (keyAndValue[0] == "sMarkBlocks")
									sMarkBlocks = keyAndValue[1];
								//	else if (keyAndValue[0] == "iPortTCP")
								//		iPortTCP = ushort.Parse(keyAndValue[1]);
								else if (keyAndValue[0] == "iDroppedItemVisibility")
									iDroppedItemVisibility = int.Parse(keyAndValue[1]);
								else if (keyAndValue[0] == "iProjectileVisibility")
									iProjectileVisibility = int.Parse(keyAndValue[1]);
								else if (keyAndValue[0] == "iVehicleVisibility")
									iVehicleVisibility = int.Parse(keyAndValue[1]);
								else if (keyAndValue[0] == "iStaticMapUpdateInterval")
									iStaticMapUpdateInterval = int.Parse(keyAndValue[1]);
								else if (keyAndValue[0] == "bDisableStaticMapStreaming")
									bDisableStaticMapStreaming = ParseBool(keyAndValue[1]);
								else if (keyAndValue[0] == "bDisableStaticMap")
									bDisableStaticMap = ParseBool(keyAndValue[1]);
								else if (keyAndValue[0] == "bDrawPaintings")
									bDrawPaintings = ParseBool(keyAndValue[1]);
								else if (keyAndValue[0] == "bShowOreHeightRelative")
									bShowOreHeightRelative = ParseBool(keyAndValue[1]);
								else if (keyAndValue[0] == "sShowOreHeightRelative")
									sShowOreHeightRelative = keyAndValue[1];
								else if (keyAndValue[0] == "bDisableAutomaticUpdateChecking")
									bDisableAutomaticUpdateChecking = ParseBool(keyAndValue[1]);
								else if (keyAndValue[0] == "sVersionToHide")
									sVersionToHide = keyAndValue[1];
								else if (keyAndValue[0] == "bShowIngameLighting")
									bShowIngameLighting = ParseBool(keyAndValue[1]);
								else if (keyAndValue[0] == "iWarningLightLevel")
									iWarningLightLevel = int.Parse(keyAndValue[1]);
								else if (keyAndValue[0] == "iStaticMapBitsPerPixel")
									iStaticMapBitsPerPixel = EnforceBitsPerPixel(int.Parse(keyAndValue[1]));
								else if (keyAndValue[0] == "bMaximized")
									bMaximized = ParseBool(keyAndValue[1]);
								else if (keyAndValue[0] == "bSMPTeleportProtection")
									bSMPTeleportProtection = ParseBool(keyAndValue[1]);
								else if (keyAndValue[0] == "MiddleClickFunction")
									MiddleClickFunction = (MiddleClickOptions)Enum.Parse(typeof(MiddleClickOptions), keyAndValue[1]);
								else if (keyAndValue[0] == "fExplosionPower")
									fExplosionPower = float.Parse(keyAndValue[1], AutoMapCulture);
								else if (keyAndValue[0] == "bMinecraftCoordinateDisplay")
									bMinecraftCoordinateDisplay = ParseBool(keyAndValue[1]);
								else if (keyAndValue[0] == "iStaticMapGeneration")
									iInternalStaticMapGeneration = iStaticMapGeneration = int.Parse(keyAndValue[1]);
								else if (keyAndValue[0] == "iTransparentLiquidMax")
									iTransparentLiquidMax = int.Parse(keyAndValue[1]);
								else if (keyAndValue[0] == "iNetherStaticMapHeightStart")
									iNetherStaticMapHeightStart = int.Parse(keyAndValue[1]);
								else if (keyAndValue[0] == "sMinecraftJar")
									sMinecraftJar = keyAndValue[1];
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message + " " + ex.StackTrace);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message + " " + ex.StackTrace);
			}
			finally
			{
				try
				{
					if (sr != null)
						sr.Close();
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message + " " + ex.StackTrace);
				}
			}
			if (iDynamicMapMaxPrivateX < iDynamicMapMinPrivateX)
				iDynamicMapMaxPrivateX = iDynamicMapMinPrivateX;
		}

		public static void Save()
		{
			StreamWriter sw = null;
			try
			{
				sw = new StreamWriter(fiSettings.FullName, false);
				sw.WriteLine("iDrawingOffsetZ=" + iDrawingOffsetZ);
				sw.WriteLine("ProcessPriority=" + (int)ProcessPriority);
				sw.WriteLine("bLavaDetection=" + bLavaDetection);
				sw.WriteLine("bWaterDetection=" + bWaterDetection);
				sw.WriteLine("bOreDetection=" + bOreDetection);
				sw.WriteLine("bShowStatusText=" + bShowStatusText);
				sw.WriteLine("bShowHotkeys=" + bShowHotkeys);
				sw.WriteLine("bDynamicMapEnabled=" + bDynamicMapEnabled);
				sw.WriteLine("bDynamicMapHiddenZoomedOut=" + bDynamicMapHiddenZoomedOut);
				sw.WriteLine("iDynamicMapMaxX=" + iDynamicMapMaxX);
				sw.WriteLine("iDynamicMapMinX=" + iDynamicMapMinX);
				sw.WriteLine("sLavaDetection=" + sLavaDetection);
				sw.WriteLine("sWaterDetection=" + sWaterDetection);
				sw.WriteLine("sOreDetection=" + sOreDetection);
				sw.WriteLine("sShowStatusText=" + sShowStatusText);
				sw.WriteLine("sShowHotkeys=" + sShowHotkeys);
				sw.WriteLine("sDynamicMapEnabled=" + sDynamicMapEnabled);
				sw.WriteLine("sShowOptions=" + sShowOptions);
				sw.WriteLine("sCI=" + sCI);
				sw.WriteLine("sCD=" + sCD);
				sw.WriteLine("sCR=" + sCR);
				sw.WriteLine("sRotate=" + sRotate);
				sw.WriteLine("sCompass=" + sCompass);
				sw.WriteLine("sStick=" + sStick);
				sw.WriteLine("iIndoorDepth=" + iIndoorDepth);
				sw.WriteLine("iOutdoorCeiling=" + iOutdoorCeiling);
				sw.WriteLine("iMaxCeilingHeight=" + iMaxCeilingHeight);
				sw.WriteLine("iOutdoorDepth=" + iOutdoorDepth);
				sw.WriteLine("bRotate=" + bRotate);
				sw.WriteLine("bCompass=" + bCompass);
				sw.WriteLine("bGlowingPath=" + bGlowingPath);
				sw.WriteLine("sGlowingPath=" + sGlowingPath);
				sw.WriteLine("sClearGlowingPath=" + sClearGlowingPath);
				sw.WriteLine("iGlowingIntensity=" + iGlowingIntensity);
				sw.WriteLine("sUpdateStaticTerrain=" + sUpdateStaticTerrain);
				sw.WriteLine("sResetRealtimeMap=" + sResetRealtimeMap);
				sw.WriteLine("sHostName=" + sHostName);
				sw.WriteLine("cBackgroundColor=" + FromColor(cBackgroundColor));
				sw.WriteLine("cNotDrawnBlockColor=" + FromColor(cNotDrawnBlockColor));
				sw.WriteLine("iDefaultMapRotation=" + iDefaultMapRotation);
				sw.WriteLine("bShowHostileNPCs=" + bShowHostileNPCs);
				sw.WriteLine("sShowHostileNPCs=" + sShowHostileNPCs);
				sw.WriteLine("bShowPassiveNPCs=" + bShowPassiveNPCs);
				sw.WriteLine("sShowPassiveNPCs=" + sShowPassiveNPCs);
				sw.WriteLine("iCreeperProximityAlert=" + iCreeperProximityAlert);
				sw.WriteLine("bHideDistantNPCs=" + bHideDistantNPCs);
				sw.WriteLine("bShowWaypoints=" + bShowWaypoints);
				sw.WriteLine("sShowWaypoints=" + sShowWaypoints);
				sw.WriteLine("sNewWaypoint=" + sNewWaypoint);
				sw.WriteLine("fZoomLevel=" + fZoomLevel.ToString(AutoMapCulture));
				sw.WriteLine("iWndX=" + iWndX);
				sw.WriteLine("iWndY=" + iWndY);
				sw.WriteLine("iWndW=" + iWndW);
				sw.WriteLine("iWndH=" + iWndH);
				sw.WriteLine("bBorderlessMode=" + bBorderlessMode);
				sw.WriteLine("bDrawWaypointLines=" + bDrawWaypointLines);
				sw.WriteLine("sDrawWaypointLines=" + sDrawWaypointLines);
				sw.WriteLine("sNPCFilter=" + sNPCFilter);
				sw.WriteLine("sItemFilter=" + sItemFilter);
				sw.WriteLine("sOresList=" + sOresList);
				sw.WriteLine("sMarkBlocks=" + sMarkBlocks);
				//sw.WriteLine("iPortTCP=" + iPortTCP);
				sw.WriteLine("iDroppedItemVisibility=" + iDroppedItemVisibility);
				sw.WriteLine("iProjectileVisibility=" + iProjectileVisibility);
				sw.WriteLine("iVehicleVisibility=" + iVehicleVisibility);
				sw.WriteLine("iStaticMapUpdateInterval=" + iStaticMapUpdateInterval);
				sw.WriteLine("bDisableStaticMapStreaming=" + bDisableStaticMapStreaming);
				sw.WriteLine("bDisableStaticMap=" + bDisableStaticMap);
				sw.WriteLine("bDrawPaintings=" + bDrawPaintings);
				sw.WriteLine("bShowOreHeightRelative=" + bShowOreHeightRelative);
				sw.WriteLine("sShowOreHeightRelative=" + sShowOreHeightRelative);
				sw.WriteLine("bDisableAutomaticUpdateChecking=" + bDisableAutomaticUpdateChecking);
				sw.WriteLine("sVersionToHide=" + sVersionToHide);
				sw.WriteLine("bShowIngameLighting=" + bShowIngameLighting);
				sw.WriteLine("iWarningLightLevel=" + iWarningLightLevel);
				sw.WriteLine("iStaticMapBitsPerPixel=" + iStaticMapBitsPerPixel);
				sw.WriteLine("bMaximized=" + bMaximized);
				sw.WriteLine("bSMPTeleportProtection=" + bSMPTeleportProtection);
				sw.WriteLine("MiddleClickFunction=" + Enum.GetName(typeof(MiddleClickOptions), MiddleClickFunction));
				sw.WriteLine("fExplosionPower=" + fExplosionPower.ToString(AutoMapCulture));
				sw.WriteLine("bMinecraftCoordinateDisplay=" + bMinecraftCoordinateDisplay);
				sw.WriteLine("iStaticMapGeneration=" + iStaticMapGeneration);
				sw.WriteLine("iTransparentLiquidMax=" + iTransparentLiquidMax);
				sw.WriteLine("iNetherStaticMapHeightStart=" + iNetherStaticMapHeightStart);
				sw.WriteLine("sMinecraftJar=" + sMinecraftJar);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message + " " + ex.StackTrace);
			}
			finally
			{
				try
				{
					if (sw != null)
						sw.Close();
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message + " " + ex.StackTrace);
				}
			}
		}

		private static bool ParseBool(string str)
		{
			return str == "1" || str.ToLower() == "true";
		}
		public static char[] splitchars = new char[] { ',', ' ', '.', '/', '\\', '\'', '"', ';', ':', '<', '>' };
		/// <summary>
		/// Takes a string containing a comma separated list of 4 byte values (0-255) indicating the R,G,B,A values of a color.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="defaultColor"></param>
		/// <returns></returns>
		internal static Microsoft.Xna.Framework.Color GetColor(string str, Microsoft.Xna.Framework.Color defaultColor)
		{
			try
			{
				if (splitchars == null)
					splitchars = new char[] { ',', ' ', '.', '/', '\\', '\'', '"', ';', ':', '<', '>' };
				// Lets check for a wide variety of separators in case people do what people do.
				string[] strs = str.Split(splitchars);
				return new Microsoft.Xna.Framework.Color(int.Parse(strs[0]), int.Parse(strs[1]), int.Parse(strs[2]), int.Parse(strs[3]));
			}
			catch (Exception)
			{
				return defaultColor;
			}
		}
		internal static string FromColor(Microsoft.Xna.Framework.Color col)
		{
			return col.R.ToString() + ',' + col.G.ToString() + ',' + col.B.ToString() + ',' + col.A.ToString();
		}

		internal static HashSet<string> StringToHashSet(string str)
		{
			HashSet<string> hs = new HashSet<string>();
			string[] strs = str.Split(',');
			for (int i = 0; i < strs.Length; i++)
				hs.Add(strs[i].Trim().ToLower());
			return hs;
		}
		internal static HashSet<int> StringToHashSetInt(string str)
		{
			HashSet<int> hs = new HashSet<int>();
			string[] strs = str.Split(',');
			for (int i = 0; i < strs.Length; i++)
				try
				{
					hs.Add(int.Parse(strs[i].Trim().ToLower()));
				}
				catch (Exception) { }
			return hs;
		}

		private static int EnforceBitsPerPixel(int bpp)
		{
			if (bpp <= 16)
				bpp = 16;
			else if (bpp >= 32)
				bpp = 32;
			else
				bpp = 24;
			return bpp;
		}
	}
}
