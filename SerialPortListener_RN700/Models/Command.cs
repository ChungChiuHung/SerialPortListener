using Newtonsoft.Json;
using System;

public interface ICommand
{
    string BuildCommand();
}

public abstract class Command : ICommand
{
    public string CommandName { get; set; }
    public object ParameterValue { get; set; }
    public int Number { get; set; }


    protected Command(string commandName, object parameterValue, int number)
    {
        CommandName = commandName;
        ParameterValue = parameterValue ?? "null";
        Number = number;
    }

    public abstract string BuildCommand();
}