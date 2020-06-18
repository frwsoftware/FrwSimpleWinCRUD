using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FrwSoftware
{
    public class SQLiteDs : DbBase, IDs
    {
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        private const string DateTimeOffsetFormat = "yyyy-MM-dd HH:mm:ss.fffffffzzz";
        private SQLiteConnectionPool pool = new SQLiteConnectionPool();
        bool storeDateTimeAsTicks = false;//todo

        bool useCache = true;
        public ConcurrentDictionary<object, object> PkCache { get; } = new ConcurrentDictionary<object, object>();
        public ConcurrentDictionary<object, object> notFoundPkCache { get; } = new ConcurrentDictionary<object, object>();


        public void Init()
        {

        }
        public void Destroy()
        {

        }
        private Dictionary<Type, string> customPathsForTypes = new Dictionary<Type, string>();

        public bool IsResolvedOnInit() { return false; }
        public bool IsCashed() { return false; }

        public void GenNextPkValue(object o)
        {
            //todo
        }
        public object EmptyObject(Type t, IDictionary<string, object> pars)
        {
            return null;
        }
        public void PostLoadEntity(Type t)
        {

        }
        public void SetEntityModified(Type t)
        {
        }
        public void SaveJoinData(Type t1, Type t2, string joinTablename = null)
        {
        }

        public bool IsEntityModified(Type t) { 
             return false;
        }
        public bool IsJoinModified(Type t1, Type t2, string joinTableName)
        {
            return false;
        }
        public void SetJoinModified(Type t1, Type t2, string joinTableName)
        {
        }

        public void SaveObject(object o)
        {
            if (o == null) return;
            Type entityType = o.GetType();
            SQLiteConnection conn = OpenConnection(entityType);

            object existingObject = null;
            object pkValue = ModelHelper.GetPKValue(o);
            if (pkValue != null) existingObject = Find(o.GetType(), pkValue);
            if (existingObject == null) InsertObject(o);
            else UpdateObject(o);
            CloseConnection(conn);
        }
         public void InsertObject(object o)
        {
            WriteRow(o, GetInsertStatement(o), null, null);
          
        }
        public IList InitiallyLoadData(Type t)
        {
            return null;
        }
        public void SaveAllEntitiesData(bool allways)
        {
        }
        public void SaveEntityData(Type type)
        {
        }
            public void SaveEntityDataToOtherLocation(IList list, Type type, string customDirPath)
            { }
        public IList FindFromJoin(object rowObject, Type foreinEntityType, string joinName = null)
        {
            IList values = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(foreinEntityType));
            return values;
        }

        public void AddJoin(object o1, object o2, string joinTableName = null)
        {
            /*
            object pk1Value = ModelHelper.GetPKValue(o1);
            object pk2Value = ModelHelper.GetPKValue(o2);
            JoinEntityData join = FindAllJoinData(o1.GetType(), o2.GetType(), joinTableName);

            JoinEntityDataItem joinObject = new JoinEntityDataItem();
            joinObject.Pk1 = pk1Value;
            joinObject.Pk2 = pk2Value;
            bool found = false;
            foreach (var c in join.DataList)
            {
                if (c.Pk1.Equals(joinObject.Pk1) && c.Pk2.Equals(joinObject.Pk2))
                {
                    found = true;
                    break;
                }
            }
            if (!found) join.DataList.Add(joinObject);
            */
        }
        public void DeleteJoin(object o1, object o2, string joinTableName = null)
        {
            /*
            object pk1Value = ModelHelper.GetPKValue(o1);
            object pk2Value = ModelHelper.GetPKValue(o2);
            JoinEntityData join = FindAllJoinData(o1.GetType(), o2.GetType(), joinTableName);
            foreach (var c in join.DataList)
            {
                if (c.Pk1.Equals(pk1Value) && c.Pk2.Equals(pk2Value))
                {
                    join.DataList.Remove(c);
                    break;
                }
            }
            */
        }

        public void UpdateFieldWithOneToManyRelation(object rowObject, object newObject, Type foreinEntityType, PropertyInfo refFieldInForeinEntity)
        {

        }
            //CREATE INDEX Parent_index ON JGeoWikidata ( Parent );
            //SQLite doesn't support the ADD CONSTRAINT
            /*
             * 
             * 
             CREATE TABLE child ( 
                id           INTEGER PRIMARY KEY, 
             parent_id    INTEGER, 
             description  TEXT,
                FOREIGN KEY (parent_id) REFERENCES parent(id)
            );
            */

        public SQLiteTransaction BeginTransaction(Type t)
        {
            //https://stackoverflow.com/questions/36778304/how-to-bulk-insert-into-sqlite-database 
            // For bulk insert into SQLITE database
            SQLiteConnection conn = OpenConnection(t);
            SQLiteTransaction tr = conn.BeginTransaction();
            return tr;
        }
         public void Commit(SQLiteTransaction tr)
        {
            tr.Commit();
 
        }
         private void WriteRow(object o, string sql, object pkValue, SQLiteConnection conn)
        {
            Type t = o.GetType();
            bool connOpenedHere = false;
            if (conn == null)
            {
                conn = OpenConnection(t);
                connOpenedHere = true;
            }
            var command = new SQLiteCommand(sql, conn);

            var props = t.GetProperties();
            foreach (var p in props)
            {

                if (IsIgnore(p)) continue;
                if (IsShortTable(t) == true && IsNecessary(p) == false) continue;
                if (IsNotStored(p)) continue;
                object value = AttrHelper.GetPropertyValue(o, p);
                if (value != null)
                {
                    if (IsComplex(p))
                    {
                        byte[] json = Encoding.UTF8.GetBytes(JsonSerializeHelper.SaveToString(value));
                        command.Parameters.Add("@" + GetColumnName(p), DbType.Binary, json.Length).Value = json;
                    }

                    else
                    {
                        if (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?))
                        {
                            if (storeDateTimeAsTicks)
                                value = ((DateTime)value).Ticks;
                            else
                                value = ((DateTime)value).ToString(DateTimeFormat);
                        }
                        else if (p.PropertyType == typeof(DateTimeOffset) || p.PropertyType == typeof(DateTimeOffset?))
                        {
                            if (storeDateTimeAsTicks)
                                value = ((DateTimeOffset)value).Ticks;
                            else
                                value = ((DateTimeOffset)value).ToString(DateTimeOffsetFormat);
                        }
                        else if (IsForeignKey(p))
                        {
                            object fk = AttrHelper.GetPropertyValue(o, p);
                            if (fk != null)
                            {
                                value = ModelHelper.GetPKValue(fk);
                            }
                        }

                        DbType dbType = GetDbType(p);
                        command.Parameters.Add("@" + GetColumnName(p), dbType).Value = value;

                    }
                }
                else command.Parameters.AddWithValue("@" + GetColumnName(p), DBNull.Value);

            }
            if (IsShortTable(t))
            {
                byte[] json = Encoding.UTF8.GetBytes(JsonSerializeHelper.SaveToString(o));
                command.Parameters.Add("@" + JSON_DATA_BLOB, DbType.Binary, json.Length).Value = json;
            }
            if (pkValue != null)
            {
                command.Parameters.Add("@" + GetColumnName(ModelHelper.GetPK(t)), DbType.String).Value = pkValue;
            }

            /*
            MemoryStream ms = new MemoryStream();
            ptbLogo.Image.Save(ms, ImageFormat.Jpeg);
            byte[] bytBLOB = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(bytBLOB, 0, System.Convert.ToInt32(ms.Length));
            SQLiteParameter prm = new SQLiteParameter("@Logo", DbType.Binary, bytBLOB.Length, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, bytBLOB);
            cmSQL.Parameters.Add(prm);
            */
            command.ExecuteNonQuery();
            if (pkValue == null)
            {
                //todo get pk value if autoincrement 
            }
            if (connOpenedHere) CloseConnection(conn);
            if (useCache) putCache(o.GetType(), ModelHelper.GetPKValue(o), o);
        }
         public void UpdateObject(object o)
        {
            WriteRow(o, GetUpdateStatement(o), ModelHelper.GetPKValue(o), null);
            
        }
        public void CreateTable(Type entityType)
        {
            SQLiteConnection conn = OpenConnection(entityType);
            var command = new SQLiteCommand(GetCreateTableStatement(entityType), conn);
            command.ExecuteNonQuery();
            CloseConnection(conn);
        }
         public void DropTable(Type entityType)
        {
            SQLiteConnection conn = OpenConnection(entityType);
            var command = new SQLiteCommand(GetDropTableStatement(entityType), conn);
            command.ExecuteNonQuery();
            CloseConnection(conn);
        }

         public void DeleteObject(object o)
        {
            if (o == null) return;//!!!
            Type entityType = o.GetType();
            SQLiteConnection conn = OpenConnection(entityType);
            var command = new SQLiteCommand(GetDeleteStatement(o), conn);
            if (o != null)
            {
                command.Parameters.Add("@" +  GetColumnName(ModelHelper.GetPK(entityType)), DbType.String).Value = ModelHelper.GetPKValue(o);
            }
            command.ExecuteNonQuery();
            CloseConnection(conn);
            if (useCache) removeFromCache(entityType, ModelHelper.GetPKValue(o));
        }
         public void DeleteAllObjects(Type entityType)
        {
            SQLiteConnection conn = OpenConnection(entityType);
            var command = new SQLiteCommand(GetDeleteStatement(null), conn);
            command.ExecuteNonQuery();
            CloseConnection(conn);
            if (useCache)  removeAllFromCache(entityType);
        }

        // public object EmptyObject(Type t, IDictionary<string, object> pars = null)
        //{

        //}

         public T Find<T>(object primaryKeValue, SQLiteConnection conn = null)
        {
            return (T)Find(typeof(T), primaryKeValue);
        }

        private string getCacheKey(Type entityType, object primaryKeValue)
        {
            return entityType.FullName + "_" + primaryKeValue;
        }
        private bool tryGetCache(Type entityType, object primaryKeValue, out object o)
        {
            return PkCache.TryGetValue(getCacheKey(entityType, primaryKeValue), out o);
        }
        private void putCache(Type entityType, object primaryKeValue, object o)
        {
            //object primaryKeValue = ModelHelper.GetPKValue(o);
            PkCache.TryAdd(getCacheKey(entityType, primaryKeValue), o);
        }
        private void removeFromCache(Type entityType, object primaryKeValue)
        {
            object o;
            PkCache.TryRemove(getCacheKey(entityType, primaryKeValue), out o);
        }
        private void removeAllFromCache(Type entityType)
        {
            //object o;
            //PkCache.Clear();//todo clear only 
            List<object> keysToRemove = new List<object>();
            foreach(var k in PkCache.Keys)
            {
                if ((k as string).StartsWith(entityType.FullName))
                    keysToRemove.Add(k);
            }
            foreach(var k in keysToRemove)
            {
                object o;
                PkCache.TryRemove(k, out o);
            }
        }

        public object Find(Type entityType, object primaryKeValue)
        {

            //transfom pk
            if (AttrHelper.IsSimple(primaryKeValue.GetType()) == false)
            {
                if (primaryKeValue.GetType() != entityType) throw new InvalidDataException("Find by pk whitch is not simple and not same type");
                primaryKeValue = ModelHelper.GetPKValue(primaryKeValue);
                if (primaryKeValue == null) throw new InvalidDataException("Find by pk whitch is not simple and not have pk");
            }

            //cache    
            object o = null;
            if (useCache)
            {
                bool res = tryGetCache(entityType, primaryKeValue, out o);
                if (res) return o;
            }
            //Console.WriteLine("*********** Find " + entityType + " pk: " + primaryKeValue);
            //find in db 
            SQLiteConnection conn = OpenConnection(entityType);
            string sql = GetBaseSelectStatement(entityType) + " " + GetWherePKForSelectStatement(entityType, primaryKeValue);
            IList list = ReadRows(entityType, sql, primaryKeValue, conn);
            if (list.Count > 1) throw new InvalidDataException("Find by PK - found more than one records");
            if (list.Count > 0) o = list[0];
            CloseConnection(conn);
            if (o == null && useCache) putCache(entityType, primaryKeValue, o);
            return o;
        }
         public object FindByNum(Type entityType, int num)
        {
            //Console.WriteLine("*********** Find " + entityType + " Num: " + num);
                SQLiteConnection conn = OpenConnection(entityType);
            object o = null;
            string sql = GetBaseSelectStatement(entityType) + " " + GetLIMITForSelectStatement(entityType, 1, num);
            IList list = ReadRows(entityType, sql, null, conn);
            if (list.Count > 1) throw new InvalidDataException("Find by Num - found more than one records");
            if (list.Count > 0) o = list[0];
            CloseConnection(conn);
            return o;
        }
         public IList<T> FindAll<T>(SQLiteConnection conn = null)
        {
            Type t = typeof(T);
            return (IList<T>)FindAll(t);
        }
        public void RegisterCustomPathForType(Type type, string path)
        {
            customPathsForTypes.Add(type, path);
        }

        public SQLiteConnection OpenConnection(Type entityType)
        {
            //todo pool 
            JEntity entityAttr = AttrHelper.GetClassAttribute<JEntity>(entityType);
            string customDirPath;
            customPathsForTypes.TryGetValue(entityType, out customDirPath);
            string datatbaseDir = customDirPath != null ? customDirPath : (FrwConfig.Instance.ProfileDir != null ? FrwConfig.Instance.ProfileDir : null);

            string path =  entityAttr.Path;
            if (!Path.IsPathRooted(path) && datatbaseDir != null) path = Path.Combine(datatbaseDir, path);
            if (File.Exists(path) == false)
            {
                FileUtils.CreateDirectoryForFileName(path);
                SQLiteConnection.CreateFile(path);
            }
            string connString = "Data Source=" + path + ";Version=3;";
            //SQLiteConnection m_dbConnection = new SQLiteConnection(connString);
            //m_dbConnection.Open();
            //return m_dbConnection;

            return pool.GetConnection(connString).SQLiteConnection;

        }
         public void CloseConnection(SQLiteConnection conn)
        {
            //conn.Close();
        }

         public IList FindAll(Type entityType)
        {
            Console.WriteLine("*********** FindAll " + entityType);
                SQLiteConnection conn = OpenConnection(entityType);
            string sql = GetBaseSelectStatement(entityType);
            IList list = ReadRows(entityType, sql, null, conn);
            CloseConnection(conn);

            return list;
        }
         public int CountAll(Type entityType)
        {
            Console.WriteLine("*********** CountAll " + entityType);
                SQLiteConnection conn = OpenConnection(entityType);
            string sql = GetCountSelectStatement(entityType);
            int count = ReadCount(entityType, sql, conn);
            CloseConnection(conn);

            return count;
        }
         public IList FindBy(Type entityType, PropertyInfo p, object fieldValue)
        {
            Console.WriteLine("*********** FindBy " + entityType);
            bool isSimple = (fieldValue == null) || AttrHelper.IsSimple(fieldValue.GetType());
            if (fieldValue != null && isSimple == false)
            {
                fieldValue = ModelHelper.GetPKValue(fieldValue);
                if (fieldValue == null) throw new InvalidDataException("FindBy for not entity complex type: " + fieldValue.GetType());
            }

            SQLiteConnection conn = OpenConnection(entityType);
            string sql = GetBaseSelectStatement(entityType) + " " + GetWhereColumnForSelectStatement(entityType, p, fieldValue);
            IList list = ReadRows(entityType, sql, null, null);
             CloseConnection(conn);

            return list;
        }
        public IList FindByParams(Type entityType, IDictionary<string, object> pars)
        {
            //Console.WriteLine("*********** FindByParams " + entityType);

            Dictionary<string, object> pars2 = new Dictionary<string, object>();
            foreach (var p in pars)
            {
                object fieldValue = p.Value;
                bool isSimple = (fieldValue == null) || AttrHelper.IsSimple(fieldValue.GetType());
                if (fieldValue != null && isSimple == false)
                {
                    fieldValue = ModelHelper.GetPKValue(fieldValue);
                    if (fieldValue == null) throw new InvalidDataException("FindBy for not entity complex type: " + fieldValue.GetType());
                }
                pars2.Add(p.Key, p.Value);
            }

            SQLiteConnection conn = OpenConnection(entityType);
            string sql = GetBaseSelectStatement(entityType) + " " + GetWhereColumnsForSelectStatement(entityType, pars2);
            IList list = ReadRows(entityType, sql, null, null);
            CloseConnection(conn);

            return list;
        }

        private IList ReadRows(Type entityType, string sql, object primaryKeValue, SQLiteConnection conn = null)
        {
            IList list = AttrHelper.CreateListEmpty(entityType);

            bool connOpenedHere = false;
            if (conn == null)
            {
                conn = OpenConnection(entityType);
                connOpenedHere = true;
            }
            var command = new SQLiteCommand(sql, conn);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    object o = null;
                    if (useCache)
                    {
                        PropertyInfo pkP = ModelHelper.GetPK(entityType);
                        object pkO = reader.GetValue(reader.GetOrdinal(GetColumnName(pkP)));
                        if (pkO != null)
                        {
                            tryGetCache(entityType, pkO, out o);
                        }
                    }
                    if (o == null)
                    {
                        //int i = 0;
                        var props = entityType.GetProperties();
                        o = Activator.CreateInstance(entityType);
                        foreach (var p in props)
                        {
                            int i = reader.GetOrdinal(GetColumnName(p));//we need to get 'i' directly due to some bug when accesing bd first time in parallet thread 
                            string strvalue = null;
                            try
                            {
                                if (IsIgnore(p)) continue;
                                if (IsShortTable(entityType) == true && IsNecessary(p) == false) continue;
                                if (IsNotStored(p)) continue;
                                if (reader.IsDBNull(i) == false)
                                {
                                    object value = null;
                                    if (IsComplex(p))
                                    {
                                        byte[] buffer = GetBytes(reader, i);
                                        strvalue = Encoding.UTF8.GetString(buffer);
                                        value = JsonSerializeHelper.LoadFromString(strvalue, p.PropertyType);
                                    }
                                    else
                                    {
                                        if (p.PropertyType == typeof(bool))
                                        {
                                            value = (reader.GetInt32(i)) > 0 ? true : false;
                                        }
                                        else if (p.PropertyType == typeof(string))
                                        {
                                            value = reader.GetString(i);
                                        }
                                        else if (p.PropertyType == typeof(byte) || p.PropertyType == typeof(ushort) ||
                                            p.PropertyType == typeof(sbyte) || p.PropertyType == typeof(short) || p.PropertyType == typeof(int) ||
                                            p.PropertyType == typeof(uint))
                                        {
                                            value = reader.GetInt32(i);
                                        }
                                        else if (p.PropertyType == typeof(long))
                                        {
                                            value = reader.GetInt64(i);
                                        }
                                        else if (p.PropertyType == typeof(float))
                                        {
                                            value = reader.GetFloat(i);
                                        }
                                        else if (p.PropertyType == typeof(double))
                                        {
                                            value = reader.GetDouble(i);
                                        }
                                        else if (p.PropertyType == typeof(decimal))
                                        {
                                            value = reader.GetDecimal(i);
                                        }
                                        else if (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?))
                                        {
                                            if (storeDateTimeAsTicks)
                                                value = new DateTime(reader.GetInt64(i));
                                            else
                                                DateTime.ParseExact(reader.GetString(i), DateTimeFormat, CultureInfo.InvariantCulture);
                                        }
                                        else if (p.PropertyType == typeof(DateTimeOffset) || p.PropertyType == typeof(DateTimeOffset?))
                                        {
                                            if (storeDateTimeAsTicks)
                                                value = new DateTimeOffset(new DateTime(reader.GetInt64(i)));//, TimeSpan.MinValue);
                                            else
                                                DateTimeOffset.ParseExact(reader.GetString(i), DateTimeOffsetFormat, CultureInfo.InvariantCulture);
                                        }
                                        else if (p.PropertyType == typeof(byte[]))
                                        {
                                            value = GetBytes(reader, i);
                                        }
                                        else
                                        {
                                            DbType dbType = GetDbType(p);
                                            if (dbType == DbType.String)
                                                value = reader.GetString(i);
                                            if (dbType == DbType.Int32)
                                                value = reader.GetInt32(i);
                                            else
                                                value = reader.GetValue(i);
                                        }
                                    }

                                    if (IsForeignKey(p))
                                    {
                                        object value1 = Activator.CreateInstance(p.PropertyType);
                                        ModelHelper.SetPKValue(value1, value);
                                        value = value1;
                                    }

                                    AttrHelper.SetPropertyValue(o, p, value);
                                }

                            }//columns
                            catch (Exception ex)
                            {
                                Log.LogError("Error reading property: " + p.Name + " i: " + i + " " + ex.StackTrace);
                                Log.LogError("Pk value: " + primaryKeValue);
                                Log.LogError("Property value: " + strvalue);
                                throw ex;
                            }
                            //i++;
                        }//foreach

                        if (IsShortTable(entityType))
                        {
                            //byte[] bytBLOB = new byte[reader.GetBytes(1, 0, null, 0, int.MaxValue) - 1];
                            //reader.GetBytes(7, 0, bytBLOB, 0, bytBLOB.Length);
                            //MemoryStream stmBLOB = new MemoryStream(bytBLOB);

                            int i = reader.GetOrdinal(JSON_DATA_BLOB);
                            byte[] buffer = GetBytes(reader, i);
                            o = JsonSerializeHelper.LoadFromString(System.Text.Encoding.UTF8.GetString(buffer), entityType);

                        }
                    }
                    list.Add(o);
                    if (useCache)
                    {
                        object primaryKeValue1 = ModelHelper.GetPKValue(o);
                        putCache(entityType, primaryKeValue1, o);
                    }
                }//rows
            }
            if (connOpenedHere) CloseConnection(conn);
            return list;
        }
         private int ReadCount(Type entityType, string sql, SQLiteConnection conn = null)
        {
            int count = -1;
            var command = new SQLiteCommand(sql, conn);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    count = reader.GetInt32(0);
                    break;
                }//rows
            }
            return count;
        }

        byte[] GetBytes(SQLiteDataReader reader, int i)
        {
            const int CHUNK_SIZE = 2 * 1024;
            byte[] buffer = new byte[CHUNK_SIZE];
            long bytesRead;
            long fieldOffset = 0;
            using (MemoryStream stream = new MemoryStream())
            {
                //if (p != null) i = reader.GetOrdinal(GetColumnName(p));
                while ((bytesRead = reader.GetBytes(i, fieldOffset, buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, (int)bytesRead);
                    fieldOffset += bytesRead;
                }
                return stream.ToArray();
            }
        }
         protected string GetInsertStatement(object o)
        {
            Type t = o.GetType();
            StringBuilder str = new StringBuilder();
            str.Append("INSERT INTO ");
            str.Append(GetTableName(t));

            str.Append(" (");

            var props = t.GetProperties();
            bool first = true;
            foreach (var p in props)
            {
                if (IsIgnore(p)) continue;
                if (IsShortTable(t) == true && IsNecessary(p) == false) continue;
                if (IsNotStored(p)) continue;
                if (first) first = false;
                else str.Append(", ");
                str.Append(GetColumnNameForStatement(p));
            }
            if (IsShortTable(t))
            {
                if (first) first = false;
                else str.Append(", ");
                str.Append(JSON_DATA_BLOB);
            }
            str.Append(") VALUES(");
            first = true;
            foreach (var p in props)
            {
                if (IsIgnore(p)) continue;
                if (IsShortTable(t) == true && IsNecessary(p) == false) continue;
                if (IsNotStored(p)) continue;
                if (first) first = false;
                else str.Append(", ");
                str.Append("@" + GetColumnName(p));
            }
            if (IsShortTable(t))
            {
                if (first) first = false;
                else str.Append(", ");
                str.Append("@" + JSON_DATA_BLOB);
            }
            str.Append(")");
            return str.ToString();
        }
         protected string GetUpdateStatement(object o)
        {
            Type t = o.GetType();
            StringBuilder str = new StringBuilder();
            str.Append("UPDATE ");
            str.Append(GetTableName(t));
            str.Append(" SET ");
            var props = t.GetProperties();
            bool first = true;
            foreach (var p in props)
            {
                if (IsIgnore(p)) continue;
                if (IsShortTable(t) == true && IsNecessary(p) == false) continue;
                if (IsNotStored(p)) continue;
                if (first) first = false;
                else str.Append(", ");
                str.Append(GetColumnNameForStatement(p));
                str.Append(" = ");
                str.Append("@" + GetColumnName(p));
            }
            if (IsShortTable(t))
            {
                if (first) first = false;
                else str.Append(", ");
                str.Append(JSON_DATA_BLOB);
                str.Append(" = ");
                str.Append("@" + JSON_DATA_BLOB);
            }
            str.Append(" WHERE ");
            str.Append(GetColumnNameForStatement(ModelHelper.GetPK(t)));
            str.Append(" = @");
            str.Append(GetColumnName(ModelHelper.GetPK(t)));
            return str.ToString();
        }
         protected string GetDeleteStatement(object o)
        {
            Type t = o.GetType();
            StringBuilder str = new StringBuilder();
            str.Append("DELETE FROM ");
            str.Append(GetTableName(t));
            if (o != null)
            {
                str.Append("");
                str.Append(" WHERE ");
                str.Append(GetColumnNameForStatement(ModelHelper.GetPK(t)));
                str.Append(" = @");
                str.Append(GetColumnName(ModelHelper.GetPK(t)));
            }
            return str.ToString();
        }
         protected string GetBaseSelectStatement(Type t)
        {
            StringBuilder str = new StringBuilder();
            str.Append("SELECT ");

            var props = t.GetProperties();
            bool first = true;
            foreach (var p in props)
            {
                if (IsIgnore(p)) continue;
                if (IsShortTable(t) == true && IsNecessary(p) == false) continue;
                if (IsNotStored(p)) continue;
                if (first) first = false;
                else str.Append(", ");
                str.Append(GetColumnNameForStatement(p));
            }
            if (IsShortTable(t))
            {
                if (first) first = false;
                else str.Append(", ");
                str.Append(JSON_DATA_BLOB);
            }

            str.Append(" FROM ");
            str.Append(GetTableName(t));
            return str.ToString();
        }
         private string GetCountSelectStatement(Type t)
        {
            StringBuilder str = new StringBuilder();
            str.Append("SELECT COUNT(*)");
            str.Append(" FROM ");
            str.Append(GetTableName(t));
            return str.ToString();
        }
         private string GetWherePKForSelectStatement(Type t, object pkValue)
        {
            StringBuilder str = new StringBuilder();
            if (pkValue != null)
            {
                str.Append(" WHERE ");
                var p = ModelHelper.GetPK(t);
                str.Append(GetColumnNameForStatement(p));
                str.Append(" = \'");
                str.Append(pkValue);
                str.Append("\'");
            }
            return str.ToString();
        }
         private string GetLIMITForSelectStatement(Type t, int limit, int offset)
        {
            if (limit < 0 && offset < 0) return "";
            StringBuilder str = new StringBuilder();
            if (limit >= 0)
            {
                str.Append(" LIMIT ");
                str.Append(limit);
            }
            if (offset >= 0)
            {
                str.Append(" OFFSET ");
                str.Append(offset);
            }
            return str.ToString();
        }

        private string GetWhereColumnsForSelectStatement(Type t, IDictionary<string, object> pars)
        {
            StringBuilder str = new StringBuilder();
            str.Append(" WHERE ");
            bool first = true;
            foreach (var par in pars) {
                PropertyInfo p = AttrHelper.GetProperty(t, par.Key);
                if (p == null) throw new InvalidDataException("Property: " + par.Key + " not found in type " + t);
                if (first) first = false;
                else str.Append(" AND ");
                str.Append(GetColumnNameForStatement(p));
                object fieldValue = par.Value;
                if (fieldValue != null)
                {
                    str.Append(" = \'");
                    if (fieldValue != null && fieldValue is string)
                    {
                        //https://stackoverflow.com/questions/26313845/escape-single-quote-in-sql-query-c-sharp
                        //todo use params
                        str.Append((fieldValue as string).Replace("'", "''"));
                    }
                    else
                        str.Append(fieldValue);
                    str.Append("\'");
                }
                else
                {
                    str.Append(" IS NULL");
                }
            }

            return str.ToString();
        }



        private string GetWhereColumnForSelectStatement(Type t, PropertyInfo p, object fieldValue)
        {
            StringBuilder str = new StringBuilder();
            str.Append(" WHERE ");
            str.Append(GetColumnNameForStatement(p));
            if (fieldValue != null)
            {
                str.Append(" = \'");
                str.Append(fieldValue);
                str.Append("\'");
            }
            else
            {
                str.Append(" IS NULL");
            }
            return str.ToString();
        }

         protected string GetCreateTableStatement(Type t)
        {
            StringBuilder str = new StringBuilder();
            str.Append("CREATE TABLE if not exists ");
            str.Append(GetTableName(t));
            str.Append(" (");
            var props = t.GetProperties();

            bool first = true;
            foreach (var p in props)
            {
                if (IsIgnore(p)) continue;
                if (IsShortTable(t) == true && IsNecessary(p) == false) continue;
                if (IsNotStored(p)) continue;
                if (first) first = false;
                else str.Append(", ");
                str.Append(SqlDecl(p));
            }
            if (IsShortTable(t))
            {
                if (first) first = false;
                else str.Append(", ");
                str.Append(JSON_DATA_BLOB);
            }
            str.Append(")");
            string s = str.ToString();
            Console.WriteLine(s);
            return s;
        }
         protected string GetDropTableStatement(Type t)
        {
            StringBuilder str = new StringBuilder();
            str.Append("DROP TABLE ");
            str.Append(GetTableName(t));
            return str.ToString();
        }
        /*
        public int CreateIndex(string indexName, string tableName, string[] columnNames, bool unique = false)
        {
            const string sqlFormat = "create {2} index if not exists \"{3}\" on \"{0}\"(\"{1}\")";
            var sql = string.Format(sqlFormat, tableName, string.Join("\", \"", columnNames), unique ? "unique" : "", indexName);
            return Execute(sql);
        }
        */

        private  string SqlDecl(PropertyInfo p)
        {
            var decl = "\"" + GetColumnName(p) + "\" " + SqlType(p) + " ";

            if (IsPK(p))
            {
                decl += "primary key ";
            }
            if (IsAutoInc(p) && (p.PropertyType == typeof(int) || p.PropertyType == typeof(long)))
            {
                decl += "autoincrement ";
            }
            if (IsMarkedNotNull(p))
            {
                //todo 
                //decl += "not null ";
            }
            //if (!string.IsNullOrEmpty(p.Collation))
            //{
            //   decl += "collate " + p.Collation + " ";
            //}
            if (GetDefaultValue(p) != null)
            {
                decl += "default('" + GetDefaultValue(p) + "') ";
            }

            return decl;
        }

        private  string SqlType(PropertyInfo p)
        {
            var clrType = p.PropertyType;
            //var interfaces = clrType.GetTypeInfo().ImplementedInterfaces.ToList();
            if (IsForeignKey(p))
            {
                Type fType = ModelHelper.GetPK(clrType).PropertyType;
                if (fType == typeof(string)) return "text";
                else if (fType == typeof(short) || fType == typeof(int) ||   fType == typeof(uint) || fType == typeof(long)) return "integer";
            }
            else if (IsComplex(p))
            {
                return "blob";
            }

            if (clrType == typeof(bool) || clrType == typeof(byte) || clrType == typeof(ushort) ||
                clrType == typeof(sbyte) || clrType == typeof(short) || clrType == typeof(int) ||
                clrType == typeof(uint) || clrType == typeof(long))
            {
                return "integer";
            }
            if (clrType == typeof(float) || clrType == typeof(double) || clrType == typeof(decimal))
            {
                return "float";
            }
            if (clrType == typeof(string))
            {
                //return "varchar(" + len.Value + ")";
                //return "varchar";
                return "text";
            }
            if (clrType == typeof(TimeSpan))
            {
                return "bigint";
            }
            if (clrType == typeof(DateTime) || clrType == typeof(DateTime?))
            {
                return storeDateTimeAsTicks ? "bigint" : "datetime";
            }
            if (clrType == typeof(DateTimeOffset) || clrType == typeof(DateTimeOffset?))
            {
                return storeDateTimeAsTicks ? "bigint" : "datetime";
            }
            if (clrType.GetTypeInfo().IsEnum)
            {
                return "integer";
            }
            if (clrType == typeof(byte[]))
            {
                return "blob";
            }
            if (clrType == typeof(Guid))
            {
                return "varchar(36)";
            }
            return "blob";
            throw new NotSupportedException("Don't know about " + clrType);
        }

        private  DbType GetDbType(PropertyInfo p)
        {
            var clrType = p.PropertyType;
            DbType dbType = DbType.String;
            if (IsForeignKey(p))
            {
                Type fType = ModelHelper.GetPK(clrType).PropertyType;
                if (fType == typeof(string)) return DbType.String;
                else if (fType == typeof(short) || fType == typeof(int) || fType == typeof(uint) || fType == typeof(long))
                    return DbType.Int32;
            }
            else if (IsComplex(p))
            {
                return DbType.Binary;
            }
            if (clrType == typeof(bool) || clrType == typeof(byte) || clrType == typeof(ushort) ||
                clrType == typeof(sbyte) || clrType == typeof(short) || clrType == typeof(int) ||
                clrType == typeof(uint) || clrType == typeof(long))
            {
                return DbType.Int32;
            }
            if (clrType == typeof(float) || clrType == typeof(double) || clrType == typeof(decimal))
            {
                return DbType.Decimal;
            }
            if (clrType == typeof(string))
            {
                return DbType.String;
            }
            if (clrType == typeof(TimeSpan))
            {
                return DbType.Int64;//"bigint";
            }
            if (clrType == typeof(DateTime) || clrType == typeof(DateTime?))
            {
                return storeDateTimeAsTicks ? DbType.Int64 : DbType.String;
            }
            if (clrType == typeof(DateTimeOffset) || clrType == typeof(DateTimeOffset?))
            {
                return storeDateTimeAsTicks ? DbType.Int64 : DbType.String;
            }
            if (clrType.GetTypeInfo().IsEnum)
            {
                return DbType.Int32;
            }
            if (clrType == typeof(byte[]))
            {
                return DbType.Binary;
            }
            if (clrType == typeof(Guid))
            {
                return DbType.String;
            }
            return dbType;
        }



    }


    #region connection pool
    public class SQLiteConnectionWithLock  
    {
        private readonly object _lockPoint = new object();
        public SQLiteConnection SQLiteConnection { get; set; }

        public SQLiteConnectionWithLock(string connectionString)
        {
            SQLiteConnection = new SQLiteConnection(connectionString);
        }


        public IDisposable Lock()
        {
            return new LockWrapper(_lockPoint);
        }

        private class LockWrapper : IDisposable
        {
            private readonly object _lockPoint;

            public LockWrapper(object lockPoint)
            {
                _lockPoint = lockPoint;
                Monitor.Enter(_lockPoint);
            }

            public void Dispose()
            {
                Monitor.Exit(_lockPoint);
            }
        }
    }
    public class SQLiteConnectionPool
    {
        private readonly Dictionary<string, Entry> _entries = new Dictionary<string, Entry>();
        private readonly object _entriesLock = new object();

        public SQLiteConnectionPool()
        {
        }

        public SQLiteConnectionWithLock GetConnection(string connectionString)
        {
            lock (_entriesLock)
            {
                Entry entry;
                var key = connectionString;

                if (!_entries.TryGetValue(key, out entry))
                {
                    entry = new Entry(connectionString);
                    _entries[key] = entry;
                }

                return entry.Connection;
            }
        }
        public void ReleaseConnection()
        {

        }


        public void Reset()
        {
            lock (_entriesLock)
            {
                foreach (var entry in _entries.Values)
                {
                    entry.OnApplicationSuspended();
                }
                _entries.Clear();
            }
        }

        /// <summary>
        ///     Call this method when the application is suspended.
        /// </summary>
        /// <remarks>Behaviour here is to close any open connections.</remarks>
        public void ApplicationSuspended()
        {
            Reset();
        }

        private class Entry
        {
            public Entry(string connectionString)
            {
                ConnectionString = connectionString;
                Connection = new SQLiteConnectionWithLock(connectionString);
                Connection.SQLiteConnection.Open();
            }

            public string ConnectionString { get; private set; }
            public SQLiteConnectionWithLock Connection { get; private set; }

            public void OnApplicationSuspended()
            {
                Connection.SQLiteConnection.Dispose();
                Connection.SQLiteConnection = null;
                Connection = null;
            }
        }
    }
    #endregion
}
