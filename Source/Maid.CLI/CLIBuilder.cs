namespace Maid.CLI;

public interface ICommand
{
	public string FullName { get; }
	public string ShortName { get; }
	public string Description { get; }
	public List<string> Usage { get; }
	public bool Execute(string[] args);
}
public abstract class Command : ICommand
{
	protected abstract string FullName { get; }
	protected abstract string ShortName { get; }
	protected abstract string Description { get; }
	protected abstract List<string> Usage { get; }

	public abstract bool Execute(string[] args);

	public List<string> GetUsage()
	{
		return Usage;
	}
}
internal class DefaultCommand : Command
{
	protected override string FullName => "";

	protected override string ShortName => "";

	protected override string Description => "";

	protected override List<string> Usage { get; } = [];

	public required Func<string[], bool?> Action { get; init; }

	public override bool Execute(string[] args)
	{
		return Action(args) ?? false;
	}
}

public class HelpCommand : Command
{
	protected override string FullName => "help";
	protected override string ShortName => "h";
	protected override string Description => "Displays help information";
	protected override List<string> Usage => [
		"help",
		"--help"
	];

	public override bool Execute(string[] args)
	{
		throw new NotImplementedException();
	}
}

public class Args
{
	public List<string> OriginalArgs { get; set; }

	public Args(string[] args)
	{
		OriginalArgs = args;
	}

	public string[] GetArgs()
	{
		return OriginalArgs.Skip(1).ToArray();
	}
}

public class CLIArgument
{
	public string Name { get; set; }
	public string Value { get; set; }
}

public class CLIFlag
{
	public string Name { get; set; }
}

public class CommandBuilder
{
	internal string _name = string.Empty;
	internal string _shortName = string.Empty;
	internal Type _commandType = default!;

	public CommandBuilder WithName(string name)
	{
		_name = name;
		return this;
	}
	public CommandBuilder WithShortName(string shortName)
	{
		_shortName = shortName;
		return this;
	}
	public CommandBuilder ExecuteWith(Type commandType)
	{
		_commandType = commandType;
		return this;
	}

	internal ICommand Build()
	{
		var command = Activator.CreateInstance(_commandType) as ICommand;
		return command ?? throw new Exception("Failed to create command");
	}
}

public class CommandList : List<ICommand>
{

}

public class CLIBuilder
{
	private CommandList _commands = [];
	private List<CommandBuilder> _commandDescriptors = [];
	public static CLIBuilder Create()
	{
		return new CLIBuilder();
	}
	public CLIBuilder MapCommand(Action<CommandBuilder> builder)
	{
		var commandBuilder = new CommandBuilder();
		builder(commandBuilder);
		_commandDescriptors.Add(commandBuilder);
		return this;
	}
	public CLIBuilder MapCommand(string name, string shortName, Func<string[], bool?> action)
	{

		var builder = new CommandBuilder()
			.WithName(name)
			.WithShortName(shortName)
			.ExecuteWith(typeof(DefaultCommand));
		return this;
	}
}
