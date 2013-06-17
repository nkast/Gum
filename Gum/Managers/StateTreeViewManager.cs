﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonFormsAndControls;
using Gum.DataTypes;
using Gum.DataTypes.Variables;
using System.Windows.Forms;
using Gum.ToolStates;

namespace Gum.Managers
{
    public partial class StateTreeViewManager
    {
        #region Fields

        static StateTreeViewManager mSelf;

        MultiSelectTreeView mTreeView;

        ElementSave mLastElementRefreshedTo;

        #endregion

        #region Properties

        public static StateTreeViewManager Self
        {
            get
            {
                if (mSelf == null)
                {
                    mSelf = new StateTreeViewManager();
                }
                return mSelf;
            }
        }

        public TreeNode SelectedNode
        {
            get { return mTreeView.SelectedNode; }
        }

        #endregion

        public TreeNode GetTreeNodeFor(StateSave stateSave)
        {
            if (stateSave == null)
            {
                return null;
            }
            // Will need to expand this when we add categories
            foreach (TreeNode node in mTreeView.Nodes)
            {
                if (node.Tag == stateSave)
                {
                    return node;
                }
            }
            return null;
        }

        public void Initialize(MultiSelectTreeView treeView)
        {
            mTreeView = treeView;
        }

        internal void OnSelect()
        {

            TreeNode treeNode = mTreeView.SelectedNode;

            object selectedObject = null;

            if (treeNode != null)
            {
                selectedObject = treeNode.Tag;
            }

            if (selectedObject == null)
            {
                // What do we do?  This is invalid.  A State should always be selected...
                // What we do is select the first one if it exists
                if (mTreeView.Nodes.Count != 0)
                {
                    selectedObject = mTreeView.Nodes[0].Tag;
                    mTreeView.SelectedNode = mTreeView.Nodes[0];
                }
            }

            SelectedState.Self.UpdateToSelectedStateSave();

        }

        public void Select(StateSave stateSave)
        {
            TreeNode treeNode = GetTreeNodeFor(stateSave);

            Select(treeNode);

        }

        public void Select(TreeNode treeNode)
        {
            if (mTreeView.SelectedNode != treeNode)
            {
                mTreeView.SelectedNode = treeNode;

                if (treeNode != null)
                {
                    treeNode.EnsureVisible();
                }
            }
        }

        public void RefreshUI(ElementSave element)
        {

            bool changed = element != mLastElementRefreshedTo;

            mLastElementRefreshedTo = element;

            StateSave lastStateSave = SelectedState.Self.SelectedStateSave;


            if (element != null)
            {
                while (mTreeView.Nodes.Count > element.States.Count)
                {
                    mTreeView.Nodes.RemoveAt(mTreeView.Nodes.Count - 1);
                }
                while (mTreeView.Nodes.Count < element.States.Count)
                {
                    mTreeView.Nodes.Add(new TreeNode());
                }

                bool wasAnythingSelected = false;
                for(int i = 0; i < element.States.Count; i++)
                {
                    StateSave state = element.States[i];

                    if (mTreeView.Nodes[i].Text != state.Name)
                    {
                        mTreeView.Nodes[i].Text = state.Name;
                    }
                    if (mTreeView.Nodes[i].Tag != state)
                    {
                        mTreeView.Nodes[i].Tag = state;
                    }

                    if (state == lastStateSave)
                    {
                        SelectedState.Self.SelectedStateSave = state;

                        wasAnythingSelected = true;
                    }
                }

                if (wasAnythingSelected == false)
                {
                    SelectedState.Self.SelectedStateSave = element.States[0];

                }
            }
            else
            {
                mTreeView.Nodes.Clear();
            }


        }


    }
}