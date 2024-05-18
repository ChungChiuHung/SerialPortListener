using System;
using System.Security.Cryptography.X509Certificates;

public static class CommandFactory
{
    public static ICommand CreateCommand(string commandName, object parameterValue, int number)
    {
        return new UserCommand(commandName, parameterValue, number);
    }

    // If you have other command types, you can define similar methods here
    // Example:
    // public static ICommand CreateProductCommand(string commandName, object parameterValue, int number)
    // {
    //     return new ProductCommand(commandName, parameterValue, number);
    // }
}