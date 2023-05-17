using System;

namespace DevConsole.Infrastructure.Commands;

public class UserActionException : Exception
{
    public UserActionException(string message) : base(message)
    {
    }
}