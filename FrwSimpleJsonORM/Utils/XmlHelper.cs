using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FrwSoftware
{
  
    public class XmlHelper
    {
        protected bool loaded = false;
        private XmlNode rootNode = null;
        public XmlNode RootNode { get { return rootNode; } }
        protected XmlDocument doc = null;

        public XmlDocument Doc
        {
            get
            {
                if (doc == null) doc = new XmlDocument();
                return doc;
            }
        }


        public XmlNode CreateRoot(string rootName)
        {
            if (rootNode != null) throw new InvalidOperationException();
            if (doc == null) doc = new XmlDocument();
            //(1) the xml declaration is recommended, but not mandatory
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlNode root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);
            //(2) string.Empty makes cleaner code
            rootNode = doc.CreateElement(string.Empty, rootName, string.Empty);
            doc.AppendChild(rootNode);
            return rootNode;
        }

        public bool TryLoadDocument(string configFilePath)
        {
            FileInfo fi = new FileInfo(configFilePath);
            //xmlFilenamePath = fi.FullName;
            if (fi.Exists == false) return false;
            //use temporary vars 
            XmlDocument tmp_doc = null;
            XmlNode tmp_root = null;
            tmp_doc = new XmlDocument();
            tmp_doc.Load(fi.FullName);
            tmp_root = tmp_doc.DocumentElement;
            doc = tmp_doc;
            rootNode = tmp_root;
            return true;
        }


        public void LoadDocumentFromString(string xml)
        {
            //use temporary vars 
            XmlDocument tmp_doc = null;
            XmlNode tmp_root = null;
            tmp_doc = new XmlDocument();
            Stream xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            tmp_doc.Load(xmlStream);
            xmlStream.Close();
            tmp_root = tmp_doc.DocumentElement;
            doc = tmp_doc;
            rootNode = tmp_root;
        }

        public void SaveDocument(string xmlFilenamePath)
        {
            SaveDocumentLocal(xmlFilenamePath);
        }
        public string GetDocumentString(Encoding enc)
        {
            MemoryStream xmlStream = new MemoryStream();
            doc.Save(xmlStream);
            return enc.GetString(xmlStream.ToArray());
        }
        private void SaveDocumentLocal(string fileName)
        {
            doc.Save(fileName);
        }

        public string getAttrValue(XmlNode node, string name, string defaultValue)
        {
            XmlNode attr = node.Attributes.GetNamedItem(name);
            if (attr == null)
            {
                if (defaultValue != null)
                {
                    attr = doc.CreateAttribute(name);
                    node.Attributes.Append((XmlAttribute)attr);
                    attr.Value = defaultValue;
                    return attr.Value;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return attr.Value;
            }
        }
        public string getAttrValue(XmlNode node, string name)
        {
            return getAttrValue(node, name, null);
        }
        public int getIntAttrValue(XmlElement element, string name, int min, int max, int def)
        {
            int value = def;
            try
            {
                value = getIntAttrValue(element, name, def);
                if (value < min)
                {
                    string strErrFormat =
                        "Attribute value \"{0}\" of element \"{1}\" less  than {2}.";
                    throw new Exception(string.Format(strErrFormat, name, element.Name, min));
                }
                if (value > max)
                {
                    string strErrFormat =
                        "Attribute value \"{0}\" of element \"{1}\" more than {2}.";
                    throw new Exception(string.Format(strErrFormat, name, element.Name, min));
                }
            }
            catch (Exception)
            {
                value = def;
            }
            return value;
        }
        public int getIntAttrValue(XmlNode node, string name, int defaultValue)
        {
            string valueStr = getAttrValue(node, name, defaultValue.ToString());
            if (!string.IsNullOrEmpty(valueStr))
            {
                return int.Parse(valueStr);
            }
            else return defaultValue;

        }
        public int getIntAttrValue(XmlNode node, string name)
        {
            string valueStr = getAttrValue(node, name, null);
            if (valueStr != null)
            {
                return int.Parse(valueStr);
            }
            else
            {
                throw new ArgumentException();
            }

        }
        public uint getUIntAttrValue(XmlElement element, string name, uint defaultValue)
        {
            uint value = defaultValue;
            string strValue = getAttrValue(element, name);
            if (!string.IsNullOrEmpty(strValue))
                try { value = uint.Parse(strValue); }
                catch (Exception) { }
            return value;
        }
        public ushort getUShortAttrValue(XmlElement element, string name, ushort defaultValue)
        {
            ushort value = defaultValue;
            string strValue = getAttrValue(element, name);
            if (!string.IsNullOrEmpty(strValue))
                try { value = ushort.Parse(strValue); }
                catch (Exception) { }
            return value;
        }
        public long getLongAttrValue(XmlNode node, string name, long defaultValue)
        {
            string valueStr = getAttrValue(node, name, defaultValue.ToString());
            if (valueStr != null)
            {
                return long.Parse(valueStr);
            }
            else
            {
                throw new ArgumentException();
            }

        }
        public long getLongAttrValue(XmlNode node, string name)
        {
            string valueStr = getAttrValue(node, name, null);
            if (valueStr != null)
            {
                return long.Parse(valueStr);
            }
            else
            {
                throw new ArgumentException();
            }

        }
        public double getDoubleAttrValue(XmlNode node, string name, double defaultValue)
        {
            string valueStr = getAttrValue(node, name, defaultValue.ToString(CultureInfo.InvariantCulture));
            if (valueStr != null)
            {
                return SafeConvert.ToDouble(valueStr);
            }
            else
            {
                throw new ArgumentException();
            }

        }
        public double getDoubleAttrValue(XmlNode node, string name)
        {
            string valueStr = getAttrValue(node, name, null);
            if (valueStr != null)
            {
                return SafeConvert.ToDouble(valueStr);
            }
            else
            {
                throw new ArgumentException();
            }

        }
        public double getDoubleAttrValue(XmlElement element, string name, double min, double max, double def)
        {
            double value = def;
            try
            {
                if (element != null)
                {
                    value = getDoubleAttrValue(element, name);
                    if (value < min)
                    {
                        string strErrFormat =
                            "Attribute value \"{0}\" of element \"{1}\" less than {2}.";
                        throw new Exception(string.Format(strErrFormat, name, element.Name, min));

                    }
                    if (value > max)
                    {
                        string strErrFormat =
                            "Attribute value \"{0}\" of element \"{1}\" more than {2}.";
                        throw new Exception(string.Format(strErrFormat, name, element.Name, max));
                        //value = max;
                    }
                }
                else
                {
                    string strErrFormat = "Element \"{0}\" not found";
                    throw new Exception(string.Format(strErrFormat, element.Name, name, def));
                }
            }
            catch (Exception)
            {
                value = def;
            }
            return value;
        }
        public bool getBoolAttrValue(XmlNode node, string name, bool defaultValue)
        {
            if (node == null)
                return defaultValue;

            string valueStr = getAttrValue(node, name, defaultValue.ToString().ToLower());
            if (valueStr != null)
                return bool.Parse(valueStr);

            return defaultValue;
        }
        public bool getBoolAttrValue(XmlNode node, string name)
        {
            string valueStr = getAttrValue(node, name, null);
            if (valueStr != null)
            {
                return bool.Parse(valueStr);
            }
            else
            {
                throw new ArgumentException();
            }
        }
        public object getEnumAttrValue(XmlElement element, string name, object def, Type type)
        {
            object value = def;
            try
            {
                if (element != null)
                {
                    string strValue = getAttrValue(element, name, def.ToString());
                    try { value = Enum.Parse(type, strValue); }
                    catch (Exception) { }
                }
                else
                {
                    string strErrFormat = "Element \"{0}\" not found.";
                    throw new Exception (string.Format(strErrFormat, element.Name, name, def));
                }
            }
            catch (Exception)
            {
                value = def;
            }
            return value;
        }

        public void setAttrValue(XmlNode node, string name, string value)
        {
            XmlNode attr = node.Attributes.GetNamedItem(name);
            if (attr == null)
            {
                attr = doc.CreateAttribute(name);
                node.Attributes.Append((XmlAttribute)attr);
            }
            attr.
                Value = value;
        }
        public void setIntAttrValue(XmlNode node, string name, int value)
        {
            setAttrValue(node, name, value.ToString());
        }
        public void setUIntAttrValue(XmlNode node, string name, uint value)
        {
            setAttrValue(node, name, value.ToString());
        }
        public void setUShortAttrValue(XmlNode node, string name, uint value)
        {
            setAttrValue(node, name, value.ToString());
        }
        public void setLongAttrValue(XmlNode node, string name, long value)
        {
            setAttrValue(node, name, value.ToString());
        }
        public void setDoubleAttrValue(XmlNode node, string name, double value)
        {
            setAttrValue(node, name, value.ToString(CultureInfo.InvariantCulture));
        }
        public void setBoolAttrValue(XmlNode node, string name, bool value)
        {
            setAttrValue(node, name, value.ToString().ToLower());
        }
    }
}
