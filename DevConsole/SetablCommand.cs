using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace UnityUtil.DevConsole
{
    public class SetableCommand : ConsoleCommand
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

        public SetableCommand(string name, Type type, FieldInfo field, string successMsg = null, string errorMsg = null)
        {
            this.name = name;
            InvokerType = type;
            this.field = field;
            this.successMsg = successMsg;
            this.errorMsg = errorMsg;
        }

        public SetableCommand(string name, Type type, PropertyInfo property, string successMsg = null, string errorMsg = null)
        {
            this.name = name;
            InvokerType = type;
            this.property = property;
            this.successMsg = successMsg;
            this.errorMsg = errorMsg;
        }

        protected override Type[] GetExecuteParmTypes()
        {
            if (field != null)
            {
                return new Type[1] { field.FieldType };
            }
            else if (property != null)
            {
                return new Type[1] { property.PropertyType };
            }
            return Array.Empty<Type>();
        }

        private void OnExecute(object value)
        {
            UnityEngine.Object[] invokers = Resources.FindObjectsOfTypeAll(InvokerType);
            if (field != null)
            {
                for (int i = 0; i < invokers.Length; i++)
                {
                    var invoker = invokers[i];
                    field.SetValue(invoker, value);
                }
            }
            else if (property != null)
            {
                for (int i = 0; i < invokers.Length; i++)
                {
                    var invoker = invokers[i];
                    property.SetValue(invoker, value);
                }
            }
        }
    }
}