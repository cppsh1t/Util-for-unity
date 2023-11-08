using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace UnityUtil.DevConsole
{
    public class InvokeableCommand : ConsoleCommand
    {
        public override string Name => name;
        public override string SuccessMessage => successMsg;
        public override string ErrorMessage => errorMsg;
        private readonly string name;
        private string successMsg;
        private string errorMsg;
        private readonly MethodInfo methodInfo;
        public Type InvokerType { get; private set; }

        public void OnExecute(object[] parameters = null)
        {
            UnityEngine.Object[] invokers = Resources.FindObjectsOfTypeAll(InvokerType);
            for (int i = 0; i < invokers.Length; i++)
            {
                methodInfo.Invoke(invokers[i], parameters);
            }
        }

        protected override Type[] GetExecuteParmTypes()
        {
            return methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
        }

        public InvokeableCommand(string name, Type type, MethodInfo methodInfo, string successMsg = null, string errorMsg = null)
        {
            this.name = name;
            this.successMsg = successMsg;
            this.errorMsg = errorMsg;
            InvokerType = type;
            this.methodInfo = methodInfo;
        }
    }
}