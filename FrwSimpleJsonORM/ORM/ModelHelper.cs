﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FrwSoftware
{
    public class ModelHelper
    {
        #region i18n
        public static string GetPropertyJDescriptionOrName(PropertyInfo p)
        {
            if (p == null) return null;
            DisplayNameAttribute displayNameAttr = p.GetCustomAttribute<DisplayNameAttribute>();
            if (displayNameAttr != null && displayNameAttr.DisplayName != null) return displayNameAttr.DisplayName;
            DisplayAttribute displayAttr = p.GetCustomAttribute<DisplayAttribute>();
            if (displayAttr != null && displayAttr.Name != null) return displayAttr.Name;
            JDisplayName descAttr = p.GetCustomAttribute<JDisplayName>();
            if (descAttr != null) return descAttr.DisplayName;
            else return p.Name;
        }
        public static string GetEntityJDescriptionOrFullName(Type t)
        {
            DisplayNameAttribute displayNameAttr = t.GetCustomAttribute<DisplayNameAttribute>();
            if (displayNameAttr != null && displayNameAttr.DisplayName != null) return displayNameAttr.DisplayName;
            JDisplayName descAttr = t.GetCustomAttribute<JDisplayName>();
            if (descAttr != null) return descAttr.DisplayName;
            else return t.FullName;
        }
        public static string GetEntityJDescriptionOrName(Type t)
        {
            DisplayNameAttribute displayNameAttr = t.GetCustomAttribute<DisplayNameAttribute>();
            if (displayNameAttr != null && displayNameAttr.DisplayName != null) return displayNameAttr.DisplayName;
            JDisplayName descAttr = t.GetCustomAttribute<JDisplayName>();
            if (descAttr != null) return descAttr.DisplayName;
            else return t.Name;
        }

        //reverse 
        public static string GetPropertyNameForDescription(Type t, string description)
        {
            foreach (var p in t.GetProperties())
            {
                JDisplayName descAttr = p.GetCustomAttribute<JDisplayName>();
                if (descAttr != null && descAttr.DisplayName != null && descAttr.DisplayName.Equals(description))
                {
                    return p.Name;
                }
            }
            return null;
        }

        #endregion

        #region Property
        public static object GetPKValue(object o)
        {
            PropertyInfo pPK = AttrHelper.GetProperty(typeof(JPrimaryKey), o.GetType());
            if (pPK != null)
            {
                return pPK.GetValue(o);
            }
            return null;
        }

        public static string ModelPropertyList(object obj, string lineDelimeter, string[] includes, string[] excludes)
        {
            Type t = obj.GetType();
            var props = t.GetProperties();
            var sb = new StringBuilder();
            foreach (var p in props)
            {
                if (excludes!= null && Array.Exists(excludes, element => element == p.Name))  continue;
                if (includes != null && !Array.Exists(includes, element => element == p.Name)) continue;
                JManyToOne manyToOneAttr = AttrHelper.GetAttribute<JManyToOne>(t, p.Name);
                JIgnore ignoreAttr = AttrHelper.GetAttribute<JIgnore>(t, p.Name);
                if (ignoreAttr != null) continue;
                string desc = GetPropertyJDescriptionOrName(p);
                object v = Dm.Instance.GetCustomPropertyValue(obj, p.Name, true, -1, -1);
                if (v != null)
                {
                    sb.Append(desc + ": " + v);
                    if (lineDelimeter != null) sb.Append(lineDelimeter);
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }




    }
    #endregion


}
