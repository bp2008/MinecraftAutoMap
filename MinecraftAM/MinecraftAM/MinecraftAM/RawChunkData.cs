using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftAM
{
    class RawChunkData
    {
        public RawChunkData(int bx, int by, int bz, byte[] rawdata)
        {
            data = rawdata;
            x = bx;
            y = by;
            z = bz;
        }
        public int x, y, z;
        public byte[] data;
    }
}
