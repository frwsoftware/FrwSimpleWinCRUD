using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FrwSoftware
{
    public interface IDs
    {
        void Init();
        void Destroy();
        void SaveObject(object o);
        void InsertObject(object o);
        void UpdateObject(object o);
        void CreateTable(Type entityType);
        void DropTable(Type entityType);
        void DeleteObject(object o);
        void DeleteAllObjects(Type entityType);
        //object EmptyObject(Type t, IDictionary<string, object> pars = null);
        object Find(Type entityType, object primaryKeValue);
        object FindByNum(Type entityType, int num);
        IList FindAll(Type entityType);
        int CountAll(Type entityType);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="p"></param>
        /// <param name="fieldValue">value to find (may be simple, complex or null). </param>
        /// <returns></returns>
        IList FindBy(Type entityType, PropertyInfo p, object fieldValue); //ResolveOneToManyRelation  ResolveOneToOneRelation
        IList FindByParams(Type t, IDictionary<string, object> pars);
        object EmptyObject(Type t, IDictionary<string, object> pars);
        void GenNextPkValue(object o);
        void PostLoadEntity(Type t);
        void SetEntityModified(Type t);
        bool IsEntityModified(Type t);
        bool IsJoinModified(Type t1, Type t2, string joinTableName);
        void SetJoinModified(Type t1, Type t2, string joinTableName);
        IList InitiallyLoadData(Type t);
        IList FindFromJoin(object rowObject, Type foreinEntityType, string joinName = null);
        void AddJoin(object o1, object o2, string joinTableName = null);
        void DeleteJoin(object o1, object o2, string joinTableName = null);
        void UpdateFieldWithOneToManyRelation(object rowObject, object newObject, Type foreinEntityType, PropertyInfo refFieldInForeinEntity);
        void SaveEntityData(Type type);
        void SaveEntityDataToOtherLocation(IList list, Type type, string customDirPath);
        void SaveJoinData(Type t1, Type t2, string joinTablename = null);
        void SaveAllEntitiesData(bool allways);
        bool IsResolvedOnInit();
        bool IsCashed();
        void RegisterCustomPathForType(Type type, string path);
    }
        
        
}
