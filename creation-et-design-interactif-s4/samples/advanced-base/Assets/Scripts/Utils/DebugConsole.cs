using System.Collections.Generic;
using System;
using System.Linq;

using UnityEngine;

// https://www.youtube.com/watch?v=VzOEM-4A2OM
// https://github.com/MinaPecheux/UnityTutorials-RTS/blob/master/Assets/Scripts/DebugConsole/DebugConsole.cs
public class DebugConsole : MonoBehaviour
{
    private bool showConsole;
    private bool showHelp;

    private string input = "";

    public static DebugCommand HELP;
    public static DebugCommand HELP_2;
    public static DebugCommand<string> TELEPORT;
    public static DebugCommand QUIT;

    public List<object> commandList;

    private Vector2 scroll;

    [SerializeField]
    private GUIStyle btnStyle;

    [SerializeField]
    private GUIStyle autocompleteResultStyle;

    private void Awake()
    {
        HELP = new DebugCommand("help", "Show all commands", "help", () =>
        {
            showHelp = true;
        });

        HELP_2 = new DebugCommand("help 2", "Show all commands", "help", () =>
        {
            showHelp = true;
        });

        QUIT = new DebugCommand("quit", "Close the help command", "quit", () =>
        {
            showConsole = false;
            showHelp = false;
        });

        TELEPORT = new DebugCommand<string>("teleport", "Teleports player into a specific place", "teleport <string Vector2>", (vector) =>
        {
            Debug.Log(GetVector2("2, 3"));
        });

        commandList = new List<object> {
            HELP,
            HELP_2,
            TELEPORT,
            QUIT
        };
    }

    public Vector2 GetVector2(string rString)
    {
        string[] temp = rString.Split(',');
        float x = System.Convert.ToSingle(temp[0]);
        float y = System.Convert.ToSingle(temp[1]);
        Vector2 rValue = new Vector2(x, y);

        return rValue;
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.I))
        {
            showConsole = true;
        }
#endif
    }

    private void OnGUI()
    {
        btnStyle = new GUIStyle(GUI.skin.button);
        autocompleteResultStyle = new GUIStyle(GUI.skin.label);

        if (!showConsole) { return; }
        float y = 0f;
        GUI.Box(new Rect(0, y, Screen.width, 30), "");

        GUI.SetNextControlName("MyTextField");
        input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), input);
        GUI.FocusControl("MyTextField");

        // Log area
        y = 30f;
        GUI.Box(new Rect(0, y, Screen.width, Screen.height - y), "");

        if (input.Length > 0)
        {
            Autocomplete(y, input);
        }

        Event e = Event.current;
        if (e.isKey)
        {
            if (e.keyCode == KeyCode.Return && input.Length > 0)
            {
                HandleInput();
                input = "";
            }
            else if (e.keyCode == KeyCode.Escape)
            {
                showConsole = false;
            }
            else if (input.Length > 0)
            {
                // Autocomplete(y, input);
            }
        }

        // if (Event.current.Equals(Event.KeyboardEvent("[enter]")))
        // {
        //     HandleInput();
        //     input = "";
        // }


        // GUI.backgroundColor = new Color(0, 0, 0, 0.25f);





        // if (showHelp)
        // {
        //     GUI.Box(new Rect(0, y, Screen.width, 100), "");
        //     Rect helpContainerViewport = new Rect(0, 0, Screen.width - 30, 20 * commandList.Count);
        //     scroll = GUI.BeginScrollView(new Rect(0, y + 5, Screen.width, 90), scroll, helpContainerViewport);

        //     for (int i = 0; i < commandList.Count; i++)
        //     {
        //         DebugCommandBase command = commandList[i] as DebugCommandBase;
        //         string commandLabel = $"{command.commandFormat} - {command.commandDescription}";

        //         Rect commandLabelRect = new Rect(5, 20 * i, helpContainerViewport.width - 100, 20);

        //         if (GUI.Button(commandLabelRect, commandLabel, btnStyle))
        //         {
        //             input = command.commandFormat;
        //         }
        //     }

        //     GUI.EndScrollView();
        //     y += 100f;
        // }

    }

    private void HandleInput()
    {
        string[] listCommandProperties = input.Split("");

        for (int i = 0; i < commandList.Count; i++)
        {
            DebugCommandBase commandBase = commandList[i] as DebugCommandBase;
            if (input.Contains(commandBase.commandId))
            {
                if (commandList[i] as DebugCommand != null)
                {
                    (commandList[i] as DebugCommand).Invoke();
                }
                else if (commandList[i] as DebugCommand<string> != null)
                {
                    (commandList[i] as DebugCommand<string>).Invoke(listCommandProperties[0]);
                }
            }
        }
    }

    private void Autocomplete(float y, string newInput)
    {
        IEnumerable<object> autocompleteCommands = commandList.Select(x => x as DebugCommandBase)
            .Where(k => k.commandId.StartsWith(newInput.ToLower()));
        // IEnumerable<string> autocompleteCommands = commandList.Cast<DebugCommandBase>()
        // .Where(k => k.commandId.StartsWith(newInput.ToLower())).Select(x => x.commandId);

        foreach (DebugCommandBase command in autocompleteCommands.Cast<DebugCommandBase>())
        {
            string commandLabel = $"{command.commandFormat} - {command.commandDescription}";
            GUI.Label(
                new Rect(10f, y, Screen.width, 20),
                commandLabel
            );
            y += 16;
        }
    }
}
