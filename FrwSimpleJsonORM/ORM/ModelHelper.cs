/**********************************************************************************
 *   FrwSimpleWinCRUD   https://github.com/frwsoftware/FrwSimpleWinCRUD
 *   The Open-Source Library for most quick  WinForm CRUD application creation
 *   MIT License Copyright (c) 2016 FrwSoftware
 *
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *   SOFTWARE.
 **********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace FrwSoftware
{
    public class ModelHelper
    {
        #region i18n
        public static string GetPropertyJDescriptionOrName(PropertyInfo p, bool getFullDescription = false)
        {
            if (p == null) return null;

            string name = null;
            DisplayNameAttribute displayNameAttr = p.GetCustomAttribute<DisplayNameAttribute>();
            if (displayNameAttr != null && displayNameAttr.DisplayName != null) name = displayNameAttr.DisplayName;
            if (name == null)
            {
                DisplayAttribute displayAttr = p.GetCustomAttribute<DisplayAttribute>();
                if (displayAttr != null && displayAttr.Name != null) name = displayAttr.Name;
            }
            if (name == null)
            {
                JDisplayName descAttr = p.GetCustomAttribute<JDisplayName>();
                if (descAttr != null) return descAttr.DisplayName;
                else name = p.Name;
            }
            if (!getFullDescription)
            {
                int dotIndex = name.IndexOfAny(new char[]{'.', '\n', '\t' }); // .IndexOf(".");
                if (dotIndex > -1) name = name.Substring(0, dotIndex);
            }
            return name;            
        }
        public static string GetEntityJDescriptionOrName(Type t)
        {
            DisplayNameAttribute displayNameAttr = t.GetCustomAttribute<DisplayNameAttribute>();
            if (displayNameAttr != null && displayNameAttr.DisplayName != null) return displayNameAttr.DisplayName;
            JDisplayName descAttr = t.GetCustomAttribute<JDisplayName>();
            if (descAttr != null) return descAttr.DisplayName;
            else return t.Name;
        }

        #endregion

        #region Property
        public static PropertyInfo GetPK(Type t)
        {
            PropertyInfo pPK = AttrHelper.GetProperty(typeof(JPrimaryKey), t);
            if (pPK != null)
            {
                return pPK;
            }
            return null;
        }
        public static object GetPKValue(object o)
        {
            PropertyInfo pPK = AttrHelper.GetProperty(typeof(JPrimaryKey), o.GetType());
            if (pPK != null)
            {
                return pPK.GetValue(o);
            }
            return null;
        }
        public static void SetPKValue(object o, object pkValue)
        {
            PropertyInfo pPK = AttrHelper.GetProperty(typeof(JPrimaryKey), o.GetType());
            if (pPK != null)
            {
                pPK.SetValue(o, pkValue);
            }
        }

        public static string ModelPropertyList(object obj, string lineDelimeter, string[] includes, string[] excludes)
        {
            Type t = obj.GetType();
            var props = t.GetProperties();
            var sb = new StringBuilder();
            foreach (var p in props)
            {
                if (excludes != null && Array.Exists(excludes, element => element == p.Name)) continue;
                if (includes != null && !Array.Exists(includes, element => element == p.Name)) continue;
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

        #endregion

        static public string GetNameForObject(object o)
        {
            if (o == null) return null;
            Type t = o.GetType();
            PropertyInfo pName = AttrHelper.GetProperty<JNameProperty>(t);
            if (pName != null)
            {
                object on = pName.GetValue(o);
                return (on != null ? on.ToString() : null);
            }
            else
            {
                return o.ToString();
            }
        }
        static public string GetShortNameForObject(object o)
        {
            if (o == null) return null;
            Type t = o.GetType();
            PropertyInfo pName = AttrHelper.GetProperty<JShortNameProperty>(t);
            if (pName != null)
            {
                object on = pName.GetValue(o);
                return (on != null ? on.ToString() : null);
            }
            else
            {
                return GetNameForObject(o);
            }
        }
        static public bool IsIsArchivedFieldPresent(Type t)
        {
            PropertyInfo IsArchivedProp = t.GetProperty("IsArchived");
            if (IsArchivedProp != null) return true;
            else return false;
        }
        static public bool IsExpiredFieldPresent(Type t)
        {
            return AttrHelper.IsPropertiesWithAttributePresent<JExpired>(t);
        }
        static public bool IsTextColoredFieldPresent(Type t)
        {
            var pl = AttrHelper.GetPropertiesWithAttribute<JDictProp>(t);
            foreach (var p in pl)
            {
                JDictProp d = AttrHelper.GetAttribute<JDictProp>(p);
                if (d.DictPropertyStyle == DisplyPropertyStyle.ColoredTextOnly || d.DictPropertyStyle == DisplyPropertyStyle.ColoredTextAndImage)
                    return true;
            }
            return false;
        }
        static public bool GetIsArchivedValue(object o)
        {
            if (o == null) return false;
            Type t = o.GetType();
            PropertyInfo IsArchivedProp = t.GetProperty("IsArchived");
            if (IsArchivedProp != null) return (bool)IsArchivedProp.GetValue(o);
            else return false;
        }
        static public string GetNameForObjectAdv(object o)
        {
            if (o == null) return null;
            string s = GetNameForObject(o);
            Type t = o.GetType();
            PropertyInfo IsArchivedProp = t.GetProperty("IsArchived");
            if (IsArchivedProp != null && IsArchivedProp.PropertyType == typeof(bool) && (bool)IsArchivedProp.GetValue(o) == true)
            {
                s = "A* " + s + " ("+ FrwUtilsRes.archived + ")";
            }
            return s;
        }
        static public string GetShortNameForObjectAdv(object o)
        {
            if (o == null) return null;
            string s = GetShortNameForObject(o);
            Type t = o.GetType();
            PropertyInfo IsArchivedProp = t.GetProperty("IsArchived");
            if (IsArchivedProp != null && IsArchivedProp.PropertyType == typeof(bool) && (bool)IsArchivedProp.GetValue(o) == true)
            {
                s = "A* " + s + " (" + FrwUtilsRes.archived + ")";
            }
            return s;
        }
        public static Color ExpiredToColor(string e, Color defautlColor)
        {
            Color color = defautlColor;
            if (e != null)
            {
                if (e.Equals("YELLOW")) color = Color.RosyBrown; //Color.Yellow  - uncontrast; 
                else if (e.Equals("RED")) color = Color.Red;
            }
            return color;
        }
        static public bool IsContainsDict(IList<string> dictField, object testedItem)
        {
            if (dictField == null) return false;
            if (dictField.Contains(testedItem.ToString())) return true;
            return false;
        }

        static public bool IsSingleHierEntity(Type t)
        {
            return (GetSelfPropertiesForEntity(t).Count == 1);
        }
        static public IList<PropertyInfo> GetSelfPropertiesForEntity(Type t)
        {
            List<PropertyInfo> list = new List<PropertyInfo>();
            foreach (var p in t.GetProperties())
            {
                JManyToOne manyToOneAttr = p.GetCustomAttribute<JManyToOne>();
                if (manyToOneAttr != null && p.PropertyType.Equals(t))
                {
                   list.Add(p);
                }
            }
            return list;
        }

        static public PropertyInfo FindRefFieldInForeinEntity(Type whatFindType, Type whereFindType, Type reationTypeToFind, string nameToFind)
        {
            IEnumerable<PropertyInfo> whereEntityRefFields = AttrHelper.GetProperties(reationTypeToFind, whereFindType);
            PropertyInfo whereEntityRefField = null;
            if (nameToFind == null)
            {
                //nameToFind Can be null, then it is assumed that the foreinEntityRefField is the only one and matches the type (previously: by name with pk)
                foreach (var p in whereEntityRefFields)
                {
                    Type refType = (typeof(JManyToOne).Equals(reationTypeToFind) || typeof(JOneToOne).Equals(reationTypeToFind)) ? p.PropertyType : AttrHelper.GetGenericListArgType(p.PropertyType);
                    if (refType.Equals(whatFindType))
                    {
                        if (whereEntityRefField != null) throw new Exception("Found more than one field referenced to this entity type");
                        whereEntityRefField = p;
                    }
                }
            }
            else
            {
                //nameToFind Set explicitly, which is relevant if the table has several fields of the same type that refer to the same table
                foreach (var p in whereEntityRefFields)
                {
                    Type refType = (typeof(JManyToOne).Equals(reationTypeToFind) || typeof(JOneToOne).Equals(reationTypeToFind)) ? p.PropertyType : AttrHelper.GetGenericListArgType(p.PropertyType);
                    if (refType.Equals(whatFindType))
                    {
                        if (
                            ((typeof(JManyToOne).Equals(reationTypeToFind) || typeof(JOneToMany).Equals(reationTypeToFind)) && (p.Name).Equals(nameToFind))
                            ||
                            (typeof(JManyToMany).Equals(reationTypeToFind) && (p.Name).Equals(nameToFind))
                            ||
                            (typeof(JOneToOne).Equals(reationTypeToFind) && (p.Name).Equals(nameToFind))
                            )//todo 
                        {
                            whereEntityRefField = p;
                            break;
                        }
                    }

                }
            }
            return whereEntityRefField;
        }

    }

}
