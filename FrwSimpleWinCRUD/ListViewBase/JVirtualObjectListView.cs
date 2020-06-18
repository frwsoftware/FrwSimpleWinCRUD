using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrightIdeasSoftware;

namespace FrwSoftware
{
    public class JVirtualObjectListView : VirtualObjectListView
    {
        public Type SourceObjectType { get; set; }
        public JVirtualObjectListView(Type sourceObjectType)
        {
            SourceObjectType = sourceObjectType;
            this.VirtualListDataSource = new JObjectListDataSource(this);

        }
    }

    public class JObjectListDataSource : AbstractVirtualListDataSource
    {
        //todo
        protected void ClearCache()
        {
            this.objectsToIndexMap.Clear();
            this.indexToObjectMap.Clear();
            count = -1;
        }
        readonly Dictionary<Object, int> objectsToIndexMap = new Dictionary<Object, int>();
        private Dictionary<int, Object> indexToObjectMap = new Dictionary<int, object>();
        int count = -1;

        public Type SourceObjectType { get; set; }
        public JObjectListDataSource(JVirtualObjectListView listView)
            : base(listView)
        {
            SourceObjectType = listView.SourceObjectType;
        }
        public override object GetNthObject(int n)
        {
            //Console.WriteLine("GetNthObject n = " + n);
            object o = null;
            indexToObjectMap.TryGetValue(n, out o);
            if (o == null)
            {
                o = Dm.Instance.FindByNum(SourceObjectType, n);
                this.objectsToIndexMap[o] = n;
                this.indexToObjectMap[n] = o;
            }
            return o;
        }
        public override int GetObjectCount()
        {
            if (count == -1)
            {
                count = Dm.Instance.CountAll(SourceObjectType);
            }
            return count;
        }
        
        public override int GetObjectIndex(object model)
        {
            int index;

            if (model != null && this.objectsToIndexMap.TryGetValue(model, out index))
                return index;

            return -1;
        }
        
    }
}
