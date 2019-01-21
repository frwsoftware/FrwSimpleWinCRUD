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

namespace FrwSoftware
{
   
    public enum ViewMode
    {
        View, // view detail of the record fields
        Edit,
        Delete,
        New, // create a new record
        List,
        ViewContent, // for example, viewing the contents of a record with a double click
        RemoveFrom // eg deleting an entry from the current category
    }

    public interface IContent
    {
        bool HideOnClose { get; set; }// the window is not removed from memory when closing (necessary for caching the window state)
        IDictionary<string, object> GetKeyParams();
        void SetKeyParams(IDictionary<string, object> pars);
        void SaveConfig();
        void ClosingContent();//for Clean up any resources being used (use this method instead Dispose())
        bool CompareKeyParams(IDictionary<string, object> pars);
        IContentContainer ContentContainer { get; set; }
    }

    public interface IViewProcessor : IContent
    {
        void CreateView();
        void ProcessView();
    }

    public interface IParentView 
    {
        string PaneUID { get; set; }
        // the unique identifier of the window instance, is needed to bind the list windows and the child view when saving the configuration  
        IEnumerable<IChildView> ChildViews { get; }
        void AddChildView(IChildView view);
        bool ContainsChildView(IChildView view);
        void RemoveChildView(IChildView view);
        event ObjectSelectEventHandler OnObjectSelectEvent;
    }


    public interface IChildView 
    {
        string RelPaneUID { get; set; }// reference to the list box identifier 
        event ChildObjectUpdateEventHandler ChildObjectUpdateEvent;
        void RegisterAsChildView(IParentView parent);
        void UnRegisterAsChildView(IParentView parent);
    }

    public interface IObjectViewProcessor : IViewProcessor
    {
        Type SourceObjectType { get; set; }
    }

    public interface IListProcessor : IObjectViewProcessor
    {
        object FilteredObject { get; set; }
        IList SelectedObjects { get; }
        IEnumerable Objects { get; set; }
        void RefreshObject(object o);
        void RefreshList();
        void RemoveSelectedItems();
    }

    public interface IPropertyProcessor : IObjectViewProcessor
    {
        ViewMode ViewMode { get; set; }
        object SourceObject { get; set; }
        bool SaveChanges();
        bool CancelChanges();

    }
}
