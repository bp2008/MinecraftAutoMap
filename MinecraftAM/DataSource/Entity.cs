using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace DataSource
{
	public class Entity : IComparable
	{
		public enum EntityType { Unknown, Hostile, Passive, Arrow, Snowball, Item, Block, Painting, PhysicsObj, Minecart, Boat, Waypoint, XPOrb, Horse };
		public static int chunkSizeM1;
		public static int tileWidth;
		public static int tileHeight;
		public static int textureSize = 16;
		public static int itemTextureSize = 16;
		public static int[] texNum;
		public int itemID = 0;
		public string name = "default";
		private string _nameWithoutTags = null;
		public string nameWithoutTags
		{
			get
			{
				if (_nameWithoutTags == null)
				{
					int idxSpace = name.IndexOf(' ');
					if (idxSpace == -1)
						_nameWithoutTags = name;
					else
						_nameWithoutTags = name.Remove(idxSpace);
				}
				return _nameWithoutTags;
			}
		}
		public double x = 0;
		public double y = 0;
		public double z = 0;
		public int ix = 0;
		public int iy = 0;
		public int iz = 0;
		public double pixelx = 0;
		public double pixely = 0;
		public double rotation = 0;
		public float pitch = 0;
		public bool isNPC = false;
		public int itemCount = 0; // Used by items and dropped blocks
		public Microsoft.Xna.Framework.Color color = Microsoft.Xna.Framework.Color.Orange;
		/// <summary>
		/// If true, this entitiy is far enough away that it should not be drawn if distant NPCs are set to be hidden.  This value must be set in the appropriate entities after each entity list is received.
		/// </summary>
		public bool isFarFromMainPlayer = false;
		private EntityType entityTypeInner = EntityType.Unknown;
		public EntityType entityType
		{
			get
			{
				if (entityTypeInner == EntityType.Unknown)
				{
					entityTypeInner = GetEntityType(name);
				}
				return entityTypeInner;
			}
		}
		public bool isPassive
		{
			get
			{
				return entityType == EntityType.Passive || entityType == EntityType.Horse;
			}
		}
		public Entity()
		{
		}
		//public Player(string name, double x, double y, double z, float rotation, float pitch)
		//{
		//    this.name = name;
		//    this.x = x;
		//    this.y = y;
		//    this.z = z;
		//    this.pitch = pitch;
		//}
		public void Calc()
		{
			ix = (int)Math.Floor(x);
			iy = (int)Math.Floor(y);
			iz = (int)Math.Floor(z);
			this.pixelx = x + chunkSizeM1;
			this.pixelx *= tileWidth;
			this.pixely = y * tileHeight;
			this.rotation = (Math.PI / 180) * (rotation - 90);
			entityTypeInner = GetEntityType(this.name);
		}
		public double ChangeAmount(Entity comparedTo)
		{
			double changeAmount = 0;
			changeAmount += Math.Abs(this.x - comparedTo.x);
			changeAmount += Math.Abs(this.y - comparedTo.y);
			changeAmount += 0.3 * (Math.Abs(this.z - comparedTo.z) / 256 /* Globals.worldHeight */);
			changeAmount += 0.3 * (Math.Abs(this.pitch - comparedTo.pitch) / 5000);
			changeAmount += Math.Min(0.3, Math.Abs(this.pitch - comparedTo.pitch) / 100);
			changeAmount += Math.Min(0.3, Math.Abs(this.rotation - comparedTo.rotation) / 360);
			return changeAmount;
		}
		public EntityType GetEntityType(string entityName)
		{
			if (entityName.StartsWith("Item|") || entityName.StartsWith("Block|"))
			{
				return handleItem();
			}
			if (entityName.StartsWith("Unknown Horse "))
				entityName = "Unknown Horse";
			else
				entityName = entityName.Split(' ')[0];
			switch (entityName)
			{
				case "Pig":
				case "Sheep":
				case "Cow":
				case "Chicken":
				case "Squid":
				case "Rabbit":
					return EntityType.Passive;
				case "Horse":
				case "Donkey":
				case "Mule":
				case "Zombie Horse":
				case "Skeleton Horse":
				case "Unknown Horse":
					return EntityType.Horse;
				case "XPOrb":
					return EntityType.XPOrb;
				case "Creeper":
				case "Skeleton":
				case "Spider":
				case "Giant":
				case "Zombie":
				case "Slime":
				case "Ghast":
				case "PigZombie":
				case "Mob":
				case "Monster":
					return EntityType.Hostile;
				case "Arrow":
					itemID = 6 + 256;
					return EntityType.Arrow;
				case "Snowball":
					itemID = 76 + 256;
					return EntityType.Snowball;
				case "Painting":
					itemID = 65 + 256;
					return EntityType.Painting;
				case "PrimedTnt":
					itemID = 46;
					return EntityType.PhysicsObj;
				case "FallingSand":
					itemID = 12;
					return EntityType.PhysicsObj;
				case "Minecart":
					itemID = 72 + 256;
					return EntityType.Minecart;
				case "Boat":
					itemID = 77 + 256;
					return EntityType.Boat;
				default:
					return EntityType.Hostile;
			}
		}

		private EntityType handleItem()
		{
			string[] parts = name.Split('|');
			if (parts.Length < 2)
				return EntityType.Hostile;
			if (!int.TryParse(parts[1], out itemID))
			{
				File.AppendAllText("errordump.txt", "\r\nItem or Block (it claims to be a " + parts[0] + ") found with " + parts[1] + " for an ID. A valid integer is required.\r\n");
				return EntityType.Hostile;
			}
			if (name.StartsWith("Item"))
			{
				if (parts.Length != 4)
				{
					File.AppendAllText("errordump.txt", "\r\nItem found with " + parts.Length + " parts. 4 parts required.\r\nItem text: " + name);
					return EntityType.Hostile;
				}
				name = parts[2];
				int count;
				if (!int.TryParse(parts[3], out count))
				{
					File.AppendAllText("errordump.txt", "\r\nItem found with " + parts[4] + " for an item count.  Item count must be a valid integer.\r\n");
					count = 0;
				}
				itemCount = count;
				return EntityType.Item;
			}
			else if (name.StartsWith("Block"))
			{
				if (itemID < 0 || itemID > 255)
					itemID = 1;
				//if (texNum == null)
				//    return EntityType.Hostile; // Hasn't been set yet.
				//SetSourceRect(texNum[itemID], false);
				if (parts.Length != 4)
				{
					File.AppendAllText("errordump.txt", "\r\nBlock found with " + parts.Length + " parts. 4 parts required.\r\nBlock text: " + name);
					return EntityType.Hostile;
				}
				name = parts[2];
				int count;
				if (!int.TryParse(parts[3], out count))
				{
					File.AppendAllText("errordump.txt", "\r\nBlock found with " + parts[3] + " for an item count.  Block count must be a valid integer.\r\n");
					count = 0;
				}
				itemCount = count;
				return EntityType.Block;
			}
			return EntityType.Hostile;
		}
		public double distanceFrom(Entity secondEntity)
		{
			double dx = x - secondEntity.x, dy = y - secondEntity.y, dz = z - secondEntity.z;
			return Math.Sqrt(dx * dx + dy * dy + dz * dz);
		}
		public override string ToString()
		{
			return name;
		}

		#region IComparable Members

		public int CompareTo(object obj)
		{
			if (obj.GetType() == typeof(Entity))
			{
				Entity OtherEntity = (Entity)obj;
				int temp = this.name.CompareTo(OtherEntity.name);
				if (temp != 0)
					return temp;
				else
				{
					temp = this.x.CompareTo(OtherEntity.x);
					if (temp != 0)
						return temp;
					else
					{
						temp = this.y.CompareTo(OtherEntity.y);
						if (temp != 0)
							return temp;
						else
						{
							temp = this.z.CompareTo(OtherEntity.z);
							if (temp != 0)
								return temp;
							else
							{
								temp = this.rotation.CompareTo(OtherEntity.rotation);
								if (temp != 0)
									return temp;
								else
								{
									return this.pitch.CompareTo(OtherEntity.pitch);
								}
							}
						}
					}
				}
			}
			return 0;
		}

		#endregion
	}
}
