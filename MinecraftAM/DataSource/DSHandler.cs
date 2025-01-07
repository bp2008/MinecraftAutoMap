using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataSource
{
    public interface DSHandlerPlayerLocation
    {
        void PlayerLocationReceived(List<Entity> players, Entity userPlayer);
    }
    public interface DSHandlerChunk
    {
        void WorldChunkReceived(int x, int y, int z, int dx, int dy, int dz, byte[] data);
    }
    public interface DSHandlerWorldPath
    {
		void WorldPathReceived(string worldPath, int dimension, string worldName, int spawnX, int spawnY, int spawnZ);
    }
	public enum ConnectionStatus{Idle, Connecting, Connected, Refused, Disconnected};
	public interface DSHandlerConnectionStatusChanged
	{
		void ConnectionStatusChanged(ConnectionStatus newStatus);
	}
	public interface DSHandlerTextureName
	{
		void TextureNameReceived(int id, string textureName);
	}
}
