using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;

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
        public event JSettingChangedEventHandler ValueChanged;
        public string Name { get; set; }
        public string ValueTypeName { get; set; }//to save in json file
        public object Value { get; set; }
        public string Description { get; set; }
        public bool IsUser { get; set; }//Custom settings are displayed in the settings window 
        [JsonIgnore]
        public bool IsAttachedToComputer { get; set; }//Settings that need to be stored on a specific computer 

        public void ForceValueChangedEvent(object sender)
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(sender, new JSettingChangedEventArgs() { Setting = this });
            }
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
        public void SetProperty(JSetting setting)
        {
            object value = GetPropertyValue(setting.Name, setting.Value);
            setting.Value = value;
            // You can also use the Item property to add new elements by setting the value of a key that does not exist in the Dictionary<TKey, TValue>; for example, myCollection[myKey] = myValue (in Visual Basic, myCollection(myKey) = myValue). However, if the specified key already exists in the Dictionary<TKey, TValue>, setting the Item property overwrites the old value. In contrast, the Addmethod throws an exception if a value with the specified key already exists.
            settings[setting.Name] = setting;
        }
        public void AddProperty(JSetting setting)
        {
            settings.Add(setting.Name, setting);
        }

        public JSetting SetPropertyValue(string name, object value)
        {
            JSetting setting = GetProperty(name);
            if (setting != null)
            {
                setting.Value = value;
                return setting;
            }
            else
            {
                setting = new JSetting() { Name = name, Value = value };
                AddProperty(setting);
                return setting;
            }
        }
        #endregion

        /// <summary>
        /// Virtual method for create you own user settings
        /// </summary>
        virtual protected void CreateProperties()
        {
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
                    if (s.ValueTypeName != null) {
                        Type type = TypeHelper.FindType(s.ValueTypeName);
                        if (type != typeof(string))
                        {
                            TypeConverter converter = TypeDescriptor.GetConverter(type);
                            if (converter != null)
                            {
                                s.Value = converter.ConvertFromString((string)s.Value);
                            }
                        }
                    }
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
                    if (s.ValueTypeName != null)
                    {
                        Type type = TypeHelper.FindType(s.ValueTypeName);
                        if (type != typeof(string))
                        {
                            TypeConverter converter = TypeDescriptor.GetConverter(type);
                            if (converter != null)
                            {
                                s.Value = converter.ConvertFromString((string)s.Value);
                            }
                        }
                    }
                    settings.Add(s.Name, s);
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
                if (s.Value != null)
                {
                    s.ValueTypeName = s.Value.GetType().FullName;
                }

                if (s.Value != null)
                {
                    Type type = s.Value.GetType();
                    if (type != typeof(string))
                    {
                        TypeConverter converter = TypeDescriptor.GetConverter(type);
                        if (converter != null)
                        {
                            s.Value = converter.ConvertToString(s.Value);
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
            JsonSerializeHelper.SaveToFile(settingsList, filename);
        }
    }
}
