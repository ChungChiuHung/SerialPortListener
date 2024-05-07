using Newtonsoft.Json;

public class UserCommand : Command
{
    public UserCommand(string commandName, object parameterValue, int number)
        : base(commandName, parameterValue, number) { }

    public override string BuildCommand()
    {
        var commandObject = new
        {
            method = CommandName,
            @params = ParameterValue ?? "null",
            id = Number
        };

        var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        return JsonConvert.SerializeObject(commandObject, settings);
    }

    public override string ToString()
    {
        return BuildCommand();
    }
}