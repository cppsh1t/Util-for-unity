using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace UnityUtil.DevConsole
{
    public class ScanableConsole : DevConsole
    {
        private const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly;

        public ScanableConsole()
        {
            BuildCommands();
        }

        protected void BuildCommands()
        {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];

                if (typeof(ConsoleCommand).IsAssignableFrom(type) && type.GetCustomAttribute<CommandTypeAttribute>() != null)
                {
                    try 
                    {
                        ConsoleCommand command = Activator.CreateInstance(type) as ConsoleCommand;
                        AddCommand(command);   
                    }
                    catch(MissingMethodException)
                    {

                    }
                    continue;
                }


                if (type.GetCustomAttribute<CommandObjectAttribute>() == null) continue;

                MethodInfo[] methods = type.GetMethods(flags);
                for (int j = 0; j < methods.Length; j++)
                {
                    MethodInfo method = methods[j];
                    CommandAttribute att = method.GetCustomAttribute<CommandAttribute>();
                    if (att == null) continue;
                    InvokeableCommand invokeableCommand =
                        new(att.Name ?? $"{type.Name.ToLower()}_{method.Name.ToLower()}", type, method, att.SuccessMessage, att.ErrorMessage);
                    AddCommand(invokeableCommand);
                }

                FieldInfo[] fields = type.GetFields(flags);
                for (int j = 0; j < fields.Length; j++)
                {
                    FieldInfo field = fields[j];
                    CommandAttribute att = field.GetCustomAttribute<CommandAttribute>();
                    if (att == null) continue;
                    string queryName = att.Name == null ? $"get_{type.Name.ToLower()}_{field.Name.ToLower()}" : $"get_{att.Name}";
                    string setName = att.Name == null ? $"set_{type.Name.ToLower()}_{field.Name.ToLower()}" : $"set_{att.Name}";
                    QueryableCommand queryableCommand = new(queryName, type, field, att.SuccessMessage, att.ErrorMessage);
                    AddCommand(queryableCommand);
                    SetableCommand setableCommand = new(setName, type, field, att.SuccessMessage, att.ErrorMessage);
                    AddCommand(setableCommand);
                }

                PropertyInfo[] props = type.GetProperties(flags);
                for (int j = 0; j < props.Length; j++)
                {
                    PropertyInfo prop = props[j];
                    CommandAttribute att = prop.GetCustomAttribute<CommandAttribute>();
                    if (att == null) continue;
                    string queryName = att.Name == null ? $"get_{type.Name.ToLower()}_{prop.Name.ToLower()}" : $"get_{att.Name}";
                    QueryableCommand queryableCommand = new(queryName, type, prop, att.SuccessMessage, att.ErrorMessage);
                    AddCommand(queryableCommand);

                    if (prop.CanWrite)
                    {
                        string setName = att.Name == null ? $"set_{type.Name.ToLower()}_{prop.Name.ToLower()}" : $"set_{att.Name}";
                        SetableCommand setableCommand = new(setName, type, prop, att.SuccessMessage, att.ErrorMessage);
                        AddCommand(setableCommand);
                    }
                }
            }
        }
    }

}