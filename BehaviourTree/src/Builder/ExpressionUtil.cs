using System;
using System.Linq.Expressions;
using System.Reflection;

namespace UnityUtil.BehaviourTree
{
    public static class ExpressionsUtil
    {
        private static MethodInfo genericPropSetterExp;
        private static MethodInfo genericFieldSetterExp;


        static ExpressionsUtil()
        {
            genericPropSetterExp = typeof(ExpressionsUtil).GetMethod("GetPropertySetterExpression", new Type[] { typeof(PropertyInfo) })!;
            genericFieldSetterExp = typeof(ExpressionsUtil).GetMethod("GetFieldSetterExpression", new Type[] { typeof(FieldInfo) })!;
        }

        public static Action<T, object> GetPropertySetterExpression<T>(PropertyInfo propertyInfo)
        {
            // 创建参数表达式
            ParameterExpression objParam = Expression.Parameter(typeof(T), "obj");
            ParameterExpression valueParam = Expression.Parameter(typeof(object), "value");

            // 将value转换为目标属性类型
            UnaryExpression valueConversion = Expression.Convert(valueParam, propertyInfo.PropertyType);

            // 创建属性表达式
            MemberExpression propertyExpression = Expression.Property(objParam, propertyInfo);

            // 创建赋值表达式
            BinaryExpression assignmentExpression = Expression.Assign(propertyExpression, valueConversion);

            // 创建Lambda表达式
            Expression<Action<T, object>> lambdaExpression = Expression.Lambda<Action<T, object>>(assignmentExpression, objParam, valueParam);

            // 编译Lambda表达式并执行
            return lambdaExpression.Compile();
        }

        public static Action<T, object> GetFieldSetterExpression<T>(FieldInfo fieldInfo)
        {
            // 创建参数表达式
            ParameterExpression objParam = Expression.Parameter(typeof(T), "obj");
            ParameterExpression valueParam = Expression.Parameter(typeof(object), "value");

            // 将value转换为目标属性类型
            UnaryExpression valueConversion = Expression.Convert(valueParam, fieldInfo.FieldType);

            // 创建属性表达式
            MemberExpression fieldExpression = Expression.Field(objParam, fieldInfo);

            // 创建赋值表达式
            BinaryExpression assignmentExpression = Expression.Assign(fieldExpression, valueConversion);

            // 创建Lambda表达式
            Expression<Action<T, object>> lambdaExpression = Expression.Lambda<Action<T, object>>(assignmentExpression, objParam, valueParam);

            // 编译Lambda表达式并执行
            return lambdaExpression.Compile();
        }

        public static Delegate GetPropertySetterExpression(Type type, PropertyInfo propertyInfo)
        {
            return (genericPropSetterExp.MakeGenericMethod(type).Invoke(null, new object[] { propertyInfo }) as Delegate)!;
        }

        public static Delegate GetFieldSetterExpression(Type type, FieldInfo fieldInfo)
        {
            return (genericFieldSetterExp.MakeGenericMethod(type).Invoke(null, new object[] { fieldInfo }) as Delegate)!;
        }
    }
}

