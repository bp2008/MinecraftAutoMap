using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DataSource;

namespace MinecraftAM
{
	public enum Holiday { None, AprilFools, Halloween, Christmas }
	/// <summary>
	/// These are holidays celbrated in the USA.  I apologize if they do not correspond to your holidays!
	/// </summary>
	public class HolidayManager
	{
		public Random RandomGenerator = new Random();
		protected DateTime dtUpdateHolidayAt = DateTime.Now;
		protected Holiday currentHoliday = Holiday.None;
		public Holiday CurrentHoliday
		{
			get
			{
				DateTime now = DateTime.Now;
				if (now > dtUpdateHolidayAt)
				{
					dtUpdateHolidayAt = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0) + TimeSpan.FromDays(1);
					currentHoliday = DetermineHoliday(now);
				}
				return currentHoliday;
			}
		}

		protected Holiday DetermineHoliday(DateTime now)
		{
			if (now.Month == 4 && now.Day == 1) return Holiday.AprilFools;
			if (now.Month == 10 && now.Day == 31) return Holiday.Halloween;
			if (now.Month == 12 && now.Day == 25) return Holiday.Christmas;
			return Holiday.None;
		}
		public HolidayManager()
		{
			InitHolidays();
		}
		public void Update()
		{
			UpdateHolidays();
		}
		#region Holiday Stuff
		#region All
		private void InitHolidays()
		{
			if (currentHoliday == Holiday.AprilFools) InitAprilFools();
		}
		private void UpdateHolidays()
		{
			DateTime now = DateTime.Now;
			if (currentHoliday == Holiday.AprilFools) UpdateAprilFools(now);
		}
		private bool RandomBool()
		{
			return RandomGenerator.NextDouble() > 0.5;
		}
		#endregion
		#region AprilFools
		public Entity Herobrine = new Entity();
		DateTime HerobrineShowTime = DateTime.Now;
		DateTime HerobrineHideTime = DateTime.Now;
		public bool showHerobrine = false;

		private void InitAprilFools()
		{
			showHerobrine = false;
			HerobrineShowTime = DateTime.Now + TimeSpan.FromMinutes(Globals.holidayManager.RandomGenerator.Next(5, 15));
			HerobrineHideTime = HerobrineShowTime + TimeSpan.FromSeconds(Globals.holidayManager.RandomGenerator.Next(30, 90));
		}
		private void UpdateAprilFools(DateTime now)
		{
			if (now > HerobrineHideTime)
				InitAprilFools();
			else if (now > HerobrineShowTime && !showHerobrine)
			{
				PositionHerobrine();
				showHerobrine = true;
			}
		}

		private void PositionHerobrine()
		{
			Entity pl = Globals.userPlayer;
			Vector4 HerobrineLocation;
			if (pl == null)
				HerobrineLocation = new Vector4((float)(RandomGenerator.NextDouble() - 0.5) * 400f, (float)(RandomGenerator.NextDouble() - 0.5) * 400f, (float)RandomGenerator.Next(5, 35), (float)(RandomGenerator.NextDouble() * Math.PI * 2));
			else
			{
				double x, y;
				if (RandomBool())
				{
					x = RandomGenerator.NextDouble() * 125 * (RandomBool() ? 1 : -1);
					y = (75 + RandomGenerator.NextDouble() * 50) * (RandomBool() ? 1 : -1);
				}
				else
				{
					x = (75 + RandomGenerator.NextDouble() * 50) * (RandomBool() ? 1 : -1);
					y = RandomGenerator.NextDouble() * 125 * (RandomBool() ? 1 : -1);
				}
				HerobrineLocation = new Vector4(
					(float)(pl.x + x),
					(float)(pl.y + y),
					(float)RandomGenerator.Next(5, 35),
					(float)(RandomGenerator.NextDouble() * Math.PI * 2));
			}
			Herobrine = new Entity();
			Herobrine.x = HerobrineLocation.X;
			Herobrine.y = HerobrineLocation.Y;
			Herobrine.z = HerobrineLocation.Z;
			Herobrine.rotation = 90;
			Herobrine.Calc();
			Herobrine.rotation = HerobrineLocation.W;
			Herobrine.name = "Herobrine";
		}
		#endregion
		#endregion
	}
}
