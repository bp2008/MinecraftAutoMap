using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace MinecraftAM
{
	public class PowerToys
	{
		static List<TeleportHistory> tpHistory = new List<TeleportHistory>();
		class TeleportHistory
		{
			private float dist;
			private DateTime time;

			public TeleportHistory(float desiredTpDistance, DateTime dateTime)
			{
				this.dist = desiredTpDistance;
				this.time = dateTime;
			}
			public float effectiveDistance
			{
				get
				{
					float timeFactor = (float)(DateTime.Now - time).TotalMilliseconds / 1000f;
					if (timeFactor < 0) timeFactor = 0;
					else if (timeFactor > 1) timeFactor = 1;
					timeFactor = 1f - timeFactor;
					float effDist = dist * timeFactor;
					return effDist;
				}
			}
		}

		public static void HandleMiddleClick(MouseState ms)
		{
			Vector2 blockCoordinates = Bicycle.instance.ClientPointToBlockCoordinates(new Point(ms.X, ms.Y));
			float x = Globals.PTB(blockCoordinates.X) - Chunk.chunkSizeM1;
			float y = Globals.PTB(blockCoordinates.Y);
			int ix = (int)Math.Floor(x);
			int iy = (int)Math.Floor(y);
			//Game1.lastMiddleClick = x + "," + y + " (" + (int)Math.Floor(x) + "," + (int)Math.Floor(y) + ")Mouse: " + ms.X + "," + ms.Y;

			double height = GetBlockHeight(ix, iy);
			switch (AMSettings.MiddleClickFunction)
			{
				case MiddleClickOptions.Teleport:
					if (height < 0) height = Globals.userPlayer.iz + 1;
					else height += 2.621;
						TryTeleport(x, y, (float)height);
					break;
				case MiddleClickOptions.Explode:
					TryExplodeHere(x, y, (float)height);
					break;
				case MiddleClickOptions.None:
				default:
					break;

			}
		}

		public static void TryExplodeHere(float x, float y, float height)
		{
			if (CanExplodeHere(x, y, height))
			{
				DataSource.DS.CreateExplosion(x, y, height, AMSettings.fExplosionPower);
			}
		}

		public static void TryTeleport(float x, float y, float height)
		{
			if (CanPerformThisTeleport(x, y, height))
			{
				DataSource.DS.SetPlayerLocation(x, y, height);
			}
		}

		private static bool CanExplodeHere(float x, float y, float height)
		{
			if (height < 0)
				return false;
			if (!Globals.multiplayer)
				return true;
			return true;
		}

		/// <summary>
		/// Gets the height of the highest visible block at this location.  If this cannot be determined, -1 is returned;
		/// </summary>
		/// <param name="ix"></param>
		/// <param name="iy"></param>
		/// <returns></returns>
		private static int GetBlockHeight(int ix, int iy)
		{
			int height = -1;
			if (Globals.flattener != null)
			{
				tilebuffer tbuffer = Globals.flattener.frontBuffer;
				if (tbuffer != null && tbuffer.buffer != null && tbuffer.topLeft != null)
				{
					int bx = (ix - tbuffer.topLeft.X) + Chunk.chunkSizeM1;
					int by = iy - tbuffer.topLeft.Y;
					if (bx >= 0 && bx < tbuffer.buffer.GetLength(0) && by >= 0 && by < tbuffer.buffer.GetLength(1))
					{
						MCTile tile = tbuffer.buffer[bx, by];
						while (tile.nextTile != null)
							tile = tile.nextTile;
						if (tile.height > 0)
							height = tile.height;
					}
				}
			}
			return height;
		}

		private static bool CanPerformThisTeleport(float x, float y, float height)
		{
			if (AMSettings.MiddleClickFunction != MiddleClickOptions.Teleport)
				return false;
			if (!Globals.multiplayer)
				return true;
			if (!AMSettings.bSMPTeleportProtection)
				return true;
			float previousTpDistances = 0;
			List<TeleportHistory> newTpHistory = new List<TeleportHistory>(tpHistory.Count);
			for (int i = 0; i < tpHistory.Count; i++)
			{
				float thisDist = tpHistory[i].effectiveDistance;
				if (thisDist > 0)
				{
					newTpHistory.Add(tpHistory[i]);
					previousTpDistances += thisDist;
				}
			}
			tpHistory = newTpHistory;
			float maxTpDistance = 9.5f - previousTpDistances;
			float desiredTpDistance = Vector3.Distance(new Vector3((float)Globals.userPlayer.x, (float)Globals.userPlayer.y, (float)Globals.userPlayer.z), new Vector3(x, y, height));
			if (desiredTpDistance <= maxTpDistance)
			{
				tpHistory.Add(new TeleportHistory(desiredTpDistance, DateTime.Now));
				return true;
			}
			return false;
		}
	}
}
