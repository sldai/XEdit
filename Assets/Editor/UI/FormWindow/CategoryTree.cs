using System;
using System.Collections.Generic;
using System.Linq;
using Form;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.UI.FormWindow
{
    /**
     * Render tree hierarchy of forms. Category is the top level roots, type is the second level, then each type has own
     * subtree hierarchy based on their schema.
     * When formDB changes, Render function should be called so that it can rebuild the tree hierarchy
     */
    public class CategoryTree
    {
        readonly FormDB formDB;

        // item data is the path from root the node
        // WorldObjects
        // |- Static
        //    |- Stone
        // The data of Stone is "WorldObjects:Static:Stone"
        private List<TreeViewItemData<string>> categoryRoots = new List<TreeViewItemData<string>>();
        
        private TreeView treeView;

        public CategoryTree(FormDB formDB, TreeView treeView, Action<string> setFilterPath)
        {
            this.formDB = formDB;
            this.treeView = treeView;
            InitCreateSubTreeFunc();
            BuildCategoryRoots();
            SetupTreeController(setFilterPath);
        }


        private void SetupTreeController(Action<string> setFilterPath)
        {
            treeView.SetRootItems(categoryRoots);
            treeView.makeItem = () => new Label();
            treeView.bindItem = (element, index) =>
            {
                element.userData = index;
                (element as Label).text = treeView.GetItemDataForIndex<string>(index).Split(':')[^1];
            };
            treeView.selectionChanged += _ =>
            {
                if (treeView.selectedIndex<0) return;
                var path = treeView.GetItemDataForIndex<string>(treeView.selectedIndex);
                setFilterPath(path);
                Debug.Log(path);
            };
        }
        

        public void Render()
        {
            BuildCategoryRoots();
            treeView.RefreshItems();
        }

        private int treeViewId = 0;

        private void BuildCategoryRoots()
        {
            categoryRoots.Clear();
            treeViewId = 0;
            foreach (var c in FormUtils.CategoryTypes.Keys)
            {
                if (c==BaseForm.NONE_CATEGORY) continue;
                var typeNodes = FormUtils.CategoryTypes[c].Select(CreateTypeSubTree).ToList();
                var categoryNode = new TreeViewItemData<string>(treeViewId++, c, typeNodes);
                categoryRoots.Add(categoryNode);
            }
            categoryRoots.Add(new TreeViewItemData<string>(treeViewId++, "All")); // display all forms 
        }

        private Dictionary<string, Func<TreeViewItemData<string>>> CreateTypeSubTreeFunc;

        void InitCreateSubTreeFunc()
        {
            CreateTypeSubTreeFunc = new Dictionary<string, Func<TreeViewItemData<string>>>
            {
                { Form.WorldObjects.StaticForm.TYPE, CreateSubTreeStatic },
            };
        }

        private TreeViewItemData<string> CreateTypeSubTree(string formType)
        {
            return CreateTypeSubTreeFunc[formType]();
        }

        private TreeViewItemData<string> CreateSubTreeStatic()
        {
            return new TreeViewItemData<string>(treeViewId++,
                Form.WorldObjects.StaticForm.CATEGORY + ":" + Form.WorldObjects.StaticForm.TYPE);
        }
    }
}