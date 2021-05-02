using System.Net.Sockets;

namespace Common
{
	public static class MessageSender
	{
		public static bool Send(Socket socket, byte[] bytes)
		{
			int sentBytes = 0;
			if (socket.Connected)
			{
				sentBytes = socket.Send(bytes, bytes.Length, SocketFlags.None);
			}
			return sentBytes == bytes.Length;
		}
	}
}
