#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using Unity.VisualScripting;

namespace UnityUtil.DevConsole
{
    public abstract class ConsoleCommand
    {
        public abstract string Name { get; }
        internal abstract string? SuccessMessage { get; }
        internal abstract string? ErrorMessage { get; }
        internal abstract Delegate CallBack { get; }

        internal string Execute(string[] args)
        {
            try
            {
                ProcessCommand(args);
                return $"[success] {SuccessMessage} -- [from] {Name}" ?? string.Empty;
            }
            catch (Exception e)
            {
                string msg = string.IsNullOrEmpty(ErrorMessage) ? e.Message : ErrorMessage;
                return $"[error] {msg} -- [from] {Name}";
            }

        }

        private void ProcessCommand(string[] args)
        {
            ParameterInfo[] parameters = CallBack.Method.GetParameters();
            if (parameters.Length == 0)
            {
                CallBack.DynamicInvoke();
            }
            else
            {
                object[] castArgs = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    Type paramType = parameters[i].ParameterType;
                    castArgs[i] = ConvertString(args[i], paramType);
                }
                CallBack.DynamicInvoke(castArgs);
            }
        }

        private static object ConvertString(string str, Type targetType)
        {
            if (targetType == typeof(string)) return str;
            if (targetType == typeof(object)) return str;

            if (targetType == typeof(int))
            {
                return Convert.ToInt32(str);
            }
            if (targetType == typeof(double))
            {
                return Convert.ToDouble(str);
            }
            if (targetType == typeof(float))
            {
                return Convert.ToSingle(str);
            }
            if (targetType == typeof(bool))
            {
                return Convert.ToBoolean(str);
            }
            if (targetType == typeof(byte))
            {
                return Convert.ToByte(str);
            }
            if (targetType == typeof(char))
            {
                return Convert.ToChar(str);
            }
            throw new InvalidConversionException($"string can't cast to {targetType}");
        }



    }

    public class GeneralConsoleCommand : ConsoleCommand
    {

        private readonly string? successMsg;
        private readonly string? errorMsg;
        private readonly string? name;
        private readonly Delegate command;

        internal override string? SuccessMessage => successMsg;

        internal override string? ErrorMessage => errorMsg;

        public override string Name => name!;

        internal override Delegate CallBack => command;

        public GeneralConsoleCommand(Delegate @delegate, string name, string? successMsg = null, string? errorMsg = null)
        {
            command = @delegate;
            this.successMsg = successMsg;
            this.errorMsg = errorMsg;
            this.name = name;
        }
    }


    public class DevConsole
    {
        protected Dictionary<string, ConsoleCommand> commandMap = new();
        protected const int capacity = 16;
        protected Queue<string> messages = new(capacity);

        public void AddCommand(ConsoleCommand command)
        {
            string commandName = command.Name;
            commandMap.TryAdd(commandName, command);
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

    }
}


