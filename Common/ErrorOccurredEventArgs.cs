using System;

namespace Common
{
	public class ErrorOccurredEventArgs : EventArgs
	{
		public ErrorOccurredEventArgs(Exception exception)
		{
			Exception = exception;
		}

		public Exception Exception { get; private set; }
	}
}
