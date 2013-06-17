﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gum.DataTypes;
using Gum.Managers;
using Gum.DataTypes.Variables;
using System.Windows.Forms;
using Gum.Wireframe;
using Gum.Plugins;
using Gum.Debug;
using System.Collections.ObjectModel;

namespace Gum.ToolStates
{
    public class SelectedState : ISelectedState
    {
        #region Fields

        static ISelectedState mSelf;
        ReadOnlyCollection<InstanceSave> mSelectedInstancesReadOnly;
        List<InstanceSave> mSelectedInstances = new List<InstanceSave>();
        
        #endregion

        #region Properties

        public static ISelectedState Self
        {
            // We usually won't use this in the actual product, but useful for testing
            set
            {
                mSelf = value;
            }
            get
            {
                if (mSelf == null)
                {
                    mSelf = new SelectedState();
                }
                return mSelf;
            }
        }

        public ScreenSave SelectedScreen
        {
            get
            {
                TreeNode treeNode = ElementTreeViewManager.Self.SelectedNode;

                while(treeNode != null)
                {
                    if(treeNode.IsScreenTreeNode())
                    {
                        return treeNode.Tag as ScreenSave;
                    }
                    else if (!treeNode.IsTopElementContainerTreeNode())
                    {
                        treeNode = treeNode.Parent;
                    }
                    else
                    {
                        return null;
                    }
                }
                return null;
            }
            set
            {

                // We don't want this to unset selected components or standards if this is set to null
                if (value != SelectedScreen && ( value != null || SelectedScreen == null || SelectedScreen is ScreenSave))
                {
                    ElementTreeViewManager.Self.Select(value);
                }
            }
        }

        public ComponentSave SelectedComponent
        {
            get
            {
                TreeNode treeNode = ElementTreeViewManager.Self.SelectedNode;

                while (treeNode != null)
                {
                    if (treeNode.IsComponentTreeNode())
                    {
                        return treeNode.Tag as ComponentSave;
                    }
                    else if (!treeNode.IsTopElementContainerTreeNode())
                    {
                        treeNode = treeNode.Parent;
                    }
                    else
                    {
                        return null;
                    }
                }
                return null;
            }
            set
            {
                if (value != SelectedComponent && (value != null || SelectedComponent == null || SelectedComponent is ComponentSave))
                {
                    ElementTreeViewManager.Self.Select(value);
                }
            }
        }

        public StandardElementSave SelectedStandardElement
        {
            get
            {
                TreeNode treeNode = ElementTreeViewManager.Self.SelectedNode;

                while (treeNode != null)
                {
                    if (treeNode.IsStandardElementTreeNode())
                    {
                        return treeNode.Tag as StandardElementSave;
                    }
                    else if (!treeNode.IsTopElementContainerTreeNode())
                    {
                        treeNode = treeNode.Parent;
                    }
                    else
                    {
                        return null;
                    }
                }
                return null;
            }
            set
            {
                if (value != SelectedStandardElement && (value != null || SelectedStandardElement == null || SelectedStandardElement is StandardElementSave))
                {
                    ElementTreeViewManager.Self.Select(value);
                }
            }
        }

        public ElementSave SelectedElement
        {
            get
            {
                if (SelectedScreen != null)
                {
                    return SelectedScreen;
                }
                else if (SelectedComponent != null)
                {
                    return SelectedComponent;
                }
                else if (SelectedStandardElement != null)
                {
                    return SelectedStandardElement;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != SelectedElement)
                {
                    if (value == null)
                    {
                        SelectedScreen = null;
                        SelectedStandardElement = null;
                        SelectedComponent = null;
                    }
                    else if (value is ScreenSave)
                    {
                        SelectedScreen = value as ScreenSave;
                    }
                    else if (value is ComponentSave)
                    {
                        SelectedComponent = value as ComponentSave;
                    }
                    else if (value is StandardElementSave)
                    {
                        SelectedStandardElement = value as StandardElementSave;
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
        }

        public StateSave SelectedStateSave
        {
            get
            {
                
                if (StateTreeViewManager.Self.SelectedNode != null)
                {
                    return StateTreeViewManager.Self.SelectedNode.Tag as StateSave;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                StateTreeViewManager.Self.Select(value);
            }
        }

        public InstanceSave SelectedInstance
        {
            get
            {
                if (ElementTreeViewManager.Self.SelectedNode != null && ElementTreeViewManager.Self.SelectedNode.IsInstanceTreeNode())
                {
                    return ElementTreeViewManager.Self.SelectedNode.Tag as InstanceSave;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != SelectedInstance)
                {
                    if (value != null)
                    {
                        ElementSave parent = value.ParentContainer;

                        ElementTreeViewManager.Self.Select(value, parent);
                    }
                    else if (value == null && SelectedInstance != null)
                    {
                        ElementSave selected = SelectedElement;

                        ElementTreeViewManager.Self.SelectedNode = null;

                        SelectedElement = selected;
                    }
                }
            }
        }

        public IEnumerable<InstanceSave> SelectedInstances
        {
            get
            {
                mSelectedInstances.Clear();

                foreach (TreeNode node in ElementTreeViewManager.Self.SelectedNodes)
                {
                    if (node.IsInstanceTreeNode())
                    {
                        mSelectedInstances.Add(node.Tag as InstanceSave);
                    }
                }

                return mSelectedInstancesReadOnly;
            }
            set
            {
                mSelectedInstances.Clear();
                foreach (var item in value)
                {
                    mSelectedInstances.Add(item);
                }

                ElementTreeViewManager.Self.Select(mSelectedInstances);
            }

        }

        public VariableSave SelectedVariableSave
        {
            get
            {
                if (SelectedStateSave != null)
                {
                    string variableName = PropertyGridManager.Self.SelectedLabel;
                    if (SelectedInstance != null)
                    {
                        variableName = SelectedInstance.Name + "." + variableName;
                    }
                    return SelectedStateSave.GetVariableSave(variableName);
                }
                else
                {
                    return null;
                }
            }

        }

        public TreeNode SelectedTreeNode
        {
            get
            {
                return ElementTreeViewManager.Self.SelectedNode;
            }
        }

        public RecursiveVariableFinder SelectedRecursiveVariableFinder
        {
            get
            {
                if (SelectedInstance != null)
                {
                    return new RecursiveVariableFinder(SelectedInstance, SelectedElement);
                }
                else
                {
                    return new RecursiveVariableFinder(SelectedStateSave);
                }
            }
        }

        #endregion

        private SelectedState()
        {
            mSelectedInstancesReadOnly = new ReadOnlyCollection<InstanceSave>(mSelectedInstances);

        }

        public void UpdateToSelectedElement()
        {
            StateTreeViewManager.Self.RefreshUI(SelectedElement);

            if (SelectedElement != null && (SelectedStateSave == null || SelectedElement.States.Contains(SelectedStateSave) == false))
            {
                SelectedStateSave = SelectedElement.States[0];
            }
            else if (SelectedElement == null)
            {
                SelectedStateSave = null;

            }
            PropertyGridManager.Self.RefreshUI();

            WireframeObjectManager.Self.RefreshAll(false);

            SelectionManager.Self.Refresh();
            
            MenuStripManager.Self.RefreshUI();

            PluginManager.Self.ElementSelected(SelectedElement);

        }

        public void UpdateToSelectedStateSave()
        {

            WireframeObjectManager.Self.RefreshAll(true);

            SelectionManager.Self.Refresh();

            StateTreeViewManager.Self.Select(SelectedStateSave);

            PropertyGridManager.Self.RefreshUI();

            MenuStripManager.Self.RefreshUI();

            ProjectVerifier.Self.AssertSelectedIpsosArePartOfRenderer();
        }

        public void UpdateToSelectedInstanceSave()
        {
            if (SelectedInstance != null)
            {
                ElementSave parent = SelectedInstance.ParentContainer;

                ProjectVerifier.Self.AssertIsPartOfProject(parent);

                SelectedElement = SelectedInstance.ParentContainer;
            }
            
            StateTreeViewManager.Self.RefreshUI(SelectedElement);

            if (SelectedElement != null && (SelectedStateSave == null || SelectedElement.States.Contains(SelectedStateSave) == false))
            {
                SelectedStateSave = SelectedElement.States[0];
            }

            if (WireframeObjectManager.Self.ElementShowing != this.SelectedElement)
            {
                WireframeObjectManager.Self.RefreshAll(false);
            }

            SelectionManager.Self.Refresh();

            MenuStripManager.Self.RefreshUI();

            PropertyGridManager.Self.RefreshUI();

            PluginManager.Self.InstanceSelected(SelectedElement, SelectedInstance);
        }
    }

    public static class IEnumerableExtensionMethods
    {
        public static int GetCount(this IEnumerable<InstanceSave> enumerable)
        {
            int toReturn = 0;


            foreach (var item in enumerable)
            {
                toReturn++;
            }

            return toReturn;
        }





    }


}