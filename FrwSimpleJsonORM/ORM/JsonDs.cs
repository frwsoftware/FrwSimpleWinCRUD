using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FrwSoftware.Properties;

namespace FrwSoftware
{

    public class SData
    {
        public Dictionary<object, object> PkCache { get; } = new Dictionary<object, object>();
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


    public class JsonDs : IDs
    {
        protected const string DATA_STORAGE = "dataStorage";//Data warehouse prefix in the profile

        //in memory caches 
        private List<SData> sdatas = null;//Cache for entities
        private List<JoinEntityData> joinDatas = null;//Cache for crosstables
        private Dictionary<Type, string> customPathsForTypes = new Dictionary<Type, string>();
        public bool IsResolvedOnInit() { return true; }
        public bool IsCashed() { return true; }
        public void Init()
        {
            sdatas = new List<SData>();//Cache for entities
            joinDatas = new List<JoinEntityData>();//Cache for crosstables

        }
        public void RegisterCustomPathForType(Type type, string path)
        {
            customPathsForTypes.Add(type, path);
        }
        public void Destroy()
        {
            SaveAllEntitiesData(false);
        }
        public object EmptyObject(Type t, IDictionary<string, object> pars)
        {
            return null;
        }
        public void PostLoadEntity(Type t)
        {

        }

        public void GenNextPkValue(object o)
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
                            int curValue = int.Parse(pkValue.ToString());
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

        }
        public void SaveObject(object o)
        {
            Type t = o.GetType();
            IList list = GetJsonList(t);
            if (list.Contains(o) == false) InsertObject(o);
            else UpdateObject(o);
        }
        public void InsertObject(object o)
        {
            Type t = o.GetType();
            IList list = FindAll(t);
            if (list.Contains(o) == false)
            {
                list.Add(o);
                AddObjectToPKCache(o);
            }
            SetEntityModified(t);
        }
        public void UpdateObject(object o)
        {
            Type t = o.GetType();
            //do nothing
            SetEntityModified(t);
        }
        public void CreateTable(Type entityType)
        {
            //do nothig
        }
        public void DropTable(Type entityType)
        {
            //todo
        }

        public void SetEntityModified(Type t)
        {
            SData sdata = GetSData(t);
            if (sdata != null)
                sdata.Modified = true;
        }
        public bool IsEntityModified(Type t)
        {
            SData sdata = GetSData(t);
            if (sdata != null)
                return sdata.Modified;
            else return false;
        }

        //for testing purpose only
        public bool IsJoinModified(Type t1, Type t2, string joinTableName)
        {
            JoinEntityData sdata = GetJoinData(t1, t2, joinTableName);
            if (sdata != null)
                return sdata.Modified;
            else return false;
        }

        public void SetJoinModified(Type t1, Type t2, string joinTableName)
        {
            JoinEntityData sdata = GetJoinData(t1, t2, joinTableName);
            if (sdata != null)
                sdata.Modified = true;
        }
        public void DeleteObject(object o)
        {
            Type t = o.GetType();
            IList list = GetJsonList(t);
            list.Remove(o);
            RemoveObjectFromPKCache(o);
            SetEntityModified(t);

        }
        public void DeleteAllObjects(Type t)
        {
            IList list = GetJsonList(t);
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

        public object Find(Type entityType, object primaryKeValue)
        {
            if (primaryKeValue == null) return null;
            if (AttrHelper.IsSimple(primaryKeValue.GetType()) == false)
            {
                if (primaryKeValue.GetType() != entityType) throw new InvalidDataException("Find by pk whitch is not simple and not same type");
                primaryKeValue = ModelHelper.GetPKValue(primaryKeValue);
                if (primaryKeValue == null) throw new InvalidDataException("Find by pk whitch is not simple and not have pk");
            }
            object o = null;
            PropertyInfo pPK = AttrHelper.GetProperty<JPrimaryKey>(entityType);
            SData sdata = GetSData(entityType);
            sdata.PkCache.TryGetValue(primaryKeValue, out o);
            return o;
        }
        public object FindByNum(Type entityType, int num)
        {
            IList list = GetJsonList(entityType);
            if (num >= 0 && num < list.Count)
                return list[num];
            else return null;
        }
        public IList FindAll(Type t)
        {
            return GetJsonList(t);
        }
        public int CountAll(Type t)
        {
            return GetJsonList(t).Count;
        }
        public IList FindBy(Type entityType, PropertyInfo p, object fieldValue)
        {
            IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(entityType));
            bool isSimple = (fieldValue == null) || AttrHelper.IsSimple(fieldValue.GetType());
            IList allMayBeReferenced = FindAll(entityType);
            foreach (object l in allMayBeReferenced)
            {
                object foreinEntityValue = p.GetValue(l);

                if (isSimple)
                {
                    if (foreinEntityValue != null)
                    {
                        //pk comparation
                        if (ModelHelper.GetPKValue(foreinEntityValue).Equals(fieldValue))
                        {
                            if (list.Contains(l) == false) list.Add(l);
                        }
                    }
                    else if (fieldValue == null)
                    {
                        if (list.Contains(l) == false) list.Add(l);
                    }
                }
                else
                {
                    //object comparation (working fine only if all records cached on init)
                    if (foreinEntityValue == fieldValue)//also null == null
                    {
                        if (list.Contains(l) == false) list.Add(l);
                    }
                }
            }
            return list;
        }

        public IList FindByParams(Type t, IDictionary<string, object> pars)
        {
            Type listType = typeof(List<>).MakeGenericType(t);
            IList values = (IList)Activator.CreateInstance(listType);
            var list = FindAll(t);
            foreach (var l in list)
            {
                bool eu = false;
                foreach (var par in pars)
                {
                    PropertyInfo p = t.GetProperty(par.Key);
                    if (p != null)
                    {
                        object v = p.GetValue(l);
                        if ((par.Value == null && v == null)
                            || (par.Value != null && par.Value.Equals(v)))
                        {
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


        ///////////////////////////
        ///  /// <summary>
        /// Save all data to file system
        /// </summary>
        public void SaveAllEntitiesData(bool allways)
        {
            foreach (var s in sdatas)
            {
                Type entityType = s.DataType;
                JEntity entityAttr = AttrHelper.GetClassAttribute<JEntity>(entityType);
                if (s.Modified || allways)
                {
                    SaveEntityData(s);
                }
            }
            foreach (var s in joinDatas)
            {
                if (s.Modified || allways)
                {
                    SaveJoinEntityDataLocal(s);
                }
            }
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
        private JoinEntityData GetJoinData(Type t1, Type t2, string crossTableName)
        {
            TypeComparer typeComparer = new TypeComparer();
            Type[] ts = new Type[] { t1, t2 };
            Array.Sort(ts, typeComparer);

            JoinEntityData sdata = null;
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
        private IList GetJsonList(Type t)
        {
            SData sdata = GetSData(t);
            return (IList)sdata.DataList;
        }
        private SData LoadEntityFromDisk(Type t)
        {
            JEntity entityAttr = AttrHelper.GetClassAttribute<JEntity>(t);
            if (entityAttr.DsType != typeof(JsonDs))
            {
                throw new InvalidOperationException("Not a json file stored entity");
            }

            //long tstart = DateTime.Now.Ticks;
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
                    catch (ArgumentException ex)
                    {
                        Log.ProcessDebug("Dublicate pk entity: " + t + " pk value:  " + pk + " Error text: " + ex);
                    }
                }
            }
            sdata.DataList = list;
            sdatas.Add(sdata);
          
            return sdata;
        }
 
        private void Notifier_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SData sdata = GetSData(sender.GetType());
            if (sdata != null)
            {
                sdata.Modified = true;
            }
        }
        protected string GetDataFilePathForType(Type dataType, string customDirPath = null)
        {
            if (customDirPath == null) customPathsForTypes.TryGetValue(dataType, out customDirPath);

            string dirPath = (customDirPath != null) ? customDirPath : Path.Combine(FrwConfig.Instance.ProfileDir, DATA_STORAGE);
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (dir.Exists == false)
            {
                Directory.CreateDirectory(dir.FullName);
            }
            string filename = Path.Combine(dirPath, dataType.FullName + ".json");
            return filename;
        }
        public string GetDataStorageDirPath()
        {
            return Path.Combine(FrwConfig.Instance.ProfileDir, DATA_STORAGE);
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
        public IList InitiallyLoadData(Type t)
        {
            SData sdata = LoadEntityFromDisk(t);
            return (IList)sdata.DataList;
        }
        public void SaveEntityData(Type type)
        {
            SData s = GetSData(type);
            if (s != null)
            {
                SaveEntityData(s);
            }
        }
        private void SaveEntityDataLocal(IList list, Type type, string customDirPath)
        {
            string filename = GetDataFilePathForType(type, customDirPath);
            var lt = typeof(List<>);
            var listType = lt.MakeGenericType(type);
            IList alist = (IList)Activator.CreateInstance(listType);
            foreach (object v in list)
            {
                object av = Dm.CloneObject(v, CloneObjectType.ForSave);
                alist.Add(av);
            }
            //
            if (alist.Count > 5000)//todo better check size
                JsonSerializeHelper.SaveToFileLageData(alist, filename);
            else
                JsonSerializeHelper.SaveToFile(alist, filename);
        }

        private void SaveEntityData(SData s)
        {
            object list = s.DataList;
            SaveEntityDataLocal((IList)list, s.DataType, null);
            s.Modified = false;
        }


        public IList FindFromJoin(object rowObject, Type foreinEntityType, string joinName = null)
        {
            IList values = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(foreinEntityType));
            if (rowObject == null) throw new InvalidDataException(" FindFromJoin: NULL object to find"); // return values;
            Type sourceEntityType = rowObject.GetType();

            PropertyInfo pkProp = AttrHelper.GetProperty<JPrimaryKey>(sourceEntityType);
            JoinEntityData cross = FindAllJoinData(sourceEntityType, foreinEntityType, joinName);
            bool reverse = false;
            if (cross.DataType1 != sourceEntityType) reverse = true;
            List<object> cValues = new List<object>();
            object sourcePKValue = AttrHelper.IsSimple(rowObject.GetType()) ? rowObject :  pkProp.GetValue(rowObject);
            if (sourcePKValue == null) throw new InvalidDataException(" FindFromJoin: wrong object to find");
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
                object foreinEntityValue = Find(foreinEntityType, foreinKeyValue);
                if (foreinEntityValue != null)
                {
                    values.Add(foreinEntityValue);
                }
                else
                {
                    //todo
                }
            }
            return values;

        }


        protected JoinEntityData FindAllJoinData(Type t1, Type t2, string joinTableName)
        {
            JoinEntityData joinData = GetJoinData(t1, t2, joinTableName);
            if (joinData == null)
            {
                long tstart = DateTime.Now.Ticks;
                
                //sort before add 
                TypeComparer typeComparer = new TypeComparer();
                Type[] ts = new Type[] { t1, t2 };
                Array.Sort(ts, typeComparer);

                joinData = new JoinEntityData() { DataType1 = ts[0], DataType2 = ts[1], JoinTableName = joinTableName };

                var listType = typeof(List<JoinEntityDataItem>);
                List<JoinEntityDataItem> list = null;
                string filename = Path.Combine(Path.Combine(FrwConfig.Instance.ProfileDir, DATA_STORAGE), joinData.DataType1.FullName + "_" + joinData.DataType2.FullName +
                    (joinTableName != null ? ("_" + joinTableName) : "") + ".json");
                FileInfo fileInfo = new FileInfo(filename);
                if (fileInfo.Exists)
                {
                    list = JsonSerializeHelper.LoadForType(filename, listType);
                }
                else
                {
                    list = (List<JoinEntityDataItem>)Activator.CreateInstance(listType);
                }
                joinData.DataList = list;

                long tstartConvert = DateTime.Now.Ticks;

                PropertyInfo pkProp1 = AttrHelper.GetProperty<JPrimaryKey>(joinData.DataType1);

                if (pkProp1.PropertyType != typeof(string))
                {
                    foreach (var l in joinData.DataList)
                    {
                        l.Pk1 = JsonSerializeHelper.DeserializeString(l.Pk1.ToString(), pkProp1.PropertyType);
                    }
                }
                PropertyInfo pkProp2 = AttrHelper.GetProperty<JPrimaryKey>(joinData.DataType1);
                if (pkProp2.PropertyType != typeof(string))
                {
                    foreach (var l in joinData.DataList)
                    {
                        l.Pk2 = JsonSerializeHelper.DeserializeString(l.Pk2.ToString(), pkProp2.PropertyType);
                    }
                }
                joinDatas.Add(joinData);
                long tend = DateTime.Now.Ticks;
                //Log.ProcessDebug("======= Loaded join data : " + sdata.DataType1 + " " + sdata.DataType2  + " Count: " + ((IList)list).Count + " Time: " + (tend - tstart) / 10000 + " mils" + " include time converting: " + (tend - tstartConvert) / 10000 + " mils");
            }
            return joinData;
        }
        public void SaveJoinData(Type t1, Type t2, string joinTablename = null)
        {
            JoinEntityData s = GetJoinData(t1, t2, joinTablename);
            SaveJoinEntityDataLocal(s);
        }

        private void SaveJoinEntityDataLocal(JoinEntityData s)
        {
            string dirPath = Path.Combine(FrwConfig.Instance.ProfileDir, DATA_STORAGE);
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (dir.Exists == false)
            {
                Directory.CreateDirectory(dir.FullName);
            }
            string filename = Path.Combine(dirPath, s.DataType1.FullName + "_" + s.DataType2.FullName + ".json");
            object list = s.DataList;
            JsonSerializeHelper.SaveToFile(list, filename);
            s.Modified = false;
        }

        public void AddJoin(object o1, object o2, string joinTableName = null)
        {
            object pk1Value = ModelHelper.GetPKValue(o1);
            object pk2Value = ModelHelper.GetPKValue(o2);
            JoinEntityData join = FindAllJoinData(o1.GetType(), o2.GetType(), joinTableName);
            bool reverse = false;
            if (join.DataType1 != o1.GetType()) reverse = true;
            bool found = false;
            foreach (var c in join.DataList)
            {
                if ((!reverse && c.Pk1.Equals(pk1Value) && c.Pk2.Equals(pk2Value))
                    ||
                    (reverse && c.Pk2.Equals(pk1Value) && c.Pk1.Equals(pk2Value)))
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                JoinEntityDataItem joinObject = new JoinEntityDataItem();
                if (reverse)
                {
                    joinObject.Pk2 = pk1Value;
                    joinObject.Pk1 = pk2Value;
                }
                else
                {
                    joinObject.Pk1 = pk1Value;
                    joinObject.Pk2 = pk2Value;
                }
                join.DataList.Add(joinObject);
                SetJoinModified(o1.GetType(), o2.GetType(), joinTableName);
            }
        }
        public void DeleteJoin(object o1, object o2, string joinTableName = null)
        {
            object pk1Value = ModelHelper.GetPKValue(o1);
            object pk2Value = ModelHelper.GetPKValue(o2);
            JoinEntityData join = FindAllJoinData(o1.GetType(), o2.GetType(), joinTableName);
            bool reverse = false;
            if (join.DataType1 != o1.GetType()) reverse = true;
            foreach (var c in join.DataList)
            {
                if ((!reverse &&  c.Pk1.Equals(pk1Value) && c.Pk2.Equals(pk2Value))
                    ||
                    (reverse && c.Pk2.Equals(pk1Value) && c.Pk1.Equals(pk2Value)))
                {
                    join.DataList.Remove(c);
                    SetJoinModified(o1.GetType(), o2.GetType(), joinTableName);
                    break;
                }
            }
        }

        public void SaveEntityDataToOtherLocation(IList list, Type type, string customDirPath)
        {
            SaveEntityDataLocal(list, type, customDirPath);
        }

   
        public void UpdateFieldWithOneToManyRelation(object rowObject, object newObject, Type foreinEntityType, PropertyInfo refFieldInForeinEntity)
        {
            IList allMayBeReferenced = GetJsonList(foreinEntityType);
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
}
