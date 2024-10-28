namespace Maid.CLI;

public interface ICommand
{
	public bool GetInfo(out string commandName, out string commandShortName, out List<string> usage);
	public bool Execute(string[] args);
}
public interface ICommandWithHelp
{
	public List<string> GetHelp();
}

public interface ICommandWithOptions
{
	public List<CLIOption> GetOptions();
}
public interface ICommandWithFlags
{
	public List<CLIFlag> GetFlags();
}
internal class ActionCommand : ICommand
{
	public required Func<string[], bool?> Action { get; init; }

	public bool GetInfo(out string commandName, out string commandShortName, out List<string> usage)
	{
		commandName = string.Empty;
		commandShortName = string.Empty;
		usage = [];
		return true;
	}

	public bool Execute(string[] args)
	{
		return Action(args) ?? false;
	}
}

public class InfoCommand : ICommand, ICommandWithHelp, ICommandWithFlags, ICommandWithOptions
{
	private static readonly CLIFlag helpFlag = CLIFlagBuilder.Create().WithName("help").WithShortName("h").Build();

	private static readonly CLIOption textOption = CLIOptionBuilder.Create().WithName("text").WithShortName("t").Build();
	public bool Execute(string[] args)
	{
		throw new NotImplementedException();
	}

	public List<CLIFlag> GetFlags()
	{
		return [
			helpFlag
			];
	}

	public List<string> GetHelp()
	{
		throw new NotImplementedException();
	}

	public bool GetInfo(out string commandName, out string commandShortName, out List<string> usage)
	{
		throw new NotImplementedException();
	}

	public List<CLIOption> GetOptions()
	{
		return [
			textOption
		];
	}
}

/// <summary>
/// Represents a command line argument - e.g. --name=john | -o="text.html"
/// </summary>
public record CLIOption
{
	public string? Name { get; set; }
	public string? Shortname { get; set; }
	public bool IsRequired { get; set; } = false;
}

public class CLIOptionBuilder
{
	CLIOption _cliArgument = new();
	public static CLIOptionBuilder Create()
	{
		return new();
	}
	public CLIOptionBuilder WithName(string name)
	{
		_cliArgument = _cliArgument with { Name = name };
		return this;
	}
	public CLIOptionBuilder WithShortName(string shortName)
	{
		_cliArgument = _cliArgument with { Shortname = shortName };
		return this;
	}
	public CLIOption Build()
	{
		return _cliArgument;
	}
}

/// <summary>
/// Represents a command line flag - e.g. --force | -f
/// </summary>
public record CLIFlag
{
	public string? Name { get; set; }
	public string? ShortName { get; set; }
}
public class CLIFlagBuilder
{
	CLIFlag _cliFlag = new();
	public static CLIFlagBuilder Create()
	{
		return new();
	}
	public CLIFlagBuilder WithName(string name)
	{
		_cliFlag = _cliFlag with { Name = name };
		return this;
	}
	public CLIFlagBuilder WithShortName(string shortName)
	{
		_cliFlag = _cliFlag with { ShortName = shortName };
		return this;
	}
	public CLIFlag Build()
	{
		return _cliFlag;
	}
}
	public class CommandBuilder
{
	internal string _name = string.Empty;
	internal string _shortName = string.Empty;
	internal Type _commandType = default!;
	internal Action<string[]> _action = default!;

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
	public CommandBuilder WithAction(Action<string[]> action)
	{
		_action = action;
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
			.ExecuteWith(typeof(ActionCommand));
		return this;
	}
}
