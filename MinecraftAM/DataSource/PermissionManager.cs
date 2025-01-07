using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataSource
{
	public static class PermissionManager
	{
		public static bool allowOreDetection = true;
		public static bool allowCaveMapping = true;
		public static bool allowPlayerDetection = true;
		public static bool allowHostileNPCDetection = true;
		public static bool allowPassiveNPCDetection = true;
		public static bool allowNeutralNPCDetection = true;
		public static bool allowItemDetection = true;
		public static bool allowPowerToys = true;

		private const byte p_OreDetection = 1;
		private const byte p_CaveMap = 1 << 1;
		private const byte p_Players = 1 << 2;
		private const byte p_HostileNPCs = 1 << 3;
		private const byte p_PassiveNPCs = 1 << 4;
		private const byte p_NeutralNPCs = 1 << 5;
		private const byte p_Items = 1 << 6;
		private const byte p_PowerToys = 1 << 7;

		public static void SetPermissionByte(byte b)
		{
			allowOreDetection = (b & p_OreDetection) == 0;
			allowCaveMapping = (b & p_CaveMap) == 0;
			allowPlayerDetection = (b & p_Players) == 0;
			allowHostileNPCDetection = (b & p_HostileNPCs) == 0;
			allowPassiveNPCDetection = (b & p_PassiveNPCs) == 0;
			allowNeutralNPCDetection = (b & p_NeutralNPCs) == 0;
			allowItemDetection = (b & p_Items) == 0;
			allowPowerToys = (b & p_PowerToys) == 0;
		}

		public static string GetDisabledFeaturesString()
		{
			List<string> l = new List<string>();
			if (!allowOreDetection)
				l.Add("Ore Detection");
			if (!allowCaveMapping)
				l.Add("Indoors/Cave Mapping");
			if (!allowPlayerDetection)
				l.Add("Player Detection");
			if (!allowHostileNPCDetection)
				l.Add("Hostile NPC Detection");
			if (!allowPassiveNPCDetection)
				l.Add("Passive NPC Detection");
			if (!allowNeutralNPCDetection)
				l.Add("Neutral NPC Detection");
			if (!allowItemDetection)
				l.Add("Item Detection");
			if (!allowPowerToys)
				l.Add("Power Toys");
			if (l.Count == 0)
			{
				l.Add("This server has not explicitly disabled any AutoMap features.  As always, use AutoMap at your own risk.  If you are caught cheating, you may be banned!");
				l.Add(" ");
				l.Add("Play smart.  Play safe.");
			}
			return string.Join("\r\n", l);
		}
	}
}
