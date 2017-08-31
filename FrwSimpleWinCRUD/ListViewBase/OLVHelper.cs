using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using static BrightIdeasSoftware.ObjectListView;

namespace FrwSoftware
{
    public enum OLVHotItemStyle
    {
        None = 0,
        TextColor = 1,
        Border = 2,
        Translucent = 3,
        Lightbox = 4
    }
    public static class OLVHelper
    {

        public static void SetHotItemStyle(ObjectListView listView, OLVHotItemStyle style)
        {
            listView.UseTranslucentHotItem = false;
            listView.UseHotItem = true;
            listView.UseExplorerTheme = false;

            switch ((int)style)
            {
                case 0:
                    listView.UseHotItem = false;
                    break;
                case 1:
                    HotItemStyle hotItemStyle = new HotItemStyle();
                    hotItemStyle.ForeColor = Color.AliceBlue;
                    hotItemStyle.BackColor = Color.FromArgb(255, 64, 64, 64);
                    listView.HotItemStyle = hotItemStyle;
                    break;
                case 2:
                    RowBorderDecoration rbd = new RowBorderDecoration();
                    rbd.BorderPen = new Pen(Color.SeaGreen, 2);
                    rbd.FillBrush = null;
                    rbd.CornerRounding = 4.0f;
                    HotItemStyle hotItemStyle2 = new HotItemStyle();
                    hotItemStyle2.Decoration = rbd;
                    listView.HotItemStyle = hotItemStyle2;
                    break;
                case 3:
                    listView.UseTranslucentHotItem = true;
                    break;
                case 4:
                    HotItemStyle hotItemStyle3 = new HotItemStyle();
                    hotItemStyle3.Decoration = new LightBoxDecoration();
                    listView.HotItemStyle = hotItemStyle3;
                    break;
                case 5:
                    listView.FullRowSelect = true;
                    listView.UseHotItem = false;
                    listView.UseExplorerTheme = true;
                    break;
            }
            listView.Invalidate();
        }

        #region Save/Restore State

        // overrides the OLV save state
        // uses JSON instead of a byte array
        // adds some parameters
        // also works with a different number of columns in the saved and current list
        public static void SaveStateAdvanced(ObjectListView olv, OLVStateAdv olvState)
        {
            olvState.VersionNumber = 1;
            olvState.NumberOfColumns = olv.AllColumns.Count;
            olvState.CurrentView = olv.View;

            // If we have a sort column, it is possible that it is not currently being shown, in which
            // case, it's Index will be -1. So we calculate its index directly. Technically, the sort
            // column does not even have to a member of AllColumns, in which case IndexOf will return -1,
            // which is works fine since we have no way of restoring such a column anyway.
            if (olv.PrimarySortColumn != null)
                olvState.SortColumn = olv.AllColumns.IndexOf(olv.PrimarySortColumn);
            olvState.LastSortOrder = olv.PrimarySortOrder;
            olvState.IsShowingGroups = olv.ShowGroups;

            //
            olvState.RowHeight = olv.RowHeight;
            olvState.CellEditActivation = olv.CellEditActivation;
            //
            if (olv.AllColumns.Count > 0 && olv.AllColumns[0].LastDisplayIndex == -1)
                RememberDisplayIndicies(olv);

            foreach (OLVColumn column in olv.AllColumns)
            {
                OLVColumnStateAdv columnState = new OLVColumnStateAdv();
                columnState.Name = column.Name;
                columnState.IsVisible = column.IsVisible;
                columnState.LastDisplayIndex = column.LastDisplayIndex;
                columnState.Width = column.Width;
                olvState.Сolumns.Add(columnState);
            }

        }

        /// <summary>
        /// Restore the state of the control from the given string, which must have been
        /// produced by SaveState()
        /// </summary>
        /// <param name="state">A byte array returned from SaveState()</param>
        /// <returns>Returns true if the state was restored</returns>
        public static void RestoreStateAdvanced(ObjectListView olv, OLVStateAdv olvState)
        {
            // The number of columns has changed. We have no way to match old
            // columns to the new ones, so we just give up.
            if (olvState == null)
                //|| olvState.NumberOfColumns != ovl.AllColumns.Count)//
                return;
            if (olvState.SortColumn == -1)
            {
                olv.PrimarySortColumn = null;
                olv.PrimarySortOrder = SortOrder.None;
            }
            else
            {
                if (olvState.SortColumn < olv.AllColumns.Count)//
                {
                    olv.PrimarySortColumn = olv.AllColumns[olvState.SortColumn];
                    olv.PrimarySortOrder = olvState.LastSortOrder;
                }
            }
            //
            olv.RowHeight = olvState.RowHeight;
            olv.CellEditActivation = olvState.CellEditActivation;
            //
            foreach(var columnState in olvState.Сolumns)
            {
                foreach(var column in olv.AllColumns)
                {
                    if (column.Name.Equals(columnState.Name))
                    {
                        column.IsVisible = columnState.IsVisible;
                        column.LastDisplayIndex = columnState.LastDisplayIndex;
                        column.Width = columnState.Width;
                        break;
                    }
                }
            }

            // ReSharper disable RedundantCheckBeforeAssignment
            if (olvState.IsShowingGroups != olv.ShowGroups)
                // ReSharper restore RedundantCheckBeforeAssignment
                olv.ShowGroups = olvState.IsShowingGroups;
            if (olv.View == olvState.CurrentView)
                olv.RebuildColumns();
            else
                olv.View = olvState.CurrentView;
        }
        static private void RememberDisplayIndicies(ObjectListView ovl)
        {
            // Remember the display indexes so we can put them back at a later date
            foreach (OLVColumn x in ovl.AllColumns)
                x.LastDisplayIndex = x.DisplayIndex;
        }
        #endregion
    }
    public class OLVStateAdv
    {
        public int VersionNumber { get; set; } = 1;
        public int NumberOfColumns { get; set; }  = 1;
        public View CurrentView { get; set; }
        public int SortColumn { get; set; } = -1;
        public bool IsShowingGroups { get; set; }
        public OLVHotItemStyle HotItemStyle { get; set; }
        public SortOrder LastSortOrder { get; set; } = SortOrder.None;
        public List<OLVColumnStateAdv> Сolumns { get; set; }  = new List<OLVColumnStateAdv>();
        ///
        public int RowHeight { get; set; }  = -1; //Setting it to -1 means use the normal calculation method
        public CellEditActivateMode CellEditActivation { get; set; }  = ObjectListView.CellEditActivateMode.DoubleClick;
        private IDictionary<string, object> userSettings = new Dictionary<string, object>();
        public IDictionary<string, object> UserSettings { get { return userSettings; } set { userSettings = value; } }
    }
    public class OLVColumnStateAdv
    {
        public string Name { get; set; } 
        public bool IsVisible { get; set; }
        public int LastDisplayIndex { get; set; }
        public int Width { get; set; }
    }
}