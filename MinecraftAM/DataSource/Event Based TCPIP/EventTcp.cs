using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
namespace DataSource
{
	/// <summary>
	/// These classes are not as efficient as they could be.  The PacketPacker class should be eliminated and copying the byte arrays should be avoided wherever possible.  These classes frequently copy and create new byte arrays when all that is really necessary is to integrate the PacketPacker functionality into the SendThread and ReceiveThread functions.  A System.Collections.Concurrent.ConcurrentQueue could be used for storing packets received and packets to be sent.  The Packet class simply wraps a byte array and could be removed.
	/// </summary>
	#region Client
	/// <summary>
	/// This class should not be used directly.  Use BytesEventTcpClient instead.
	/// </summary>
	public abstract class EventTcpClient
	{
		#region Events
		#region OnRefused
		/// <summary>
		/// 
		/// </summary>
		public delegate void OnRefusedEventHandler();
		/// <summary>
		/// This event is raised when a connection is refused (server not running).
		/// </summary>
		public event OnRefusedEventHandler OnRefused;
		/// <summary>
		/// This function raises the OnRefused event.
		/// </summary>
		protected void RaiseRefused()
		{
			if (OnRefused != null)
				OnRefused();
		}
		#endregion
		#region OnConnect
		/// <summary>
		/// This event handler handles the OnConnect event.
		/// </summary>
		public delegate void OnConnectEventHandler();
		/// <summary>
		/// This event is raised when a connection is established.
		/// </summary>
		public event OnConnectEventHandler OnConnect;
		/// <summary>
		/// This function raises the OnConnect event.
		/// </summary>
		protected virtual void RaiseConnect()
		{
			if (OnConnect != null)
				OnConnect();
		}
		#endregion
		#region OnDisconnect
		/// <summary>
		/// This event handler handles the OnDisconnect event.
		/// </summary>
		public delegate void OnDisconnectEventHandler();
		/// <summary>
		/// This event is raised when the connection is closed.
		/// </summary>
		public event OnDisconnectEventHandler OnDisconnect;
		/// <summary>
		/// This function raises the OnDisconnect event.
		/// </summary>
		protected virtual void RaiseDisconnect()
		{
			if (OnDisconnect != null && !bDisconnectRaised)
			{
				bDisconnectRaised = true;
				OnDisconnect();
			}
		}
		#endregion
		#endregion
		#region Globals / Constructor
		protected object ClientLock = new object();
		protected TcpClient tcpc;
		protected Thread tReceive;
		protected Thread tSend;
		protected volatile bool bAbort = false;
		protected volatile bool bDisconnectRaised = false;
		protected ConcurrentQueue<byte[]> messagesToSend;
		public EventTcpClient()
		{
			messagesToSend = new ConcurrentQueue<byte[]>();
		}
		public bool Connected
		{
			get
			{
				try
				{
					return SocketConnected();
				}
				catch (Exception) { }
				return false;
			}
		}
		#endregion
		#region Connect / Disconnect
		public void Connect(string host = "localhost", ushort port = 50191)
		{
			try
			{
				lock (ClientLock)
					if (tcpc != null && SocketConnected()) return;
				tReceive = new Thread(ReceiveThread);
				tReceive.Name = "Receive Thread";
				tReceive.Start(new object[] { host, port });
			}
			catch (Exception) { }
		}
		protected bool SocketConnected()
		{
			lock (ClientLock)
				return tcpc != null && tcpc.Connected;
		}
		public void Disconnect()
		{
			bAbort = true;
			//try
			//{
			//    if (tReceive != null)
			//        tReceive.Abort();
			//}
			//catch (Exception) { }
			//try
			//{
			//    if (tSend != null)
			//        tSend.Abort();
			//}
			//catch (Exception) { }
			try
			{
				if (tcpc != null)
					tcpc.Close();
			}
			catch (Exception) { }
			RaiseDisconnect();
		}
		#endregion
		#region Send / Receive
		protected void ReceiveThread(object param)
		{
			try
			{
				object[] parameters = (object[])param;
				string host = (string)parameters[0];
				ushort port = (ushort)parameters[1];
				messagesToSend = new ConcurrentQueue<byte[]>();
				tcpc = new TcpClient(host, port);
				RaiseConnect();
				tSend = new Thread(SendThread);
				tSend.Name = "Send Thread";
				tSend.Start();
				NetworkStream ns = tcpc.GetStream();
				while (SocketConnected() && !bAbort)
				{
					// We need to wait for at least 4 bytes to be available.  Each packet starts with 4 bytes that indicate the length of the packet.
					while (!bAbort && SocketConnected() && tcpc.Available < 4)
						Thread.Sleep(1);
					if (bAbort || !SocketConnected())
						break;
					// Data is available.  See how large the next packet is.
					int iPacketLength = ByteConverter.ToInt32(ReadNBytes(4, ns), 0);
					// Next packet is iPacketLength bytes long.  Read that packet.
					byte[] packet = ReadNBytes(iPacketLength, ns);
					if (!bAbort && SocketConnected())
						HandleReceivedData(packet);
				}
			}
			catch (SocketException ex)
			{
				if (ex.ErrorCode == 10061)
				{
					// Connection was actively refused
					RaiseRefused();
					return;
				}
				Console.Error.WriteLine(ex.ToString());
			}
			catch (ObjectDisposedException)
			{
				// We have been disconnected (This can happen when the Disconnect() function is called)
			}
			catch (Exception ex)
			{
				if (ex.Message == "Cannot access a disposed object.\r\nObject name: 'System.Net.Sockets.Socket'.")
				{
					// We have been disconnected (This can happen when the Disconnect() function is called)
				}
				else
				{
					Console.Error.WriteLine(ex.ToString());
				}
			}
			Disconnect();
		}
		private byte[] ReadNBytes(int n, NetworkStream ns)
		{
			byte[] packet = new byte[n];
			if (n == 0)
				return packet;
			int read = 0;
			while (read < n && SocketConnected())
				read += ns.Read(packet, read, n - read);
			return packet;
		}
		protected void SendThread()
		{
			try
			{
				NetworkStream ns = tcpc.GetStream();
				while (!bAbort && SocketConnected())
				{
					byte[] messageToSend;
					while (messagesToSend.TryDequeue(out messageToSend))
					{
						byte[] messageLength = ByteConverter.GetBytes(messageToSend.Length);
						ns.Write(messageLength, 0, 4);
						if (messageToSend.Length > 0)
							ns.Write(messageToSend, 0, messageToSend.Length);
					}
					Thread.Sleep(1);
				}
			}
			catch (SocketException ex)
			{
				Console.Error.WriteLine(ex.ToString());
			}
			catch (Exception ex)
			{
				// Happens on disconnect: Unable to write data to the transport connection: An existing connection was forcibly closed by the remote host.
				Console.Error.WriteLine(ex.ToString());
			}
			Disconnect();
		}
		protected void SendToServer(byte[] message)
		{
			messagesToSend.Enqueue(message);
		}
		protected abstract void HandleReceivedData(byte[] bytes);
		#endregion
	}
	/// <summary>
	/// Handles the sending and receiving of byte arrays.
	/// </summary>
	public class BytesEventTcpClient : EventTcpClient
	{
		#region OnDataReceived
		/// <summary>
		/// </summary>
		public delegate void BytesReceivedEventHandler(byte[] bytes);
		/// <summary>
		/// This event is raised when data is received.
		/// </summary>
		public event BytesReceivedEventHandler OnDataReceived;
		/// <summary>
		/// This function raises the OnDataReceived event.
		/// </summary>
		protected void RaiseDataReceived(byte[] bytes)
		{
			//byte[] copiedBytes = new byte[bytes.Length];
			// bytes.CopyTo(copiedBytes, 0);
			if (OnDataReceived != null)
				OnDataReceived(bytes);
		}
		#endregion
		#region Globals/Constructor
		public BytesEventTcpClient()
			: base()
		{
		}
		#endregion
		#region HandleReceivedData, Send Functions
		protected override void HandleReceivedData(byte[] bytes)
		{
			RaiseDataReceived(bytes);
		}
		public void Send(byte[] data)
		{
			SendToServer(data);
		}
		#endregion
	}
	/// <summary>
	/// Handles the sending and receiving of strings using the EventTcp protocol.
	/// </summary>
	public class StringEventTcpClient : EventTcpClient
	{
		#region OnDataReceived
		/// <summary>
		/// </summary>
		public delegate void StringReceivedEventHandler(string message);
		/// <summary>
		/// This event is raised when data is received.
		/// </summary>
		public event StringReceivedEventHandler OnDataReceived;
		/// <summary>
		/// This function raises the OnDataReceived event.
		/// </summary>
		protected void RaiseDataReceived(string message)
		{
			if (OnDataReceived != null)
				OnDataReceived(message);
		}
		#endregion
		#region Globals/Constructor
		public StringEventTcpClient()
			: base()
		{
		}
		#endregion
		#region HandleReceivedData, Send Functions
		protected override void HandleReceivedData(byte[] bytes)
		{
			try
			{
				RaiseDataReceived(ByteConverter.ToString(bytes, 0, bytes.Length));
			}
			catch (Exception) { }
		}
		public void Send(string message)
		{
			SendToServer(ByteConverter.GetBytes(message));
		}
		#endregion
	}
	#endregion
	#region Server
	/// <summary>
	/// This class should not be used directly.  Use BytesEventTcpServer instead.
	/// </summary>
	public abstract class EventTcpServer
	{
		#region Events
		#region OnClientConnect
		/// <summary>
		/// This event handler handles the OnClientConnect event.
		/// </summary>
		/// <param name="sender">The object which raised the event.</param>
		/// <param name="e"></param>
		public delegate void OnClientConnectEventHandler(string clientKey);
		/// <summary>
		/// This event is raised when a client connection is created.
		/// </summary>
		public event OnClientConnectEventHandler OnClientConnect;
		/// <summary>
		/// This function raises the OnClientConnect event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void RaiseClientConnect(string clientKey)
		{
			if (OnClientConnect != null)
				OnClientConnect(clientKey);
		}
		#endregion
		#region OnDisconnect
		/// <summary>
		/// This event handler handles the OnDisconnect event.
		/// </summary>
		/// <param name="clientKey">The key associated with the client that was disconnected.</param>
		public delegate void OnDisconnectEventHandler(string clientKey);
		/// <summary>
		/// This event is raised when a client connection is closed.
		/// </summary>
		public event OnDisconnectEventHandler OnClientDisconnect;
		/// <summary>
		/// This function raises the OnDisconnect event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void RaiseClientDisconnect(string clientKey)
		{
			if (OnClientDisconnect != null)
				OnClientDisconnect(clientKey);
		}
		#endregion
		#region OnListenStop
		/// <summary>
		/// This event handler handles the OnListenStop event.
		/// </summary>
		/// <param name="sender">The object which raised the event.</param>
		/// <param name="e"></param>
		public delegate void OnListenStopEventHandler();
		/// <summary>
		/// This event is raised when the server stops listening.
		/// </summary>
		public event OnListenStopEventHandler OnListenStop;
		/// <summary>
		/// This function raises the OnListenStop event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void RaiseListenStop()
		{
			if (OnListenStop != null)
				OnListenStop();
		}
		#endregion
		#endregion
		#region Globals/Constructor
		protected object ListenerStartStopLock = new object();
		protected Thread tListen;
		protected TcpListener tcpl;
		protected SortedList<string, ClientThreadPack> clientList;
		protected IPAddress address = IPAddress.Any;
		protected volatile bool bListening = false;
		public bool Listening
		{
			get { return bListening; }
		}
		public EventTcpServer()
		{
			clientList = new SortedList<string, ClientThreadPack>();
		}
		#endregion
		#region Listen
		public void Listen(IPAddress ip = null, ushort port = 58910)
		{
			if (bListening) return;
			lock (clientList)
				clientList.Clear();
			if (ip == null) ip = IPAddress.Any;
			tListen = new Thread(ListenThread);
			tListen.Start(new object[] { ip, port });
		}
		public void StopListening()
		{
			lock (ListenerStartStopLock)
			{
				bListening = false;
				tcpl.Stop();
			}
		}
		// Listen Loop
		protected void ListenThread(object param)
		{
			// Start Listening
			try
			{
				object[] parameters = (object[])param;
				IPAddress ip = (IPAddress)parameters[0];
				ushort port = (ushort)parameters[1];
				IPEndPoint ipep = new IPEndPoint(ip, port);
				lock (ListenerStartStopLock)
				{
					if (tcpl == null)
					{
						tcpl = new TcpListener(ipep);
						tcpl.AllowNatTraversal(true);
					}
					bListening = true;
					tcpl.Start(10);
				}
			}
			catch (Exception ex)
			{
				bListening = false;
				ex.ToString();
				try
				{
					if (tcpl != null)
						tcpl.Stop();
				}
				catch (Exception) { }
				RaiseListenStop();
				return;
			}
			// Start accepting clients
			TcpClient client = null;
			string key = "";
			try
			{
				while (bListening)
				{
					client = null;
					key = "";
					client = tcpl.AcceptTcpClient();
					key = client.Client.RemoteEndPoint.ToString();
					Thread clientReadThread;
					Thread clientSendThread;
					lock (clientList)
					{
						if (clientList.ContainsKey(key))
						{
							clientList[key].Disconnect();
							clientList.Remove(key);
							RaiseClientDisconnect(key);
						}
						clientReadThread = new Thread(ClientReceiveThread);
						clientSendThread = new Thread(ClientSendThread);
						clientList.Add(key, new ClientThreadPack(clientReadThread, clientSendThread, client, key));
					}
					clientReadThread.Start(key);
					clientSendThread.Start(key);
					RaiseClientConnect(key);
				}
			}
			catch (SocketException ex)
			{
				if (ex.ErrorCode == 10004)
				{
					// TcpListener Stop function was called while waiting for a client to connect
				}
				ex.ToString();
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
			// We are done listening.
			bListening = false;
			try
			{
				if (tcpl != null)
					tcpl.Stop();
			}
			catch (Exception) { }
			// I believe this disconnects a client that may be not completely finished connecting.
			if (client != null)
				try
				{
					client.Close();
				}
				catch (Exception) { }
			DisconnectAllClients();
			RaiseListenStop();
		}
		#endregion
		#region Client Handling Threads / Functions
		protected void ClientReceiveThread(object param)
		{
			ClientThreadPack ctp = null;
			string myKey = "";
			try
			{
				myKey = (string)param;
				lock (clientList)
				{
					if (!clientList.ContainsKey(myKey))
						return;
					ctp = clientList[myKey];
				}
				NetworkStream ns = ctp.client.GetStream();
				while (bListening)
				{
					// We need to wait for at least 4 bytes to be available.  Each packet starts with 4 bytes that indicate the length of the packet.
					while (bListening && !ctp.Abort && ctp.Connected && ctp.client.Available < 4)
						Thread.Sleep(1);
					if (!bListening || ctp.Abort || !ctp.Connected)
						break;
					// Data is available.  See how large the next packet is.
					int iPacketLength = ByteConverter.ToInt32(ReadNBytes(4, ns, ctp), 0);
					// Next packet is iPacketLength bytes long.  Read that packet.
					byte[] packet = ReadNBytes(iPacketLength, ns, ctp);
					if (bListening && !ctp.Abort && ctp.Connected)
						HandleReceivedData(packet, myKey);
				}
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
			if (ctp != null)
				ctp.Disconnect(true);
			lock (clientList)
				if (clientList.ContainsKey(myKey))
					clientList.Remove(myKey);
			RaiseClientDisconnect(myKey);
		}
		private byte[] ReadNBytes(int n, NetworkStream ns, ClientThreadPack ctp)
		{
			byte[] packet = new byte[n];
			if (n == 0)
				return packet;
			int read = 0;
			while (read < n && bListening && !ctp.Abort && ctp.Connected)
				read += ns.Read(packet, read, n - read);
			return packet;
		}
		protected void ClientSendThread(object param)
		{
			ClientThreadPack ctp = null;
			string myKey = "";
			try
			{
				myKey = (string)param;
				lock (clientList)
				{
					if (!clientList.ContainsKey(myKey))
						return;
					ctp = clientList[myKey];
				}
				NetworkStream ns = ctp.client.GetStream();
				while (bListening && !ctp.Abort)
				{
					byte[] messageToSend;
					while (ctp.messagesToSend.TryDequeue(out messageToSend))
					{
						byte[] messageLength = ByteConverter.GetBytes(messageToSend.Length);
						ns.Write(messageLength, 0, 4);
						if (messageToSend.Length > 0)
							ns.Write(messageToSend, 0, messageToSend.Length);
					}
					Thread.Sleep(1);
				}
			}
			catch (SocketException ex)
			{
				ex.ToString();
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
			if (ctp != null)
				ctp.Disconnect(true);
		}
		protected void SendToClient(string key, byte[] message)
		{
			ClientThreadPack ctp = null;
			try
			{
				lock (clientList)
				{
					if (!clientList.ContainsKey(key))
						return;
					ctp = clientList[key];
				}
				ctp.messagesToSend.Enqueue(message);
			}
			catch (Exception) { }
		}
		private void DisconnectAllClients()
		{
			lock (clientList)
			{
				foreach (ClientThreadPack ctp in clientList.Values)
				{
					ctp.Disconnect();
					RaiseClientDisconnect(ctp.key);
				}
			}
		}
		protected abstract void HandleReceivedData(byte[] bytes, string key);
		#endregion
	}
	/// <summary>
	/// Handles the sending and receiving of byte arrays.
	/// </summary>
	public class BytesEventTcpServer : EventTcpServer
	{
		#region OnDataReceived
		/// <summary>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public delegate void BytesReceivedEventHandler(string senderKey, byte[] bytes);
		/// <summary>
		/// This event is raised when data is received.
		/// </summary>
		public event BytesReceivedEventHandler OnDataReceived;
		/// <summary>
		/// This function raises the OnDataReceived event.
		/// </summary>
		/// <param name="e"></param>
		protected void RaiseDataReceived(string senderKey, byte[] bytes)
		{
			//byte[] copiedBytes = new byte[bytes.Length];
			//bytes.CopyTo(copiedBytes, 0);
			if (OnDataReceived != null)
				OnDataReceived(senderKey, bytes);
		}
		#endregion
		#region Globals/Constructor
		public BytesEventTcpServer()
			: base()
		{
		}
		#endregion
		#region HandleReceivedData, Send Functions
		protected override void HandleReceivedData(byte[] bytes, string key)
		{
			RaiseDataReceived(key, bytes);
		}
		public void Send(string clientKey, byte[] data)
		{
			SendToClient(clientKey, data);
		}
		#endregion
	}
	/// <summary>
	/// Handles the sending and receiving of strings using the EventTcp protocol.
	/// </summary>
	public class StringEventTcpServer : EventTcpServer
	{
		#region OnDataReceived
		/// <summary>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public delegate void StringReceivedEventHandler(string senderKey, string message);
		/// <summary>
		/// This event is raised when data is received.
		/// </summary>
		public event StringReceivedEventHandler OnDataReceived;
		/// <summary>
		/// This function raises the OnDataReceived event.
		/// </summary>
		/// <param name="e"></param>
		protected void RaiseDataReceived(string senderKey, string message)
		{
			if (OnDataReceived != null)
				OnDataReceived(senderKey, message);
		}
		#endregion
		#region Globals/Constructor
		public StringEventTcpServer()
			: base()
		{
		}
		#endregion
		#region HandleReceivedData, Send Functions
		protected override void HandleReceivedData(byte[] bytes, string key)
		{
			RaiseDataReceived(key, ByteConverter.ToString(bytes, 0, bytes.Length));
		}

		public void Send(string clientKey, string message)
		{
			SendToClient(clientKey, ByteConverter.GetBytes(message));
		}
		#endregion
	}
	#endregion
	#region Helper Classes
	public class ClientThreadPack
	{
		public string key;
		public Thread readingThread;
		public Thread sendingThread;
		public TcpClient client;
		public ConcurrentQueue<byte[]> messagesToSend;
		public bool Abort = false;
		public bool Connected
		{
			get
			{
				try
				{
					return client != null && client.Connected;
				}
				catch (Exception) { return false; }
			}
		}
		public ClientThreadPack(Thread rt, Thread st, TcpClient c, string k)
		{
			readingThread = rt;
			sendingThread = st;
			client = c;
			key = k;
			messagesToSend = new ConcurrentQueue<byte[]>();
		}
		public void Disconnect(bool skipThreadAborts = false)
		{
			Abort = true;
			if (!skipThreadAborts)
			{
				try
				{
					if (readingThread != null)
						readingThread.Abort();
				}
				catch (Exception) { }
				try
				{
					if (sendingThread != null)
						sendingThread.Abort();
				}
				catch (Exception) { }
			}
			try
			{
				if (client != null)
					client.Close();
			}
			catch (Exception) { }
		}
	}
	//public static class SocketExtensions
	//{
	//    //public static bool IsConnected(this TcpClient client)
	//    //{
	//    //    if (client == null)
	//    //        return false;
	//    //    if (client.Connected == false)
	//    //        return false;
	//    //    // try
	//    //    // {
	//    //    return true;// return !(client.Available == 0 && client.Client.Poll(100, SelectMode.SelectRead));
	//    //    // }
	//    //    //   catch (SocketException) { return false; }
	//    //    //  catch (Exception) { return false; }
	//    //}
	//}
	#endregion
}
