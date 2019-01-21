﻿/**********************************************************************************
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
using FrwSoftware.Properties;
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
using System.Drawing;

namespace FrwSoftware
{
    public enum TruncatedValueSufix
    {
        None,
        Dots,
        DotsAndShown
    }
    public enum SecLevelEnum
    {
        Low = 0,
        Middle = 1,
        High = 2
    }

    public class DictNames
    {
        public const string YesNo = "YesNo";
        public const string NotificationType = "NotificationType";
        public const string RunningJobStage = "RunningJobStage";
        public const string JobConcurrentType = "JobConcurrentType";
        public const string SecLevel = "SecLevel";
        public const string InfoHeaderType = "InfoHeaderType";
        public const string Protocol = "Protocol";

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
        ForExport,//AllPropertiesAllRelPK
        ForNew//AllPropertiesNewLists  and generate new pk and initial dates 
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

    public class RefEntityInfo
    {
        public Type RefEntity = null;
        public HashSet<object> Records = null;
        public PropertyInfo thisProperty = null;
        public PropertyInfo foreinProperty = null;

        public bool IsSelfRelation()
        {
            if (foreinProperty != null && foreinProperty.PropertyType.Equals(RefEntity)) return true;
            else return false;
        }
        public string GetRelDescription()
        {
            string relDescr = ModelHelper.GetEntityJDescriptionOrName(this.RefEntity);
            if (this.thisProperty != null || this.foreinProperty != null)
            {
                relDescr = relDescr + "(" +
                      ((this.thisProperty != null) ? ModelHelper.GetPropertyJDescriptionOrName(this.thisProperty) : "")
                    + ((this.foreinProperty != null) ? (" /" + ModelHelper.GetPropertyJDescriptionOrName(this.foreinProperty)) : "")
                    + ")";
            }
            return relDescr;
        }
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
        protected const string DATA_CACHE = "dataCache";//Cache prefix in the profile
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

        public void AddDictionary(JDictionary dict)
        {
            dictionaries.Add(dict);
        }

        virtual protected void InitDictionaries()
        {
            //create some tipical dictionaries 
            //Note! Dictionary int field has defautl value 0. Dictionary string field has defautl value null (no value).

            JDictionary dict = null;
            dict = new JDictionary() { Id = DictNames.YesNo };
            dictionaries.Add(dict);
            dict.Items.Add(new JDictItem() { Key = "true", Text = FrwUtilsRes.Yes });
            dict.Items.Add(new JDictItem() { Key = "false", Text = FrwUtilsRes.No });

            dict = new JDictionary() { Id = DictNames.NotificationType };
            dictionaries.Add(dict);
            dict.Items.Add(new JDictItem() { Key = NotificationTypeEnum.error.ToString(), Text = FrwUtilsRes.Error });
            dict.Items.Add(new JDictItem() { Key = NotificationTypeEnum.warning.ToString(), Text = FrwUtilsRes.Warning });
            dict.Items.Add(new JDictItem() { Key = NotificationTypeEnum.task.ToString(), Text = FrwUtilsRes.Task });

            dict = new JDictionary() { Id = DictNames.RunningJobStage };
            dictionaries.Add(dict);
            dict.Items.Add(new JDictItem() { Key = RunningJobStageEnum.initial.ToString(), Image = Properties.Resources.if_application_task_45823, Text = FrwUtilsRes.Initial });
            dict.Items.Add(new JDictItem() { Key = RunningJobStageEnum.waiting.ToString(), Image = Properties.Resources.if_msn_busy_4054, Text = FrwUtilsRes.Waiting });
            dict.Items.Add(new JDictItem() { Key = RunningJobStageEnum.concurrent.ToString(), Image = Properties.Resources.if_Cancel_85238, Text = FrwUtilsRes.Concurrent });
            dict.Items.Add(new JDictItem() { Key = RunningJobStageEnum.running.ToString(), Image = Properties.Resources.execute, Text = FrwUtilsRes.Running });
            dict.Items.Add(new JDictItem() { Key = RunningJobStageEnum.paused.ToString(), Image = Properties.Resources.if_Pause_657901, Text = FrwUtilsRes.Paused });
            dict.Items.Add(new JDictItem() { Key = RunningJobStageEnum.aborted.ToString(), Image = Properties.Resources.if_cancel_43747, Text = FrwUtilsRes.Aborted });
            dict.Items.Add(new JDictItem() { Key = RunningJobStageEnum.error.ToString(), Image = Properties.Resources.if_Error_132716, Text = FrwUtilsRes.Error });
            dict.Items.Add(new JDictItem() { Key = RunningJobStageEnum.warning.ToString(), Image = Properties.Resources.if_Warning_132616, Text = FrwUtilsRes.Completed_with_Warning });
            dict.Items.Add(new JDictItem() { Key = RunningJobStageEnum.complated.ToString(), Image = Properties.Resources.if_tick_circle_12510, Text = FrwUtilsRes.Complated_with_OK });
            dict.Items.Add(new JDictItem() { Key = RunningJobStageEnum.exception.ToString(), Image = Properties.Resources.if_exclamation_diamond_frame_26309, Text = FrwUtilsRes.Exception });

            dict = new JDictionary() { Id = DictNames.JobConcurrentType };
            dictionaries.Add(dict);
            dict.Items.Add(new JDictItem() { Key = JobConcurrentTypeEnum.Allow.ToString(), Text = FrwUtilsRes.Allow });
            dict.Items.Add(new JDictItem() { Key = JobConcurrentTypeEnum.Wait.ToString(), Text = FrwUtilsRes.Wait });
            dict.Items.Add(new JDictItem() { Key = JobConcurrentTypeEnum.Cancel.ToString(), Text = FrwUtilsRes.Cancel });

            dict = new JDictionary() { Id = DictNames.SecLevel };
            dictionaries.Add(dict);
            dict.Items.Add(new JDictItem() { Key = ((int)SecLevelEnum.Low).ToString(), Text = FrwUtilsRes.Low, TextColor = Color.Black});
            dict.Items.Add(new JDictItem() { Key = ((int)SecLevelEnum.Middle).ToString(), Text = FrwUtilsRes.Middle, TextColor = Color.Blue });//green
            dict.Items.Add(new JDictItem() { Key = ((int)SecLevelEnum.High).ToString(), Text = FrwUtilsRes.High, TextColor = Color.Green });//blue



            dict = new JDictionary() { Id = DictNames.InfoHeaderType };
            dictionaries.Add(dict);
            dict.Items.Add(new JDictItem() { Key = InfoHeaderEnum.C.ToString(), Image = Properties.Resources.catalog, Text = "Catalogue" });
            dict.Items.Add(new JDictItem() { Key = InfoHeaderEnum.D.ToString(), Image = Properties.Resources.folder, Text = "Folder" });
            dict.Items.Add(new JDictItem() { Key = InfoHeaderEnum.F.ToString(), Image = Properties.Resources.file, Text = "File" });
            dict.Items.Add(new JDictItem() { Key = InfoHeaderEnum.H.ToString(), Image = Properties.Resources.header, Text = "Header" });

            dict = new JDictionary() { Id = DictNames.Protocol };
            dictionaries.Add(dict);
            dict.Items.Add(new JDictItem() { Key = ProtocolEnum.http.ToString() });
            dict.Items.Add(new JDictItem() { Key = ProtocolEnum.https.ToString() });
            dict.Items.Add(new JDictItem() { Key = ProtocolEnum.ftp.ToString() });
            dict.Items.Add(new JDictItem() { Key = ProtocolEnum.sftp.ToString() });
            dict.Items.Add(new JDictItem() { Key = ProtocolEnum.ssh.ToString() });
            dict.Items.Add(new JDictItem() { Key = ProtocolEnum.rdp.ToString() });
            dict.Items.Add(new JDictItem() { Key = ProtocolEnum.tcp.ToString() });

        }
        public string GetRealPath(string path, object rowObject)
        {
            if (path != null && path.StartsWith(Dm.STORAGE_PREFIX))
            {
                path = path.Substring(Dm.STORAGE_PREFIX.Length);
                path = Path.Combine(Path.Combine(GetCommonStoragePath(), GetStoragePrefixForObject(rowObject)), path);
            }
            return path;
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
            //Finally: dll present in AppDomain.CurrentDomain.GetAssemblies() only when it loaded (after first call of some classes)
            var entityTypes = AttrHelper.GetTypesWithAttribute<JEntity>(true);
            foreach (var sourceEntityType in entityTypes)
            {
                entities.Add(sourceEntityType);
                bool pkFound = false;
                foreach (PropertyInfo p in sourceEntityType.GetProperties())
                {
                    JPrimaryKey pkAttr = AttrHelper.GetAttribute<JPrimaryKey>(p);
                    if (pkAttr != null) pkFound = true;
                    JOneToOne oneToOneAttr = AttrHelper.GetAttribute<JOneToOne>(p);
                    JManyToOne manyToOneAttr = AttrHelper.GetAttribute<JManyToOne>(p);
                    JOneToMany oneToManyAttr = AttrHelper.GetAttribute<JOneToMany>(p);
                    JManyToMany manyToManyAttr = AttrHelper.GetAttribute<JManyToMany>(p);
                    if (oneToOneAttr != null)
                    {
                        if (manyToOneAttr != null || oneToManyAttr != null || manyToManyAttr != null)
                            throw new Exception("Property can not be marked more than one relation attribute. Property " +
                               p.Name + " in enitty " + sourceEntityType);
                        Type foreinEntityType = p.PropertyType;
                        if (AttrHelper.IsAttributeDefinedForType<JEntity>(foreinEntityType, true) == false)
                        {
                            throw new Exception("Not registred (marked JEntity by attribute) referenced entity "
                               + foreinEntityType + " which referenced by  relation property " +
                               p.Name + " in enitty " + sourceEntityType);
                        }
                        PropertyInfo refFieldInForeinEntity = FindRefFieldInForeinEntity(sourceEntityType, foreinEntityType, typeof(JOneToOne), null);

                    }
                    else if (manyToOneAttr != null)
                    {
                        if (oneToOneAttr != null || oneToManyAttr != null || manyToManyAttr != null)
                            throw new Exception("Property can not be marked more than one relation attribute. Property " +
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
                        if (oneToOneAttr != null || manyToManyAttr != null || manyToManyAttr != null)
                            throw new Exception("Property can not be marked more than one relation attribute. Property " +
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
                        if (oneToOneAttr != null || oneToManyAttr != null || manyToOneAttr != null)
                            throw new Exception("Property can not be marked more than one relation attribute. Property " +
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
                    else if (AttrHelper.IsGenericList(p.PropertyType))
                    {
                        Type listArgType = AttrHelper.GetGenericListArgType(p.PropertyType);
                        JEntity listArgTypeEntity = AttrHelper.GetClassAttribute<JEntity>(listArgType);
                        if (listArgTypeEntity != null && listArgTypeEntity.CustomLoad == false)
                        {
                            throw new Exception("Property " + p.Name + " in enitty " + sourceEntityType + " is generic List without relation can not be marked by JEntity attribute");
                        }
                        if (listArgTypeEntity == null)
                        {
                            foreach (PropertyInfo p1 in listArgType.GetProperties())
                            {
                                if (AttrHelper.GetAttribute<JOneToOne>(p1) != null ||
                                AttrHelper.GetAttribute<JManyToOne>(p1) != null ||
                                AttrHelper.GetAttribute<JOneToMany>(p1) != null ||
                                AttrHelper.GetAttribute<JManyToMany>(p1) != null)
                                {
                                    throw new Exception("Type of Property " + p.Name + " in enitty " + sourceEntityType + " whith is generic List without relation can not contains fields with relation attributes.");
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
            else {
                //todo: if not load we must register all Entities and jointables
                throw new NotImplementedException("if not load we must register all Entities and jointables");
            }
        }


        #region Relation
        virtual public object GetCustomSettingValue(JSetting setting, bool asPlainText = true, int maxCount = TRUNCATED_VALUE_MAX_ITEM_COUNT, int maxLength = TRUNCATED_VALUE_MAX_STRING_LENGTH, TruncatedValueSufix truncatedValueSufix = TruncatedValueSufix.DotsAndShown, string delimeter = ", ")
        {

            if (setting.Value == null) return null;
            JEntity entityAttr = AttrHelper.GetClassAttribute<JEntity>(setting.ValueType);
            if (setting.DictId != null)
            {
                return Dm.MakeStringFromObjectList(ResolveDictionaryForSetting(setting), maxCount, maxLength, truncatedValueSufix, delimeter);

            }
            else if (entityAttr != null)
            {
                if (setting.AllowMultiValues)
                {
                    return (setting.Value != null) ? Dm.MakeStringFromObjectList((IList)setting.Value, maxCount, maxLength, truncatedValueSufix, delimeter) : null;
                }
                else return (setting.Value != null) ? Dm.MakeStringFromObjectList(new List<object>() { setting.Value }, maxCount, maxLength, truncatedValueSufix, delimeter) : null;
            }
            else return setting.Value;
        }

        virtual public object GetCustomPropertyValue(object rowObject, string aspectName, bool asPlainText = true, int maxCount = TRUNCATED_VALUE_MAX_ITEM_COUNT, int maxLength = TRUNCATED_VALUE_MAX_STRING_LENGTH, TruncatedValueSufix truncatedValueSufix = TruncatedValueSufix.DotsAndShown, string delimeter = ", ")
        {
            if (rowObject == null) return null;
            Type sourceObjectType = rowObject.GetType();
            Type pType = AttrHelper.GetPropertyType(sourceObjectType, aspectName);
            PropertyInfo propInfo = sourceObjectType.GetProperty(aspectName);
            if (pType == null) return null;//if aspectName not present for sourceObjectType
            if (AttrHelper.GetAttribute<JPrimaryKey>(propInfo) != null)
            {
                object o = AttrHelper.GetPropertyValue(rowObject, propInfo);
                if (o != null)
                {
                    string oStr = o.ToString();
                    return oStr.Length >= 10 ? oStr : (string.Concat(new string(' ', 10 - oStr.Length), oStr));
                }
                else return null;
            }
            else if (AttrHelper.GetAttribute<JText>(propInfo) != null)
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
            else if (AttrHelper.GetAttribute<JManyToOne>(propInfo) != null
                || AttrHelper.GetAttribute<JOneToOne>(propInfo) != null)
            {
                Dm.Instance.ResolveRelation(rowObject, sourceObjectType, propInfo);//do nothing
                object value = AttrHelper.GetPropertyValue(rowObject, propInfo);
                return (value != null) ? Dm.MakeStringFromObjectList(new List<object>() { value }, maxCount, maxLength, truncatedValueSufix, delimeter) : null;
            }
            else if (AttrHelper.GetAttribute<JDictProp>(propInfo) != null)
            {
                JDictProp dictAttr = AttrHelper.GetAttribute<JDictProp>(propInfo);
                return Dm.MakeStringFromObjectList(ResolveDictionary(rowObject, aspectName), maxCount, maxLength, truncatedValueSufix, delimeter);
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
            else if (AttrHelper.IsGenericList(pType))// AttrHelper.IsSameOrSubclass(typeof(IList), pType))//must be last check 
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


        protected static string MakeStringFromObjectList(IList list, int maxCount = Dm.TRUNCATED_VALUE_MAX_ITEM_COUNT, int maxLength = TRUNCATED_VALUE_MAX_STRING_LENGTH, TruncatedValueSufix truncatedValueSufix = TruncatedValueSufix.DotsAndShown, string delimeter = ", ")
        {
            if (list == null) return null;
            int totalCount = list.Count;
            List<string> list2 = new List<string>();
            foreach (var l in list)
            {
                string name = ModelHelper.GetNameForObjectAdv(l);
                list2.Add(name);
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
            JOneToOne oneToOneAttr = AttrHelper.GetAttribute<JOneToOne>(p);
            JManyToOne manyToOneAttr = AttrHelper.GetAttribute<JManyToOne>(p);
            JOneToMany oneToManyAttr = AttrHelper.GetAttribute<JOneToMany>(p);
            JManyToMany manyToManyAttr = AttrHelper.GetAttribute<JManyToMany>(p);
            if (oneToOneAttr != null)
            {
                Type foreinEntityType = p.PropertyType;
                JEntity foreinEntityAttr = AttrHelper.GetClassAttribute<JEntity>(foreinEntityType);
                if (foreinEntityAttr.CustomLoad == true) return;//temp todo

                IList allMayBeReferenced = FindAll(foreinEntityType);
                if (newObject != null)
                {
                    //PropertyInfo foreinPkProp = AttrHelper.GetProperty<JPrimaryKey>(foreinEntityType);
                    //object foreinPKValue = AttrHelper.GetPropertyValue(newObject, foreinPkProp.Name);
                    //if (Find(foreinEntityType, foreinPKValue) == null) throw new Exception("Object from field " + p.Name + " not present in referenced entity " + foreinEntityType);
                    if (!allMayBeReferenced.Contains(newObject)) throw new Exception("Object from field " + p.Name + " not present in referenced entity " + foreinEntityType);
                }

                AttrHelper.SetPropertyValue(rowObject, p, newObject);
                // reverse 
                PropertyInfo refFieldInForeinEntity = FindRefFieldInForeinEntity(sourceEntityType, foreinEntityType, typeof(JOneToOne), null);
                if (refFieldInForeinEntity != null)//if reverse relation field present
                {
                    foreach (var l in allMayBeReferenced)
                    {
                        object foreinEntityValue = refFieldInForeinEntity.GetValue(l);
                        if (newObject == null && rowObject.Equals(foreinEntityValue) == true)
                        {
                            //unset on not this relation
                            refFieldInForeinEntity.SetValue(l, null);
                        }
                        else if (newObject != null)
                        {
                            if (newObject.Equals(l) && rowObject.Equals(foreinEntityValue) == false)
                            {
                                //set
                                refFieldInForeinEntity.SetValue(l, rowObject);
                            }
                            else if (newObject.Equals(l) == false && rowObject.Equals(foreinEntityValue) == true)
                            {
                                //unset
                                refFieldInForeinEntity.SetValue(l, null);
                            }
                        }
                    }
                }
            }
            else if (manyToOneAttr != null)
            {
                Type foreinEntityType = p.PropertyType;
                JEntity foreinEntityAttr = AttrHelper.GetClassAttribute<JEntity>(foreinEntityType);
                if (foreinEntityAttr.CustomLoad == true) return;//temp todo

                IList allMayBeReferenced = FindAll(foreinEntityType);
                if (newObject != null) {
                    //PropertyInfo foreinPkProp = AttrHelper.GetProperty<JPrimaryKey>(foreinEntityType);
                    //object foreinPKValue = AttrHelper.GetPropertyValue(newObject, foreinPkProp.Name);
                    //if (Find(foreinEntityType, foreinPKValue) == null) throw new Exception("Object from field " + p.Name + " not present in referenced entity " + foreinEntityType);

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
                JEntity foreinEntityAttr = AttrHelper.GetClassAttribute<JEntity>(foreinEntityType);
                if (foreinEntityAttr.CustomLoad == true) return;//temp todo

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
                JEntity foreinEntityAttr = AttrHelper.GetClassAttribute<JEntity>(foreinEntityType);
                if (foreinEntityAttr.CustomLoad == true) return;//temp todo

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
            JOneToOne oneToOneAttr = AttrHelper.GetAttribute<JOneToOne>(p);
            JManyToOne manyToOneAttr = AttrHelper.GetAttribute<JManyToOne>(p);
            JOneToMany oneToManyAttr = AttrHelper.GetAttribute<JOneToMany>(p);
            JManyToMany manyToManyAttr = AttrHelper.GetAttribute<JManyToMany>(p);

            if (oneToOneAttr != null || manyToOneAttr != null)
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
                if (sourceEntityValue != null)
                {
                    values.Add(sourceEntityValue);
                }
                else { 
                    //todo
                }
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
        public object ResolveOneToOneRelation(object rowObject, Type foreinEntityType, string refFieldNameInForeinEntity = null)
        {
            //todo optimization if present field in rowObject
            if (rowObject == null) return null;
            Type sourceEntityType = rowObject.GetType();
            PropertyInfo foreinEntityPK = AttrHelper.GetProperty<JPrimaryKey>(foreinEntityType);
            PropertyInfo foreinEntityOneToOne = FindRefFieldInForeinEntity(sourceEntityType, foreinEntityType, typeof(JOneToOne), refFieldNameInForeinEntity);
            IList list = FindAll(foreinEntityType);
            foreach (var l in list)
            {
                var foreinEntityValue = foreinEntityOneToOne.GetValue(l);
                if (rowObject.Equals(foreinEntityValue))
                {
                    return l;
                }
            }
            return null; 
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
                    Type refType = (typeof(JManyToOne).Equals(attrTypeToFind) || typeof(JOneToOne).Equals(attrTypeToFind)) ? p.PropertyType : AttrHelper.GetGenericListArgType(p.PropertyType);
                    if (refType.Equals(sourceEntityType))
                    {
                        if (foreinEntityRefField != null) throw new Exception("Found more than one field referenced to this entity type");
                        foreinEntityRefField = p;
                    }
                }
            }
            else
            {
                //if (typeof(JOneToOne).Equals(attrTypeToFind)) throw new ArgumentException();
                
                //nameToFind Set explicitly, which is relevant if the table has several fields of the same type that refer to the same table
                foreach (var p in foreinEntityRefFields)
                {
                    Type refType = (typeof(JManyToOne).Equals(attrTypeToFind) || typeof(JOneToOne).Equals(attrTypeToFind)) ? p.PropertyType : AttrHelper.GetGenericListArgType(p.PropertyType);
                    if (refType.Equals(sourceEntityType))
                    {
                        if (
                            ((typeof(JManyToOne).Equals(attrTypeToFind) || typeof(JOneToMany).Equals(attrTypeToFind)) && (p.Name).Equals(nameToFind))
                            || 
                            (typeof(JManyToMany).Equals(attrTypeToFind) && (p.Name).Equals(nameToFind))
                            ||
                            (typeof(JOneToOne).Equals(attrTypeToFind) && (p.Name).Equals(nameToFind))
                            )//todo 
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
            GenerateAndSetNewPkAndDateForObject(o);
            if (AttrHelper.IsAttributeDefinedForType<JEntity>(t, true))
            {
                ResolveToManyRelations(o);
            }
            if (pars != null)
            {
                foreach (var d in pars)
                {
                    object value = d.Value;
                    IEnumerable<PropertyInfo> props = props = o.GetType().GetProperties();
                    bool found = false;
                    foreach (var p in props)
                    {
                        if (p.Name.Equals(d.Key))
                        {
                            JOneToMany oneToManyAttr = AttrHelper.GetAttribute<JOneToMany>(o.GetType(), p.Name);
                            JManyToMany manyToManyAttr = AttrHelper.GetAttribute<JManyToMany>(o.GetType(), p.Name);
                            JDictProp dictAttr = AttrHelper.GetAttribute<JDictProp>(o.GetType(), p.Name);
                            if (oneToManyAttr != null || manyToManyAttr != null || (dictAttr != null && dictAttr.AllowMultiValues == true))
                            {
                                IList list = (IList)AttrHelper.GetPropertyValue(o, p.Name);
                                if (AttrHelper.GetGenericListArgType(p.PropertyType).Equals(value.GetType()) == false)
                                    throw new ArgumentException("Wrong type " + value.GetType() + " for field " + p.Name + " (field list type: " + AttrHelper.GetGenericListArgType(p.PropertyType) + ")");
                                list.Add(value);
                            }
                            else
                            {
                                //todo lists 
                                if (p.PropertyType.Equals(value.GetType()) == false)
                                    throw new ArgumentException("Wrong type " + value.GetType() + " for field " + p.Name + " (field type: " + p.PropertyType + ")");
                                p.SetValue(o, value);
                            }
                            found = true;
                        }
                    }
                    if (!found) throw new ArgumentException("Wrong name of params " + d.Key + " - not found from properties of " + o.GetType().FullName);
                }
            }
            return o;
        }
        private void GenerateAndSetNewPkAndDateForObject(object o)
        {
            Type t = o.GetType();
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
            RemoveAllObjectsFromPKCache(t);
        }
        virtual public void DeleteObject(object o)
        {
            if (o == null) return;
            Type t = o.GetType();
            IList list = FindAll(t);
            list.Remove(o);
            RemoveObjectFromPKCache(o);
            PropertyInfo[] ps = t.GetProperties();
            foreach (var p in ps)
            {
                JOneToOne oneToOneAttr = AttrHelper.GetAttribute<JOneToOne>(p);
                JManyToOne manyToOneAttr = AttrHelper.GetAttribute<JManyToOne>(p);
                //one to one and many to one is necessary, so we can start from this entity 
                if (oneToOneAttr != null)
                {
                    Type foreinEntityType = p.PropertyType;
                    PropertyInfo refFieldInForeinEntity = FindRefFieldInForeinEntity(t, foreinEntityType, typeof(JOneToOne), null);
                    if (refFieldInForeinEntity != null)//if reverse relation field present
                    {
                        object refValue = AttrHelper.GetPropertyValue(o, p);
                        if (refValue != null)
                        {
                            object foreinEntityValue = refFieldInForeinEntity.GetValue(refValue);
                            if (foreinEntityValue != null)
                            {
                                refFieldInForeinEntity.SetValue(refValue, null);
                            }
                        }
                    }
                }
                else if (manyToOneAttr != null)
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
                    JOneToOne oneToOneAttr = AttrHelper.GetAttribute<JOneToOne>(p);
                    JManyToOne manyToOneAttr = AttrHelper.GetAttribute<JManyToOne>(p);
                    if (oneToOneAttr != null || manyToOneAttr != null)
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
        public List<RefEntityInfo> GetAllReferencedToEntity(object o, bool fillRecords = true)
        {
            return GetAllReferencedToEntityLocal(o, fillRecords);
        }
 

        private List<RefEntityInfo> GetAllReferencedToEntityLocal(object o, bool fillRecords = true)
        {
            if (o == null) return null;
            Type sourceEntityType = o.GetType();

            List<RefEntityInfo> rels = new List<RefEntityInfo>();
            //Dictionary<Type, HashSet<object>> rels = new Dictionary<Type, HashSet<object>>();
            PropertyInfo pkProp = AttrHelper.GetProperty<JPrimaryKey>(sourceEntityType);
            if (pkProp == null) return rels;
            object sourcePKValue = pkProp.GetValue(o);
            foreach (var entity in entities)
            {
                foreach (PropertyInfo p in entity.GetProperties())
                {
                    JOneToOne oneToOneAttr = AttrHelper.GetAttribute<JOneToOne>(p);
                    JManyToOne manyToOneAttr = AttrHelper.GetAttribute<JManyToOne>(p);
                    if (oneToOneAttr != null || manyToOneAttr != null)
                    {
                        Type foreinEntityType = p.PropertyType;
                        if (foreinEntityType == sourceEntityType)
                        {
                            RefEntityInfo refEntityInfo = new RefEntityInfo();
                            if (fillRecords) refEntityInfo.Records = new HashSet<object>();
                            refEntityInfo.RefEntity = entity;
                            refEntityInfo.foreinProperty = p;
                            rels.Add(refEntityInfo);
                            //todo rel name
                            foreach (PropertyInfo pp in sourceEntityType.GetProperties())
                            {
                                JOneToMany oneToManyAttr = AttrHelper.GetAttribute<JOneToMany>(pp);
                                if (oneToManyAttr != null)
                                {
                                    Type foreinEntityType1 = AttrHelper.GetGenericListArgType(pp.PropertyType);
                                    if (foreinEntityType1 == foreinEntityType)
                                    {
                                        refEntityInfo.thisProperty = pp;
                                    }
                                }
                            }                            /*
                            HashSet<object> referenced = null;
                            rels.TryGetValue(entity, out referenced);
                            if (referenced == null)
                            {
                                referenced = new HashSet<object>();
                                rels.Add(entity, referenced);
                            }
                            */
                            if (fillRecords)
                            {
                                IList allMayBeReferenced = FindAll(entity);
                                foreach (object l in allMayBeReferenced)
                                {
                                    object foreinEntityValue = p.GetValue(l);
                                    if (foreinEntityValue != null && foreinEntityValue == o)
                                    {
                                        if (refEntityInfo.Records.Contains(l) == false) refEntityInfo.Records.Add(l);
                                    }
                                }
                            }
                        }

                    }
                }
            }
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
                    RefEntityInfo refEntityInfo = new RefEntityInfo();
                    if (fillRecords) refEntityInfo.Records = new HashSet<object>();
                    refEntityInfo.RefEntity = foreinEntityType;
                    rels.Add(refEntityInfo);

                    //todo join name 
                    foreach (PropertyInfo p in foreinEntityType.GetProperties())
                    {
                        JManyToMany manyToManyAttr = AttrHelper.GetAttribute<JManyToMany>(p);
                        if (manyToManyAttr != null)
                        {
                            Type foreinEntityType1 = AttrHelper.GetGenericListArgType(p.PropertyType);
                            if (foreinEntityType1 == sourceEntityType)
                            {
                                refEntityInfo.foreinProperty = p;
                            }
                        }
                    }
                    //todo join name
                    foreach (PropertyInfo p in sourceEntityType.GetProperties())
                    {
                        JManyToMany manyToManyAttr = AttrHelper.GetAttribute<JManyToMany>(p);
                        if (manyToManyAttr != null)
                        {
                            Type foreinEntityType1 = AttrHelper.GetGenericListArgType(p.PropertyType);
                            if (foreinEntityType1 == foreinEntityType)
                            {
                                refEntityInfo.thisProperty = p;
                            }
                        }
                    }

                    /*
                    HashSet<object> referenced = null;
                    rels.TryGetValue(foreinEntityType, out referenced);
                    if (referenced == null)
                    {
                        referenced = new HashSet<object>();
                        rels.Add(foreinEntityType, referenced);
                    }
                    */
                    //from crosstable
                    if (fillRecords)
                    {
                        foreach (var l in s.DataList)
                        {
                            if (reverse)
                            {
                                if (sourcePKValue.Equals(l.Pk2))
                                {
                                    object refO = Find(foreinEntityType, l.Pk1);
                                    if (refO != null) refEntityInfo.Records.Add(refO);
                                }
                            }
                            else
                            {
                                if (sourcePKValue.Equals(l.Pk1))
                                {
                                    object refO = Find(foreinEntityType, l.Pk2);
                                    if (refO != null) refEntityInfo.Records.Add(refO);
                                }
                            }
                        }
                    }
                }
            }
            return rels;
        }

        public string GetDependencyReport(object o, string br = "\r\n")
        {
            List<RefEntityInfo> rels = GetAllReferencedToEntityLocal(o);

            string hr = "=============================================" + br;
            string usingInStr = FrwUtilsRes.Dm_UsedInEntity + " ";
            string thisField = "This entity field" + " ";
            string refField = "Referenced entity field" + " ";
            string totalStr = " " + FrwUtilsRes.Dm_Total + ": ";

            StringBuilder str = new StringBuilder();

            str.Append(ModelHelper.GetEntityJDescriptionOrFullName(o.GetType()) + ": " + ModelHelper.GetNameForObjectAdv(o));
            str.Append(br);
            str.Append(br);

            foreach (var rt in rels)
            {
                HashSet<object> refs = rt.Records;
                if (refs.Count > 0)
                {
                    str.Append(usingInStr + ModelHelper.GetEntityJDescriptionOrFullName(rt.RefEntity) + totalStr + refs.Count);
                    if (rt.thisProperty != null)
                    {
                        str.Append(br);
                        str.Append(thisField);
                        str.Append(": ");
                        str.Append(ModelHelper.GetPropertyJDescriptionOrName(rt.thisProperty));
                    }
                    if (rt.foreinProperty != null)
                    {
                        str.Append(br);
                        str.Append(refField);
                        str.Append(": ");
                        str.Append(ModelHelper.GetPropertyJDescriptionOrName(rt.foreinProperty));
                    }
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

        #region pk cache
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
        private void RemoveObjectFromPKCache(object o)
        {
            Type t = o.GetType();
            PropertyInfo pkProp = AttrHelper.GetProperty<JPrimaryKey>(t);
            if (pkProp != null)
            {
                object pk = pkProp.GetValue(o);
                GetSData(t).PkCache.Remove(pk);
            }
        }
        private void RemoveAllObjectsFromPKCache(Type t)
        {
            GetSData(t).PkCache.Clear();
        }
        //todo Update Cache when user modify pk
        #endregion

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
                ResolveOneToOneAndManyToOneRelationsForEntity(sdata);
                ResolveToManyRelationsForEntity(sdata);
            }
            return (IList)sdata.DataList;
        }
        virtual public IList FindRootList(Type t)
        {
            PropertyInfo p = ModelHelper.GetSelfPropertiesForEntity(t).FirstOrDefault();
            List<object> rootList = new List<object>();
            IList list = Dm.Instance.FindAll(t);
            foreach (var l in list)
            {
                if (p.GetValue(l) == null) rootList.Add(l);
            }
            return rootList;
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
                    ResolveOneToOneAndManyToOneRelationsForEntity(sdata);
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
            PostLoadEntity(t);
            return sdata;
        }

        /// <summary>
        /// Executes after entity data load from disk, but before resolving relations 
        /// </summary>
        /// <param name="t"></param>
        virtual protected void PostLoadEntity(Type t)
        {
        }

        private void ResolveOneToOneAndManyToOneRelationsForEntity(SData sdata)
        {
            Type t = sdata.DataType;
            IList list = (IList)sdata.DataList;
            long tstartResolveManyToOne = DateTime.Now.Ticks;
            foreach (var l in (IList)list)
            {
                var props = t.GetProperties();
                foreach (var p in props)
                {
                    JReadOnly readOnly = AttrHelper.GetAttribute<JReadOnly>(t, p.Name);
                    if (p.CanWrite == false || readOnly != null) continue;

                    JOneToOne oneToOneAttr = AttrHelper.GetAttribute<JOneToOne>(t, p.Name);
                    JManyToOne manyToOneAttr = AttrHelper.GetAttribute<JManyToOne>(t, p.Name);
                    if (oneToOneAttr != null || manyToOneAttr != null)
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
                                try
                                {
                                    pvReal = Find(pt, pkValue);
                                }
                                catch (Exception ex)
                                {
                                    PropertyInfo nameProp = AttrHelper.GetProperty<JNameProperty>(pt);
                                    if (nameProp != null) nameProp.SetValue(pvSaved, Resources.Dm_ErrorFinding + ex);
                                }
                                if (pvReal != null)
                                {
                                    p.SetValue(l, pvReal);
                                }
                                else
                                {
                                    //may be removed 
                                    PropertyInfo nameProp = AttrHelper.GetProperty<JNameProperty>(pt);
                                    if (nameProp != null) nameProp.SetValue(pvSaved, Resources.Dm_NotFound);
                                }
                            }
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

        virtual protected string GetDataFilePathForType(Type dataType, string customDirPath = null)
        {
            string dirPath = (customDirPath != null) ? customDirPath : Path.Combine(FrwConfig.Instance.ProfileDir, DATA_STORAGE);
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
            else if (cloneType == CloneObjectType.ForNew) copyRectLevel = CopyRestrictLevel.AllPropertiesNewLists;
            else if (cloneType == CloneObjectType.ForExport) copyRectLevel = CopyRestrictLevel.AllPropertiesAllRelPK;

            if (o == null) return null;
            Type t = o.GetType();
            object destObject = Activator.CreateInstance(t);
            CopyObjectProperties(o, destObject, copyRectLevel);
            if (cloneType == CloneObjectType.ForNew)
            {
                GenerateAndSetNewPkAndDateForObject(destObject);
            }
            return destObject;
        }
        public object CloneObjectToOtherType(object o, Type destType)
        {
            if (o == null) return null;
            object destObject = Activator.CreateInstance(destType);
            CopyObjectProperties(o, destObject, CopyRestrictLevel.AllPropertiesNewLists);
            return destObject;
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
            Type destT = destObject.GetType();
            foreach (PropertyInfo p in t.GetProperties())
            {
                if (p.GetSetMethod() != null)
                {
                    JOneToOne oneToOneAttr = AttrHelper.GetAttribute<JOneToOne>(p);
                    JManyToOne manyToOneAttr = AttrHelper.GetAttribute<JManyToOne>(p);
                    JOneToMany oneToManyAttr = AttrHelper.GetAttribute<JOneToMany>(p);
                    JManyToMany manyToManyAttr = AttrHelper.GetAttribute<JManyToMany>(p);

                    if (cloneLevel > CopyRestrictLevel.AllPropertiesFullCopy
                        && (oneToOneAttr != null || manyToOneAttr != null || oneToManyAttr != null || manyToManyAttr != null))
                    {
                        if (cloneLevel >= CopyRestrictLevel.OnlySimleProperties)
                        {
                            //do not copy rels
                        }
                        else {
                            if (cloneLevel >= CopyRestrictLevel.AllPropertiesManytoOnePK)
                            {
                                if (oneToOneAttr != null || manyToOneAttr != null)
                                {
                                    //replace by entity with pk only
                                    Type foreinEntityType = p.PropertyType;
                                    object realObject = p.GetValue(o);
                                    if (realObject != null)
                                    {
                                        if (destT.GetProperty(p.Name) != null && p.PropertyType.Equals(destT.GetProperty(p.Name).PropertyType))
                                        {
                                            p.SetValue(destObject, AttrHelper.ReplaceObjectByPkOnlyObject(realObject));
                                        }
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

                                if (oneToOneAttr != null || manyToOneAttr != null)
                                {
                                    object realObject = p.GetValue(o);
                                    if (realObject != null)
                                    {
                                        if (destT.GetProperty(p.Name) != null && p.PropertyType.Equals(destT.GetProperty(p.Name).PropertyType))
                                        {
                                            p.SetValue(destObject, AttrHelper.ReplaceObjectByPkOnlyObject(realObject));
                                        }
                                    }
                                }
                                else if (oneToManyAttr != null || manyToManyAttr != null)
                                {
                                    IList value = (IList)AttrHelper.GetPropertyValue(o, p);
                                    if (value != null)
                                    {
                                        if (destT.GetProperty(p.Name) != null && p.PropertyType.Equals(destT.GetProperty(p.Name).PropertyType))
                                        {
                                            Type destForeinEntityType = AttrHelper.GetGenericListArgType(destT.GetProperty(p.Name).PropertyType, true);
                                            if (foreinEntityType.Equals(destForeinEntityType))
                                            {

                                                IList values = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(foreinEntityType));
                                                foreach (var realObject in value)
                                                {
                                                    values.Add(AttrHelper.ReplaceObjectByPkOnlyObject(realObject));
                                                }

                                                p.SetValue(destObject, values);
                                            }
                                        }
                                    }
                                }

                            }
                            else if (cloneLevel >= CopyRestrictLevel.AllPropertiesNewLists)
                            {
                                if (oneToOneAttr != null || manyToOneAttr != null)
                                {
                                    if (destT.GetProperty(p.Name) != null && p.PropertyType.Equals(destT.GetProperty(p.Name).PropertyType))
                                    {
                                        //simple copy
                                        p.SetValue(destObject, p.GetValue(o));
                                    }
                                }
                                else
                                {
                                    //copy to new list
                                    IList value = (IList)AttrHelper.GetPropertyValue(o, p);
                                    if (value != null)
                                    {
                                        if (destT.GetProperty(p.Name) != null && p.PropertyType.Equals(destT.GetProperty(p.Name).PropertyType))
                                        {
                                            Type foreinEntityType = null;
                                            if (manyToManyAttr != null || oneToManyAttr != null) foreinEntityType = AttrHelper.GetGenericListArgType(p.PropertyType);
                                            else foreinEntityType = p.PropertyType;
                                            Type destForeinEntityType = AttrHelper.GetGenericListArgType(destT.GetProperty(p.Name).PropertyType, true);
                                            if (foreinEntityType.Equals(destForeinEntityType))
                                            {

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

                        }
                    }
                    else
                    {
                        if (destT.GetProperty(p.Name) != null && p.PropertyType.Equals(destT.GetProperty(p.Name).PropertyType))
                        {
                            //simple property or no restriction
                            p.SetValue(destObject, p.GetValue(o));
                        }
                    }
                }//no set method
            }
        }

        public void SaveEntityDataToOtherLocation(IList list, Type type, string customDirPath)
        {
            SaveEntityDataLocal(list, type, customDirPath);
        }

        private void SaveEntityDataLocal(IList list, Type type, string customDirPath)
        {
            string filename = GetDataFilePathForType(type, customDirPath);
            var lt = typeof(List<>);
            var listType = lt.MakeGenericType(type);
            IList alist = (IList)Activator.CreateInstance(listType);
            foreach (object v in list)
            {
                object av = CloneObject(v, CloneObjectType.ForSave);
                alist.Add(av);
            }
            //
            JsonSerializeHelper.SaveToFile(alist, filename);
        }

        private void SaveEntityData(SData s)
        {
            object list = s.DataList;
            SaveEntityDataLocal((IList)list, s.DataType, null);
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

        public string GetCommonStoragePath()
        {
            return Path.Combine(FrwConfig.Instance.ProfileDir, DATA_STORAGE);
        }
        public string GetCommonCachePath()
        {
            return Path.Combine(FrwConfig.Instance.ProfileDir, DATA_CACHE);
        }
        public string GetCommonTempPath()
        {
            return Path.Combine(FrwConfig.Instance.ProfileDir, TempDirPrefix);
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
            return Path.Combine(GetCommonStoragePath(), GetStoragePrefixForObject(o));
        }
        public string GetCacheFullPathForObject(object o)
        {
            return Path.Combine(GetCommonCachePath(), GetStoragePrefixForObject(o));
        }
        public string GetTempFullPathForObject(object o)
        {
            return Path.Combine(GetCommonTempPath(), GetStoragePrefixForObject(o));
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
            JDictProp dictAttr = AttrHelper.GetAttribute<JDictProp>(rowObject, aspectName);
            if (dictAttr != null)
            {
                object value = AttrHelper.GetPropertyValue(rowObject, aspectName);
                return ResolveDictionaryLocal(dictAttr.Id, dictAttr.AllowMultiValues, value);
            }
            else return null;
        }
        private IList ResolveDictionaryForSetting(JSetting setting)
        {
            if (setting.Value == null) return null;
            if (setting.DictId != null)
            {
                return ResolveDictionaryLocal(setting.DictId, setting.AllowMultiValues, setting.Value);
            }
            else return null;
        }


        private IList ResolveDictionaryLocal(string dictId, bool allowMultiValues, object value)
        {
            IList oList = new List<object>();
            if (value != null)
            {
                if (allowMultiValues == true)
                {
                    IList vList = (IList)value;
                    foreach (var v in vList)
                    {
                        JDictItem dictItem = Dm.Instance.GetDictText(dictId, v.ToString());
                        if (dictItem != null) oList.Add(dictItem.Text);
                        else oList.Add("Text not found for: " + v);
                    }

                }
                else
                {
                    JDictItem dictItem = Dm.Instance.GetDictText(dictId, value.ToString());
                    if (dictItem != null) oList.Add(dictItem.Text);
                    else oList.Add("Text not found for: " + value);
                }
            }
            return oList;
        }

        #endregion
    }
}
