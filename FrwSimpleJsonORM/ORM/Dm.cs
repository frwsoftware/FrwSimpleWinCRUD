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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Windows.Forms;

namespace FrwSoftware
{
    public enum TruncatedValueSufix
    {
        None,
        Dots,
        DotsAndShown
    }

    public class DictNames
    {
        public const string YesNo = "YesNo";
    }

    public class SData
    {
        private Dictionary<object, object> pkCache = new Dictionary<object, object>();
        public Dictionary<object, object> PkCache { get { return pkCache ; } }

        public Type DataType { get; set; }
        public object DataList { get; set; }
        public bool Modified { get; set; }
        public int? MaxId { get; set; }
    }

    public class JoinEntityDataItem
    {
        public object Pk1 { get; set; }
        public object Pk2 { get; set; }
    }

    public class JoinEntityData
    {
        public Type DataType1 { get; set; }
        public Type DataType2 { get; set; }
        public string JoinTableName { get; set; }
        public List<JoinEntityDataItem> DataList { get; set; }
        public bool Modified { get; set; }
    }

    public class TypeComparer : IComparer<Type>
    {
        public int Compare(Type x, Type y)
        {
            return x.FullName.CompareTo(y.FullName);
        }
    }
    public enum CopyRestrictLevel
    {
        /// <summary>
        /// Full copy  
        ///  many-to-one, one-to-many and many-to-many properies copied as references
        /// </summary>
        AllPropertiesFullCopy = 1,
        /// <summary>
        /// many-to-one, one-to-many and many-to-many properies copied to new Lists.
        /// Used to clone before editing. 
        /// </summary>
        AllPropertiesNewLists = 2,
        /// <summary>
        /// many-to-one, one-to-many and many-to-many properies replaced by entity wtih pk only.
        /// Used to remote export. 
        /// </summary>
        AllPropertiesAllRelPK = 3,
        /// <summary>
        /// many-to-one properies replaced by entity wtih pk only.
        /// one-to-many and many-to-many properies replaced by null
        /// Used to store to disk;
        /// </summary>
        AllPropertiesManytoOnePK = 4,
        /// <summary>
        /// many-to-one, one-to-many and many-to-many properies replaced by null.
        /// </summary>
        OnlySimleProperties = 5
    }

    public enum CloneObjectType
    {
        ForSave,// AllPropertiesManytoOnePK
        ForTemp,//AllPropertiesNewLists
        ForExport//AllPropertiesAllRelPK
    }

    /// <summary>
    ///     System.ComponentModel.DataAnnotations.CompareAttribute
    ///     System.ComponentModel.DataAnnotations.CustomValidationAttribute
    ///     System.ComponentModel.DataAnnotations.DataTypeAttribute
    /// 
    ///     CreditCard, Currency, Custom, Date,  DateTime,  Duration,  EmailAddress, Html, ImageUrl, MultilineText, Password, PhoneNumber, PostalCode, Text, Time, Upload, Url
    /// 
    ///     System.ComponentModel.DataAnnotations.MaxLengthAttribute
    ///     System.ComponentModel.DataAnnotations.MinLengthAttribute
    ///     System.ComponentModel.DataAnnotations.RangeAttribute
    ///     System.ComponentModel.DataAnnotations.RegularExpressionAttribute
    ///     System.ComponentModel.DataAnnotations.RequiredAttribute
    ///     System.ComponentModel.DataAnnotations.StringLengthAttribute
    ///     System.Web.Security.MembershipPasswordAttribute
    /// </summary>


    public class JValidationError
    {
        public string PropertyName { get; set; }
        public string Message { get; set; }
    }
    public class JValidationErrorAdv : JValidationError
    {
        public PropertyInfo Property { get; set; }
    }
    public class JValidationResult
    {
        public List<JValidationError> JValidationErrors { get; }
        public List<ValidationResult> ValidationResults { get; set; }
        public bool isError
        {
            get
            {
                return (JValidationErrors.Count > 0 || ValidationResults.Count > 0);
            }
        }
        public JValidationResult()
        {
            JValidationErrors = new List<JValidationError>();
            ValidationResults  = new List<ValidationResult>();
        }
        public string GetFullErrorString(string br = "\r\n")
        {

            StringBuilder str = new StringBuilder();
            str.Append(br);

            foreach (var v in ValidationResults)
            {
                str.Append(v.ErrorMessage);
                str.Append(br);
            }

            return str.ToString();
        }
    }

    public class JValidationException : Exception
    {
        public JValidationException()
        {

        }
        public JValidationException(JValidationResult validationResult)
        {
            ValidationResult = validationResult;
            //Message = 
        }
        public JValidationResult ValidationResult { get; set; }
    }

    public partial class Dm
    {
        public static Dm Instance
        {
            get { return Dm.instance ?? (Dm.instance = new Dm()); }
            set
            {
                if (Dm.instance != null) throw new InvalidOperationException("Custom instance can be set only once and before first getting");
                Dm.instance = value;
            }
        }
        private static Dm instance;

        static public string TempDirPrefix = "tempDir";
        static public string TemplatesDirPrefix = "templatesDir";
        public const string STORAGE_PREFIX = "storage:";//Prefix for internal storage
        protected const string DATA_STORAGE = "dataStorage";//Data warehouse prefix in the profile
        public const int TRUNCATED_VALUE_MAX_ITEM_COUNT = 10;
        public const int TRUNCATED_VALUE_MAX_STRING_LENGTH = 300;


        //in memory caches 
        private List<SData> sdatas = null;//Cache for entities
        private List<JoinEntityData> joinDatas = null;//Cache for crosstables
        protected List<JDictionary> dictionaries = null;//cache for dicrionaries
        private List<Type> entities = new List<Type>();


        public string GetDataStorageDirPath()
        {
            return Path.Combine(FrwConfig.Instance.ProfileDir, DATA_STORAGE);
        }
 
        virtual public void Init()
        {
            //create caches in Init method - this is good check that no CRUD methods can be called before initialization 
            sdatas = new List<SData>();//Cache for entities
            joinDatas = new List<JoinEntityData>();//Cache for crosstables
            dictionaries = new List<JDictionary>();//cache for dicrionaries

            ComplateAndVerifyEntityRegistration(true);            
            InitDictionaries();
        }
        virtual protected void InitDictionaries()
        {
            //create some tipical dictionaries 
            JDictionary dict = null;
            dict = new JDictionary() { Id = DictNames.YesNo };
            dictionaries.Add(dict);
            dict.Items.Add(new JDictItem() { Key = "true", Text = FrwUtilsRes.Yes });
            dict.Items.Add(new JDictItem() { Key = "false", Text = FrwUtilsRes.No });
        }

        virtual public void Destroy()
        {
            SaveAllEntitiesData(false);
        }

        public object FindTypeAddFindInstance(string fullTypeName, string pkValue)
        {
            Type type = TypeHelper.FindType(fullTypeName);
            if (type == null) throw new InvalidOperationException("Type not found for: " + fullTypeName);
            return Dm.instance.Find(type, pkValue);
        }



        private void ComplateAndVerifyEntityRegistration(bool loadData)
        {
            //entities loading based on AppDomain.CurrentDomain.GetAssemblies()
            //Note: in some cases not all Assemblies loaded well. For example in separate UnitTest project for 
            //project MyProject main assemble MyProject.exe not present in assembles list.  
            var entityTypes = AttrHelper.GetTypesWithAttribute<JEntity>(true);
            foreach (var sourceEntityType in entityTypes)
            {
                entities.Add(sourceEntityType);
                bool pkFound = false;
                foreach (PropertyInfo p in sourceEntityType.GetProperties())
                {
                    JPrimaryKey pkAttr = AttrHelper.GetAttribute<JPrimaryKey>(p);
                    if (pkAttr != null) pkFound = true;
                    JManyToOne manyToOneAttr = AttrHelper.GetAttribute<JManyToOne>(p);
                    JOneToMany oneToManyAttr = AttrHelper.GetAttribute<JOneToMany>(p);
                    JManyToMany manyToManyAttr = AttrHelper.GetAttribute<JManyToMany>(p);
                    if (manyToOneAttr != null)
                    {
                        if (oneToManyAttr != null || manyToManyAttr != null)
                            throw new Exception("Property can not be marked more than one ralation attribute. Property " +
                               p.Name + " in enitty " + sourceEntityType);
                        Type foreinEntityType = p.PropertyType;
                        if (AttrHelper.IsAttributeDefinedForType<JEntity>(foreinEntityType, true) == false)
                        {
                            throw new Exception("Not registred (marked JEntity by attribute) referenced entity "
                               + foreinEntityType + " which referenced by  relation property " +
                               p.Name + " in enitty " + sourceEntityType);
                        }
                        PropertyInfo refFieldInForeinEntity = FindRefFieldInForeinEntity(sourceEntityType, foreinEntityType, typeof(JOneToMany), manyToOneAttr.RefFieldNameInForeinEntity);
                        if (refFieldInForeinEntity == null)//may be not present 
                        {
                            if (manyToOneAttr.RefFieldNameInForeinEntity != null)
                            {
                                JOneToMany foreinOneToManyAttr = AttrHelper.GetAttribute<JOneToMany>(refFieldInForeinEntity);
                                if (p.Name.Equals(foreinOneToManyAttr.RefFieldNameInForeinEntity) == false)
                                {
                                    throw new Exception("In named 'Many to one' relationship  (marked JEntity by attribute) referenced entity "
                                       + foreinEntityType + " which referenced by  relation property " +
                                       p.Name + " in enitty " + sourceEntityType + " forein 'One To many' relationship must be also named like relation property");

                                }
                            }
                        }
                        
                    }
                    else if (oneToManyAttr != null)
                    {
                        if (manyToManyAttr != null || manyToManyAttr != null)
                            throw new Exception("Property can not be marked more than one ralation attribute. Property " +
                               p.Name + " in enitty " + sourceEntityType);
                        Type foreinEntityType = AttrHelper.GetGenericListArgType(p.PropertyType);//difference from manytoone!
                        if (AttrHelper.IsAttributeDefinedForType<JEntity>(foreinEntityType, true) == false)
                        {
                            throw new Exception("Not registred (marked JEntity by attribute) referenced entity "
                               + foreinEntityType + " which referenced by relation property " +
                               p.Name + " in enitty " + sourceEntityType);
                        }
                        PropertyInfo refFieldInForeinEntity = FindRefFieldInForeinEntity(sourceEntityType, foreinEntityType, typeof(JManyToOne), oneToManyAttr.RefFieldNameInForeinEntity);
                        if (refFieldInForeinEntity == null)
                        {
                            throw new Exception("Many to one relation field not found in entity "
                                + foreinEntityType + " which referenced by one to many relation property " +
                                p.Name + " in enitty " + sourceEntityType);
                        }
                        else
                        {
                            if (oneToManyAttr.RefFieldNameInForeinEntity != null)
                            {
                                JManyToOne foreinManyToOneAttr = AttrHelper.GetAttribute<JManyToOne>(refFieldInForeinEntity);
                                if (p.Name.Equals(foreinManyToOneAttr.RefFieldNameInForeinEntity) == false)
                                {
                                    throw new Exception("In named 'One to many' relationship  (marked JEntity by attribute) referenced entity "
                                       + foreinEntityType + " which referenced by  relation property " +
                                       p.Name + " in enitty " + sourceEntityType + " forein 'Many To one'  relationship must be also named like relation property");

                                }
                            }

                        }

                    }
                    else if (manyToManyAttr != null)
                    {
                        if (oneToManyAttr != null || manyToOneAttr != null)
                            throw new Exception("Property can not be marked more than one ralation attribute. Property " +
                               p.Name + " in enitty " + sourceEntityType);

                        Type foreinEntityType = AttrHelper.GetGenericListArgType(p.PropertyType);
                        if (AttrHelper.IsAttributeDefinedForType<JEntity>(foreinEntityType, true) == false)
                        {
                            throw new Exception("Not registred (marked JEntity by attribute) referenced entity "
                               + foreinEntityType + " which referenced by many to one relation property " +
                               p.Name + " in enitty " + sourceEntityType);
                        }
                        PropertyInfo refFieldInForeinEntity = FindRefFieldInForeinEntity(sourceEntityType, foreinEntityType, typeof(JManyToMany), manyToManyAttr.JoinName);
                        if (refFieldInForeinEntity != null)                            //may be not present 
                        {
                            if (manyToManyAttr.JoinName != null)
                            {
                                JManyToMany foreinManyToOneAttr = AttrHelper.GetAttribute<JManyToMany>(refFieldInForeinEntity);
                                if (manyToManyAttr.JoinName.Equals(foreinManyToOneAttr.JoinName) == false)
                                {
                                    throw new Exception("In named 'Many to Many' relationship referenced entity "
                                       + foreinEntityType + " which referenced by  relation property " +
                                       p.Name + " in enitty " + sourceEntityType + " forein 'Many To Many'  relationship must be also named the same");

                                }
                            }

                        }

                    }
                }
                if (!pkFound) throw new Exception("Primary key property not found in enitty " + sourceEntityType);
            }
            if (loadData)
            {
                LoadAllEntitiesData(entities);
            }
            else{
                //todo: if not load we must register all Entities and jointables
                throw new NotImplementedException("if not load we must register all Entities and jointables");
            }
        }


        #region Relation

        virtual public object GetCustomPropertyValue(object rowObject, string aspectName, bool asPlainText = true, int maxCount = TRUNCATED_VALUE_MAX_ITEM_COUNT, int maxLength = TRUNCATED_VALUE_MAX_STRING_LENGTH, TruncatedValueSufix truncatedValueSufix = TruncatedValueSufix.DotsAndShown, string delimeter = ", ")
        {
            if (rowObject == null) return null;
            Type sourceObjectType = rowObject.GetType();
            Type pType = AttrHelper.GetPropertyType(sourceObjectType, aspectName);
            PropertyInfo propInfo = sourceObjectType.GetProperty(aspectName);
            if (pType == null) return null;//if aspectName not present for sourceObjectType
            if (AttrHelper.GetAttribute<JText>(propInfo) != null)
            {
                object o = AttrHelper.GetPropertyValue(rowObject, propInfo);
                if (o != null)
                {
                    string s = o.ToString();
                    if (asPlainText && HtmlUtils.CheckIsHtmlFull(s))
                    {
                        s = HtmlUtils.ConvertHtmlToPlainTextRegexp(s);
                    }
                    if (s != null)
                    {
                        if (maxLength > 0 && s.Length > maxLength)
                        {
                            int totalLength = s.Length;
                            s = s.Substring(0, maxLength);
                            if (truncatedValueSufix == TruncatedValueSufix.Dots || truncatedValueSufix == TruncatedValueSufix.DotsAndShown)
                            {
                                s = s + "...";
                            }
                            if (truncatedValueSufix == TruncatedValueSufix.DotsAndShown)
                            {
                                s = s + " [" + maxLength + "b/" + totalLength + "b]";
                            }
                        }
                        return s;
                    }
                    else return null;
                }
                else return null;
            }
            else if (AttrHelper.GetAttribute<JOneToMany>(propInfo) != null ||
                AttrHelper.GetAttribute<JManyToMany>(propInfo) != null)
            {
                IList value = (IList)AttrHelper.GetPropertyValue(rowObject, propInfo);
                if (value == null)
                {
                    Dm.Instance.ResolveRelation(rowObject, sourceObjectType, propInfo);
                    value = (IList)AttrHelper.GetPropertyValue(rowObject, propInfo);
                }
                return Dm.MakeStringFromObjectList(value, maxCount, maxLength, truncatedValueSufix, delimeter);
            }
            else if (AttrHelper.GetAttribute<JManyToOne>(propInfo) != null)
            {
                Dm.Instance.ResolveRelation(rowObject, sourceObjectType, propInfo);//do nothing
                object value = AttrHelper.GetPropertyValue(rowObject, propInfo);
                return (value != null) ? Dm.MakeStringFromObjectList(new List<object>() { value }, maxCount, maxLength, truncatedValueSufix, delimeter) : null;
            }
            else if (AttrHelper.GetAttribute<JDictProp>(propInfo) != null)
            {
                JDictProp dictAttr = AttrHelper.GetAttribute<JDictProp>(propInfo);
                if (dictAttr.DictPropertyStyle == DisplyPropertyStyle.ImageOnly) return null;
                else
                {
                    return Dm.MakeStringFromObjectList(ResolveDictionary(rowObject, aspectName), maxCount, maxLength, truncatedValueSufix, delimeter);
                }
            }
            else if (AttrHelper.GetAttribute<JImageName>(propInfo) != null
                && AttrHelper.GetAttribute<JImageName>(propInfo).DictPropertyStyle == DisplyPropertyStyle.ImageOnly)
            {
                return null;
            }
            else if (AttrHelper.GetAttribute<JImageRef>(propInfo) != null
                && AttrHelper.GetAttribute<JImageRef>(propInfo).DictPropertyStyle == DisplyPropertyStyle.ImageOnly)
            {
                return null;
            }
            else if (pType == typeof(DateTime) || pType == typeof(DateTime?))
            {
                DateTime? o = AttrHelper.GetPropertyValue(rowObject, propInfo) as DateTime?;
                if (o != null && o != DateTime.MinValue)
                    return ((DateTime)o).ToString(DataUtils.DATE_TIME_PATTERN_FOR_DISPLAY);//to valid sort  TODO config
                else return null;
            }
            else if (pType == typeof(DateTimeOffset) || pType == typeof(DateTimeOffset?))
            {
                DateTimeOffset? o = AttrHelper.GetPropertyValue(rowObject, propInfo) as DateTimeOffset?;
                if (o != null && o != DateTimeOffset.MinValue)
                    return ((DateTimeOffset)o).ToString(DataUtils.DATE_TIME_PATTERN_FOR_DISPLAY);
                else return null;
            }
            else if (pType == typeof(JAttachment))
            {
                JAttachment o = (JAttachment)AttrHelper.GetPropertyValue(rowObject, propInfo);
                if (o != null) return o.Path;
                else return null;
            }
            else if (AttrHelper.IsGenericListTypeOf(pType, typeof(JAttachment)))
            {
                List<JAttachment> l = (List<JAttachment>)AttrHelper.GetPropertyValue(rowObject, propInfo);
                return Dm.MakeStringFromObjectList(l, maxCount, maxLength, truncatedValueSufix, delimeter);
            }
            else if (AttrHelper.IsSameOrSubclass(typeof(IList), pType))//must be last check 
            {
                IList s = AttrHelper.GetPropertyValue(rowObject, propInfo) as IList;
                return Dm.MakeStringFromObjectList(s, maxCount, maxLength, truncatedValueSufix, delimeter);
            }
            else
            {
                object o = AttrHelper.GetPropertyValue(rowObject, propInfo);
                if (o != null) return o; //.ToString();
                else return null;
            }
        }


        private static string MakeStringFromObjectList(IList list, int maxCount = Dm.TRUNCATED_VALUE_MAX_ITEM_COUNT, int maxLength = TRUNCATED_VALUE_MAX_STRING_LENGTH, TruncatedValueSufix truncatedValueSufix = TruncatedValueSufix.DotsAndShown, string delimeter = ", ")
        {
            if (list == null) return null;
            int totalCount = list.Count;
            List<string> list2 = new List<string>();
            foreach (var l in list)
            {
                Type t = l.GetType();
                PropertyInfo pName = AttrHelper.GetProperty<JNameProperty>(t);
                if (pName != null)
                {
                    object o = pName.GetValue(l);
                    list2.Add(o != null ? o.ToString() : null);
                }
                else
                {
                    list2.Add(l.ToString());
                }
            }
            return MakeStringFromStringList(list2,  totalCount, maxCount,  maxLength, truncatedValueSufix, delimeter);
        }
        private static string MakeStringFromStringList(List<string> list, int totalCount, int maxCount,  int maxLength, TruncatedValueSufix truncatedValueSufix, string delimeter = ", ")
        {
            if (list == null) return null;
            StringBuilder s = new StringBuilder();
            int count = 1;
            foreach (var l in list)
            {
                if ((maxCount > 0 && count >= maxCount) || (maxLength > 0 && s.Length > maxLength))
                {
                    if (truncatedValueSufix == TruncatedValueSufix.Dots || truncatedValueSufix == TruncatedValueSufix.DotsAndShown) { 
                        s.Append("...");
                    }
                    if (truncatedValueSufix == TruncatedValueSufix.DotsAndShown)
                    {
                        s.Append(" [");
                        s.Append(count);
                        s.Append("/");
                        s.Append(totalCount);
                        s.Append("]");
                    }
                    break;
                }

                if (s.Length > 0) s.Append(delimeter);
                s.Append(l);
               
                count++;
            }
            return s.Length > 0 ? s.ToString() : null;
        }
     
        private void UpdateRelations(object o)
        {
            if (o == null) return;
            Type t = o.GetType();
            foreach (var p in t.GetProperties())
            {
                UpdateRelation(o, p, p.GetValue(o));
            }
        }

        private IList FindDublicatesInList(IList list)
        {
            var myList = new List<object>();
            var duplicates = new List<object>();
            foreach (var s in list)
            {
                if (!myList.Contains(s))
                    myList.Add(s);
                else
                    duplicates.Add(s);
            }
            if (duplicates.Count == 0) return null;
            else return duplicates;
        }

        private void UpdateRelation(object rowObject, PropertyInfo p, object newObject)
        {
            if (rowObject == null) return;
            Type sourceEntityType = rowObject.GetType();
            PropertyInfo pkProp = AttrHelper.GetProperty<JPrimaryKey>(sourceEntityType);
            JManyToOne manyToOneAttr = AttrHelper.GetAttribute<JManyToOne>(p);
            JOneToMany oneToManyAttr = AttrHelper.GetAttribute<JOneToMany>(p);
            JManyToMany manyToManyAttr = AttrHelper.GetAttribute<JManyToMany>(p);
            if (manyToOneAttr != null)
            {
                Type foreinEntityType = p.PropertyType;
                IList allMayBeReferenced = FindAll(foreinEntityType);
                if (newObject != null) {
                    if (!allMayBeReferenced.Contains(newObject)) throw new Exception("Object from field " + p.Name + " not present in referenced entity " + foreinEntityType);
                }

                AttrHelper.SetPropertyValue(rowObject, p, newObject);
                // reverse 
                PropertyInfo refFieldInForeinEntity = FindRefFieldInForeinEntity(sourceEntityType, foreinEntityType, typeof(JOneToMany), manyToOneAttr.RefFieldNameInForeinEntity);
                if (refFieldInForeinEntity != null)//if reverse relation field present
                {
                    foreach (var l in allMayBeReferenced)
                    {
                        IList foreinEntityValue = (IList)refFieldInForeinEntity.GetValue(l);
                        if (foreinEntityValue != null)//update only filled
                        {
                            if (newObject == null || newObject.Equals(l) == false)//unset on not this relation
                            {
                                if (foreinEntityValue.Contains(rowObject))
                                {
                                    foreinEntityValue.Remove(rowObject);
                                }
                            }
                            else //this relation
                            {
                                if (foreinEntityValue.Contains(rowObject) == false)
                                {
                                    foreinEntityValue.Add(rowObject);
                                }
                            }
                        }
                    }
                }
            }
            else if (oneToManyAttr != null)
            {
                IList newRelObjects = (IList)newObject;
                if (FindDublicatesInList(newRelObjects) != null) throw new Exception("Found dublicates in values list of property " + p.Name + "  entity " + sourceEntityType); 
                Type foreinEntityType = AttrHelper.GetGenericListArgType(p.PropertyType);//difference from manytoone!
                IList allMayBeReferenced = FindAll(foreinEntityType);
                foreach (var o in newRelObjects)
                {
                    if (!allMayBeReferenced.Contains(o)) throw new Exception("Object from field " + p.Name + " not present in referenced entity " + foreinEntityType);
                }
                //set
                AttrHelper.SetPropertyValue(rowObject, p, newRelObjects);
                if (newRelObjects != null)
                {
                    //reverse 
                    PropertyInfo refFieldInForeinEntity = FindRefFieldInForeinEntity(sourceEntityType, foreinEntityType, typeof(JManyToOne), oneToManyAttr.RefFieldNameInForeinEntity);
                    if (refFieldInForeinEntity != null)//if reverse relation field present
                    {
                        foreach (object l in allMayBeReferenced)
                        {
                            object foreinEntityValue = refFieldInForeinEntity.GetValue(l);
                            if (newRelObjects != null)
                            {
                                bool found = false;
                                foreach (var o in newRelObjects)
                                {
                                    if (o.Equals(l))//present in direct
                                    {
                                        //check for present in reverse
                                        if (foreinEntityValue == null)
                                        {
                                            refFieldInForeinEntity.SetValue(l, rowObject);
                                        }
                                        else if (foreinEntityValue.Equals(rowObject) == false)
                                        {
                                            // old 
                                            refFieldInForeinEntity.SetValue(l, rowObject);

                                            if (foreinEntityValue != null)
                                            {
                                                IList oldEntityList = (IList)p.GetValue(foreinEntityValue);
                                                if (oldEntityList != null)
                                                {
                                                    oldEntityList.Remove(l);
                                                }
                                            }
                                        }
                                        found = true;
                                        break; //todo
                                    }
                                }
                                if (found == false)
                                {
                                    if (foreinEntityValue != null && foreinEntityValue.Equals(rowObject) == true)
                                        refFieldInForeinEntity.SetValue(l, null);
                                }
                            }
                        }
                    }
                }
            }
            else if (manyToManyAttr != null)
            {
                IList newRelObjects = (IList)newObject;
                if (FindDublicatesInList(newRelObjects) != null) throw new Exception("Found dublicates in values list of property " + p.Name + "  entity " + sourceEntityType);
                Type foreinEntityType = AttrHelper.GetGenericListArgType(p.PropertyType);//difference from manytoone!
                IList allMayBeReferenced = FindAll(foreinEntityType);
                foreach (var o in newRelObjects)
                {
                    if (!allMayBeReferenced.Contains(o)) throw new Exception("Object from field " + p.Name + " not present in referenced entity " + foreinEntityType);
                }

                if (newRelObjects != null)
                {
                    object sourcePKValue = AttrHelper.GetPropertyValue(rowObject, pkProp.Name);
                    TypeComparer typeComparer = new TypeComparer();
                    Type[] ts = new Type[] { sourceEntityType, foreinEntityType };
                    Array.Sort(ts, typeComparer);
                    JoinEntityData cross = FindAllJoinData(ts[0], ts[1], manyToManyAttr.JoinName);
                    bool reverse = false;
                    if (ts[0].Equals(sourceEntityType) == false) reverse = true;

                    //get old to compare
                    List<object> cValues = new List<object>();
                    //from crosstable
                    foreach (var l in cross.DataList)
                    {
                        if (reverse)
                        {
                            if (sourcePKValue.Equals(l.Pk2)) cValues.Add(l.Pk1);
                        }
                        else
                        {
                            if (sourcePKValue.Equals(l.Pk1)) cValues.Add(l.Pk2);
                        }
                    }

                    IList oldList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(foreinEntityType));
                    foreach (var foreinKeyValue in cValues)
                    {
                        object sourceEntityValue = Find(foreinEntityType, foreinKeyValue);
                        oldList.Add(sourceEntityValue);
                    }
                    //set
                    AttrHelper.SetPropertyValue(rowObject, p, newRelObjects);
                    //modify Jointable
                    IList addList = null;
                    IList removeList = null;
                    CompareLists(oldList, newRelObjects, out addList, out removeList);
                    PropertyInfo pPK1 = AttrHelper.GetProperty<JPrimaryKey>(sourceEntityType);
                    PropertyInfo pPK2 = AttrHelper.GetProperty<JPrimaryKey>(foreinEntityType);
                    foreach (var ro in removeList)
                    {
                        var fk2ValueR = pPK2.GetValue(ro);
                                                          
                        foreach (var l in cross.DataList)
                        {
                            object fk1Value = null;
                            object fk2Value = null;
                            if (reverse)
                            {
                                fk1Value = l.Pk2;
                                fk2Value = l.Pk1;
                            }
                            else
                            {
                                fk1Value = l.Pk1;
                                fk2Value = l.Pk2;
                            }
                            if (sourcePKValue.Equals(fk1Value) && fk2ValueR.Equals(fk2Value))
                            {
                                DeleteJoinTableRow(ts[0], ts[1], manyToManyAttr.JoinName, l.Pk1, l.Pk2);
                                break;
                            }
                        }
                    }
                    foreach (var ro in addList)
                    {
                        var fk2ValueR = pPK2.GetValue(ro);//get from primiry key addList
                        if (reverse) SaveJoinTableRow(ts[0], ts[1], manyToManyAttr.JoinName, fk2ValueR, sourcePKValue);
                        else SaveJoinTableRow(ts[0], ts[1], manyToManyAttr.JoinName, sourcePKValue, fk2ValueR);
                    }
                    if (addList.Count > 0 || removeList.Count > 0)
                    {
                        SetJoinEntityModified(ts[0], ts[1], manyToManyAttr.JoinName);
                        SetEntityModified(foreinEntityType);
                    }

                    //reverse 
                    PropertyInfo refFieldInForeinEntity = FindRefFieldInForeinEntity(sourceEntityType, foreinEntityType, typeof(JManyToMany), manyToManyAttr.JoinName);
                    if (refFieldInForeinEntity != null)//if reverse relation field present
                    {
                        foreach (object l in allMayBeReferenced)
                        {
                            IList foreinEntityValue = (IList)refFieldInForeinEntity.GetValue(l);
                            if (foreinEntityValue != null && newRelObjects != null)//update only filled
                            {
                                //direct test 
                                foreach (var o in newRelObjects)
                                {
                                    if (o.Equals(l))//present in direct
                                    {
                                        //check for present in reverse
                                        if (foreinEntityValue.Contains(rowObject) == false)
                                        {
                                            foreinEntityValue.Add(rowObject);
                                        }
                                    }
                                }
                                //reverse test 
                                foreach (var o2 in foreinEntityValue)
                                {
                                    if (o2.Equals(rowObject))
                                    {
                                        if (newRelObjects.Contains(l) == false)
                                        {
                                            foreinEntityValue.Remove(rowObject);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }

        virtual public void SaveJoinTableRow(Type t1, Type t2, string joinTableName, object pk1Value, object pk2Value)
        {
            JoinEntityData cross = FindAllJoinData(t1, t2, joinTableName);

            JoinEntityDataItem joinObject = new JoinEntityDataItem();
            joinObject.Pk1 = pk1Value;
            joinObject.Pk2 = pk2Value;
            bool found = false;
            foreach(var c in cross.DataList)
            {
                if (c.Pk1.Equals(joinObject.Pk1) && c.Pk2.Equals(joinObject.Pk2))
                {
                    found = true;
                    break;
                }
            }
            if (!found) cross.DataList.Add(joinObject);
        }
        virtual protected void DeleteJoinTableRow(Type t1, Type t2, string joinTableName, object pk1Value, object pk2Value)
        {
            JoinEntityData join = FindAllJoinData(t1, t2, joinTableName);
            foreach (var c in join.DataList)
            {
                if (c.Pk1.Equals(pk1Value) && c.Pk2.Equals(pk2Value))
                {
                    join.DataList.Remove(c);
                    break;
                }
            }
        }

        public void ResolveRelation(object rowObject, string aspectName)
        {
            if (rowObject == null) return;// null;
            Type sourceEntityType = rowObject.GetType();
            PropertyInfo p = sourceEntityType.GetProperty(aspectName);
            ResolveRelation(rowObject, sourceEntityType, p);
        }


        private void ResolveRelation(object rowObject, Type sourceEntityType, PropertyInfo p)
        {
            
            PropertyInfo pkProp = AttrHelper.GetProperty<JPrimaryKey>(sourceEntityType);
            JManyToOne manyToOneAttr = AttrHelper.GetAttribute<JManyToOne>(p);
            JOneToMany oneToManyAttr = AttrHelper.GetAttribute<JOneToMany>(p);
            JManyToMany manyToManyAttr = AttrHelper.GetAttribute<JManyToMany>(p);

            if (manyToOneAttr != null)
            {
                //JManyToOne - autoresolved
            }
            else if (oneToManyAttr != null)
            {
                Type foreinEntityType = AttrHelper.GetGenericListArgType(p.PropertyType);
                IList values = ResolveOneToManyRelation(rowObject, foreinEntityType, oneToManyAttr.RefFieldNameInForeinEntity);
                p.SetValue(rowObject, values);
            }
            else if (manyToManyAttr != null)
            {
                Type foreinEntityType = AttrHelper.GetGenericListArgType(p.PropertyType);
                IList values = ResolveManyToManyRelation(rowObject, foreinEntityType, manyToManyAttr.JoinName);
                p.SetValue(rowObject, values);
            }
            else return;
        }
        public IList<T> ResolveManyToManyRelation<T>(object rowObject, string joinName = null)
        {
            Type t = typeof(T);
            return (IList<T>)ResolveManyToManyRelation(rowObject, t, joinName);
        }

        public IList ResolveManyToManyRelation(object rowObject, Type foreinEntityType, string joinName = null)
        {
            Type sourceEntityType = rowObject.GetType();
            PropertyInfo pkProp = AttrHelper.GetProperty<JPrimaryKey>(sourceEntityType);
            IList values = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(foreinEntityType));
            if (rowObject == null) return values;

            TypeComparer typeComparer = new TypeComparer();
            Type[] ts = new Type[] { sourceEntityType, foreinEntityType };
            Array.Sort(ts, typeComparer);
            JoinEntityData cross = FindAllJoinData(ts[0], ts[1], joinName);
            bool reverse = false;
            if (ts[0].Equals(sourceEntityType) == false) reverse = true;
            List<object> cValues = new List<object>();
            object sourcePKValue = pkProp.GetValue(rowObject);
            //from crosstable
            foreach (var l in cross.DataList)
            {
                if (reverse)
                {
                    if (sourcePKValue.Equals(l.Pk2))
                    {
                        cValues.Add(l.Pk1);
                    }
                }
                else
                {
                    if (sourcePKValue.Equals(l.Pk1))
                    {
                        cValues.Add(l.Pk2);
                    }
                }
            }

            foreach (var foreinKeyValue in cValues)
            {
                object sourceEntityValue = Find(foreinEntityType, foreinKeyValue);
                values.Add(sourceEntityValue);
            }
            return values;
        }
        public IList<T> ResolveOneToManyRelation<T>(object rowObject, string refFieldNameInForeinEntity = null)
        {
            Type t = typeof(T);
            return (IList<T>)ResolveOneToManyRelation(rowObject, t, refFieldNameInForeinEntity);
        }
        public IList ResolveOneToManyRelation(object rowObject, Type foreinEntityType, string refFieldNameInForeinEntity = null)
        {
            IList values = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(foreinEntityType));
            if (rowObject == null) return values;
            Type sourceEntityType = rowObject.GetType();
            PropertyInfo foreinEntityPK = AttrHelper.GetProperty<JPrimaryKey>(foreinEntityType);
            PropertyInfo pName = AttrHelper.GetProperty<JNameProperty>(foreinEntityType);
            PropertyInfo foreinEntityManyToOne = FindRefFieldInForeinEntity(sourceEntityType, foreinEntityType, typeof(JManyToOne), refFieldNameInForeinEntity);
            IList list = FindAll(foreinEntityType);
            //Selection of all values corresponding to our value 
            foreach (var l in list)
            {
                var foreinEntityValue = foreinEntityManyToOne.GetValue(l);
                if (rowObject.Equals(foreinEntityValue))
                {
                    values.Add(l);
                }
            }
            return values;
        }


        private void CompareLists(IList oldList, IList newList, out IList addList, out IList removeList)
        {
            addList = new List<object>();
            removeList = new List<object>();
            foreach (var oo in oldList)
            {
                if (newList.Contains(oo) == false) removeList.Add(oo);
            }
            foreach (var oo in newList)
            {
                if (oldList.Contains(oo) == false) addList.Add(oo);
            }
        }


        static private PropertyInfo FindRefFieldInForeinEntity(Type sourceEntityType, Type foreinEntityType, Type attrTypeToFind, string nameToFind)
        {
            IEnumerable<PropertyInfo> foreinEntityRefFields = AttrHelper.GetProperties(attrTypeToFind, foreinEntityType);
            PropertyInfo foreinEntityRefField = null;
            if (nameToFind == null)
            {
                //nameToFind Can be null, then it is assumed that the foreinEntityRefField is the only one and matches the type (previously: by name with pk)
                foreach (var p in foreinEntityRefFields)
                {
                    Type refType = (typeof(JManyToOne).Equals(attrTypeToFind)) ? p.PropertyType : AttrHelper.GetGenericListArgType(p.PropertyType);
                    if (refType.Equals(sourceEntityType))
                    {
                        if (foreinEntityRefField != null) throw new Exception("Found more than one field referenced to this entity type");
                        foreinEntityRefField = p;
                    }
                }
            }
            else
            {
                //nameToFind Set explicitly, which is relevant if the table has several fields of the same type that refer to the same table
                foreach (var p in foreinEntityRefFields)
                {
                    Type refType = (typeof(JManyToOne).Equals(attrTypeToFind)) ? p.PropertyType : AttrHelper.GetGenericListArgType(p.PropertyType);
                    if (refType.Equals(sourceEntityType))
                    {
                        if (
                            ((typeof(JManyToOne).Equals(attrTypeToFind) || typeof(JOneToMany).Equals(attrTypeToFind)) && (p.Name).Equals(nameToFind))
                            || 
                            (typeof(JManyToMany).Equals(attrTypeToFind) && (p.Name).Equals(nameToFind)))//todo 
                        {
                            foreinEntityRefField = p;
                            break;
                        }
                    }

                }
            }
            return foreinEntityRefField;
        }

        #endregion

        #region SData

   

        /// <summary>
        /// Search by primary key 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="primaryKeValue"></param>
        /// <returns></returns>
        public T Find<T>(object primaryKeValue)
        {
            return (T)Find(typeof(T), primaryKeValue);
        }
        virtual public object Find(Type entityType, object primaryKeValue)
        {
            if (primaryKeValue == null) throw new ArgumentException();// return null;
            //Type pkType = primaryKeValue.GetType();
            PropertyInfo pPK = AttrHelper.GetProperty<JPrimaryKey>(entityType);
            SData sdata = GetSData(entityType);
            object o = null;
            sdata.PkCache.TryGetValue(primaryKeValue, out o);
            return o;
            /*
            IList list = FindAll(entityType);
            foreach (var l in list)
            {
                var pkValue = pPK.GetValue(l);
                if (primaryKeValue.Equals(pkValue))
                {
                    o = l;
                    break;
                }
            }
            return o;
            */
        }

        /// <summary>
        /// Creates an empty object and implements an autoincrement pk
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public T EmptyObject<T>(IDictionary<string, object> pars = null)
        {
            return (T)EmptyObject(typeof(T), pars);
        }
        virtual public object EmptyObject(Type t, IDictionary<string, object> pars = null)
        {
            object o = Activator.CreateInstance(t);
            PropertyInfo pPK = AttrHelper.GetProperty<JPrimaryKey>(t);
            if (pPK != null)
            {
                if (AttrHelper.IsAttributePresent<JAutoIncrement>(t, pPK.Name))
                {
                    //string maxValue = DataUtils.genKey(null);

                    IList list = FindAll(t);
                    SData sdata = GetSData(t);
                    int maxValue = 0;
                    if (sdata.MaxId == null)
                    {
                        foreach (var l in list)
                        {
                            var pkValue = pPK.GetValue(l);
                            int curValue = Int32.Parse(pkValue.ToString());
                            if (curValue > maxValue) maxValue = curValue;
                        }
                        sdata.MaxId = maxValue;
                    }
                    sdata.MaxId = sdata.MaxId + 1;
                    if (pPK.PropertyType == typeof(string))
                        pPK.SetValue(o, sdata.MaxId.ToString());
                    else
                        pPK.SetValue(o, sdata.MaxId);
                }

            }
            DateTime cur = DateTime.Now;
            IEnumerable<PropertyInfo> props = AttrHelper.GetPropertiesWithAttribute<JInitCurrentDate>(o.GetType());
            foreach (var p in props)
            {
                if (p.PropertyType == typeof(DateTime))
                    p.SetValue(o, cur);
                else if (p.PropertyType == typeof(DateTimeOffset))
                    p.SetValue(o, new DateTimeOffset(cur));
            }
            if (pars != null)
            {
                props = o.GetType().GetProperties();
                foreach (var p in props)
                {
                    object value = DictHelper.Get(pars, p.Name);
                    if (value != null)
                    {
                        //todo check type
                        p.SetValue(o, value);
                    }
                }
            }
            ResolveToManyRelations(o);
            return o;
        }
        virtual public void DeleteAllObjects(Type t)
        {
            IList list = FindAll(t);
            IList cloneList = new List<object>();
            foreach (var l in list)
            {
                cloneList.Add(l);
            }
            foreach (var l in cloneList)
            {
                //Must be called to be called  CascadeDeleteRelation
                //However, in the case of working with a physical base, this can be implemented by a database
                DeleteObject(l);
            }
        }
        virtual public void DeleteObject(object o)
        {
            if (o == null) return;
            Type t = o.GetType();
            IList list = FindAll(t);
            list.Remove(o);

            PropertyInfo[] ps = t.GetProperties();
            foreach (var p in ps)
            {
                JManyToOne manyToOneAttr = AttrHelper.GetAttribute<JManyToOne>(p);
                //many to one is necessary, so we can start from this entity 
                if (manyToOneAttr != null)
                {
                    Type foreinEntityType = p.PropertyType;
                    PropertyInfo refFieldInForeinEntity = FindRefFieldInForeinEntity(t, foreinEntityType, typeof(JOneToMany), manyToOneAttr.RefFieldNameInForeinEntity);
                    if (refFieldInForeinEntity != null)//if reverse relation field present
                    {
                        object refValue = AttrHelper.GetPropertyValue(o, p);
                        if (refValue != null)
                        {
                            IList foreinEntityValue = (IList)refFieldInForeinEntity.GetValue(refValue);
                            if (foreinEntityValue != null)
                            {
                                if (foreinEntityValue.Contains(o))
                                {
                                    foreinEntityValue.Remove(o);
                                }
                            }
                        }
                    }
                }
                //other realationship property is optional, so we must find all reference form other entities
            }

            //find all many to many 
            foreach (JoinEntityData s in joinDatas)
            {
                if (s.DataType1.Equals(t) || s.DataType2.Equals(t))
                {
                    bool reverse = false;
                    Type foreinEntityType = null;
                    if (s.DataType2.Equals(t))
                    {
                        reverse = true;
                        foreinEntityType = s.DataType1;
                    }
                    else foreinEntityType = s.DataType2;

                    PropertyInfo pkProp = AttrHelper.GetProperty<JPrimaryKey>(t);
                    object sourcePKValue = pkProp.GetValue(o);
                    List<JoinEntityDataItem> cValues = new List<JoinEntityDataItem>();
                    foreach (var l in s.DataList)
                    {
                        if (reverse)
                        {
                            if (sourcePKValue.Equals(l.Pk2)) cValues.Add(l);
                        }
                        else
                        {
                            if (sourcePKValue.Equals(l.Pk1)) cValues.Add(l);
                        }
                    }
                    bool changed = false;
                    foreach (JoinEntityDataItem v in cValues)
                    {
                        DeleteJoinTableRow(s.DataType1, s.DataType2, s.JoinTableName, v.Pk1, v.Pk2);//use this virtual function  instead of cross.DataList.Remove(v);
                        changed = true;
                    }
                    if (changed)
                        SetJoinEntityModified(s.DataType1, s.DataType2, s.JoinTableName);
                    //remove from forein entity
                    PropertyInfo refFieldInForeinEntity = FindRefFieldInForeinEntity(t, foreinEntityType, typeof(JManyToMany), s.JoinTableName);
                    if (refFieldInForeinEntity != null)//if reverse relation field present
                    {
                        changed = false;
                        IList allMayBeReferenced = FindAll(foreinEntityType);
                        foreach (object l in allMayBeReferenced)
                        {
                            IList foreinEntityValue = (IList)refFieldInForeinEntity.GetValue(l);
                            if (foreinEntityValue.Contains(o))
                            {
                                foreinEntityValue.Remove(o);
                                changed = true;
                            }
                        }
                    }
                }
            }

            //var entityTypes = AttrHelper.GetTypesWithAttribute<JEntity>(true);
            foreach (var potentialRefEntityType in entities)
            {
                bool modified = false;
                foreach (PropertyInfo p in potentialRefEntityType.GetProperties())
                {
                    JManyToOne manyToOneAttr = AttrHelper.GetAttribute<JManyToOne>(p);
                    if (manyToOneAttr != null)
                    {
                        Type foreinEntityType = p.PropertyType;
                        if (foreinEntityType == t)
                        {
                            IList allMayBeReferenced = FindAll(potentialRefEntityType);
                            foreach (object l in allMayBeReferenced)
                            {
                                object foreinEntityValue = p.GetValue(l);
                                if (foreinEntityValue != null && foreinEntityValue == o)
                                {
                                    //set null
                                    p.SetValue(l, null);
                                    modified = true;
                                    //cascade delete
                                    //if (l != o) DeleteObject(l);
                                }
                            }
                        }

                    }
                }
                if (modified) SetEntityModified(potentialRefEntityType);
            }

            SetEntityModified(t);
        }

        public string GetDependencyReport(object o, string br = "\r\n")
        {
            if (o == null) return null;
            Type sourceEntityType = o.GetType();
            PropertyInfo pkProp = AttrHelper.GetProperty<JPrimaryKey>(sourceEntityType);
            object sourcePKValue = pkProp.GetValue(o);
            Dictionary<Type, HashSet<object>> rels = new Dictionary<Type, HashSet<object>>();
            foreach (var entity in entities)
            {
                foreach (PropertyInfo p in entity.GetProperties())
                {
                    JManyToOne manyToOneAttr = AttrHelper.GetAttribute<JManyToOne>(p);
                    if (manyToOneAttr != null)
                    {
                        Type foreinEntityType = p.PropertyType;
                        if (foreinEntityType == sourceEntityType)
                        {
                            HashSet<object> referenced = null;
                            rels.TryGetValue(entity, out referenced);
                            if (referenced == null)
                            {
                                referenced = new HashSet<object>();
                                rels.Add(entity, referenced);
                            }

                            IList allMayBeReferenced = FindAll(entity);
                            foreach (object l in allMayBeReferenced)
                            {
                                object foreinEntityValue = p.GetValue(l);
                                if (foreinEntityValue != null && foreinEntityValue == o)
                                {
                                    if (referenced.Contains(l) == false) referenced.Add(l);
                                }
                            }
                        }

                    }
                }
            }
            /*
            JManyToMany manyToManyAttr = AttrHelper.GetAttribute<JManyToMany>(p);
            if (manyToManyAttr != null)
            {
                Type foreinEntityType = AttrHelper.GetGenericListArgType(p.PropertyType);
                if (foreinEntityType == sourceEntityType)
                {
                    HashSet<object> referenced = null;
                    rels.TryGetValue(entity, out referenced);
                    if (referenced == null)
                    {
                        referenced = new HashSet<object>();
                        rels.Add(entity, referenced);
                    }

                    TypeComparer typeComparer = new TypeComparer();
                    Type[] ts = new Type[] { sourceEntityType, foreinEntityType };
                    Array.Sort(ts, typeComparer);
                    JoinEntityData cross = FindAllJoinData(ts[0], ts[1], manyToManyAttr.JoinName);
                    bool reverse = false;
                    if (ts[0].Equals(sourceEntityType) == false) reverse = true;
                    //from crosstable
                    foreach (var l in cross.DataList)
                    {
                        if (reverse)
                        {
                            if (sourcePKValue.Equals(l.Pk2))
                            {
                                object refO = Find(foreinEntityType, l.Pk1);
                                if (refO != null) referenced.Add(refO);
                            }
                        }
                        else
                        {
                            if (sourcePKValue.Equals(l.Pk1))
                            {
                                object refO = Find(foreinEntityType, l.Pk2);
                                if (refO != null) referenced.Add(refO);
                            }
                        }
                    }

                }

            }
            */
            foreach (JoinEntityData s in joinDatas)
            {
                if (s.DataType1.Equals(sourceEntityType) || s.DataType2.Equals(sourceEntityType))
                {
                    bool reverse = false;
                    Type foreinEntityType = null;
                    if (s.DataType2.Equals(sourceEntityType))
                    {
                        reverse = true;
                        foreinEntityType = s.DataType1;
                    }
                    else foreinEntityType = s.DataType2;

                    HashSet<object> referenced = null;
                    rels.TryGetValue(foreinEntityType, out referenced);
                    if (referenced == null)
                    {
                        referenced = new HashSet<object>();
                        rels.Add(foreinEntityType, referenced);
                    }

                    //from crosstable
                    foreach (var l in s.DataList)
                    {
                        if (reverse)
                        {
                            if (sourcePKValue.Equals(l.Pk2))
                            {
                                object refO = Find(foreinEntityType, l.Pk1);
                                if (refO != null) referenced.Add(refO);
                            }
                        }
                        else
                        {
                            if (sourcePKValue.Equals(l.Pk1))
                            {
                                object refO = Find(foreinEntityType, l.Pk2);
                                if (refO != null) referenced.Add(refO);
                            }
                        }
                    }
                }
            }


            string hr = "=============================================" + br;
            string usingInStr = FrwUtilsRes.Dm_UsedInEntity + " ";
            string totalStr = " " + FrwUtilsRes.Dm_Total + ": ";

            StringBuilder str = new StringBuilder();

            str.Append(ModelHelper.GetEntityJDescriptionOrFullName(o.GetType()) + ": " + ModelHelper.GetNameForObjectAdv(o));
            str.Append(br);
            str.Append(br);

            foreach (var rt in rels)
            {
                HashSet<object> refs = rt.Value;
                if (refs.Count > 0)
                {
                    str.Append(usingInStr + ModelHelper.GetEntityJDescriptionOrFullName(rt.Key) + totalStr + refs.Count);
                    str.Append(br);
                    str.Append(hr);
                    foreach (var m in refs)
                    {
                        str.Append(ModelHelper.GetNameForObjectAdv(m));
                        str.Append(br);
                    }
                    str.Append(hr);
                    str.Append(br);
                }
            }
            return str.ToString();

        }


        private void InsertObject(object o)
        {
            if (o == null) return;
            Type t = o.GetType();
            IList list = FindAll(t);
            if (list.Contains(o) == false)
            {
                list.Add(o);
                AddObjectToPKCache(o);
            }
            SetEntityModified(t);
        }

        private void AddObjectToPKCache(object o)
        {
            Type t = o.GetType();
            PropertyInfo pkProp = AttrHelper.GetProperty<JPrimaryKey>(t);
            if (pkProp != null)
            {
                object pk = pkProp.GetValue(o);
                GetSData(t).PkCache.Add(pk, o);
            }
        }

        /// <summary>
        /// This method is called after the object is changed. Now it actually only changes the flag of the modification
        /// 
        /// 
        /// </summary>
        /// <param name="o"></param>
        private void UpdateObject(object o)
        {
            if (o == null) return;
            Type t = o.GetType();
            SetEntityModified(t);
        }
        virtual public void SaveObject(object o)
        {
            Console.WriteLine("!!!!!!!!!!!!!!!! SaveObject " + o.GetType().Name + " " + ModelHelper.GetNameForObject(o));

            if (o == null) return;
            Type t = o.GetType();
            IList list = FindAll(t);
            JValidationResult result = ValidateObject(o);
            if (result.isError) throw new JValidationException(result);
            if (list.Contains(o) == false) InsertObject(o);
            else UpdateObject(o);
            UpdateRelations(o);
        }
        virtual public void SaveObject(object o, string updatedPropertyName)
        {
            if (o == null) return;
            else if (updatedPropertyName != null) SaveObject(o);//todo
            else SaveObject(o);
        }

        public JValidationResult ValidateObject(object o)
        {
            JValidationResult result = new JValidationResult();
            MethodInfo method = AttrHelper.GetMethod(typeof(JValidate), o.GetType());
            if (method != null)
            {
                try
                {
                    method.Invoke(o, new object[] { result });
                    return result;
                }
                catch (TargetInvocationException ex)
                {
                    Console.WriteLine("ERROR VALIDATING " + ex);
                }
            }

            
            var results = new List<ValidationResult>();
            var context = new ValidationContext(o);
            if (!Validator.TryValidateObject(o, context, results, true))
            {
                result.ValidationResults = results;
                foreach (var error in results)
                {
                    Console.WriteLine(error.ErrorMessage);
                    result.JValidationErrors.Add(new JValidationError() { Message = error.ErrorMessage });
                }
            }


            return result; 
        }

        public void SetEntityModified<T>()
        {
            SetEntityModified(typeof(T));
        }
        public void SetEntityModified(Type t)
        {
            SData sdata = GetSData(t);
            if (sdata != null)
                sdata.Modified = true;
        }
        public bool IsEntityModified<T>()
        {
            return IsEntityModified(typeof(T));
        }
        public bool IsEntityModified(Type t)
        {
            SData sdata = GetSData(t);
            if (sdata != null)
                return sdata.Modified;
            else return false;
        }

        //for testing 
        public bool IsJoinEntityModified(Type t1, Type t2, string joinTableName)
        {
            TypeComparer typeComparer = new TypeComparer();
            Type[] ts = new Type[] { t1, t2 };
            Array.Sort(ts, typeComparer);
            JoinEntityData sdata = GetJoinData(ts[0], ts[1], joinTableName);
            if (sdata != null)
                return sdata.Modified;
            else return false;
        }

       public void SetJoinEntityModified(Type t1, Type t2, string joinTableName)
        {
            JoinEntityData sdata = GetJoinData(t1, t2, joinTableName);
            if (sdata != null)
                sdata.Modified = true;
        }
        private void ResolveToManyRelations(object o)
        {
            if (o == null) return;
            Type t = o.GetType();
            foreach (var p in t.GetProperties())
            {
                ResolveRelation(o, t, p);
            }
        }

        private SData GetSData<T>()
        {
            return GetSData(typeof(T));
        }
        private SData GetSData(Type t)
        {
            SData sdata = null;
            foreach (var s in sdatas)
            {
                if (s.DataType.Equals(t))
                {
                    sdata = s;
                    break;
                }
            }
            return sdata;
        }
        private SData GetSDataForCollection(object collection)
        {
            SData sdata = null;
            foreach (var s in sdatas)
            {
                if (s.DataList.Equals(collection))
                {
                    sdata = s;
                    break;
                }
            }
            return sdata;
        }
        private JoinEntityData GetJoinData<T1, T2>(string crossTableName)
        {
            return GetJoinData(typeof(T1), typeof(T2), crossTableName);
        }
        private JoinEntityData GetJoinData(Type t1, Type t2, string crossTableName)
        {
            JoinEntityData sdata = null;
            foreach (var s in joinDatas)
            {
                if (s.DataType1.Equals(t1) && s.DataType2.Equals(t2) && (crossTableName == null || crossTableName.Equals(s.JoinTableName) ))
                {
                    sdata = s;
                    break;
                }
            }
            return sdata;
        }


        /// <summary>
        /// Loads one table
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        virtual public IList FindAll(Type t)
        {
            SData sdata = GetSData(t);
            if (sdata == null)
            {
                sdata = LoadEntityFromDisk(t);
                ResolveManyToRelationsForEntity(sdata);
                ResolveToManyRelationsForEntity(sdata);
            }
            return (IList)sdata.DataList;
        }
        public IList<T> FindAll<T>()
        {
            Type t = typeof(T);
            return (IList<T>)FindAll(t);
        }

        private void LoadAllEntitiesData(List<Type> entities)
        {

            long mem1 = GC.GetTotalMemory(true);

            long t1 = DateTime.Now.Ticks;
            int count = 0;
            foreach (Type t in entities)
            {
                JEntity entityAttr = AttrHelper.GetClassAttribute<JEntity>(t);
                if (entityAttr.CustomLoad == false)
                {
                    long tstart = DateTime.Now.Ticks;
                    SData sdata = LoadEntityFromDisk(t);
                    long tend = DateTime.Now.Ticks;
                    count = count + ((IList)sdata.DataList).Count;
                    //Log.ProcessDebug("Loaded entity: " + t.FullName + " Count: " + ((IList)sdata.DataList).Count +
                       // " Time: " + (tend - tstart) / 10000 + " mils");
                }
            }
            long t2 = DateTime.Now.Ticks;
            Log.ProcessDebug("Load all entities with time " + (t2 - t1) / 10000 + " ms. Total records: " + count);

            t1 = DateTime.Now.Ticks;
            foreach (SData sdata in sdatas)
            {
                JEntity entityAttr = AttrHelper.GetClassAttribute<JEntity>(sdata.DataType);
                if (entityAttr.CustomLoad == false)
                {
                    long tstart = DateTime.Now.Ticks;
                    ResolveManyToRelationsForEntity(sdata);
                    long tend = DateTime.Now.Ticks;
                    //Log.ProcessDebug("Resolve ManyTo: " + sdata.DataType.FullName + " Count: " + ((IList)sdata.DataList).Count +
                        //" Time: " + (tend - tstart) / 10000 + " mils");
                }
            }
            t2 = DateTime.Now.Ticks;
            Log.ProcessDebug("Resolved 'Many To Many' and 'Many To One' relationship for all entities with time " + (t2 - t1) / 10000 + " ms.");

            t1 = DateTime.Now.Ticks;
            foreach (SData sdata in sdatas)
            {
                JEntity entityAttr = AttrHelper.GetClassAttribute<JEntity>(sdata.DataType);
                if (entityAttr.CustomLoad == false)
                {
                    long tstart = DateTime.Now.Ticks;
                    ResolveToManyRelationsForEntity(sdata);
                    long tend = DateTime.Now.Ticks;
                    //Log.ProcessDebug("Resolve ToMany: " + sdata.DataType.FullName + " Count: " + ((IList)sdata.DataList).Count +
                        //" Time: " + (tend - tstart) / 10000 + " mils");
                }
            }
            t2 = DateTime.Now.Ticks;
            Log.ProcessDebug("Resolved 'One To Many' relationship for all entities with time " + (t2 - t1) / 10000 + " ms.");
            long mem2 = GC.GetTotalMemory(true);
            Log.ProcessDebug("Allocated memory after loading  all entities: " + (mem2 - mem1) / 1000 + " Kb.");
        }

        private SData LoadEntityFromDisk(Type t)
        {
            long tstart = DateTime.Now.Ticks;
            SData sdata = new SData() { DataType = t };
            Type lt = typeof(List<>);
            Type listType = lt.MakeGenericType(t);
            object list = null;
            string filename = GetDataFilePathForType(t);
            FileInfo fileInfo = new FileInfo(filename);
            if (fileInfo.Exists)
            {
                list = JsonSerializeHelper.LoadForType(filename, listType);
            }
            else
            {
                list = Activator.CreateInstance(listType);
            }
            foreach (var x in (IList)list)
            {
                INotifyPropertyChanged notifier = x as INotifyPropertyChanged;
                if (notifier != null)
                {
                    notifier.PropertyChanged += Notifier_PropertyChanged;
                }
            }
            PropertyInfo pkProp = AttrHelper.GetProperty<JPrimaryKey>(t);
            if (pkProp != null)
            {
                foreach (var o in (IList)list)
                {
                    object pk = pkProp.GetValue(o);
                    try
                    {
                        sdata.PkCache.Add(pk, o);
                    }
                    catch(ArgumentException ex)
                    {
                        Log.ProcessDebug("Dublicate pk entity: " + t + " pk value:  " + pk + " Error text: " + ex);
                    }
                }
            }
            sdata.DataList = list;
            sdatas.Add(sdata);
            return sdata;
        }



        private void ResolveManyToRelationsForEntity(SData sdata)
        {
            Type t = sdata.DataType;
            IList list = (IList)sdata.DataList;
            long tstartResolveManyToOne = DateTime.Now.Ticks;
            foreach (var l in (IList)list)
            {
                var props = t.GetProperties();
                foreach (var p in props)
                {
                    JManyToOne manyToOneAttr = AttrHelper.GetAttribute<JManyToOne>(t, p.Name);
                    if (manyToOneAttr != null)
                    {
                        Type pt = p.PropertyType;
                        object pvSaved = p.GetValue(l);
                        if (pvSaved != null)
                        {
                            object pvReal = null;
                            PropertyInfo pkProp = AttrHelper.GetProperty<JPrimaryKey>(pt);
                            if (pkProp == null) throw new Exception("Primary key not found in referenced entity");
                            object pkValue = pkProp.GetValue(pvSaved);
                            if (pkValue != null)
                            {
                                pvReal = Find(pt, pkValue);
                            }
                            else
                            {
                                //may be removed 
                            }
                            p.SetValue(l, pvReal);
                        }
                    }
                }

            }

        }
        private void ResolveToManyRelationsForEntity(SData sdata)
        {
            Type t = sdata.DataType;
            IList list = (IList)sdata.DataList;
            //resolve toMany relations
            foreach (var l in (IList)list)
            {
                ResolveToManyRelations(l);
            }
        }

        //Use IEnumerable to prevent changing collections
        virtual public IEnumerable FindByParams(Type t, IDictionary<string, object> pars)
        {
            Type listType = typeof(List<>).MakeGenericType(t);
            IList values = (IList)Activator.CreateInstance(listType);
            var list = FindAll(t);
            foreach(var l in list)
            {
                bool eu = false;
                foreach (var par in pars)
                {
                    PropertyInfo p = t.GetProperty(par.Key);
                    if (p != null)
                    {
                        object v = p.GetValue(l);
                        if ((par.Value == null && v == null)
                            || (par.Value != null && par.Value.Equals(v))){
                            eu = true;    
                        }
                        break;
                    }
                    if (eu == false) break;
                }
                if (eu) values.Add(l);
            }
            return values;
        }
        public IEnumerable<T> FindByParams<T>(IDictionary<string, object> pars)
        {
            Type t = typeof(T);
            return (IEnumerable<T>)FindByParams(t, pars);
        }


     

        private void Notifier_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SData sdata = GetSData(sender.GetType());
            if (sdata != null)
            {
                sdata.Modified = true;
            }
        }



        /// <summary>
        /// Preserves the entire database of typed data
        /// </summary>
        public void SaveAllEntitiesData(bool allways)
        {
            foreach (var s in sdatas)
            {
                if (s.Modified || allways)
                {
                    SaveEntityData(s);
                }
            }
            foreach (var s in joinDatas)
            {
                if (s.Modified || allways)
                {
                    SaveJoinEntityData(s);
                }
            }
        }
        public void SaveEntityData(Type type)
        {
            SData s = GetSData(type);
            if (s != null)
            {
                SaveEntityData(s);
            }
        }

        virtual protected string GetDataFilePathForType(Type dataType)
        {
            string dirPath = Path.Combine(FrwConfig.Instance.ProfileDir, DATA_STORAGE);
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (dir.Exists == false)
            {
                Directory.CreateDirectory(dir.FullName);
            }
            string filename = Path.Combine(dirPath, dataType.FullName + ".json");
            return filename;
        }

        public object GetObjectForExport(object o)
        {
            if (o == null) return null;
            return CloneObject(o, CloneObjectType.ForExport);
        }


        public void SaveImportedRemoteObject(object o)
        {
            if (o == null) return;
            Type t = o.GetType();

            if ( AttrHelper.IsAttributeDefinedForType<JEntity>(t, true) == false)
            {
                throw new Exception("Non entity type: " + t.FullName);
            }
            //find and replace all rel objects
            foreach (PropertyInfo p in t.GetProperties())
            {
                if (p.GetSetMethod() != null)
                {
                    JManyToOne manyToOneAttr = AttrHelper.GetAttribute<JManyToOne>(p);
                    JOneToMany oneToManyAttr = AttrHelper.GetAttribute<JOneToMany>(p);
                    JManyToMany manyToManyAttr = AttrHelper.GetAttribute<JManyToMany>(p);
                    Type foreinEntityType = null;
                    if (manyToManyAttr != null || oneToManyAttr != null) foreinEntityType = AttrHelper.GetGenericListArgType(p.PropertyType);
                    else foreinEntityType = p.PropertyType;

                    if (manyToManyAttr != null)
                    {
                        object blankObject = p.GetValue(o);
                        if (blankObject != null)
                        {
                            object realObject = FindObjectByPkOnlyObject(blankObject);
                            if (realObject != null)
                            {
                                p.SetValue(o, realObject);
                            }
                            else
                            {
                                //todo warning
                            }
                        }
                    }
                    else if (oneToManyAttr != null || manyToManyAttr != null)
                    {
                        IList value = (IList)AttrHelper.GetPropertyValue(o, p);
                        if (value != null)
                        {
                            IList values = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(foreinEntityType));
                            foreach (var blankObject in value)
                            {
                                object realObject = FindObjectByPkOnlyObject(blankObject);
                                if (realObject != null) {
                                    values.Add(realObject);
                                }
                                else
                                {
                                    //todo warning
                                }
                            }
                            p.SetValue(o, values);
                        }
                    }
                }
            }


            PropertyInfo pkProp = AttrHelper.GetProperty<JPrimaryKey>(t);
            object pkValue = pkProp.GetValue(o);

            object o1 = Find(t, pkValue);
            if (o1 == null) SaveObject(o);
            else
            {
                CopyObjectProperties(o, o1, CopyRestrictLevel.AllPropertiesFullCopy);//todo
                SaveObject(o1);
            }

        }

        public object CloneObject(object o, CloneObjectType cloneType)
        {
            CopyRestrictLevel copyRectLevel = CopyRestrictLevel.OnlySimleProperties;
            if (cloneType == CloneObjectType.ForSave) copyRectLevel = CopyRestrictLevel.AllPropertiesManytoOnePK;
            else if (cloneType == CloneObjectType.ForTemp) copyRectLevel = CopyRestrictLevel.AllPropertiesNewLists;
            else if (cloneType == CloneObjectType.ForExport) copyRectLevel = CopyRestrictLevel.AllPropertiesAllRelPK;

            if (o == null) return null;
            Type t = o.GetType();
            object destObject = Activator.CreateInstance(t);
            CopyObjectProperties(o, destObject, copyRectLevel);
            return destObject;
        }

        private object ReplaceObjectByPkOnlyObject(object realObject)
        {
            if (realObject != null)
            {
                Type foreinEntityType = realObject.GetType();
                object blankObject = Activator.CreateInstance(foreinEntityType);
                PropertyInfo fePkProp = AttrHelper.GetProperty<JPrimaryKey>(foreinEntityType);
                if (fePkProp == null) throw new Exception("No pk property in entity type: " + foreinEntityType.FullName);
                object pkValue = fePkProp.GetValue(realObject);
                if (pkValue == null) throw new Exception("Empty pk value found in entity type: " + foreinEntityType.FullName);
                fePkProp.SetValue(blankObject, pkValue);
                return blankObject;
            }
            else return null;
        }
        private object FindObjectByPkOnlyObject(object blankObject)
        {
            if (blankObject != null)
            {
                Type foreinEntityType = blankObject.GetType();
                PropertyInfo fePkProp = AttrHelper.GetProperty<JPrimaryKey>(foreinEntityType);
                if (fePkProp == null) throw new Exception("No pk property in entity type: " + foreinEntityType.FullName);
                object pkValue = fePkProp.GetValue(blankObject);
                if (pkValue == null) throw new Exception("Empty pk value found in entity type: " + foreinEntityType.FullName);
                return Find(foreinEntityType, pkValue);
            }
            else return null;
        }

        public void CopyObjectProperties(object o, object destObject, CopyRestrictLevel cloneLevel)
        {
            Type t = o.GetType();
            foreach (PropertyInfo p in t.GetProperties())
            {
                if (p.GetSetMethod() != null)
                {
                    JManyToOne manyToOneAttr = AttrHelper.GetAttribute<JManyToOne>(p);
                    JOneToMany oneToManyAttr = AttrHelper.GetAttribute<JOneToMany>(p);
                    JManyToMany manyToManyAttr = AttrHelper.GetAttribute<JManyToMany>(p);

                    if (cloneLevel > CopyRestrictLevel.AllPropertiesFullCopy
                        && (manyToOneAttr != null || oneToManyAttr != null || manyToManyAttr != null))
                    {
                        if (cloneLevel >= CopyRestrictLevel.OnlySimleProperties)
                        {
                            //do not copy rels
                        }
                        else {
                            if (cloneLevel >= CopyRestrictLevel.AllPropertiesManytoOnePK)
                            {
                                if (manyToOneAttr != null)
                                {
                                    //replace by entity with pk only
                                    Type foreinEntityType = p.PropertyType;
                                    object realObject = p.GetValue(o);
                                    if (realObject != null)
                                    {
                                        p.SetValue(destObject, ReplaceObjectByPkOnlyObject(realObject));
                                    }
                                }
                                //other rels do not copy
                            }
                            else if (cloneLevel >= CopyRestrictLevel.AllPropertiesAllRelPK)
                            {
                                //replace by entity with pk only

                                Type foreinEntityType = null;
                                if (manyToManyAttr != null || oneToManyAttr != null) foreinEntityType = AttrHelper.GetGenericListArgType(p.PropertyType);
                                else foreinEntityType = p.PropertyType;

                                if (manyToOneAttr != null)
                                {
                                    object realObject = p.GetValue(o);
                                    if (realObject != null)
                                    {
                                        p.SetValue(destObject, ReplaceObjectByPkOnlyObject(realObject));
                                    }
                                }
                                else if (oneToManyAttr != null || manyToManyAttr != null)
                                {
                                    IList value = (IList)AttrHelper.GetPropertyValue(o, p);
                                    if (value != null)
                                    {
                                        IList values = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(foreinEntityType));
                                        foreach (var realObject in value)
                                        {
                                            values.Add(ReplaceObjectByPkOnlyObject(realObject));
                                        }
                                        p.SetValue(destObject, values);
                                    }
                                }

                            }
                            else if (cloneLevel >= CopyRestrictLevel.AllPropertiesNewLists)
                            {
                                if (manyToOneAttr != null)
                                {
                                    //simple copy
                                    p.SetValue(destObject, p.GetValue(o));
                                }
                                else
                                {
                                    //copy to new list
                                    IList value = (IList)AttrHelper.GetPropertyValue(o, p);
                                    if (value != null)
                                    {
                                        Type foreinEntityType = null;
                                        if (manyToManyAttr != null || oneToManyAttr != null) foreinEntityType = AttrHelper.GetGenericListArgType(p.PropertyType);
                                        else foreinEntityType = p.PropertyType;
                                        IList values = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(foreinEntityType));
                                        foreach (var v in value)
                                        {
                                            values.Add(v);
                                        }
                                        p.SetValue(destObject, values);
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        //simple property or no restriction
                        p.SetValue(destObject, p.GetValue(o));
                    }
                }//no set method
            }
        }

        private void SaveEntityData(SData s)
        {
            string filename = GetDataFilePathForType(s.DataType);// Path.Combine(dirPath, s.DataType.Name + ".json");
            object list = s.DataList;
            var lt = typeof(List<>);
            var listType = lt.MakeGenericType(s.DataType);
            IList alist = (IList)Activator.CreateInstance(listType);
            foreach (object v in (IList)list)
            {
                object av = CloneObject(v, CloneObjectType.ForSave);
                alist.Add(av);
            }
 
            /*
            //clear relations 
            foreach (var v in (IList)list)
            {
                Type t = v.GetType();
                foreach (PropertyInfo p in t.GetProperties())
                {
                    JManyToOne manyToOneAttr = AttrHelper.GetAttribute<JManyToOne>(v, p.Name);
                    JOneToMany oneToManyAttr = AttrHelper.GetAttribute<JOneToMany>(v, p.Name);
                    JManyToMany manyToManyAttr = AttrHelper.GetAttribute<JManyToMany>(v, p.Name);
                    if (oneToManyAttr != null || manyToManyAttr != null)//(manyToOneAttr != null)
                        p.SetValue(v, null);
                    else if (manyToOneAttr != null)
                    {
                        //new
                        object realObject = p.GetValue(v);
                        if (realObject != null)
                        {
                            object blankObject = Activator.CreateInstance(p.PropertyType);
                            PropertyInfo pkProp = AttrHelper.GetProperty<JPrimaryKey>(p.PropertyType);
                            if (pkProp == null) throw new Exception("No pk property in entity type: " + p.PropertyType.FullName);
                            object pkValue = pkProp.GetValue(realObject);
                            if (pkValue == null) throw new Exception("Empty pk value found in entity type: " + p.PropertyType.FullName);
                            pkProp.SetValue(blankObject, pkValue);
                            p.SetValue(v, blankObject);
                            //todo final 
                            //set null to fk
                        }
                    }

                }
            }
            */
            //
            JsonSerializeHelper.SaveToFile(alist, filename);

            //save join data
            foreach (var p in s.DataType.GetProperties())
            {
                JManyToMany manyToManyAttr = AttrHelper.GetAttribute<JManyToMany>(p);
                if (manyToManyAttr != null)
                {
                    Type foreinEntityType = AttrHelper.GetGenericListArgType(p.PropertyType);
                    TypeComparer typeComparer = new TypeComparer();
                    Type[] ts = new Type[] { s.DataType, foreinEntityType };
                    Array.Sort(ts, typeComparer);
                    JoinEntityData sdata = GetJoinData(ts[0], ts[1], manyToManyAttr.JoinName);
                    if (sdata != null && sdata.Modified)
                    {
                        SaveJoinEntityData(sdata);
                    }
                }
            }
            s.Modified = false;
        }


        virtual protected JoinEntityData FindAllJoinData(Type t1, Type t2, string joinTableName)
        {
            JoinEntityData sdata = GetJoinData(t1, t2, joinTableName);
            if (sdata == null)
            {
                long tstart = DateTime.Now.Ticks;
                sdata = new JoinEntityData() { DataType1 = t1, DataType2 = t2, JoinTableName = joinTableName };

                var listType = typeof(List<JoinEntityDataItem>);
                List<JoinEntityDataItem> list = null;
                string filename = Path.Combine(Path.Combine(FrwConfig.Instance.ProfileDir, DATA_STORAGE), t1.FullName + "_" + t2.FullName + 
                    (joinTableName != null ? ("_" + joinTableName) : "") +".json");
                FileInfo fileInfo = new FileInfo(filename);
                if (fileInfo.Exists)
                {
                    list = JsonSerializeHelper.LoadForType(filename, listType);
                }
                else
                {
                    list = (List<JoinEntityDataItem>) Activator.CreateInstance(listType);
                }
                sdata.DataList = list;

                long tstartConvert = DateTime.Now.Ticks;

                PropertyInfo pkProp1 = AttrHelper.GetProperty<JPrimaryKey>(sdata.DataType1);
               
                if (pkProp1.PropertyType != typeof(string))
                {
                    foreach (var l in sdata.DataList)
                    {
                        l.Pk1 = JsonSerializeHelper.DeserializeString(l.Pk1.ToString(), pkProp1.PropertyType);
                    }
                }
                PropertyInfo pkProp2 = AttrHelper.GetProperty<JPrimaryKey>(sdata.DataType1);
                if (pkProp2.PropertyType != typeof(string))
                {
                    foreach (var l in sdata.DataList)
                    {
                        l.Pk2 = JsonSerializeHelper.DeserializeString(l.Pk2.ToString(), pkProp2.PropertyType);
                    }
                }
                joinDatas.Add(sdata);
                long tend = DateTime.Now.Ticks;
                //Log.ProcessDebug("======= Loaded join data : " + sdata.DataType1 + " " + sdata.DataType2  + " Count: " + ((IList)list).Count + " Time: " + (tend - tstart) / 10000 + " mils" + " include time converting: " + (tend - tstartConvert) / 10000 + " mils");
            }
            return sdata;
        }

        private void SaveJoinEntityData(JoinEntityData s)
        {
            string dirPath = Path.Combine(FrwConfig.Instance.ProfileDir, DATA_STORAGE);
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (dir.Exists == false)
            {
                Directory.CreateDirectory(dir.FullName);
            }
            string filename = Path.Combine(dirPath, s.DataType1.FullName + "_" + s.DataType2.FullName +".json");
            object list = s.DataList;
            JsonSerializeHelper.SaveToFile(list, filename);
            s.Modified = false;
        }

        public string GetCommonStoragePathForObject(object o)
        {
            return Path.Combine(FrwConfig.Instance.ProfileDir, DATA_STORAGE);
        }
        public string GetStoragePrefixForObject(object o)
        {
            Type oType = o.GetType();
            object pkValue = ModelHelper.GetPKValue(o);
            if (pkValue == null) throw new InvalidOperationException();
            return Path.Combine(oType.FullName, pkValue.ToString());
        }
        public string GetStorageFullPathForObject(object o)
        {
            return Path.Combine(GetCommonStoragePathForObject(o), GetStoragePrefixForObject(o));
        }

        #endregion

        #region dictionary 
        private JDictionary GetDictionary(string id)
        {
            JDictionary sdata = null;
            foreach (var s in dictionaries)
            {
                if (s.Id.Equals(id))
                {
                    sdata = s;
                    break;
                }
            }
            return sdata;
        }
        public List<JDictItem> GetDictionaryItems(string id)
        {
            JDictionary dict = GetDictionary(id);
            if (dict != null) return dict.Items;
            else return null;
        }
        public JDictItem GetDictText(string id, string key)
        {
            JDictionary dict = GetDictionary(id);
            JDictItem sdata = null;
            if (dict != null)
            {
                foreach (var s in dict.Items)
                {
                    if (s.Key.Equals(key))
                    {
                        sdata = s;
                        break;
                    }
                }
                return sdata;
            }
            else return null;
        }


        private IList ResolveDictionary(object rowObject, string aspectName)
        {
            if (rowObject == null) return null;
            PropertyInfo p = AttrHelper.GetProperty(rowObject.GetType(), aspectName);
            //Type sourceEntityType = rowObject.GetType();
            //PropertyInfo pkProp = AttrHelper.GetProperty<JPrimaryKey>(sourceEntityType);
            JDictProp dictAttr = AttrHelper.GetAttribute<JDictProp>(rowObject, aspectName);
            if (dictAttr != null)
            {
                IList oList = new List<object>();
                object value = AttrHelper.GetPropertyValue(rowObject, aspectName);
                if (value != null)
                {
                    if (dictAttr.AllowMultiValues == true)
                    {
                        IList vList = (IList)value;
                        foreach (var v in vList)
                        {
                            JDictItem dictItem = Dm.Instance.GetDictText(dictAttr.Id, v.ToString());
                            if (dictItem != null) oList.Add(dictItem.Text);
                            else oList.Add("Text not found for: " + v);
                        }

                    }
                    else
                    {
                        JDictItem dictItem = Dm.Instance.GetDictText(dictAttr.Id, value.ToString());
                        if (dictItem != null) oList.Add(dictItem.Text);
                        else oList.Add("Text not found for: " + value);
                    }
                }
                return oList;
            }
            else return null;

        }

        #endregion


       

    }
}
