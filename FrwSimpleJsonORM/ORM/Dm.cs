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
    public class JoinEntityInfo
    {
        public Type DataType1 { get; set; }
        public Type DataType2 { get; set; }
        public string JoinTableName { get; set; }
    }
    public class DictNames
    {
        public const string YesNo = "YesNo";
        public const string NotificationType = "NotificationType";
        public const string RunningJobStage = "RunningJobStage";
        public const string JobConcurrentType = "JobConcurrentType";
        public const string InfoHeaderType = "InfoHeaderType";
        public const string Protocol = "Protocol";

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
        public RefEntityInfo(Type sourceEntity)
        {
            SourceEntity = sourceEntity;
        }

        public Type SourceEntity = null;
        public Type ForeignEntity = null;
        public HashSet<object> Records = null;
        public PropertyInfo PropertyInSource = null;
        public PropertyInfo PropertyInForeign = null;

        public string Name
        {
            get
            {
                if (PropertyInSource != null) return PropertyInSource.Name;
                else if (PropertyInForeign != null) return ForeignEntity.FullName + "_" + PropertyInForeign.Name;
                else return ForeignEntity.FullName;
            }
        }

        public bool IsSelfRelation()
        {
            if (PropertyInForeign != null && PropertyInForeign.PropertyType.Equals(ForeignEntity)) return true;
            else return false;
        }
        public string GetRelDescription()
        {
            StringBuilder str = new StringBuilder();
            if (this.PropertyInSource != null)
            {
                str.Append(ModelHelper.GetPropertyJDescriptionOrName(this.PropertyInSource));
                if (this.ForeignEntity != null)
                {
                    str.Append(" (");
                    str.Append(ModelHelper.GetEntityJDescriptionOrName(this.ForeignEntity));
                    str.Append(")");
                }
            }
            else
            {
                str.Append(ModelHelper.GetEntityJDescriptionOrName(this.ForeignEntity));
                //str.Append(" (");
                //str.Append("Список");
                //str.Append(")");
                if (this.PropertyInForeign != null)
                {
                    str.Append(" (List from field: " + ModelHelper.GetPropertyJDescriptionOrName(this.PropertyInForeign) + ")");
                }
            }
            return str.ToString();
        }
    }

    public partial class Dm
    {
        protected const string DATA_STORAGE = "dataStorage";//Data warehouse prefix in the profile
        static private string BROWSER_CACHE_PATH = "browserCommonCache";

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
        protected const string DATA_CACHE = "dataCache";//Cache prefix in the profile
        public const int TRUNCATED_VALUE_MAX_ITEM_COUNT = 10;
        public const int TRUNCATED_VALUE_MAX_STRING_LENGTH = 300;
        static public string CPUId { get; set; }
        static public string UserName { get; set; }



        protected List<JDictionary> dictionaries = null;//cache for dicrionaries
        private List<Type> entities = new List<Type>();
        private List<JoinEntityInfo> joinDatas = new List<JoinEntityInfo>();//Cache for crosstables

        private Dictionary<Type, List<IEntityPlugin>> entityPluginsMap = new Dictionary<Type, List<IEntityPlugin>>();


        private Dictionary<Type, IDs> dsTypeMap = new Dictionary<Type, IDs>();
        private Dictionary<Type, IDs> dsMap = new Dictionary<Type, IDs>();
        public void RegisterDsType(Type dsType, IDs ds)
        {
            dsTypeMap[dsType] = ds;
        }

        public IDs GetDs(JoinEntityInfo join)
        {
            IDs ds1 = GetDs(join.DataType1);
            IDs ds2 = GetDs(join.DataType2);
            if (ds1.GetType() == ds2.GetType()) return ds1;
            else if (ds1.GetType() == typeof(JsonDs)) return ds1;
            else if (ds2.GetType() == typeof(JsonDs)) return ds2;
            else return ds1;
        }

        public IDs GetDs(Type t)
        {
            IDs ds;
            dsMap.TryGetValue(t, out ds);

            if (ds == null)
            {
                JEntity entityAttr = AttrHelper.GetClassAttribute<JEntity>(t);
                dsTypeMap.TryGetValue(entityAttr.DsType, out ds);
                if (ds == null)
                {
                    ds = (IDs)Activator.CreateInstance(entityAttr.DsType);
                    ds.Init();
                    dsTypeMap[entityAttr.DsType] = ds;
                }
                dsMap[t] = ds;
                //throw new InvalidOperationException("DS not found for type: " + t);
            }
            return ds;
        }

        public List<IEntityPlugin> GetPlugins(Type t)
        {
            List<IEntityPlugin> ds;
            entityPluginsMap.TryGetValue(t, out ds);
            return ds;
        }
        public List<IEntityPlugin> RegisterPlugin(Type t, IEntityPlugin plugin)
        {
            List<IEntityPlugin> ds;
            entityPluginsMap.TryGetValue(t, out ds);
            if (ds == null)
            {
                ds = new List<IEntityPlugin>();
                entityPluginsMap[t] = ds;
            }
            ds.Add(plugin);
            return ds;
        }


        virtual public void Init()
        {
            //create caches in Init method - this is good check that no CRUD methods can be called before initialization 
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
            foreach(var v in dsMap.Values)
            {
                v.Destroy();
            }
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
                        PropertyInfo refFieldInForeinEntity = ModelHelper.FindRefFieldInForeinEntity(sourceEntityType, foreinEntityType, typeof(JOneToOne), null);

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
                        PropertyInfo refFieldInForeinEntity = ModelHelper.FindRefFieldInForeinEntity(sourceEntityType, foreinEntityType, typeof(JOneToMany), manyToOneAttr.RefFieldNameInForeinEntity);
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
                        PropertyInfo refFieldInForeinEntity = ModelHelper.FindRefFieldInForeinEntity(sourceEntityType, foreinEntityType, typeof(JManyToOne), oneToManyAttr.RefFieldNameInForeinEntity);
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
                        PropertyInfo refFieldInForeinEntity = ModelHelper.FindRefFieldInForeinEntity(sourceEntityType, foreinEntityType, typeof(JManyToMany), manyToManyAttr.JoinName);
                        if (refFieldInForeinEntity != null)                            //may be not present 
                        {
                            if (manyToManyAttr.JoinName != null)
                            {
                                JManyToMany foreinManyToManyAttr = AttrHelper.GetAttribute<JManyToMany>(refFieldInForeinEntity);
                                if (manyToManyAttr.JoinName.Equals(foreinManyToManyAttr.JoinName) == false)
                                {
                                    throw new Exception("In named 'Many to Many' relationship referenced entity "
                                       + foreinEntityType + " which referenced by  relation property " +
                                       p.Name + " in enitty " + sourceEntityType + " forein 'Many To Many'  relationship must be also named the same");

                                }
                            }

                        }
                        AddJoinInfo(sourceEntityType, foreinEntityType, manyToManyAttr.JoinName);

                    }
                    else if (AttrHelper.IsGenericList(p.PropertyType))
                    {
                        Type listArgType = AttrHelper.GetGenericListArgType(p.PropertyType);
                        JEntity listArgTypeEntity = AttrHelper.GetClassAttribute<JEntity>(listArgType);
                        if (listArgTypeEntity != null)
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
            //register plugins
            var entityPluginTypes = AttrHelper.GetTypesWithAttribute<JEntityPlugin>(true);
            foreach(var entityPluginType in entityPluginTypes)
            {
                JEntityPlugin entityPluginAttr = AttrHelper.GetClassAttribute<JEntityPlugin>(entityPluginType);
                Type eType = entityPluginAttr.EntityType;
                JEntity entityAttr = AttrHelper.GetClassAttribute<JEntity>(eType);
                if (entityAttr == null)
                {
                    throw new Exception("Plugin is designed for non entity type. " + entityPluginType + " Type " + eType + " not marked as JEntity");
                }
                bool found = false;
                foreach (var e in entityTypes)
                {
                    if (eType == e)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    throw new Exception("Entity type for plugin not found. " + entityPluginType + " Type " + eType);
                }
                object o = Activator.CreateInstance(entityPluginType);
                if (!(o is IEntityPlugin))
                {
                    throw new Exception("Plugin is not implements valid interfaces . " + entityPluginType + " Type " + eType);
                }
                try
                {
                    IEntityPlugin entityPlugin = (IEntityPlugin)o;
                    RegisterPlugin(eType, entityPlugin);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error created plugin" + entityPluginType + " Type " + eType, ex);
                }
            }




        }
        private void LoadAllEntitiesData(List<Type> entities)
        {
            long mem1 = GC.GetTotalMemory(true);

            long t1 = DateTime.Now.Ticks;
            int count = 0;
            List<SData>  sdatas = new List<SData>();
            foreach (var t in entities)
            {
                IDs ds = GetDs(t);
                if (ds.IsCashed())
                {
                    SData s = new SData();
                    s.DataType = t;
                    s.DataList = ds.InitiallyLoadData(t);
                    if (s.DataList != null)
                        sdatas.Add(s);
                }
                PostLoadEntity(t);
                //count = count + ((IList)sdata.DataList).Count;
            }
            long t2 = DateTime.Now.Ticks;
            Log.ProcessDebug("Load all entities with time " + (t2 - t1) / 10000 + " ms. Total records: " + count);
            //resolve relations
            t1 = DateTime.Now.Ticks;
            foreach (var sdata in sdatas)
            {
                IDs ds = GetDs(sdata.DataType);
                if (ds.IsResolvedOnInit())
                    ResolveOneToOneAndManyToOneRelationsForEntities(sdata);
            }

            t2 = DateTime.Now.Ticks;
            Log.ProcessDebug("Resolved 'Many To Many' and 'Many To One' relationship for all entities with time " + (t2 - t1) / 10000 + " ms.");
            t1 = DateTime.Now.Ticks;
            foreach (var sdata in sdatas)
            {
                IDs ds = GetDs(sdata.DataType);
                if (ds.IsResolvedOnInit())
                    ResolveToManyRelationsForEntities(sdata);
            }
            t2 = DateTime.Now.Ticks;
            Log.ProcessDebug("Resolved 'One To Many' relationship for all entities with time " + (t2 - t1) / 10000 + " ms.");
            long mem2 = GC.GetTotalMemory(true);
            Log.ProcessDebug("Allocated memory after loading  all entities: " + (mem2 - mem1) / 1000 + " Kb.");
        }
        /// <summary>
        /// Executes after entity data load from disk, but before resolving relations 
        /// </summary>
        /// <param name="t"></param>
        protected void PostLoadEntity(Type t)
        {
            IDs ds = GetDs(t);
            ds.PostLoadEntity(t);
        }
        private void ResolveOneToOneAndManyToOneRelationsForEntities(SData sdata)
        {
            Type t = sdata.DataType;
            IList list = (IList)sdata.DataList;
            long tstartResolveManyToOne = DateTime.Now.Ticks;
            foreach (var l in list)
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
                            try
                            {
                                //pvReal = Find(pt, pkValue);
                                pvReal = Resolve_ToOneRelationLocal(l, p);
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
                                try
                                {
                                    //may be removed 
                                    PropertyInfo nameProp = AttrHelper.GetProperty<JNameProperty>(pt);
                                    if (nameProp != null) nameProp.SetValue(pvSaved, Resources.Dm_NotFound);
                                }
                                catch(Exception)
                                {
                                }
                            }
                        }
                    }
                }

            }
        }

        public void ResolveAllRelations(object rowObject)
        {
            if (rowObject == null) return;
            Resolve_ToManyRelationsForEntity(rowObject);
            Type t = rowObject.GetType();
            foreach (var p in t.GetProperties())
            {
                JOneToOne oneToOneAttr = AttrHelper.GetAttribute<JOneToOne>(p);
                JManyToOne manyToOneAttr = AttrHelper.GetAttribute<JManyToOne>(p);
                if (manyToOneAttr != null || oneToOneAttr != null)
                {
                    object fo = Resolve_ToOneRelationLocal(rowObject, p);
                    p.SetValue(rowObject, fo);
                }
            }
        }

        private void ResolveToManyRelationsForEntities(SData sdata)
        {
            Type t = sdata.DataType;
            IList list = (IList)sdata.DataList;
            //resolve toMany relations
            foreach (var l in (IList)list)
            {
                Resolve_ToManyRelationsForEntity(l);
            }
        }
        private void Resolve_ToManyRelationsForEntity(object rowObject)
        {
            if (rowObject == null) return;
            Type t = rowObject.GetType();
            foreach (var p in t.GetProperties())
            {
                JOneToMany oneToManyAttr = AttrHelper.GetAttribute<JOneToMany>(p);
                JManyToMany manyToManyAttr = AttrHelper.GetAttribute<JManyToMany>(p);
                if (oneToManyAttr != null)
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
            }
        }


        #region Relation
        public object GetCustomSettingValue(JSetting setting, bool asPlainText = true, int maxCount = TRUNCATED_VALUE_MAX_ITEM_COUNT, int maxLength = TRUNCATED_VALUE_MAX_STRING_LENGTH, TruncatedValueSufix truncatedValueSufix = TruncatedValueSufix.DotsAndShown, string delimeter = ", ")
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
            else if (AttrHelper.GetAttribute<JOneToOne>(propInfo) != null)
            {
                object value = null;
                Type foreinEntityType = propInfo.PropertyType;
                IDs ds = GetDs(foreinEntityType);
                if (ds.IsResolvedOnInit())
                    value = AttrHelper.GetPropertyValue(rowObject, propInfo);
                else
                    value = Dm.Instance.ResolveOneToOneRelation(rowObject, propInfo);
                return (value != null) ? Dm.MakeStringFromObjectList(new List<object>() { value }, maxCount, maxLength, truncatedValueSufix, delimeter) : null;
            }
            else if (AttrHelper.GetAttribute<JOneToMany>(propInfo) != null)
            {
                IList value = null;
                Type foreinEntityType = AttrHelper.GetGenericListArgType(propInfo.PropertyType);
                IDs ds = GetDs(foreinEntityType);
                if (ds.IsResolvedOnInit())
                    value = (IList)AttrHelper.GetPropertyValue(rowObject, propInfo);
                else
                    value = Dm.Instance.ResolveOneToManyRelation(rowObject, foreinEntityType);
                return Dm.MakeStringFromObjectList(value, maxCount, maxLength, truncatedValueSufix, delimeter);
            }
            else if (AttrHelper.GetAttribute<JManyToOne>(propInfo) != null)
            {
                object value = null;
                Type foreinEntityType = propInfo.PropertyType;
                IDs ds = GetDs(foreinEntityType);
                if (ds.IsResolvedOnInit())
                    value = AttrHelper.GetPropertyValue(rowObject, propInfo);
                else
                    value = Dm.Instance.ResolveManyToOneRelation(rowObject, propInfo);
                return (value != null) ? Dm.MakeStringFromObjectList(new List<object>() { value }, maxCount, maxLength, truncatedValueSufix, delimeter) : null;
            }
            else if (AttrHelper.GetAttribute<JManyToMany>(propInfo) != null)
            {
                IList value = null;
                Type sourceEntityType = rowObject.GetType();
                Type foreinEntityType = AttrHelper.GetGenericListArgType(propInfo.PropertyType);
                IDs ds = GetDs(GetJoinInfo(sourceEntityType, foreinEntityType, AttrHelper.GetAttribute<JManyToMany>(propInfo).JoinName));
                if (ds.IsResolvedOnInit())
                    value = (IList)AttrHelper.GetPropertyValue(rowObject, propInfo);
                else
                    value = Dm.Instance.ResolveManyToManyRelation(rowObject, foreinEntityType);
                return Dm.MakeStringFromObjectList(value, maxCount, maxLength, truncatedValueSufix, delimeter);
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
  
        /*
        private void ResolveRelation(object rowObject, PropertyInfo p)
        {
            Type sourceEntityType = rowObject.GetType();
            PropertyInfo pkProp = AttrHelper.GetProperty<JPrimaryKey>(sourceEntityType);
            JOneToOne oneToOneAttr = AttrHelper.GetAttribute<JOneToOne>(p);
            JManyToOne manyToOneAttr = AttrHelper.GetAttribute<JManyToOne>(p);
            JOneToMany oneToManyAttr = AttrHelper.GetAttribute<JOneToMany>(p);
            JManyToMany manyToManyAttr = AttrHelper.GetAttribute<JManyToMany>(p);

            if (oneToOneAttr != null || manyToOneAttr != null)
            {
                //JManyToOne - autoresolved
                Resolve_ToOneRelationLocal(rowObject, p);
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
        */
        public IList<T> ResolveManyToManyRelation<T>(object rowObject, string joinName = null)
        {
            Type t = typeof(T);
            return (IList<T>)ResolveManyToManyRelation(rowObject, t, joinName);
        }

        public IList ResolveManyToManyRelation(object rowObject, Type foreinEntityType, string joinName = null)
        {
            Type sourceEntityType = rowObject.GetType();
            JoinEntityInfo join = GetJoinInfo(sourceEntityType, foreinEntityType, joinName);
            IDs ds = GetDs(join);
            return ds.FindFromJoin(rowObject, foreinEntityType, joinName);
        }
        public IList<T> ResolveOneToManyRelation<T>(object rowObject, string refFieldNameInForeinEntity = null)
        {
            Type t = typeof(T);
            return (IList<T>)ResolveOneToManyRelation(rowObject, t, refFieldNameInForeinEntity);
        }
        public object ResolveOneToOneRelation(object rowObject, PropertyInfo fkProp)
        {
            JOneToOne oneToOneAttr = AttrHelper.GetAttribute<JOneToOne>(fkProp);
            if (oneToOneAttr == null) throw new InvalidDataException("Not a OneToOne relation");
            return Resolve_ToOneRelationLocal(rowObject, fkProp);
        }
        public object ResolveManyToOneRelation(object rowObject, string fkPropertyName)
        {
            if (rowObject == null) return null;
            PropertyInfo fkProp = AttrHelper.GetProperty(rowObject.GetType(), fkPropertyName);
            return ResolveManyToOneRelation(rowObject, fkProp);
        }
        public object ResolveManyToOneRelation(object rowObject, PropertyInfo fkProp)
        {
            JManyToOne manyToOneAttr = AttrHelper.GetAttribute<JManyToOne>(fkProp);
            if (manyToOneAttr == null) throw new InvalidDataException("Not a ManyToOne relation");
            return Resolve_ToOneRelationLocal(rowObject, fkProp);
        }
        private object Resolve_ToOneRelationLocal(object rowObject, PropertyInfo fkProp)
        {
            Type sourceEntityType = rowObject.GetType();
            Type foreinEntityType = fkProp.PropertyType;
            object value = AttrHelper.GetPropertyValue(rowObject, fkProp);
            if (value == null) return null;
            object valuePk = ModelHelper.GetPKValue(value);
            if (valuePk == null) return null;
            IDs ds = GetDs(foreinEntityType);
            return ds.Find(foreinEntityType, valuePk);
        }
        public IList ResolveOneToManyRelation(object rowObject, Type foreinEntityType, string refFieldNameInForeinEntity = null)
        {
            if (rowObject == null) return (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(foreinEntityType)); 
            Type sourceEntityType = rowObject.GetType();
            PropertyInfo foreinEntityPK = AttrHelper.GetProperty<JPrimaryKey>(foreinEntityType);
            PropertyInfo foreinEntityManyToOne = ModelHelper.FindRefFieldInForeinEntity(sourceEntityType, foreinEntityType, typeof(JManyToOne), refFieldNameInForeinEntity);
            IDs ds = GetDs(foreinEntityType);
            return ds.FindBy(foreinEntityType, foreinEntityManyToOne, rowObject);
        }
        public object ResolveOneToOneRelationReverse(object rowObject, Type foreinEntityType, string refFieldNameInForeinEntity = null)
        {
            //todo optimization if present field in rowObject
            if (rowObject == null) return null;
            Type sourceEntityType = rowObject.GetType();
            PropertyInfo foreinEntityOneToOne = ModelHelper.FindRefFieldInForeinEntity(sourceEntityType, foreinEntityType, typeof(JOneToOne), refFieldNameInForeinEntity);
            IDs ds = GetDs(foreinEntityType);
            IList list =  ds.FindBy(foreinEntityType, foreinEntityOneToOne, rowObject);
            if (list != null && list.Count == 1) return list[0];
            else if (list != null && list.Count > 1) throw new InvalidDataException();
            else return null;
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
        public object Find(Type entityType, object primaryKeValue)
        {
            if (primaryKeValue == null) throw new ArgumentException();// return null;
            IDs ds = GetDs(entityType);
            return ds.Find(entityType, primaryKeValue);
        }

        public object FindByNum(Type entityType, int num)
        {
            if (num < 0) throw new ArgumentException();// return null;
            IDs ds = GetDs(entityType);
            return ds.FindByNum(entityType, num);
        }
        public int CountAll(Type entityType)
        {
            IDs ds = GetDs(entityType);
            return ds.CountAll(entityType);
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
        public object EmptyObject(Type t, IDictionary<string, object> pars = null)
        {
            JEntity entityAttr = AttrHelper.GetClassAttribute<JEntity>(t);
            object o = null; 
            if (entityAttr != null)
            {
                IDs ds = GetDs(t);
                o = ds.EmptyObject(t, pars);
            }
            if (o == null) o = Activator.CreateInstance(t);
            GenerateAndSetNewPkAndDateForObject(o);
            if (entityAttr != null)
            {
                IDs ds = GetDs(t);
                if (ds.IsResolvedOnInit())
                {
                    //todo set blank lists
                    Resolve_ToManyRelationsForEntity(o);
                }
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
        static private void GenerateAndSetNewPkAndDateForObject(object o)
        {
            Type t = o.GetType();
            JEntity entityAttr = AttrHelper.GetClassAttribute<JEntity>(t);
            if (entityAttr != null)
            {
                IDs ds = Dm.Instance.GetDs(t);
                ds.GenNextPkValue(o);
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
        public void DeleteAllObjects(Type t)
        {
            IDs ds = GetDs(t);
            ds.DeleteAllObjects(t);
            //todo
            //UpdateRelationForDelete(o);
        }
        public void DeleteObject(object o)
        {
            if (o == null) return;
            Type t = o.GetType();
            IDs ds = GetDs(t);
            ds.DeleteObject(o);
            UpdateRelationForDelete(o);
        }
        //used in TreeList and in Add Context Menu 
        public List<RefEntityInfo> GetAllReferencedToEntity(object o, bool fillRecords = true)
        {
            return GetAllReferencedToEntityLocal(o, fillRecords);
        }
        private JoinEntityInfo GetJoinInfo(Type t1, Type t2, string crossTableName)
        {
            TypeComparer typeComparer = new TypeComparer();
            Type[] ts = new Type[] { t1, t2 };
            Array.Sort(ts, typeComparer);

            JoinEntityInfo sdata = null;
            foreach (var s in joinDatas)
            {
                if (s.DataType1.Equals(ts[0]) && s.DataType2.Equals(ts[1]) && (crossTableName == null || crossTableName.Equals(s.JoinTableName)))
                {
                    sdata = s;
                    break;
                }
            }
            return sdata;
        }
        private void AddJoinInfo(Type t1, Type t2, string crossTableName)
        {
            TypeComparer typeComparer = new TypeComparer();
            Type[] ts = new Type[] { t1, t2 };
            Array.Sort(ts, typeComparer);

            JoinEntityInfo sdata = null;
            foreach (var s in joinDatas)
            {
                if (s.DataType1.Equals(ts[0]) && s.DataType2.Equals(ts[1]) && (crossTableName == null || crossTableName.Equals(s.JoinTableName)))
                {
                    sdata = s;
                    break;
                }
            }
            if (sdata == null)
            {
                sdata = new JoinEntityInfo() { DataType1 = ts[0], DataType2 = ts[1], JoinTableName = crossTableName };
                joinDatas.Add(sdata);
            }
        }

        private List<RefEntityInfo> GetAllReferencedToEntityLocal(object objectRefTo, bool fillRecords = true)
        {
            if (objectRefTo == null) return null;
            Type objectRefToType = objectRefTo.GetType();
            JEntity objectRefToEntityAttr = AttrHelper.GetClassAttribute<JEntity>(objectRefToType);
            List<RefEntityInfo> rels = new List<RefEntityInfo>();
            PropertyInfo objectRefToPkProp = AttrHelper.GetProperty<JPrimaryKey>(objectRefToType);
            if (objectRefToPkProp == null) return rels;
            object objectRefToPKValue = objectRefToPkProp.GetValue(objectRefTo);

            foreach(PropertyInfo prop in objectRefToType.GetProperties())
            {
                JOneToOne oneToOneAttr = AttrHelper.GetAttribute<JOneToOne>(prop);
                JManyToOne manyToOneAttr = AttrHelper.GetAttribute<JManyToOne>(prop);
                if (oneToOneAttr != null || manyToOneAttr != null)
                {
                    Type refEntityType = prop.PropertyType;
                    RefEntityInfo refEntityInfo = new RefEntityInfo(objectRefToType);
                    if (fillRecords) refEntityInfo.Records = new HashSet<object>();
                    refEntityInfo.ForeignEntity = refEntityType;// objectRefToType;
                    //refEntityInfo.RefFromProperty = prop;
                    refEntityInfo.PropertyInSource = prop;
                    rels.Add(refEntityInfo);
                    //todo rel name
                    /*
                    foreach (PropertyInfo pp in objectRefToType.GetProperties())
                    {
                        JOneToMany oneToManyAttr = AttrHelper.GetAttribute<JOneToMany>(pp);
                        if (oneToManyAttr != null)
                        {
                            Type foreinEntityType1 = AttrHelper.GetGenericListArgType(pp.PropertyType);
                            if (foreinEntityType1 == refFromEntityType)
                            {
                                refEntityInfo.RefToProperty = pp;
                            }
                        }
                    }
                    */
                    if (fillRecords)
                    {
                        object value = AttrHelper.GetPropertyValue(objectRefTo, prop);
                        if (value != null)
                        {
                            refEntityInfo.Records.Add(value);
                        }
                    }
                }
            }

            foreach (var refFromEntity in entities)
            {
                foreach (PropertyInfo refFromProp in refFromEntity.GetProperties())
                {
                    JOneToOne oneToOneAttr = AttrHelper.GetAttribute<JOneToOne>(refFromProp);
                    JManyToOne manyToOneAttr = AttrHelper.GetAttribute<JManyToOne>(refFromProp);
                    if (oneToOneAttr != null || manyToOneAttr != null)
                    {
                        Type refFromEntityType = refFromProp.PropertyType;
                        if (refFromEntityType == objectRefToType)
                        {
                            RefEntityInfo refEntityInfo = new RefEntityInfo(objectRefToType);
                            if (fillRecords) refEntityInfo.Records = new HashSet<object>();
                            refEntityInfo.ForeignEntity = refFromEntity;
                            refEntityInfo.PropertyInForeign = refFromProp;
                            rels.Add(refEntityInfo);
                            //todo rel name
                            foreach (PropertyInfo pp in objectRefToType.GetProperties())
                            {
                                JOneToMany oneToManyAttr = AttrHelper.GetAttribute<JOneToMany>(pp);
                                if (oneToManyAttr != null)
                                {
                                    Type foreinEntityType1 = AttrHelper.GetGenericListArgType(pp.PropertyType);
                                    if (foreinEntityType1 == refFromEntityType)
                                    {
                                        refEntityInfo.PropertyInSource = pp;
                                    }
                                }
                            }                            
                            if (fillRecords)
                            {
                                IDs ds = GetDs(refFromEntity);
                                IList ll = ds.FindBy(refFromEntity, refFromProp, objectRefTo);
                                foreach (var l3 in ll)
                                {
                                    refEntityInfo.Records.Add(l3);
                                }
                            }
                        }
                    }
                }
            }
            foreach (JoinEntityInfo s in joinDatas)
            {
                if (s.DataType1.Equals(objectRefToType) || s.DataType2.Equals(objectRefToType))
                {
                    RefEntityInfo refEntityInfo = null;
                    if (s.DataType2.Equals(objectRefToType))
                    {
                        refEntityInfo = new RefEntityInfo(s.DataType2);
                        refEntityInfo.ForeignEntity = s.DataType1;
                    }
                    else
                    {
                        refEntityInfo = new RefEntityInfo(s.DataType1);
                        refEntityInfo.ForeignEntity = s.DataType2;
                    }
                    if (fillRecords) refEntityInfo.Records = new HashSet<object>();
                    rels.Add(refEntityInfo);

                    //todo join name 
                    foreach (PropertyInfo p in refEntityInfo.ForeignEntity.GetProperties())
                    {
                        JManyToMany manyToManyAttr = AttrHelper.GetAttribute<JManyToMany>(p);
                        if (manyToManyAttr != null)
                        {
                            Type foreinEntityType1 = AttrHelper.GetGenericListArgType(p.PropertyType);
                            if (foreinEntityType1 == objectRefToType)
                            {
                                refEntityInfo.PropertyInForeign = p;
                            }
                        }
                    }
                    //todo join name
                    foreach (PropertyInfo p in objectRefToType.GetProperties())
                    {
                        JManyToMany manyToManyAttr = AttrHelper.GetAttribute<JManyToMany>(p);
                        if (manyToManyAttr != null)
                        {
                            Type foreinEntityType1 = AttrHelper.GetGenericListArgType(p.PropertyType);
                            if (foreinEntityType1 == refEntityInfo.ForeignEntity)
                            {
                                refEntityInfo.PropertyInSource = p;
                            }
                        }
                    }
                    //from crosstable
                    if (fillRecords)
                    {
                        IDs ds = GetDs(s); 
                        IList ll = ds.FindFromJoin(objectRefTo, refEntityInfo.ForeignEntity, s.JoinTableName);
                        foreach (var l in ll)
                        {
                            refEntityInfo.Records.Add(l);
                        }
                    }
                }
            }
            return rels;
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
                IDs fds = GetDs(foreinEntityType);
               

                AttrHelper.SetPropertyValue(rowObject, p, newObject);//todo
                // reverse 
                PropertyInfo refFieldInForeinEntity = ModelHelper.FindRefFieldInForeinEntity(sourceEntityType, foreinEntityType, typeof(JOneToOne), null);
                if (refFieldInForeinEntity != null)//if reverse relation field present
                {
                    //находим все, кто ссылается на изменяемый объект 
                    IList allMayBeReferenced = fds.FindBy(foreinEntityType, refFieldInForeinEntity, rowObject);

                    foreach (var l in allMayBeReferenced)
                    {//спикок всех кто сслылается на изменяемый 
                        object foreinEntityValue = refFieldInForeinEntity.GetValue(l);
                        //если ссылка в изменяемом объекте удалена, в ссылающемся осталась - удалить ее 
                        //если ссылка в изменяемом объекте есть, а изменяемый уже не ссылается на этот объект 
                        if ((newObject == null && ModelHelper.GetPKValue(rowObject).Equals(ModelHelper.GetPKValue(foreinEntityValue)) == true)
                            || (newObject != null && ModelHelper.GetPKValue(newObject).Equals(ModelHelper.GetPKValue(l)) == false))
                        {
                            //unset on not this relation
                            refFieldInForeinEntity.SetValue(l, null);
                            //здеь нужно сохранять объект, т.к. данная связь персистентная, а не виртуальная 
                            fds.SaveObject(l);//todo
                        }
                    }
                    if (newObject != null)
                    {
                        //найти новый объект на кторый ссылаются 
                        object newRefObject =fds.Find(foreinEntityType, ModelHelper.GetPKValue(newObject));
                        //если он не существует это ошибка 
                        if (newRefObject == null) throw new Exception("Object from field " + p.Name + " not present in referenced entity " + foreinEntityType);
                        //проверить что он не нулевой и ссылается на тот же 
                        object foreinEntityValue = refFieldInForeinEntity.GetValue(newRefObject);
                        if (foreinEntityValue != null)
                        {
                            if (ModelHelper.GetPKValue(rowObject).Equals(ModelHelper.GetPKValue(foreinEntityValue)) == false)
                            {
                                refFieldInForeinEntity.SetValue(newRefObject, rowObject);
                                fds.SaveObject(newRefObject);//todo
                            }
                        }
                        else
                        {
                            refFieldInForeinEntity.SetValue(newRefObject, rowObject);
                            fds.SaveObject(newRefObject);//todo
                        }
                    }
                }
            }
            else if (manyToOneAttr != null)
            {
                Type foreinEntityType = p.PropertyType;
                JEntity foreinEntityAttr = AttrHelper.GetClassAttribute<JEntity>(foreinEntityType);

                IDs fds = GetDs(foreinEntityType);
                if (newObject != null)
                {
                    //найти новый объект на кторый ссылаются 
                    object newRefObject = fds.Find(foreinEntityType, ModelHelper.GetPKValue(newObject));
                    //если он не существует это ошибка 
                    if (newRefObject == null) throw new Exception("Object from field " + p.Name + " not present in referenced entity " + foreinEntityType);
                }

                PropertyInfo refFieldInForeinEntity = ModelHelper.FindRefFieldInForeinEntity(sourceEntityType, foreinEntityType, typeof(JOneToMany), manyToOneAttr.RefFieldNameInForeinEntity);
                if (refFieldInForeinEntity != null)//if reverse relation field present
                {
                    fds.UpdateFieldWithOneToManyRelation(rowObject, newObject, foreinEntityType, refFieldInForeinEntity);
                }
            }
            else if (oneToManyAttr != null)
            {
                IList newRelObjects = (IList)newObject;
                if (AttrHelper.FindDublicatesInList(newRelObjects) != null) throw new Exception("Found dublicates in values list of property " + p.Name + "  entity " + sourceEntityType);
                Type foreinEntityType = AttrHelper.GetGenericListArgType(p.PropertyType);//difference from manytoone!
                PropertyInfo refFieldInForeinEntity = ModelHelper.FindRefFieldInForeinEntity(sourceEntityType, foreinEntityType, typeof(JManyToOne), oneToManyAttr.RefFieldNameInForeinEntity);
                IDs fds = GetDs(foreinEntityType);

                //находим всех, кто ссылается на изменяемый объект 
                IList allMayBeReferenced = fds.FindBy(foreinEntityType, refFieldInForeinEntity, rowObject);
                //если кого то из них нет в новом списке - обнуляем 
                foreach (var ro in allMayBeReferenced)
                {
                    if (AttrHelper.IsListContainsObjectWithSamePk(newRelObjects, ro) == false)
                    {
                        //unset on not this relation
                        refFieldInForeinEntity.SetValue(ro, null);
                        //здеь нужно сохранять объект, т.к. данная связь персистентная, а не виртуальная 
                        fds.SaveObject(ro);//todo
                    }
                }
                //проходим по новому списку и ищем все объекты на кторые он ссылается 
                foreach (var no in newRelObjects)
                {
                    //если у кого то не установлена обратня связь - устанавливаем 
                    //найти новый объект на который ссылаются 
                    object newRefObject = fds.Find(foreinEntityType, no);
                    //если он не существует это ошибка 
                    if (newRefObject == null) throw new Exception("Object from field " + p.Name + " not present in referenced entity " + foreinEntityType);
                    //проверить что он не нулевой и ссылается на тот же 
                    object foreinEntityValue = refFieldInForeinEntity.GetValue(newRefObject);
                    if (foreinEntityValue != null)
                    {
                        if (ModelHelper.GetPKValue(rowObject).Equals(ModelHelper.GetPKValue(foreinEntityValue)) == false)
                        {
                            object oldObject = foreinEntityValue;
                            refFieldInForeinEntity.SetValue(newRefObject, rowObject);
                            fds.SaveObject(newRefObject);//todo
                            //необходимо удалить ссылку из списка у старого объекта
                            IList oldObjectList = (IList)p.GetValue(oldObject);
                            if (oldObjectList != null && oldObjectList.Contains(newRefObject))
                            {
                                oldObjectList.Remove(newRefObject);
                            }
                        }
                    }
                    else
                    {
                        refFieldInForeinEntity.SetValue(newRefObject, rowObject);
                        fds.SaveObject(newRefObject);//todo
                    }
                }
            }
            else if (manyToManyAttr != null)
            {
                IList newRelObjects = (IList)newObject;
                if (AttrHelper.FindDublicatesInList(newRelObjects) != null) throw new Exception("Found dublicates in values list of property " + p.Name + "  entity " + sourceEntityType);
                Type foreinEntityType = AttrHelper.GetGenericListArgType(p.PropertyType);//difference from manytoone!
                PropertyInfo refFieldInForeinEntity = ModelHelper.FindRefFieldInForeinEntity(sourceEntityType, foreinEntityType, typeof(JManyToMany), manyToManyAttr.JoinName);
                //JEntity foreinEntityAttr = AttrHelper.GetClassAttribute<JEntity>(foreinEntityType);
                //if (foreinEntityAttr.CustomLoad == true) return;//temp todo

                JoinEntityInfo joinInfo = GetJoinInfo(sourceEntityType, foreinEntityType, manyToManyAttr.JoinName);
                IDs jds = GetDs(joinInfo);
                IDs fds = GetDs(foreinEntityType);
                //получить старый список связей (в данном случае он доступен, т.к. хранится отдельно)  
                IList joinList = jds.FindFromJoin(rowObject, foreinEntityType, manyToManyAttr.JoinName);
                foreach (var ro in joinList)
                {
                    if (AttrHelper.IsListContainsObjectWithSamePk(newRelObjects, ro) == false)
                    {
                        //remove join 
                        jds.DeleteJoin(rowObject, ro, manyToManyAttr.JoinName);
                        //remove from forein
                        if (refFieldInForeinEntity != null)
                        {
                            object foreinObject = fds.Find(foreinEntityType, ModelHelper.GetPKValue(ro));
                            if (foreinObject != null)
                            {
                                IList foreinEntityValue = (IList)refFieldInForeinEntity.GetValue(ro);
                                if (foreinEntityValue != null && foreinEntityValue.Contains(rowObject))
                                {
                                    foreinEntityValue.Remove(rowObject);
                                }

                            }
                        }
                    }
                }
                foreach (var o in newRelObjects)
                {
                    //найти новый объект на который ссылаются 
                    object newRefObject = fds.Find(foreinEntityType, o);
                    //если он не существует это ошибка 
                    if (newRefObject == null) throw new Exception("Object from field " + p.Name + " not present in referenced entity " + foreinEntityType);

                    if (AttrHelper.IsListContainsObjectWithSamePk(joinList, o) == false)
                    {
                        //add join 
                        jds.AddJoin(rowObject, o, manyToManyAttr.JoinName);
                        //add to forein
                        if (refFieldInForeinEntity != null)
                        {
                            object foreinObject = fds.Find(foreinEntityType, ModelHelper.GetPKValue(o));
                            if (foreinObject != null)
                            {
                                IList foreinEntityValue = (IList)refFieldInForeinEntity.GetValue(o);
                                if (foreinEntityValue != null && foreinEntityValue.Contains(rowObject) == false)
                                {
                                    foreinEntityValue.Add(rowObject);
                                }

                            }
                        }
                    }
                }
            }
            
        }

        private void UpdateRelationForDelete(object o)
        {
            
            Type t = o.GetType();
            PropertyInfo[] ps = t.GetProperties();
            foreach (var p in ps)
            {
                JOneToOne oneToOneAttr = AttrHelper.GetAttribute<JOneToOne>(p);
                JManyToOne manyToOneAttr = AttrHelper.GetAttribute<JManyToOne>(p);
                //one to one and many to one is necessary, so we can start from this entity 
                if (oneToOneAttr != null)
                {
                    Type foreinEntityType = p.PropertyType;
                    PropertyInfo refFieldInForeinEntity = ModelHelper.FindRefFieldInForeinEntity(t, foreinEntityType, typeof(JOneToOne), null);
                    if (refFieldInForeinEntity != null)//if reverse relation field present
                    {
                        IDs fds = GetDs(foreinEntityType);
                        object refValue = AttrHelper.GetPropertyValue(o, p);
                        if (refValue != null)
                        {
                            object foreinEntityValue = refFieldInForeinEntity.GetValue(refValue);
                            if (foreinEntityValue != null)
                            {
                                refFieldInForeinEntity.SetValue(refValue, null);
                                fds.SaveObject(refValue);
                            }
                        }
                    }
                }
                else if (manyToOneAttr != null)
                {

                    Type foreinEntityType = p.PropertyType;
                    PropertyInfo refFieldInForeinEntity = ModelHelper.FindRefFieldInForeinEntity(t, foreinEntityType, typeof(JOneToMany), manyToOneAttr.RefFieldNameInForeinEntity);
                    if (refFieldInForeinEntity != null)//if reverse relation field present
                    {
                        IDs fds = GetDs(foreinEntityType);
                        object refValue = AttrHelper.GetPropertyValue(o, p);
                        if (refValue != null)
                        {
                            IList foreinEntityValue = (IList)refFieldInForeinEntity.GetValue(refValue);
                            if (foreinEntityValue != null)
                            {
                                if (foreinEntityValue.Contains(o))
                                {
                                    foreinEntityValue.Remove(o);
                                    //fds.SaveObject(refValue);
                                }
                            }
                        }
                    }
                }
            }

            //find all many to many 
            foreach (JoinEntityInfo s in joinDatas)
            {
                if (s.DataType1.Equals(t) || s.DataType2.Equals(t))
                {
                    Type foreinEntityType = null;
                    if (s.DataType2.Equals(t))
                    {
                        foreinEntityType = s.DataType1;
                    }
                    else foreinEntityType = s.DataType2;

                    PropertyInfo refFieldInForeinEntity = ModelHelper.FindRefFieldInForeinEntity(t, foreinEntityType, typeof(JManyToMany), s.JoinTableName);

                    IDs jds = GetDs(s);
                    IList joins = jds.FindFromJoin(o, foreinEntityType, s.JoinTableName);
                    bool changed = false;
                    foreach (var joinO in joins)
                    {
                        jds.DeleteJoin(o, joinO, s.JoinTableName);
                        //remove from forein entity
                        if (refFieldInForeinEntity != null)//if reverse relation field present
                        {
                            IDs fds = GetDs(joinO.GetType());
                            object l = fds.Find(joinO.GetType(), ModelHelper.GetPKValue(joinO));
                            IList foreinEntityValue = (IList)refFieldInForeinEntity.GetValue(l);
                            if (foreinEntityValue.Contains(o))
                            {
                                foreinEntityValue.Remove(o);
                            }
                        }
                        changed = true;
                    }
                    if (changed)
                    {
                        //SetJoinEntityModified(s.DataType1, s.DataType2, s.JoinTableName);
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

            str.Append(ModelHelper.GetEntityJDescriptionOrName(o.GetType()) + ": " + ModelHelper.GetNameForObjectAdv(o));
            str.Append(br);
            str.Append(br);

            foreach (var rt in rels)
            {
                HashSet<object> refs = rt.Records;
                if (refs.Count > 0)
                {
                    str.Append(usingInStr + ModelHelper.GetEntityJDescriptionOrName(rt.ForeignEntity) + totalStr + refs.Count);
                    if (rt.PropertyInSource != null)
                    {
                        str.Append(br);
                        str.Append(thisField);
                        str.Append(": ");
                        str.Append(ModelHelper.GetPropertyJDescriptionOrName(rt.PropertyInSource));
                    }
                    if (rt.PropertyInForeign != null)
                    {
                        str.Append(br);
                        str.Append(refField);
                        str.Append(": ");
                        str.Append(ModelHelper.GetPropertyJDescriptionOrName(rt.PropertyInForeign));
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
        public string GetStaticticsReport(object o, string br = "\r\n")
        {
            List<RefEntityInfo> rels = GetAllReferencedToEntityLocal(o);

            string hr = "=============================================" + br;
            //string usingInStr = FrwUtilsRes.Dm_UsedInEntity + " ";
            //string totalStr = " " + FrwUtilsRes.Dm_Total + ": ";

            StringBuilder str = new StringBuilder();

            str.Append(ModelHelper.GetEntityJDescriptionOrName(o.GetType()) + ": " + ModelHelper.GetNameForObjectAdv(o));
            str.Append(br);
            str.Append(br);
            str.Append("Total records count: ");
            Type t = o.GetType();
            IList list = FindAll(t);
            str.Append(list.Count);

            str.Append(br);
            str.Append(hr);
            str.Append(br);
            str.Append("Fields full : ");
            str.Append(br);
            var props = t.GetProperties();
            foreach (var p in props)
            {
                JIgnore ignoreAttr = AttrHelper.GetAttribute<JIgnore>(t, p.Name);
                if (ignoreAttr != null) continue;
                string desc = ModelHelper.GetPropertyJDescriptionOrName(p);

                int vCount = 0;
                foreach (var lo in list)
                {
                    object lv = AttrHelper.GetPropertyValue(lo, p);
                    if (lv != null)
                    {
                        vCount++;
                    }
                }
                str.Append(br);
                str.Append(desc);
                str.Append(": ");
                str.Append(vCount);
                str.Append(" (");
                str.Append((int)(((double)vCount/(double)list.Count)*100.0));
                str.Append("%)");
            }
            return str.ToString();

        }


        private void InsertObject(object o)
        {
            if (o == null) return;
            Type t = o.GetType();
            IDs ds = GetDs(t);
            ds.InsertObject(o);

        }
        public void SaveAllEntitiesData(bool allways)
        {
            foreach (var ds in dsMap.Values)
            {
                ds.SaveAllEntitiesData(allways);
            }
        }
        public void SaveEntityData(Type t)
        {
            IDs ds = GetDs(t);
            ds.SaveEntityData(t);

            //save join data
            foreach (var p in t.GetProperties())
            {
                JManyToMany manyToManyAttr = AttrHelper.GetAttribute<JManyToMany>(p);
                if (manyToManyAttr != null)
                {
                    Type foreinEntityType = AttrHelper.GetGenericListArgType(p.PropertyType);
                    JoinEntityInfo join = GetJoinInfo(t, foreinEntityType, manyToManyAttr.JoinName);
                    IDs jds = GetDs(join);
                    if (jds.IsJoinModified(t, foreinEntityType, manyToManyAttr.JoinName))
                    {
                        jds.SaveJoinData(t, foreinEntityType, manyToManyAttr.JoinName);
                    }
                }
            }

        }
        public void SaveEntityDataToOtherLocation(IList list, Type type, string customDirPath)
        {
            IDs ds = GetDs(type);
            ds.SaveEntityDataToOtherLocation(list, type, customDirPath);
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
            IDs ds = GetDs(t);
            ds.UpdateObject(o);
        }
        public void SaveObject(object o)
        {
            Console.WriteLine("SaveObject " + o.GetType().Name + " " + ModelHelper.GetNameForObject(o));
            if (o == null) return;
            JValidationResult result = ValidateObject(o);
            if (result.isError) throw new JValidationException(result);
            Type t = o.GetType();
            IDs ds = GetDs(t);
            ds.SaveObject(o);
            UpdateRelations(o);
        }
        public void SaveObject(object o, string updatedPropertyName)
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
        public void RegisterCustomPathForType(Type type, string path)
        {
            IDs ds = GetDs(type);
            ds.RegisterCustomPathForType(type, path);
        }
        public void SetEntityModified<T>()
        {
            SetEntityModified(typeof(T));
        }
        public void SetEntityModified(Type t)
        {
            IDs ds = GetDs(t);
            ds.SetEntityModified(t);
        }
        public bool IsEntityModified<T>()
        {
            return IsEntityModified(typeof(T));
        }
        public bool IsEntityModified(Type t)
        {
            IDs ds = GetDs(t);
            return ds.IsEntityModified(t);
        }
        public bool IsJoinModified(Type t1, Type t2, string joinTableName)
        {
            JoinEntityInfo join = GetJoinInfo(t1, t2, joinTableName);
            IDs ds = GetDs(join);
            return ds.IsJoinModified(t1, t2, joinTableName);
        }
        public void SetJoinModified(Type t1, Type t2, string joinTableName)
        {
            JoinEntityInfo join = GetJoinInfo(t1, t2, joinTableName);
            IDs ds = GetDs(join);
            ds.SetJoinModified(t1, t2, joinTableName);
        }

        public IList FindAll(Type t)
        {
            IDs ds = GetDs(t);
            return ds.FindAll(t);
        }

        public IList FindRootList(Type t)
        {
            PropertyInfo p = ModelHelper.GetSelfPropertiesForEntity(t).FirstOrDefault();
            List<object> rootList = new List<object>();

            IDs ds = GetDs(t);
            return ds.FindBy(t, p, null);
        }
        public IList FindBy(Type t, PropertyInfo p, object valueOrObjectToFind)
        {
            IDs ds = GetDs(t);
            return ds.FindBy(t, p, null);
        }
        public IList FindBy(Type t, string propertyName, object valueOrObjectToFind)
        {
            IDs ds = GetDs(t);
            PropertyInfo p = AttrHelper.GetProperty(t, propertyName);
            if (p == null) throw new InvalidDataException("Wrong property name (" + propertyName + ") for type " + t);
            return ds.FindBy(t, p, null);
        }
        public IList<T> FindBy<T>(string propertyName, object valueOrObjectToFind)
        {
            Type t = typeof(T);
            return (IList<T>)FindBy(t, propertyName, valueOrObjectToFind);
        }
        public IList<T> FindBy<T>(PropertyInfo p, object valueOrObjectToFind)
        {
            Type t = typeof(T);
            return (IList<T>)FindBy(t, p, valueOrObjectToFind);
        }
        public IList<T> FindAll<T>()
        {
            Type t = typeof(T);
            return (IList<T>)FindAll(t);
        }
 

        public IEnumerable FindByParams(Type t, IDictionary<string, object> pars)
        {
            IDs ds = GetDs(t);
            return ds.FindByParams(t, pars);
        }
        public IList<T> FindByParams<T>(IDictionary<string, object> pars)
        {
            Type t = typeof(T);
            return (IList<T>)FindByParams(t, pars);
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

        static public object CloneObject(object o, CloneObjectType cloneType)
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
        static public object CloneObjectToOtherType(object o, Type destType)
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

        static public void CopyObjectProperties(object o, object destObject, CopyRestrictLevel cloneLevel)
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

   

        public string GetCommonStoragePath()
        {
            return Path.Combine(FrwConfig.Instance.ProfileDir, DATA_STORAGE);
        }
        public string GetCommonCachePath()
        {
            return Path.Combine(FrwConfig.Instance.ProfileDir, DATA_CACHE);
        }
        public string GetCommonCachePathUniqueForCompAndUser()
        {
            return Path.Combine(GetCommonCachePath(), BaseNetworkUtils.GetCompAndUserUniqueId());
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
        public string GetCacheFullPathForObjectUniqueForCompAndUser(object o)
        {
            return Path.Combine(GetCommonCachePathUniqueForCompAndUser(), GetStoragePrefixForObject(o));
        }
        
        public string GetBrowserCommonCachePathUniqueForCompAndUser()
        {
            return Path.Combine(Dm.Instance.GetCommonCachePathUniqueForCompAndUser(), BROWSER_CACHE_PATH);
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
