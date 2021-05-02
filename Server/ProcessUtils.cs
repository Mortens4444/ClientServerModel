using System;
using System.Diagnostics;
using Common;

namespace ClientServerModel
{
	public static class ProcessUtils
	{
		public static void Start(ICommand command)
		{
			Start(command.Command, command.Parameters);
		}

		public static void Start(string process, string arguments = null)
		{
			try
			{
				System.Diagnostics.Process.Start(new ProcessStartInfo
				{
					FileName = process,
					Arguments = arguments
				});
			}
			catch (Exception ex)
			{
				Console.Error.Write(ex);
			}
		}
	}
}
