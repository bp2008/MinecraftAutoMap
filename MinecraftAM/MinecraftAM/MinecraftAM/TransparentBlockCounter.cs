using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftAM.MinecraftAM
{
	class TransparentBlockCounter
	{
		int transparentLiquidCount = 0;
		int iTransparentLiquidMax = 1;

		public TransparentBlockCounter(int iTransparentLiquidMax)
		{
			this.iTransparentLiquidMax = iTransparentLiquidMax;
		}
		public bool TreatAsTransparent(int blockType)
		{
			if (ItemDrawing.transparentBlocks[blockType])
			{
				if (blockType > 7 && blockType < 12)
					return transparentLiquidCount++ < iTransparentLiquidMax;
				return true;
			}
			return false;
		}
	}
}
