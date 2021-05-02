using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace ClientServerModel
{
	class ServerSocket : Socket
	{
		public delegate void DataArrivedEventHandler(object sender, DataArrivedEventArgs e);
		public delegate void ErrorOccurredEventHandler(object sender, ErrorOccurredEventArgs e);

		public int ListenerPortOfServer { get; set; }
		public Encoding Encoding = Encoding.UTF8;
		public event DataArrivedEventHandler DataArrived;
		public event ErrorOccurredEventHandler ErrorOccurred;

		private const int MAX_PENDING_CONNECTION = 10;
		private bool working;

		public List<ICommand> Commands { get; } = new List<ICommand>();

		public ServerSocket(int listenerPort)
			: base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
		{
			working = true;
			ListenerPortOfServer = listenerPort;

			Bind(new IPEndPoint(IPAddress.Any, listenerPort));
			Listen(MAX_PENDING_CONNECTION);
			DataArrived += new DataArrivedEventHandler(DataArrivedHandler);
			Task.Factory.StartNew(() => { ListenerEngine(); });
		}

		public void Stop()
		{
			working = false;
		}

		public void RegisterCommand(ICommand command)
		{
			Commands.Add(command);
		}

		public bool Send(Socket socket, string message)
		{
			return MessageSender.Send(socket, Encoding.GetBytes(message));
		}

		public bool Send(Socket socket, byte[] bytes)
		{
			return MessageSender.Send(socket, bytes);
		}

		private void ListenerEngine()
		{
			try
			{
				while (working)
				{
					if (Poll(10, SelectMode.SelectRead))
					{
						BeginAccept(new AsyncCallback(AcceptCallback), this);
					}
				}
			}
			catch (Exception ex)
			{
				OnErrorOccurred(ex);
			}
		}

		private void AcceptCallback(IAsyncResult ar)
		{
			try
			{
				var state = new StateObject
				{
					Socket = ((Socket)ar.AsyncState).EndAccept(ar)
				};
				state.Socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, new AsyncCallback(ServerReadCallback), state);
			}
			catch (Exception ex)
			{
				OnErrorOccurred(ex);
			}
		}

		private void ServerReadCallback(IAsyncResult ar)
		{
			Socket handler = null;
			try
			{
				var state = (StateObject)ar.AsyncState;
				handler = state.Socket;
				if (handler.Connected)
				{
					int read = handler.EndReceive(ar);
					if (read > 0)
					{
						byte[] data = new byte[read];
						Array.Copy(state.Buffer, 0, data, 0, read);
						OnDataArrived(new DataArrivedEventArgs(handler, (IPEndPoint)handler.RemoteEndPoint, data));
						handler.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, new AsyncCallback(ServerReadCallback), state);
					}
				}
			}
			catch (InvalidOperationException ex)
			{
				OnErrorOccurred(ex);
			}
			catch (Exception ex)
			{
				SocketUtils.CloseSocket(handler);
				OnErrorOccurred(ex);
			}
		}

		protected virtual void OnDataArrived(DataArrivedEventArgs e)
		{
			DataArrived?.Invoke(this, e);
		}

		protected virtual void OnErrorOccurred(Exception exception)
		{
			ErrorOccurred?.Invoke(this, new ErrorOccurredEventArgs(exception));
		}

		private void DataArrivedHandler(object sender, DataArrivedEventArgs e)
		{
			try
			{
				var serverSocket = (ServerSocket)sender;
				var message = serverSocket.Encoding.GetString(e.Response);

				foreach (ICommand command in Commands)
				{
					if (command.Name == message)
					{
						ProcessUtils.Start(command);
					}
				}
			}
			catch (Exception ex)
			{
				OnErrorOccurred(ex);
			}
		}
	}
}
