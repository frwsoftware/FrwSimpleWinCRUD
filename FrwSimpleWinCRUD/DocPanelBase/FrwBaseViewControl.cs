using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FrwSoftware
{
    public partial class FrwBaseViewControl : UserControl, IContent
    {

        public const string PersistStringSeparator = "-,-";
        public const string PersistStringSeparatorKeyValue = "-:-";
        public const string PersistStringTypeParameter = "Type";
        public const string PersistStringPaneUIDParameter = "PaneUID";
        public const string PersistStringRelPaneUIDParameter = "RelPaneUID";

      


        public bool HideOnClose { get; set; }
        //public string Text { get; set; }
        public IContentContainer ContentContainer { get; set; }
        public FrwBaseViewControl()
        {
            InitializeComponent();
        }
        public virtual IDictionary<string, object> GetKeyParams()
        {
            Dictionary<string, object> pars = new Dictionary<string, object>();
            pars.Add(FrwBaseViewControl.PersistStringTypeParameter, GetType().ToString());
            //if (RelPaneUID != null) pars.Add(FrwBaseViewControl.PersistStringRelPaneUIDParameter, RelPaneUID);
            return pars;
        }
        public virtual void SetKeyParams(IDictionary<string, object> pars)
        {
            
        }
        public virtual void LoadConfig()
        {

        }
        public virtual void SaveConfig()
        {
        }
        public virtual bool CompareKeyParams(IDictionary<string, object> pars)
        {
            return true;
        }
        protected bool compareObjectKey(object key, object value)
        {
            //если ключ не задан сравнение по нему не производится 
            if (key != null)
            {
                if (value != null)
                    return key.Equals(value);
                else return false;
            }
            else return true;
            //else if (value == null) return true;
            //else return false;
        }
        protected bool compareStringKey(object key, string value)
        {
            //если ключ не задан сравнение по нему не производится 
            if (key != null)
            {
                if (key is string)
                {
                    if (value != null)
                        return ((string)key).Equals(value);
                    else return false;
                }
                else throw new ArgumentException();
            }
            else return true;
            //else if (value == null) return true;
            //else return false;
        }
        protected bool compareLongKey(object key, long value)
        {
            //если ключ не задан сравнение по нему не производится 
            if (key != null)
            {
                if (key is long || key is int)
                {
                    return ((long)key == value);
                }
                else if (key is string)
                {
                    long intKey;
                    if (long.TryParse((string)key, out intKey) == false) throw new ArgumentException(); ;
                    if (value == intKey) return true;
                    else return false;
                }
                else throw new ArgumentException();
            }
            else return true;
        }
        protected bool compareBoolKey(object key, bool value)
        {
            //если ключ не задан сравнение по нему не производится 
            if (key != null)
            {
                if (key is bool)
                {
                    return ((bool)key == value);
                }
                else if (key is string)
                {
                    bool boolKey;
                    if (bool.TryParse((string)key, out boolKey) == false) throw new ArgumentException(); ;
                    if (value == boolKey) return true;
                    else return false;
                }
                else throw new ArgumentException();
            }
            else return true;
        }
        public void SetNewCaption(string caption)
        {
            Text = caption;
            FrwDocContent c = FindDocContentContainer(this);
            if (c != null) c.Text = caption;
        }
        private FrwDocContent FindDocContentContainer(Control c)
        {
            if (c == null) return null;
            if (c is FrwDocContent) return c as FrwDocContent;
            else return FindDocContentContainer(c.Parent);
        }

    }
}
