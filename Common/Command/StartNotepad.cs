namespace Common.Command
{
	public class StartNotepad : ICommand
	{
		public string Name { get => "Notepad"; }

		public string Command { get => "notepad"; }

		public string Parameters { get => null; }
	}
}
