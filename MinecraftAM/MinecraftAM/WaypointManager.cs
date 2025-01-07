using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using DataSource;

namespace MinecraftAM
{
	public class WaypointManager
	{
		public delegate void WaypointAddedEventHandler(Entity waypoint);
		public static event WaypointAddedEventHandler WaypointAdded;
		private static void RaiseWaypointAdded(Entity waypoint)
		{
			if (WaypointAdded != null)
				WaypointAdded(waypoint);
		}

		private static Vector2 balloonOrigin = new Vector2(16, 64);
		private static Vector2 waypointPointerOrigin = new Vector2(1.5f, 100);
		private static MapLabel waypointLabel = new MapLabel();
		private static Texture2D balloon64;
		private static Texture2D WaypointPointer;
		public static List<Entity> lWaypoints = new List<Entity>();
		public static void LoadContent(ContentManager Content)
		{
			balloon64 = Content.Load<Texture2D>("balloon64");
			WaypointPointer = Content.Load<Texture2D>("WaypointPointer");
		}
		public static void NewWaypoint()
		{
			InputBox.Show("Create a waypoint", "Please provide a waypoint name:", WaypointNameEntered);
		}
		public static void WaypointNameEntered(InputResult ir)
		{
			if (ir.WasCancelled || string.IsNullOrEmpty(ir.Input))
				return;
			Load();
			Entity newWaypoint = new Entity();
			newWaypoint.name = ir.Input;
			newWaypoint.x = Globals.userPlayer.x;
			newWaypoint.y = Globals.userPlayer.y;
			newWaypoint.z = Globals.userPlayer.z;
			newWaypoint.Calc();
			lWaypoints.Add(newWaypoint);
			lWaypoints.Sort();
			Save();
			RaiseWaypointAdded(newWaypoint);
		}
		public static void Load()
		{
			StreamReader sr = null;
			try
			{
				if (File.Exists(GetFileName(true)))
				{
					if (File.Exists(GetFileName()))
						File.Delete(GetFileName());
					File.Move(GetFileName(true), GetFileName());
				}
				FileInfo file = new FileInfo(GetFileName());
				if (!file.Exists)
					return;
				sr = new StreamReader(file.FullName);
				lock (lWaypoints)
				{
					lWaypoints.Clear();
					while (!sr.EndOfStream)
					{
						string line = sr.ReadLine();
						int idxFirstEquals = line.IndexOf('=');
						if (idxFirstEquals == -1)
							continue;
						string name = line.Substring(0, idxFirstEquals);
						string[] coordsColor = line.Substring(idxFirstEquals + 1).Split(new char[] { ';' });
						Color color = Color.Orange;
						if (coordsColor.Length > 1)
							color = AMSettings.GetColor(coordsColor[1], color);
						else if (coordsColor.Length < 1)
							continue;
						string[] coords = coordsColor[0].Split(new char[] { ',' });
						if (coords.Length != 3)
							continue;
						double x = double.Parse(coords[0], AMSettings.AutoMapCulture);
						double y = double.Parse(coords[1], AMSettings.AutoMapCulture);
						double z = double.Parse(coords[2], AMSettings.AutoMapCulture);
						Entity waypoint = new Entity();
						waypoint.name = name;
						waypoint.x = x;
						waypoint.y = y;
						waypoint.z = z;
						waypoint.color = color;
						waypoint.Calc();

						lWaypoints.Add(waypoint);
					}
					lWaypoints.Sort();
				}
			}
			catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.ToString()); }
			finally
			{
				if (sr != null)
					sr.Close();
			}
		}

		public static void Save()
		{
			StreamWriter sw = null;
			try
			{
				sw = new StreamWriter(GetFileName());
				lock (lWaypoints)
				{
					for (int i = 0; i < lWaypoints.Count; i++)
						sw.WriteLine(lWaypoints[i].name + "=" + lWaypoints[i].x.ToString(AMSettings.AutoMapCulture) + "," + lWaypoints[i].y.ToString(AMSettings.AutoMapCulture) + "," + lWaypoints[i].z.ToString(AMSettings.AutoMapCulture) + ";" + AMSettings.FromColor(lWaypoints[i].color));
				}
			}
			catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.ToString()); }
			finally
			{
				if (sw != null)
					sw.Close();
			}
		}
		private static Rectangle reusableRectangle = new Rectangle();
		public static void Draw(SpriteBatch spriteBatch, SpriteFont toolTipFont, float scale)
		{
			if (!AMSettings.bShowWaypoints) return;

			float HWRatioPointer = 100.0f / 3.0f;
			foreach (Entity w in lWaypoints)
			{
				float rotation;
				if (Globals.userPlayer == null)
					rotation = AMSettings.bRotate ? 0 : -Globals.DegreeToRadian(AMSettings.iDefaultMapRotation);
				else
					rotation = AMSettings.bRotate ? (float)Globals.userPlayer.rotation : -Globals.DegreeToRadian(AMSettings.iDefaultMapRotation);
				Globals.getPlayerWorldLocation(ref reusableRectangle, w, 0.75f, 2);
				spriteBatch.Draw(balloon64, reusableRectangle, null, w.color, rotation, balloonOrigin, SpriteEffects.None, 0.0f);
				if (AMSettings.bDrawWaypointLines && Globals.userPlayer != null)
				{
					float desiredAngle = (float)(Math.Atan2(w.y - Globals.userPlayer.y, w.x - Globals.userPlayer.x) + Math.PI / 2);
					Globals.getPlayerWorldLocation(ref reusableRectangle, Globals.userPlayer, 0.1f, HWRatioPointer);
					spriteBatch.Draw(WaypointPointer, reusableRectangle, null, w.color, desiredAngle, waypointPointerOrigin, SpriteEffects.None, 0.0f);
				}
				waypointLabel.Draw(spriteBatch,
					w.name + " (" + w.iz + ")",
					(int)w.pixelx,
					(int)w.pixely,
					scale,
					rotation, toolTipFont, MapLabel.Style.OrangeRed, 0, 20, w.color);
			}
		}
		private static float TurnToFace(Vector2 position, Vector2 faceThis)
		{

			float x = faceThis.X - position.X;
			float y = faceThis.Y - position.Y;

			// we'll use the Atan2 function. Atan will calculates the arc tangent of 
			// y / x for us, and has the added benefit that it will use the signs of x
			// and y to determine what cartesian quadrant to put the result in.
			// http://msdn2.microsoft.com/en-us/library/system.math.atan2.aspx
			float desiredAngle = (float)Math.Atan2(y, x);

			// so, the closest we can get to our target is currentAngle + difference.
			// return that, using WrapAngle again.
			return WrapAngle(desiredAngle);
		}

		/// <summary>
		/// Returns the angle expressed in radians between -Pi and Pi.
		/// </summary>
		private static float WrapAngle(float radians)
		{
			while (radians < -MathHelper.Pi)
			{
				radians += MathHelper.TwoPi;
			}
			while (radians > MathHelper.Pi)
			{
				radians -= MathHelper.TwoPi;
			}
			return radians;
		}
		#region Helpers
		private static string GetFileName(bool oldFormat = false)
		{
			if (oldFormat)
				return Globals.worldID + "Waypoints.txt";
			return "Map Data/" + Globals.worldID + "Waypoints.txt";
		}
		#endregion
	}
}
