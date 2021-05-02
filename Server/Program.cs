using System;
using System.Threading;
using Common.Command;

namespace ClientServerModel
{
	class Program
	{
		static void Main(string[] args)
		{
			var server = new ServerSocket(4444);
			server.RegisterCommand(new StartPaint());
			server.RegisterCommand(new StartNotepad());
			Console.WriteLine($"Server is up and running on port {server.ListenerPortOfServer}");

			while (true)
			{
				Thread.Sleep(100);
			}
		}
	}
}
