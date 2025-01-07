using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MinecraftAM
{
	public class Bicycle
	{
		public static Bicycle instance = new Bicycle();
		private Bicycle()
		{
		}
		public Vector2 ClientPointToBlockCoordinates(Point point)
		{
			Matrix m = MAM2DLayer.CreateMatrix();
			Vector2 blockCoords = Vector2.Transform(new Vector2(point.X, point.Y), Matrix.Invert(m));
			return blockCoords;
		}
	}
}
