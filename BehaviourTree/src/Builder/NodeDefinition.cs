using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityUtil.BehaviourTree
{
    public class NodeDefinition
    {
        public enum NodeType
        {
            Root,
            Action,
            Decorator,
            Composite
        }

        public Type NodeObjectType { get; private set; }
        public string NodeName {get; private set;}
        public NodeType ChildrenType {get; private set;}

        private readonly Dictionary<string, Delegate> fieldSetterMap = new();
        private readonly Dictionary<string, Delegate> propertySetterMap = new();
        public readonly Dictionary<string, Type> memberTypeMap = new();
        private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private NodeDefinition() { }

        public void SetMemberValue(string memberName, object obj, object value)
        {
            Type targetType = memberTypeMap.GetValueOrDefault(memberName) 
                ?? throw new InvalidOperationException($"Can't Find {memberName} on NodeDefinition {this}");
            value = Convert.ChangeType(value, targetType);

            Delegate @delegate = fieldSetterMap.GetValueOrDefault(memberName, propertySetterMap.GetValueOrDefault(memberName));
            @delegate.DynamicInvoke(obj, value);
        }

        public static NodeDefinition CretaeDefinition(Type type)
        {
            NodeDefinition definition = new()
            {
                NodeObjectType = type,
                NodeName = type.Name,
            };

            if (typeof(ActionNode).IsAssignableFrom(type))
            {
                definition.ChildrenType = NodeType.Action;
            } 
            else if(typeof(DecoratorNode).IsAssignableFrom(type))
            {
                definition.ChildrenType = NodeType.Decorator;
            }
            else if(typeof(CompositeNode).IsAssignableFrom(type))
            {
                definition.ChildrenType = NodeType.Composite;
            }
            else
            {
                throw new InvalidOperationException($"Wrong Node Type: {type}.");
            }

            IEnumerable<FieldInfo> fieldInfos = type.GetFields(bindingFlags)
                .Where(field => field.IsPublic || field.GetCustomAttribute<SerializeField>() != null);

            IEnumerable<PropertyInfo> propertyInfos = type.GetProperties(bindingFlags)
                .Where(prop => prop.CanWrite && (prop.GetCustomAttribute<SerializeField>() != null));

            foreach (var fieldInfo in fieldInfos)
            {
                Delegate @delegate = ExpressionsUtil.GetFieldSetterExpression(type, fieldInfo);
                string name = char.ToLower(fieldInfo.Name[0]) + fieldInfo.Name[1..];
                definition.fieldSetterMap[name] = @delegate;
                definition.memberTypeMap[name] = fieldInfo.FieldType;
            }

            foreach (var propertyInfo in propertyInfos)
            {
                Delegate @delegate = ExpressionsUtil.GetPropertySetterExpression(type, propertyInfo);
                string name = char.ToLower(propertyInfo.Name[0]) + propertyInfo.Name[1..];
                definition.propertySetterMap[name] = @delegate;
                definition.memberTypeMap[name] = propertyInfo.PropertyType;
            }
            return definition;
        }


        public static NodeDefinition[] CretaeDefinition(IEnumerable<Type> types)
        {
            NodeDefinition[] definitions = new NodeDefinition[types.Count()];
            int index = 0;
            foreach(var type in types)
            {
                definitions[index] = CretaeDefinition(type);
                index++;
            }
            return definitions;
        }
    }
}