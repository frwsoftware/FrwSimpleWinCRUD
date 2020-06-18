using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FrwSoftware
{
    public class DbBase
    {
        protected static string JSON_DATA_BLOB = "JSON_DATA_BLOB";

        static public string GetTableName(Type t)
        {
            return t.Name;
        }
        static public string GetColumnName(PropertyInfo p)
        {
            return p.Name;
        }
        static public string GetColumnNameForStatement(PropertyInfo p)
        {
            return "[" + GetColumnName(p) + "]";
        }

        static protected string NameToDB(string name)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < name.Length; i++)
            {
                if (char.IsUpper(name[i]))
                {
                    if (i != 0) str.Append("_");
                    str.Append(name[i]);
                }
                else str.Append(char.ToUpper(name[i]));
            }
            return str.ToString();
        }
        protected static bool IsPK(PropertyInfo p)
        {
            return (AttrHelper.GetAttribute<JPrimaryKey>(p) != null);
        }


        protected static bool IsAutoInc(PropertyInfo p)
        {
            return (AttrHelper.GetAttribute<JAutoIncrement>(p) != null);
        }

        protected static int? MaxStringLength(PropertyInfo p)
        {
            return null;//todo
        }

        protected static object GetDefaultValue(PropertyInfo p)
        {
            return null;//todo
        }

        protected static bool IsMarkedNotNull(PropertyInfo p)
        {
            return (AttrHelper.GetAttribute<JRequired>(p) != null);
        }

        static protected bool IsIgnore(PropertyInfo p)
        {
            if (AttrHelper.GetAttribute<JIgnore>(p) != null
                || AttrHelper.GetAttribute<JsonIgnoreAttribute>(p) != null
                || AttrHelper.GetAttribute<JManyToMany>(p) != null
                || AttrHelper.GetAttribute<JOneToMany>(p) != null
                )
            {
                return true;
            }
            else return false;
        }
        static protected bool IsForeignKey(PropertyInfo p)
        {
            if (AttrHelper.GetAttribute<JManyToOne>(p) != null 
                || AttrHelper.GetAttribute<JOneToOne>(p) != null)
                return true;
            else return false;
        }
        static protected bool IsNotStored(PropertyInfo p)
        {
            if (p.CanWrite)
                return false;
            else return true;
        }
        static protected bool IsComplex(PropertyInfo p)
        {
            //date
            if (p.PropertyType == typeof(DateTime)
                || p.PropertyType == typeof(DateTimeOffset))
                return false;
            //simple
            if (AttrHelper.IsSimple(p.PropertyType)) return false;
            if (IsForeignKey(p)) return false;//for any case 
            //else
            return true;
        }
        static protected bool IsNecessary(PropertyInfo p)
        {
            if (AttrHelper.GetAttribute<JManyToOne>(p) != null
                || AttrHelper.GetAttribute<JOneToOne>(p) != null
                || AttrHelper.GetAttribute<JPrimaryKey>(p) != null)
            {
                return true;
            }
            else return false;
        }
        static protected bool IsShortTable(Type t)
        {
            JEntity entityAttr = AttrHelper.GetClassAttribute<JEntity>(t);
            return entityAttr.ShortTable;
        }

    }
}
