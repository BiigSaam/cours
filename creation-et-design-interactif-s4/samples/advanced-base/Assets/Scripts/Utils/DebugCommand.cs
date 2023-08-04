using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DebugCommandBase
{
    private string _commandId;
    private string _commandDescription;
    private string _commandFormat;

    public string commandId { get; private set; }
    public string commandDescription { get; private set; }
    public string commandFormat { get; private set; }

    public DebugCommandBase(string id, string description, string format)
    {
        commandId = id;
        description = commandDescription;
        commandFormat = format;
    }
}

public class DebugCommand : DebugCommandBase
{
    private Action command;

    public DebugCommand(string id, string description, string format, Action _command) : base(id, description, format)
    {
        command = _command;
    }

    public void Invoke() {
        command.Invoke();
    }
}

public class DebugCommand<T1> : DebugCommandBase
{
    private Action<T1> command;

    public DebugCommand(string id, string description, string format, Action<T1> _command) : base(id, description, format)
    {
        command = _command;
    }

    public void Invoke(T1 val) {
        command.Invoke(val);
    }
}
