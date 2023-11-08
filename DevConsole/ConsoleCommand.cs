using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityUtil.DevConsole
{
    public abstract class ConsoleCommand
    {
        public abstract string Name { get; }
        public abstract string SuccessMessage { get; }
        public abstract string ErrorMessage { get; }
        private MethodInfo executeMethod;
        private Type[] executeParmTypes;
        private bool hasInit = false;

        protected virtual MethodInfo GetExecuteMethod()
        {
            return GetType().GetMethod("OnExecute", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                ?? throw new MissingMethodException("ConsoleCommand need execute the method named OnExecute");
        }

        protected virtual Type[] GetExecuteParmTypes()
        {
            return executeMethod.GetParameters().Select(p => p.ParameterType).ToArray();
        }

        private void Init()
        {
            executeMethod = GetExecuteMethod();
            executeParmTypes = GetExecuteParmTypes();
        }

        internal string Execute(string[] args)
        {
            if (!hasInit)
            {
                Init();
                hasInit = true;
            }

            try
            {
                DoCommand(args);
                return $"[success] {SuccessMessage} -- [from] {Name}" ?? string.Empty;
            }
            catch (Exception e)
            {
                string msg = string.IsNullOrEmpty(ErrorMessage) ? e.Message : ErrorMessage;
                return $"[error] {msg} -- [from] {Name}";
            }

        }

        private void DoCommand(string[] args)
        {
            ParameterInfo[] parameters = executeMethod.GetParameters();
            if (executeParmTypes.Length == 0)
            {
                if (parameters.Length == 1 && parameters.First().ParameterType.IsArray)
                {
                    executeMethod.Invoke(this, new object[1] { Array.Empty<object>() });
                }
                else
                {
                    executeMethod.Invoke(this, null);
                }

            }
            else
            {
                object[] castArgs = new object[executeParmTypes.Length];
                for (int i = 0; i < executeParmTypes.Length; i++)
                {
                    Type paramType = executeParmTypes[i];
                    castArgs[i] = ConvertString(args[i], paramType);
                }

                if (parameters.Length == 1 && parameters.First().ParameterType.IsArray)
                {
                    executeMethod.Invoke(this, new object[1] { castArgs });
                }
                else
                {
                    executeMethod.Invoke(this, castArgs);
                }
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
            throw new InvalidOperationException($"string can't cast to {targetType}");
        }



    }


    public class GeneralCommand : ConsoleCommand
    {
        public override string Name => name;
        public override string SuccessMessage => successMsg;
        public override string ErrorMessage => errorMsg;

        private readonly string successMsg;
        private readonly string errorMsg;
        private readonly string name;
        private readonly object invoker;
        private readonly MethodInfo insideMethodInfo;
        private readonly Delegate insideDelegate;

        public GeneralCommand(MethodInfo methodInfo, object invoker, string name, string successMsg = null, string errorMsg = null)
        {
            insideMethodInfo = methodInfo;
            this.invoker = invoker;
            this.name = name;
            this.successMsg = successMsg;
            this.errorMsg = errorMsg;
        }

        public GeneralCommand(Delegate @delegate, string name, string successMsg = null, string errorMsg = null)
        {
            insideDelegate = @delegate;
            this.name = name;
            this.successMsg = successMsg;
            this.errorMsg = errorMsg;
        }

        public GeneralCommand(Action action, string name, string successMsg = null, string errorMsg = null)
        {
            insideDelegate = action;
            this.name = name;
            this.successMsg = successMsg;
            this.errorMsg = errorMsg;
        }

        public void OnExecute(object[] args)
        {
            if (insideDelegate != null)
            {
                insideDelegate.DynamicInvoke(args);
            }
            else
            {
                insideMethodInfo.Invoke(invoker, args);
            }
        }

        protected override Type[] GetExecuteParmTypes()
        {
            if (insideDelegate != null)
            {
                return insideDelegate.Method.GetParameters().Select(p => p.ParameterType).ToArray();
            }
            else
            {
                return insideMethodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
            }
        }
    }
}


