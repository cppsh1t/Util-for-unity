using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityUtil.DevConsole
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CommandObjectAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class CommandAttribute : Attribute
    {
        public string Name { get; private set; } = null;
        public string SuccessMessage { get; private set; } = null;
        public string ErrorMessage { get; private set; } = null;
        public CommandAttribute() { }
        public CommandAttribute(string name) => Name = name;

        public CommandAttribute(string name, string successMsg, string errorMsg)
        {
            Name = name;
            SuccessMessage = successMsg;
            ErrorMessage = errorMsg;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CommandTypeAttribute : Attribute
    {
        
    }
}