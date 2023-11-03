using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityUtil.DevConsole;

namespace UnityUtil.DevConsole
{
    public class WrapperConsole
    {
        private readonly DevConsole console = new();
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

    }
}

