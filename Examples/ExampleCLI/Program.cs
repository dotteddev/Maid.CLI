using Maid.CLI;

CLIBuilder.Create()
	.MapCommand("help", "h", (args) => Console.WriteLine("Help!"));