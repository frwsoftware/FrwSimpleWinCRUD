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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FrwSoftware
{

    #region JModel Attributes
    [AttributeUsage(AttributeTargets.Property)]
    public class JPrimaryKey : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class JAutoIncrement : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class JInitCurrentDate : Attribute
    {
    }
    /// <summary>
    /// This field is used as the base for listing
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class JNameProperty : Attribute
    {
    }
    /// <summary>
    /// This field is used as the base for listing (prior to JNameProperty) 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class JShortNameProperty : Attribute
    {
    }
    //todo
    [AttributeUsage(AttributeTargets.Property)]
    public class JRequired : Attribute
    {
    }
    //todo
    [AttributeUsage(AttributeTargets.Property)]
    public class JUnique : Attribute
    {
    }
    /// <summary>
    /// The field is ignored on output in lists and forms
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class JIgnore: Attribute
    {
    }
    /// <summary>
    /// Field that can not be edited in lists and forms
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class JReadOnly : Attribute
    {
    }

    //localized DysplayNameAttribute
    [AttributeUsage(AttributeTargets.All)]
    public class JDisplayName : DisplayNameAttribute
    {
        public JDisplayName(string description) : base(description)
        {
            
        }
        public JDisplayName(Type resourceManagerProvider, string descriptionResourceKey) 
            : base(AttrUtils.LookupResource(resourceManagerProvider, descriptionResourceKey))
        {
            
        }
     
    }

 
    [AttributeUsage(AttributeTargets.Property)]
    public class JHeaderImage : Attribute
    {
        public JHeaderImage(string headerImageName)
        {
            this.HeaderImageName = headerImageName;
        }
        public string HeaderImageName { get; set; }
    }
    #region method attributes
    [AttributeUsage(AttributeTargets.Method)]
    public class JValidate : Attribute
    {
    }
    #endregion

    public class AttrUtils
    {
        internal static string LookupResource(Type resourceManagerProvider, string resourceKey)
        {
            foreach (PropertyInfo staticProperty in resourceManagerProvider.GetProperties(BindingFlags.Static | BindingFlags.NonPublic))
            {
                if (staticProperty.PropertyType == typeof(System.Resources.ResourceManager))
                {
                    System.Resources.ResourceManager resourceManager = (System.Resources.ResourceManager)staticProperty.GetValue(null, null);
                    return resourceManager.GetString(resourceKey);
                }
            }

            return resourceKey; // Fallback with the key name
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class JEntity : Attribute
    {
        public JEntity()
        {
            this.CustomLoad = false;
        }
        public JEntity(bool customLoad)
        {
            this.CustomLoad = customLoad;
        }
        public bool CustomLoad { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class JUrl : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class JAttachments : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class JText : Attribute
    {
    }
    public enum DisplyPropertyStyle
    {
        TextOnly,
        ImageOnly,
        TextAndImage
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class JImageName : Attribute
    {
        public JImageName()
        {
            DictPropertyStyle = DisplyPropertyStyle.TextOnly;
        }
        public JImageName(DisplyPropertyStyle dictPropertyStyle)
        {
            DictPropertyStyle = dictPropertyStyle;
        }
        public DisplyPropertyStyle DictPropertyStyle { get; set; }
    }


    [AttributeUsage(AttributeTargets.Property)]
    public class JImageRef : Attribute
    {
        public JImageRef(DisplyPropertyStyle dictPropertyStyle)
        {
            DictPropertyStyle = dictPropertyStyle;
        }
        public JImageRef(DisplyPropertyStyle dictPropertyStyle, string imagePropertyName)
        {
            this.ImagePropertyName = imagePropertyName;
            DictPropertyStyle = dictPropertyStyle;
        }
        public string ImagePropertyName { get; set; }
        public DisplyPropertyStyle DictPropertyStyle { get; set; }
    }

    #region dictionary
    [AttributeUsage(AttributeTargets.Property)]
    public class JDictProp : Attribute
    {
        public JDictProp(string id, bool allowMultiValues, DisplyPropertyStyle dictPropertyStyle)
        {
            Id = id;
            AllowMultiValues = allowMultiValues;
            DictPropertyStyle = dictPropertyStyle;
        }
        public string Id { get; set; }
        public bool AllowMultiValues { get; set; }
        public DisplyPropertyStyle DictPropertyStyle { get; set; }
    }
    #endregion

    #region relation

    [AttributeUsage(AttributeTargets.Property)]
    public class JOneToOne : Attribute
    {
    }


    [AttributeUsage(AttributeTargets.Property)]
    public class JManyToOne : Attribute
    {
        public JManyToOne()
        {
        }
        public JManyToOne(string refFieldNameInForeinEntity)
        {
            RefFieldNameInForeinEntity = refFieldNameInForeinEntity;
        }
        public string RefFieldNameInForeinEntity { get; set; }//может быть равен null, тогда считается что ref единственный, который совпадает по типу 

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class JOneToMany : Attribute
    {
        public JOneToMany()
        {
        }
        public JOneToMany(string refFieldNameInForeinEntity)
        {
            RefFieldNameInForeinEntity = refFieldNameInForeinEntity;
        }
        public string RefFieldNameInForeinEntity { get; set; }//может быть равен null, тогда считается что ref единственный, который совпадает по типу 
    }
    
    [AttributeUsage(AttributeTargets.Property)]
    public class JManyToMany : Attribute
    {
        public JManyToMany()
        {
        }
        public JManyToMany(string joinTableName)
        {
            JoinName = joinTableName;
        }
        //Name of the cross-link (must match in the reference JManyToMany fields of both entities)
        //Can be null, then it is assumed that the ref fields in both entities are the only ones that match the type with the entity referenced
        public string JoinName { get; set; }
    }
    #endregion

    #endregion
}
