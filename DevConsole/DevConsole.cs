using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using TMPro;
using UnityEngine.UI;

namespace UnityUtil.DevConsole
{
    public class DevConsole
    {
        protected Dictionary<string, ConsoleCommand> commandMap = new();
        protected const int capacity = 16;
        protected Queue<string> messages = new(capacity);

        public void AddCommand(ConsoleCommand command)
        {
            string commandName = command.Name;
            bool success = commandMap.TryAdd(commandName, command);
            if (!success)
            {
                Debug.LogWarning($"{commandName} has been added before");
            }
        }

        public void AddCommand(params ConsoleCommand[] commands)
        {
            foreach (var command in commands)
            {
                string commandName = command.Name;
                commandMap.TryAdd(commandName, command);
            }
        }

        public void AddCommand(IEnumerable<ConsoleCommand> commands)
        {
            foreach (var command in commands)
            {
                string commandName = command.Name;
                commandMap.TryAdd(commandName, command);
            }
        }

        public void ExecuteCommand(string commandName, string[] args)
        {
            if (commandMap.ContainsKey(commandName))
            {
                string msg = commandMap[commandName].Execute(args);

                if (messages.Count == capacity)
                {
                    messages.Dequeue();
                }
                messages.Enqueue($"{Time.realtimeSinceStartup: 0.00}: {msg}");
            }
        }

        public string[] GetMessages()
        {
            return messages.ToArray();
        }


        public void DumpCommands()
        {
            commandMap.Dump();
        }
    }

    public class WrapperConsole
    {
        private readonly DevConsole console = new ScanableConsole();
        private readonly TMP_Text history;
        private readonly Button btn;
        private readonly TMP_InputField inputField;
        private readonly LoopFixedCircle<string> commandCircle = new(10);

        public WrapperConsole(TMP_Text history, Button btn, TMP_InputField inputField)
        {
            this.history = history;
            this.btn = btn;
            this.inputField = inputField;
            btn.onClick.AddListener(SendCommand);
        }

        public void AddCommand(ConsoleCommand command)
        {
            console.AddCommand(command);
        }

        public void AddCommand(params ConsoleCommand[] commands)
        {
            console.AddCommand(commands);
        }

        public void AddCommand(IEnumerable<ConsoleCommand> commands)
        {
            console.AddCommand(commands);
        }

        public void SendCommand()
        {
            string command = inputField.text;
            commandCircle.Push(command);
            if (string.IsNullOrEmpty(command)) return;
            if (command.Contains(";"))
            {
                string[] commands = command.Split(";");
                for (int i = 0; i < commands.Length; i++)
                {
                    ExecuteCommand(commands[i]);
                }
            }
            else
            {
                ExecuteCommand(command);
            }

            inputField.text = "";
            history.text = string.Join("\n", console.GetMessages());
            inputField.ActivateInputField();
        }

        private void ExecuteCommand(string command)
        {
            if (command.Contains(" "))
            {
                string[] all = command.Split(" ");
                command = all.First();
                string[] args = all.Skip(1).ToArray();
                console.ExecuteCommand(command, args);
            }
            else
            {
                console.ExecuteCommand(command, null);
            }

        }

        public string GetNextCommand()
        {
            return commandCircle.Next();
        }

        public string GetLastCommand()
        {
            return commandCircle.Last();
        }


        public void DumpCommands()
        {
            console.DumpCommands();
        }
    }
}