namespace Common
{
	public interface ICommand
	{
		string Name { get; }

		string Command { get; }

		string Parameters { get; }
	}
}
