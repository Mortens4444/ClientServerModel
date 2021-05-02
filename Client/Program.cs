using Common;

namespace Client
{
	class Program
	{
		static void Main(string[] args)
		{
			var client = new ClientSocket("127.0.0.1", 4444);
			client.Send("Paint");
		}
	}
}
