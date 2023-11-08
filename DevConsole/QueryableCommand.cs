using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace UnityUtil.DevConsole
{
    public class QueryableCommand : ConsoleCommand
    {
        public override string Name => name;
        public override string SuccessMessage => successMsg;
        public override string ErrorMessage => errorMsg;
        public Type InvokerType { get; private set; }
        private readonly string name;
        private string successMsg;
        private string errorMsg;
        private readonly FieldInfo field;
        private readonly PropertyInfo property;

        public QueryableCommand(string name, Type type, FieldInfo field, string successMsg = null, string errorMsg = null)
        {
            this.name = name;
            InvokerType = type;
            this.field = field;
            this.successMsg = successMsg;
            this.errorMsg = errorMsg;
        }

        public QueryableCommand(string name, Type type, PropertyInfo property, string successMsg = null, string errorMsg = null)
        {
            this.name = name;
            InvokerType = type;
            this.property = property;
            this.successMsg = successMsg;
            this.errorMsg = errorMsg;
        }

        public void OnExecute()
        {
            MemberInfo target = field == null ? property : field;
            UnityEngine.Object[] invokers = Resources.FindObjectsOfTypeAll(InvokerType);
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine($"get {target.Name} from {InvokerType}: ");

            if (field != null)
            {
                for (int i = 0; i < invokers.Length; i++)
                {
                    var invoker = invokers[i];
                    stringBuilder.AppendLine($"GameObjectName: {invoker.name}, value: {field.GetValue(invoker)}");
                }
            }
            else if (property != null)
            {
                for (int i = 0; i < invokers.Length; i++)
                {
                    var invoker = invokers[i];
                    stringBuilder.AppendLine($"GameObjectName: {invoker.name}, value: {property.GetValue(invoker)}");
                }
            }
            if (successMsg != null)
            {
                stringBuilder.AppendLine(successMsg);
            }
            successMsg = stringBuilder.ToString();
        }
    }
}