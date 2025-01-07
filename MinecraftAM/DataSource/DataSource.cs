using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;

namespace DataSource
{
	public class DS
	{
		private static int SHORT_BYTES = 2;
		private static int INT_BYTES = 4;
		//private static int LONG_BYTES = 8;
		private static int FLOAT_BYTES = 4;
		private static int DOUBLE_BYTES = 8;
		private const byte GET_PLAYER_LOCATION = 0;
		private const byte GET_WORLD_CHUNK = 1;
		//private static byte GET_FULL_COLUMN = 2;
		private const byte GET_WORLD_PATH = 20;
		private const byte SET_PLAYER_LOCATION = 30;
		private const byte CREATE_EXPLOSION = 40;
		private const byte PERMISSION_CONFIRMATION = 50;
		private const byte GET_TEXTURE_NAME = 60;
		private static Thread worldChunkThread;
		private static ConcurrentQueue<byte[]> chunkDataQueue = new ConcurrentQueue<byte[]>();

		/// <summary>
		/// True if connected.
		/// </summary>
		public static bool isConnected
		{
			get
			{
				return client != null && client.Connected;
			}
		}
		public static void ShutDown()
		{
			shuttingDown = true;
			if (worldChunkThread != null)
				worldChunkThread.Abort();
			client.Disconnect();
		}
		private static bool shuttingDown = false;
		protected static BytesEventTcpClient client;
		protected static DSHandlerPlayerLocation playerLocationHandler;
		public static void InitializePlayerLocation(DSHandlerPlayerLocation handlerparam, int tileWidth, int tileHeight, int chunkSize)
		{
			playerLocationHandler = handlerparam;
			Entity.chunkSizeM1 = chunkSize - 1;
			Entity.tileHeight = tileHeight;
			Entity.tileWidth = tileWidth;
		}
		protected static DSHandlerChunk chunkHandler;
		public static void InitializeChunks(DSHandlerChunk handlerparam)
		{
			chunkHandler = handlerparam;
		}
		protected static DSHandlerWorldPath worldPathHandler;
		public static void InitializeWorldPath(DSHandlerWorldPath handlerparam)
		{
			worldPathHandler = handlerparam;
		}
		protected static DSHandlerConnectionStatusChanged connectionStatusChangedHandler;
		public static void InitializeConnectionStatusChanged(DSHandlerConnectionStatusChanged handlerparam)
		{
			connectionStatusChangedHandler = handlerparam;
		}

		protected static DSHandlerTextureName textureNameHandler;
		public static void InitializeTextureNameCallback(DSHandlerTextureName handlerparam)
		{
			textureNameHandler = handlerparam;
		}

		private static void ConnectionStatusChanged(ConnectionStatus connectionStatus)
		{
			if (connectionStatusChangedHandler != null)
				connectionStatusChangedHandler.ConnectionStatusChanged(connectionStatus);
		}
		//public static string url = "http://localhost:8080/MCAutomapSrv/AutomapChunk";
		/// <summary>
		/// Requests from the Minecraft Client a chunk of custom location and size.
		/// </summary>
		/// <param name="x">X coordinate</param>
		/// <param name="y">Y coordinate</param>
		/// <param name="z">Z coordinate</param>
		/// <param name="dx">Distance to grab from the X coordinate. (Width)</param>
		/// <param name="dy">Distance to grab from the Y coordinate. (Height)</param>
		/// <param name="dz">Distance to grab from the Z coordinate. (Depth)</param>
		public static void GetChunkData(int x, int y, int z, int dx, int dy, int dz)
		{
			if (isConnected)
			{
				byte[] bytes = new byte[(INT_BYTES * 6) + 1];
				int i = 0;
				bytes[i] = GET_WORLD_CHUNK;
				i++;
				InsertBytes(bytes, i, ByteConverter.GetBytes(x)); // X
				i += INT_BYTES;
				InsertBytes(bytes, i, ByteConverter.GetBytes(y)); // Y
				i += INT_BYTES;
				InsertBytes(bytes, i, ByteConverter.GetBytes(z)); // Z
				i += INT_BYTES;
				InsertBytes(bytes, i, ByteConverter.GetBytes(dx)); // DX
				i += INT_BYTES;
				InsertBytes(bytes, i, ByteConverter.GetBytes(dy)); // DY
				i += INT_BYTES;
				InsertBytes(bytes, i, ByteConverter.GetBytes(dz)); // DZ
				client.Send(bytes);
			}
		}
		/// <summary>
		/// Requests from the Minecraft Client the player's location.
		/// </summary>
		public static void GetPlayerLocation()
		{
			if (isConnected)
			{
				client.Send(new byte[] { GET_PLAYER_LOCATION });
			}
		}
		/// <summary>
		/// Requests from the Minecraft Client the player's location.
		/// </summary>
		public static void GetWorldPath()
		{
			if (isConnected)
			{
				client.Send(new byte[] { GET_WORLD_PATH });
			}
		}

		/// <summary>
		/// Requests from the Minecraft Client the name of the texture for the item or block id.
		/// </summary>
		public static void GetTextureName(int id)
		{
			if (id < 0)
				return;
			if (isConnected)
			{
				byte[] bytes = new byte[INT_BYTES + 1];
				int i = 0;
				bytes[i] = GET_TEXTURE_NAME;
				i++;
				InsertBytes(bytes, i, ByteConverter.GetBytes(id)); // Block or Item ID
				client.Send(bytes);
			}
		}

		/// <summary>
		/// Tells the Minecraft Client to move the player.
		/// </summary>
		public static void SetPlayerLocation(double x, double y, double z)
		{
			if (isConnected)
			{
				byte[] bytes = new byte[(DOUBLE_BYTES * 3) + 1];
				int i = 0;
				bytes[i] = SET_PLAYER_LOCATION;
				i++;
				InsertBytes(bytes, i, ByteConverter.GetBytes(x)); // X
				i += DOUBLE_BYTES;
				InsertBytes(bytes, i, ByteConverter.GetBytes(y)); // Y
				i += DOUBLE_BYTES;
				InsertBytes(bytes, i, ByteConverter.GetBytes(z)); // Z
				client.Send(bytes);
			}
		}

		/// <summary>
		/// Tells the Minecraft Client to create an explosion at the specified location with the specified power.
		/// </summary>
		public static void CreateExplosion(double x, double y, double z, float power)
		{
			if (isConnected)
			{
				byte[] bytes = new byte[(DOUBLE_BYTES * 3) + FLOAT_BYTES + 1];
				int i = 0;
				bytes[i] = CREATE_EXPLOSION;
				i++;
				InsertBytes(bytes, i, ByteConverter.GetBytes(x)); // X
				i += DOUBLE_BYTES;
				InsertBytes(bytes, i, ByteConverter.GetBytes(y)); // Y
				i += DOUBLE_BYTES;
				InsertBytes(bytes, i, ByteConverter.GetBytes(z)); // Z
				i += DOUBLE_BYTES;
				InsertBytes(bytes, i, ByteConverter.GetBytes(power)); // Z
				client.Send(bytes);
			}
		}

		//private static System.Diagnostics.Stopwatch sw;
		/// <summary>
		/// Handles a message received from the Minecraft Client.
		/// </summary>
		/// <param name="message">The string received from the Minecraft Client.</param>
		private static void HandleMessage(byte[] message)
		{
			if (message.Length <= 0)
				return;
			byte messageType = message[0];
			if (messageType == GET_PLAYER_LOCATION)
			{
				//if (sw == null)
				//    sw = new System.Diagnostics.Stopwatch();
				//else
				//{
				//    sw.Stop();
				//    File.AppendAllText("getplayerlocationlog.txt", sw.ElapsedTicks + Environment.NewLine);
				//    sw.Reset();
				//}
				HandlePlayerLocation(message);
				//sw.Start();
			}
			else if (messageType == GET_WORLD_CHUNK)
			{
				HandleWorldChunk(message);
			}
			else if (messageType == GET_WORLD_PATH)
			{
				HandleWorldPath(message);
			}
			else if (messageType == PERMISSION_CONFIRMATION)
			{
				// A permission packet v1 is 3 bytes long
				// Byte 1: PERMISSION_CONFIRMATION (message type)
				// Byte 2: version number (1)
				// Byte 3: permission byte
				//
				// I hope I never have to change this!
				//
				if (message.Length >= 3)
				{
					if (message[1] == 1)
					{
						HandlePermissionConfirmation(message[2]);
					}
				}
			}
			else if (messageType == GET_TEXTURE_NAME)
			{
				HandleTextureName(message);
			}
		}

		private static void HandlePermissionConfirmation(byte permissionLevel)
		{
			PermissionManager.SetPermissionByte(permissionLevel);
			client.Send(new byte[] { PERMISSION_CONFIRMATION });
		}

		private static void HandlePlayerLocation(byte[] message)
		{
			try
			{
				if (playerLocationHandler == null)
					return;
				List<Entity> players = new List<Entity>();
				int reqIdx = 1;
				int sizeOfPlayerName = ByteConverter.ToInt32(message, reqIdx);
				reqIdx += INT_BYTES;
				string playerName = ByteConverter.ToString(message, reqIdx, sizeOfPlayerName);
				reqIdx += sizeOfPlayerName;
				Entity playerMe = null;
				while (reqIdx < message.Length)
				{
					Entity p = new Entity();
					p.x = ByteConverter.ToDouble(message, reqIdx);
					reqIdx += DOUBLE_BYTES;
					p.y = ByteConverter.ToDouble(message, reqIdx);
					reqIdx += DOUBLE_BYTES;
					p.z = ByteConverter.ToDouble(message, reqIdx);
					reqIdx += DOUBLE_BYTES;
					p.rotation = ByteConverter.ToSingle(message, reqIdx);
					reqIdx += FLOAT_BYTES;
					p.pitch = ByteConverter.ToSingle(message, reqIdx);
					reqIdx += FLOAT_BYTES;
					int namesize = ByteConverter.ToInt32(message, reqIdx);
					reqIdx += INT_BYTES;
					p.name = ByteConverter.ToString(message, reqIdx, namesize);
					reqIdx += namesize;
					p.isNPC = ByteConverter.ToBoolean(message, reqIdx);
					reqIdx++;
					p.Calc();
					AddEntity(players, p, playerName);
					if (p.name == playerName)
						playerMe = p;
				}
				if (playerMe != null)
				{
					try
					{
						playerLocationHandler.PlayerLocationReceived(players, playerMe);
					}
					catch (Exception ex)
					{
						File.AppendAllText("errordump.txt", "\r\nAn error occurred while handling a player location.\r\n" + ex.ToString() + "\r\n");
						System.Windows.Forms.MessageBox.Show("An error occurred while handling a player location." + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
					}
				}
			}
			catch (Exception ex)
			{
				Eat(ex);
				return;
			}
		}
		/// <summary>
		/// Checks PermissionManager and adds the entity to the list if allowed.
		/// </summary>
		/// <param name="players"></param>
		/// <param name="p"></param>
		private static void AddEntity(List<Entity> entities, Entity e, string playerName)
		{
			if (!e.isNPC)
			{
				if (PermissionManager.allowPlayerDetection || e.name == playerName)
					entities.Add(e);
			}
			else if (e.entityType == Entity.EntityType.Hostile)
			{
				if (PermissionManager.allowHostileNPCDetection)
					entities.Add(e);
			}
			else if (e.entityType == Entity.EntityType.Passive)
			{
				if (PermissionManager.allowPassiveNPCDetection || e.entityType == Entity.EntityType.Horse)
					entities.Add(e);
			}
			else if (e.entityType == Entity.EntityType.Item || e.entityType == Entity.EntityType.XPOrb)
			{
				if (PermissionManager.allowItemDetection)
					entities.Add(e);
			}
			else
			{
				if (PermissionManager.allowNeutralNPCDetection)
					entities.Add(e);
			}
		}
		private static void Eat(Exception ex, string Location = "")
		{
			if (ex.GetType() == typeof(ThreadAbortException))
				return;
			File.AppendAllText("errordump.txt", "\r\nAn error occurred on the TCP/IP Message Handling Thread. " + Location + "\r\n" + ex.ToString() + "\r\n");
			System.Windows.Forms.MessageBox.Show("OMG an exception in the Socket communication.  What a surprise!" + Environment.NewLine + ex.ToString());
		}
		#region Handle Messages
		private static void HandleWorldChunk(byte[] message)
		{
			if (Thread.CurrentThread != worldChunkThread)
			{
				chunkDataQueue.Enqueue(message);
				return;
			}
			try
			{
				if (chunkHandler == null)
					return;
				int originalRequestLength = (INT_BYTES * 6) + 1;
				if (message.Length < originalRequestLength)
					return;
				int x, y, z, dx, dy, dz, reqIdx = 1;
				x = ByteConverter.ToInt32(message, reqIdx);
				reqIdx += INT_BYTES;
				y = ByteConverter.ToInt32(message, reqIdx);
				reqIdx += INT_BYTES;
				z = ByteConverter.ToInt32(message, reqIdx);
				reqIdx += INT_BYTES;
				dx = ByteConverter.ToInt32(message, reqIdx);
				reqIdx += INT_BYTES;
				dy = ByteConverter.ToInt32(message, reqIdx);
				reqIdx += INT_BYTES;
				dz = ByteConverter.ToInt32(message, reqIdx);
				reqIdx += INT_BYTES;
				int chunklength = dx * dy * dz * (SHORT_BYTES + 2);
				if (chunklength + originalRequestLength != message.Length)
					return;
				byte[] newChunkData = new byte[chunklength];
				Array.Copy(message, originalRequestLength, newChunkData, 0, chunklength);
				try
				{
					chunkHandler.WorldChunkReceived(x, y, z, dx, dy, dz, newChunkData);
				}
				catch (Exception ex)
				{
					Eat(ex, "World Chunk Handling function");
				}
			}
			catch (Exception ex)
			{
				Eat(ex);
				return;
			}
		}
		private static void HandleWorldPath(byte[] message)
		{
			string worldInfo = ByteConverter.ToString(message, 1, message.Length - 1);
			if (worldInfo == "Unknown")
				if (worldPathHandler != null)
					try
					{
						worldPathHandler.WorldPathReceived("Unknown", 0, "Unknown", 0, 0, 0);
					}
					catch (Exception ex)
					{
						File.AppendAllText("errordump.txt", "\r\nAn error occurred while handling a world path.\r\n" + ex.ToString() + "\r\n");
						System.Windows.Forms.MessageBox.Show("An error occurred while handling a world path." + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
					}

			string[] parts = worldInfo.Split('|');
			if (parts.Length < 6)
				return;
			else if (parts.Length > 6)
			{
				// In case the path SOMEHOW had a | in it, the string array will be messed up.  Maybe I am just paranoid.
				StringBuilder sbPathParts = new StringBuilder();
				for (int i = 0; i <= parts.Length - 6; i++)
					sbPathParts.Append(parts[i]);
				string[] partsNew = new string[6];
				partsNew[0] = sbPathParts.ToString();
				for (int i = parts.Length - 5, j = 1; i < parts.Length; i++, j++)
					partsNew[j] = parts[i];
				parts = partsNew;
			}
			int dimension;
			if (!int.TryParse(parts[1], out dimension))
				dimension = 0;
			int spawnX;
			if (!int.TryParse(parts[3], out spawnX))
				spawnX = 0;
			int spawnY;
			if (!int.TryParse(parts[3], out spawnY))
				spawnY = 0;
			int spawnZ;
			if (!int.TryParse(parts[3], out spawnZ))
				spawnZ = 0;
			if (worldPathHandler != null)
				try
				{
					worldPathHandler.WorldPathReceived(parts[0], dimension, parts[2], spawnX, spawnY, spawnZ);
				}
				catch (Exception ex)
				{
					File.AppendAllText("errordump.txt", "\r\nAn error occurred while handling a world path.\r\n" + ex.ToString() + "\r\n");
					System.Windows.Forms.MessageBox.Show("An error occurred while handling a world path." + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
				}
		}
		private static void HandleTextureName(byte[] message)
		{
			int id = ByteConverter.ToInt32(message, 1);
			int reqIdx = 1 + INT_BYTES;
			string textureName = ByteConverter.ToString(message, reqIdx, message.Length - reqIdx);
			if (textureNameHandler != null)
				textureNameHandler.TextureNameReceived(id, textureName);
		}
		#endregion

		#region Helpers
		public static void InsertBytes(byte[] destination, int destinationOffset, byte[] source)
		{
			if (destination.Length - destinationOffset < source.Length)
				return;
			for (int i = 0; i < source.Length; i++, destinationOffset++)
				destination[destinationOffset] = source[i];
		}
		private static void WorldChunkLoop()
		{
			while (true)
			{
				try
				{
					byte[] chunkData;
					while (chunkDataQueue.TryDequeue(out chunkData))
						HandleWorldChunk(chunkData);
				}
				catch (ThreadAbortException) { return; }
				catch (Exception ex)
				{
					File.AppendAllText("errordump.txt", DateTime.Now.ToString() + "\r\nAn error occurred in the World Chunk Handling Thread.\r\n" + ex.ToString() + "\r\n");
				}
				Thread.Sleep(1);
			}
		}
		#endregion

		#region Networking
		protected static string lastHost = "";
		protected static ushort lastPort = 12345;
		public static void Connect(string host, ushort port, bool isRetry = false)
		{
			if (shuttingDown)
				return;
			if (worldChunkThread == null)
			{
				worldChunkThread = new Thread(WorldChunkLoop);
				worldChunkThread.Name = "World Chunk Handling Thread";
				worldChunkThread.Start();
			}
			if (!isRetry)
				ConnectionStatusChanged(ConnectionStatus.Connecting);
			client = new BytesEventTcpClient();
			client.OnConnect += new EventTcpClient.OnConnectEventHandler(client_OnConnect);
			client.OnDataReceived += new BytesEventTcpClient.BytesReceivedEventHandler(client_OnDataReceived);
			client.OnDisconnect += new EventTcpClient.OnDisconnectEventHandler(client_OnDisconnect);
			client.OnRefused += new EventTcpClient.OnRefusedEventHandler(client_OnRefused);
			lastHost = host;
			lastPort = port;
			client.Connect(host, port);
		}

		/// <summary>
		/// Called if a connection is refused (unable to connect).
		/// </summary>
		static void client_OnRefused()
		{
			ConnectionStatusChanged(ConnectionStatus.Refused);
			//System.Windows.Forms.MessageBox.Show("Connection refused.  If Minecraft is already running and a world is loaded, you may have forgotten to install Risugami's ModLoader, or the Automap mod may not be installed correctly.");
			Console.WriteLine("Connection Refused");
			Connect(lastHost, lastPort, true);
		}

		/// <summary>
		/// Called when we get disconnected.
		/// </summary>
		static void client_OnDisconnect()
		{
			ConnectionStatusChanged(ConnectionStatus.Disconnected);
			//if (!shuttingDown)
			//	System.Windows.Forms.MessageBox.Show("Disconnected");
			Console.WriteLine("Disconnected");
			Connect(lastHost, lastPort, true);
		}

		/// <summary>
		/// Called when data is received.
		/// </summary>
		/// <param name="message">A byte array that has been received.</param>
		static void client_OnDataReceived(byte[] message)
		{
			HandleMessage(message);
		}

		/// <summary>
		/// Called when we establish a connection.
		/// </summary>
		static void client_OnConnect()
		{
			ConnectionStatusChanged(ConnectionStatus.Connected);
			Console.WriteLine("Connected");
		}
		#endregion
	}
}
