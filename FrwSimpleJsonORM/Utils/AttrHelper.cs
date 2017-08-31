using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FrwSoftware
{
    #region  AttrHelper
    public class AttrHelper
    {
        #region  Attributes
        public static bool IsAttributePresent(object o, string aspectName)
        {
            if (o == null) return false;
            Type t = o.GetType();
            return IsAttributePresent(t, aspectName);
        }
        public static bool IsAttributePresent<T>(Type t, string aspectName) where T : Attribute
        {
            T attr = GetAttribute<T>(t, aspectName);
            return (attr != null);
        }
        public static T GetAttribute<T>(object o, string aspectName) where T : Attribute
        {
            if (o == null) return null;
            Type t = o.GetType();
            return GetAttribute<T>(t, aspectName);
        }
        public static T GetAttribute<T>(Type t, string aspectName) where T : Attribute
        {
            PropertyInfo p = t.GetProperty(aspectName);
            if (p == null) return null;
            return GetAttribute<T>(p);
        }
        public static T GetAttribute<T>(PropertyInfo p) where T : Attribute
        {
            if (p == null) return null;
            T attr = p.GetCustomAttributes(typeof(T), false).Cast<T>().FirstOrDefault();
            return attr;
        }
        public static T GetClassAttribute<T>(Type t) where T : Attribute
        {
            return t.GetCustomAttribute<T>();
        }
        public static IEnumerable<PropertyInfo> GetPropertiesWithAttribute<T>(Type t) where T : Attribute
        {
            IEnumerable<PropertyInfo> props = t.GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(T)));
            return props;
        }
  
        public static Type GetTypeWithAttribute<TAttribute>() where TAttribute : System.Attribute
        {
            IEnumerable<Type> types = GetTypesWithAttribute<TAttribute>(true);
            return types.FirstOrDefault();
        }
        public static IEnumerable<Type> GetTypesWithAttribute<TAttribute>(bool inherit)
                          where TAttribute : System.Attribute
        {
            return from a in AppDomain.CurrentDomain.GetAssemblies()
                   from t in a.GetTypes()
                   where t.IsDefined(typeof(TAttribute), inherit)
                   select t;
        }
        public static bool IsAttributeDefinedForType<TAttribute>(Type type, bool inherit)
                          where TAttribute : System.Attribute
        {
            return type.IsDefined(typeof(TAttribute), inherit);
        }
        #endregion
        #region properties
        public static PropertyInfo GetProperty(Type t, string aspectName)
        {
            PropertyInfo p = t.GetProperty(aspectName);
            if (p == null) return null;
            return p;
        }
        public static Type GetPropertyType(Type t, string aspectName)
        {
            PropertyInfo p = t.GetProperty(aspectName);
            if (p == null) return null;
            return p.PropertyType;
        }
        public static PropertyInfo GetProperty<T>(Type t) where T : Attribute
        {
            return GetProperty(typeof(T), t);
        }
        public static IEnumerable<PropertyInfo> GetProperties<T>(Type t) where T : Attribute
        {
            return GetProperties(typeof(T), t);
        }
        public static PropertyInfo GetProperty(Type attrTYpe, Type t)
        {
            return GetProperties(attrTYpe, t).FirstOrDefault();
        }
        public static IEnumerable<PropertyInfo> GetProperties(Type attrTYpe, Type t)
        {
            var props = t.GetProperties().Where(
                 prop => Attribute.IsDefined(prop, attrTYpe));
            return props;
        }
        public static object GetPropertyValue(object o, string aspectName)
        {
            if (o == null) return null;
            PropertyInfo p = o.GetType().GetProperty(aspectName);
            if (p != null) return p.GetValue(o);
            else return null;
        }
        public static object GetPropertyValue(object o, PropertyInfo p)
        {
            if (o == null) return null;
            if (p != null) return p.GetValue(o);
            else return null;
        }
        public static void SetPropertyValue(object o, string aspectName, object value)
        {

            if (o == null) return;
            PropertyInfo p = o.GetType().GetProperty(aspectName);
            if (p == null) return;

            p.SetValue(o, value);

        }
        public static void SetPropertyValue(object o, PropertyInfo p, object value)
        {

            if (o == null) return;
            if (p == null) return;
            p.SetValue(o, value);

        }

        #endregion

        static public bool IsSameOrSubclass(Type potentialBase, Type potentialDescendant)
        {
            //http://stackoverflow.com/questions/2742276/how-do-i-check-if-a-type-is-a-subtype-or-the-type-of-an-object
            //IsSubclassOf - not work fo interface and class 
            //IsAssignableFrom work but wrong get true in case of typeof(int[]).IsAssignableFrom(typeof(uint[])).Dump();  
            return potentialBase.IsAssignableFrom(potentialDescendant) || potentialDescendant.IsSubclassOf(potentialBase)
                   || potentialDescendant == potentialBase;
        }

        static public Type GetGenericListArgType(Type type)
        {
            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>) || type.GetGenericTypeDefinition() == typeof(IList<>)
                || type.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                return type.GetGenericArguments()[0];
            }
            else throw new Exception("Not generic type " + type.FullName);
        }

        static public bool IsGenericListTypeOf(Type type, Type testedType)
        {
            if (type.IsGenericType && (typeof(List<>) == type.GetGenericTypeDefinition()  || typeof(IList<>) == type.GetGenericTypeDefinition()
                || typeof(IEnumerable<>) == type.GetGenericTypeDefinition()))
            {
                Type argType = type.GetGenericArguments()[0];
                if (IsSameOrSubclass(testedType, argType)) return true;
                //if (argType == testedType) return true;
            }
            return false;
        }
        //for dynamic
        public static bool IsPropertyExist(dynamic settings, string name)
        {
            if (settings == null) return false;

            Type objType = settings.GetType();
            if (objType == typeof(ExpandoObject))
            {
                return ((IDictionary<string, object>)settings).ContainsKey(name);
            }
            return objType.GetProperty(name) != null;

        }

        // this is alternative for typeof(T).GetProperties()
        // that returns base class properties before inherited class properties
        public static PropertyInfo[] GetBasePropertiesFirst(Type type)
        {
            var orderList = new List<Type>();
            var iteratingType = type;
            do
            {
                orderList.Insert(0, iteratingType);
                iteratingType = iteratingType.BaseType;
            } while (iteratingType != null);

            var props = type.GetProperties()
                .OrderBy(x => orderList.IndexOf(x.DeclaringType))
                .ToArray();
            return props;
        }
        #region clone
        //!!! clone get error when recurence 
        public static T Clone<T>(T obj)
        {
            DataContractSerializer dcSer = new DataContractSerializer(obj.GetType());
            MemoryStream memoryStream = new MemoryStream();

            dcSer.WriteObject(memoryStream, obj);
            memoryStream.Position = 0;

            T newObject = (T)dcSer.ReadObject(memoryStream);
            return newObject;
        }
        public static object Clone(object obj)
        {
            DataContractSerializer dcSer = new DataContractSerializer(obj.GetType());
            MemoryStream memoryStream = new MemoryStream();

            dcSer.WriteObject(memoryStream, obj);
            memoryStream.Position = 0;

            object newObject = dcSer.ReadObject(memoryStream);
            return newObject;
        }
        #endregion

    }

    #endregion
}
