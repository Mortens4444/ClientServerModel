namespace Common.Command
{
	public class StartPaint : ICommand
	{
		public string Name { get => "Paint"; }

		public string Command { get => "mspaint"; }

		public string Parameters { get => null; }
	}
}
