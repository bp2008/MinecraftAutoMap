using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MinecraftAM
{
	public class CoordinateTranslator
	{
		//public static IntVector3 toAMCoords(IntVector3 minecraftCoords)
		//{
		//    return new IntVector3(minecraftCoords.Z * -1, minecraftCoords.X, minecraftCoords.Y);
		//}
		//public static IntVector3 toMinecraftCoords(IntVector3 amCoords)
		//{
		//    return new IntVector3(amCoords.Y, amCoords.Z, amCoords.X * -1);
		//}
		public static Vector3 toAMCoords(Vector3 minecraftCoords)
		{
			return new Vector3(minecraftCoords.Z * -1, minecraftCoords.X, minecraftCoords.Y);
		}
		public static Vector3 toMinecraftCoords(Vector3 amCoords)
		{
			return new Vector3(amCoords.Y, amCoords.Z, amCoords.X * -1);
		}
		//public static IntVector3 translateFromAMCoordsAccordingToSettings(IntVector3 amCoords)
		//{
		//    if (AMSettings.bMinecraftCoordinateDisplay)
		//        return toMinecraftCoords(amCoords);
		//    return amCoords;
		//}
		public static Vector3 translateFromAMCoordsAccordingToSettings(Vector3 amCoords)
		{
			if (AMSettings.bMinecraftCoordinateDisplay)
				return toMinecraftCoords(amCoords);
			return amCoords;
		}

		public static Vector3 translateToAMCoordsAccordingToSettings(Vector3 coords)
		{
			if (AMSettings.bMinecraftCoordinateDisplay)
				return toAMCoords(coords);
			return coords;
		}
	}
}
