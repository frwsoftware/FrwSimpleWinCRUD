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
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using FrwSoftware.Properties;
using Newtonsoft.Json;
using System.Text;

namespace FrwSoftware
{

    public class JSettingChangedEventArgs : EventArgs
    {
        public JSetting Setting { get; set; }
    }

    /// <summary>
    /// occurs when settings value changed 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void JSettingChangedEventHandler(object sender, JSettingChangedEventArgs e);

    public class JSetting
    {
        /// <summary>
        /// System name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Full Name of Class Type of value
        /// We need this field to save json
        /// </summary>
        public string ValueTypeName {
            get
            {
                return ValueType != null ? ValueType.FullName : null;
            }
            set
            {
                if (value == null) ValueType = null;
                else ValueType = TypeHelper.FindType(value);
            }
        }
        /// <summary>
        /// Class Type of value
        /// </summary>
        [JsonIgnore]
        public Type ValueType { get; set; }
        /// <summary>
        /// Value
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// Brief description to display in gri column
        /// </summary>
        public string Description {
            get
            {
                if (string.IsNullOrEmpty(description) == false) return description;
                else return Name;
            }
            set
            {
                description = value;
            }
        }
        private string description = null;
        /// <summary>
        /// Group name (to display in the settings window)
        /// </summary>
        public string Group { get; set; }
        /// <summary>
        /// Full description
        /// </summary>
        public string Help { get; set; }
        /// <summary>
        /// Id of dictionary (dictionaty setting)
        /// </summary>
        public string DictId { get; set; }
        /// <summary>
        /// Allow multi values
        /// </summary>
        public bool AllowMultiValues { get; set; }
        /// <summary>
        /// Custom settings are displayed in the settings window
        /// </summary>
        public bool IsUser { get; set; }
        /// <summary>
        /// Settings that need to be stored on a specific computer 
        /// </summary>
        [JsonIgnore]
        public bool IsAttachedToComputer { get; set; }

        public event JSettingChangedEventHandler ValueChanged;
        public void ForceValueChangedEvent(object sender)
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(sender, new JSettingChangedEventArgs() { Setting = this });
            }
        }
        public bool IsCustomSetting()
        {
            if (this.ValueType != null)
            {
                if (DictId != null) return true;
                else if (ValueType != typeof(string))
                {
                    JEntity entityAttr = AttrHelper.GetClassAttribute<JEntity>(ValueType);
                    if (entityAttr != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }


    /// <summary>
    /// Configuration manager
    /// </summary>
    public class FrwConfig
    {

        static public string DEFAULT_PROFILE_PREFIX = "Data\\Profile";

        public static FrwConfig Instance
        {
            get {
                return instance ?? (instance = new FrwConfig());
            }
            set
            {
                if (instance != null) throw new InvalidOperationException("Custom instance can be set only once and before first getting");
                instance = value;
            }
        }
        static public bool IsInstanceSet
        {
            get
            {
                return (FrwConfig.instance != null);
            }
        }
        private static FrwConfig instance;

        public bool DevelopMode { get; set; }
        /// <summary>
        ///Global directory. 
        //In the simplest case, the global directory is the same as the program's start directory.
        //But sometimes it is convenient to define it in a special way. For example, then you can work 
        //with the same database files and settings in debug and working mode without permanently copying 
        //them to the working directory from the project directory and back. 
        /// </summary>
        public string GlobalDir { get; set; }
        /// <summary>
        ///Directory for recording settings that need to be stored on a specific computer         
        /// </summary>
        public string ComputerUserDir { get; set; }
        /// <summary>
        ///Profile directory (database, saving state of winform objects, user settings, etc.) 
        /// </summary>
        public string ProfileDir { get; set; }
        /// <summary>
        /// User temporary directiory
        /// </summary>
        public string UserTempDir { get; set; }
        const string APPCONFIG_SUFFIX = "appconfig";

        /// <summary>
        /// Subdirecrory for user settings and winform objected saved state
        /// </summary>
        public string ProfileConfigDir
        {
            get {
                if (ProfileDir != null) return Path.Combine(ProfileDir, APPCONFIG_SUFFIX);
                else return APPCONFIG_SUFFIX;
            }
        }

        protected Dictionary<string, JSetting> settings = new Dictionary<string, JSetting>();
        protected string settingsFileName = "app.json";

        /// <summary>
        /// User settings list 
        /// </summary>
        public IEnumerable<JSetting> Settings
        {
            get
            {
                return settings.Values;
            }
        }
        #region propery get anad set
        public JSetting GetProperty(string name)
        {
            JSetting setting = null;
            settings.TryGetValue(name, out setting);
            return setting;
        }
        public object GetPropertyValue(string name)
        {
            return GetPropertyValue(name, null);
        }
        public object GetPropertyValue(string name, object defaultValue)
        {
            JSetting setting = GetProperty(name);
            if (setting != null)
            {
                return setting.Value;
            }
            else return defaultValue;
        }
        public string GetPropertyValueAsString(string name)
        {
            JSetting setting = GetProperty(name);
            if (setting != null && setting.Value != null)
            {
                return setting.Value.ToString();
            }
            else return null;
        }
        public bool GetPropertyValueAsBool(string name)
        {
            return GetPropertyValueAsBool(name, false);
        }
        public bool GetPropertyValueAsBool(string name, bool defaultValue)
        {
            object value = GetPropertyValue(name);
            if (value == null) return defaultValue;
            if (value is bool) return (bool)value;
            else if (value is string)
            {
                return bool.Parse(value as string);
            }
            else
            {
                throw new ArgumentException();
            }
        }
        public JSetting CreatePropertyIfNotExist(JSetting newSetting)
        {
            JSetting setting = GetProperty(newSetting.Name);
            if (setting == null)
            {
                setting = newSetting;

                if (string.IsNullOrEmpty(setting.Name)) throw new ArgumentException("Name can not be empty");
                //if (setting.Value == null && setting.ValueType == null) throw new ArgumentException("Both Value and ValueType can not be empty");
                //set valuetype
                if (setting.ValueType == null) {
                    if (setting.Value != null) setting.ValueType = setting.Value.GetType();
                    else setting.ValueType = typeof(string);
                }
                else if (setting.Value != null && setting.ValueType != null) {
                    if (setting.Value.GetType().Equals(setting.ValueType) == false) throw new ArgumentException("ValueTypeName (" + setting.ValueType.FullName + ") not equals Value type (" + setting.Value.GetType() + ")");
                }

                settings[setting.Name] = setting;
            }
            else
            {
                //wee need due to i18n
                setting.Description = newSetting.Description;
                setting.Help = newSetting.Help;
                setting.Group = newSetting.Group;
            }
            return setting;
        }
        
        public JSetting CreatePropertyIfNotExist(string name, string description, object defaultValue = null)
        {
            JSetting setting = new JSetting();
            setting.Name = name;
            setting.Description = description;
            setting.Value = defaultValue;
            return CreatePropertyIfNotExist(setting);
        }
        
        public JSetting SetPropertyValue(string name, object value)
        {
            JSetting setting = GetProperty(name);
            if (setting != null)
            {
                setting.Value = value;
                if (setting.Value != null) setting.ValueType = setting.Value.GetType();
                return setting;
            }
            else
            {
                throw new ArgumentException("Setting not found withd name: " + name + ". Create it first.");
            }
        }
        #endregion

        /// <summary>
        /// Virtual method for create you own user settings
        /// </summary>
        virtual protected void CreateProperties()
        {
        }

        public void ComplateSettingsRelations()
        {
            foreach (var s in settings.Values)
            {
                if (s.Value != null)
                {
                    Type type = s.ValueType;
                    JEntity entityAttr = AttrHelper.GetClassAttribute<JEntity>(type);
                    if (entityAttr != null)//todo list 
                    {
                        PropertyInfo pkProp = AttrHelper.GetProperty<JPrimaryKey>(type);
                        if (pkProp == null) throw new Exception("Primary key not found in referenced entity");

                        if (s.AllowMultiValues)
                        {
                            IList list = (IList)s.Value;
                            IList values = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));
                            foreach (var val in list)
                            {
                                object pvReal = null;
                                object pkValue = pkProp.GetValue(val);
                                if (pkValue != null)
                                {
                                    try
                                    {
                                        pvReal = Dm.Instance.Find(type, pkValue);
                                    }
                                    catch (Exception ex)
                                    {
                                        PropertyInfo nameProp = AttrHelper.GetProperty<JNameProperty>(type);
                                        if (nameProp != null) nameProp.SetValue(val, Resources.Dm_ErrorFinding + ex);
                                        pvReal = val;
                                    }
                                    if (pvReal != null)
                                    {
                                    }
                                    else
                                    {
                                        //may be removed 
                                        PropertyInfo nameProp = AttrHelper.GetProperty<JNameProperty>(type);
                                        if (nameProp != null) nameProp.SetValue(val, Resources.Dm_NotFound);
                                        pvReal = val;
                                    }
                                }
                                values.Add(pvReal);
                            }
                            s.Value = values;
                        }
                        else
                        {
                           
                            object pvReal = null;
                            object pkValue = pkProp.GetValue(s.Value);
                            if (pkValue != null)
                            {
                                try
                                {
                                    pvReal = Dm.Instance.Find(type, pkValue);
                                }
                                catch (Exception ex)
                                {
                                    PropertyInfo nameProp = AttrHelper.GetProperty<JNameProperty>(type);
                                    if (nameProp != null) nameProp.SetValue(s.Value, Resources.Dm_ErrorFinding + ex);
                                }
                                if (pvReal != null)
                                {
                                    s.Value = pvReal;
                                }
                                else
                                {
                                    //may be removed 
                                    PropertyInfo nameProp = AttrHelper.GetProperty<JNameProperty>(type);
                                    if (nameProp != null) nameProp.SetValue(s.Value, Resources.Dm_NotFound);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads all user settings. 
        /// </summary>
        public void LoadConfig()
        {
            string filename = Path.Combine(ProfileConfigDir, settingsFileName);
            FileInfo fileInfo = new FileInfo(filename);
            DirectoryInfo dir = fileInfo.Directory;
            if (dir.Exists == false)
            {
                Directory.CreateDirectory(dir.FullName);
            }
            if (fileInfo.Exists)
            {
                List<JSetting> settingsList = null;
                settingsList = JsonSerializeHelper.LoadFromFile<List<JSetting>>(filename);
                foreach (var s in settingsList)
                {
                    s.IsAttachedToComputer = false;
                    settings.Add(s.Name, s);
                }
            }

            //Properties.Settings.Default path
            //https://msdn.microsoft.com/en-us/library/system.environment.specialfolder.aspx 

            //stage 2
            filename = Path.Combine(ComputerUserDir, settingsFileName);
            fileInfo = new FileInfo(filename);
            dir = fileInfo.Directory;
            if (dir.Exists == false)
            {
                Directory.CreateDirectory(dir.FullName);
            }
            if (fileInfo.Exists)
            {
                List<JSetting> settingsList = null;
                settingsList = JsonSerializeHelper.LoadFromFile<List<JSetting>>(filename);
                foreach (var s in settingsList)
                {
                    s.IsAttachedToComputer = true;
                    settings[s.Name] = s;
                }
            }

            foreach (JSetting setting in settings.Values)
            {
                if (setting.Value != null)
                {
                    Type type = setting.ValueType;
                    if (type != typeof(string))
                    {
                        JEntity entityAttr = AttrHelper.GetClassAttribute<JEntity>(type);
                        if (entityAttr != null)
                        {
                            if (setting.AllowMultiValues)
                            {
                                IList list = (IList)setting.Value;
                                IList values = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));
                                foreach (var pkOnlyObject in list)
                                {
                                    string str = JsonSerializeHelper.SaveToString(pkOnlyObject);
                                    object realObject = JsonSerializeHelper.LoadFromString(str, type);
                                    values.Add(realObject);
                                }
                                setting.Value = values;
                            }
                            else
                            {
                                setting.Value = JsonSerializeHelper.LoadFromString(JsonSerializeHelper.SaveToString(setting.Value), type);
                            }
                        }
                        else
                        {
                            TypeConverter converter = TypeDescriptor.GetConverter(type);
                            if (converter != null)//for system types (Font, etc.)
                            {
                                setting.Value = converter.ConvertFromString((string)setting.Value);
                            }
                        }
                    }
                }
            }
            CreateProperties();
        }
        /// <summary>
        /// Saves all user settings
        /// </summary>
        public void SaveConfig()
        {
            foreach (var s in settings.Values)
            {
                if (s.Value != null && s.ValueType == null)
                {
                    s.ValueType = s.Value.GetType();
                }
                if (s.Value != null)
                {
                    if (s.ValueType != typeof(string))
                    {
                        JEntity entityAttr = AttrHelper.GetClassAttribute<JEntity>(s.ValueType);
                        if (entityAttr != null)
                        {
                            if (s.AllowMultiValues)
                            {
                                IList list = (IList)s.Value;
                                IList values = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(s.ValueType));
                                foreach (var realObject in list)
                                {
                                   values.Add(AttrHelper.ReplaceObjectByPkOnlyObject(realObject));
                                }
                                s.Value = values;
                            }
                            else
                            {
                                s.Value = AttrHelper.ReplaceObjectByPkOnlyObject(s.Value);
                            }
                        }
                        else {
                            TypeConverter converter = TypeDescriptor.GetConverter(s.ValueType);
                            if (converter != null)
                            {
                                s.Value = converter.ConvertToString(s.Value);
                            }
                        }
                    }
                }
            }

            string filename = Path.Combine(ProfileConfigDir, settingsFileName);
            FileInfo fileInfo = new FileInfo(filename);
            DirectoryInfo dir = fileInfo.Directory;
            if (dir.Exists == false)
            {
                Directory.CreateDirectory(dir.FullName);
            }
            List<JSetting> settingsList = new List<JSetting>();
            foreach (var s in settings.Values)
            {
                if (!s.IsAttachedToComputer)
                    settingsList.Add(s);
            }
            JsonSerializeHelper.SaveToFile(settingsList, filename);
            //stage 2
            filename = Path.Combine(ComputerUserDir, settingsFileName);
            fileInfo = new FileInfo(filename);
            dir = fileInfo.Directory;
            if (dir.Exists == false)
            {
                Directory.CreateDirectory(dir.FullName);
            }
            settingsList = new List<JSetting>();
            foreach (var s in settings.Values)
            {
                if (s.IsAttachedToComputer)
                    settingsList.Add(s);
            }
            File.WriteAllText(filename, JsonSerializeHelper.SaveToString(settingsList), Encoding.UTF8);//short
        }
    }
}
