using System;
using System.Collections.Generic;
using Editor.Scripts;
using Form;
using UnityEditor;
using UnityEngine.UIElements;

namespace Editor.UI.FormWindow
{
    public class FormWindow : EditorWindow
    {
        private List<Action> disposeBag = new ();
        private FormDB formDB;

        private string filterPath; // "Actors:Actor" display Actor type forms or "All" display all forms
        private string filterName; // regex match editorID
        
        private static VisualTreeAsset visualTree;
        private CategoryTree categoryTree;
        private FormTable formTable;
        
        [MenuItem("ModView/FormWindow")]
        public static void OpenWindow()
        {
            GetWindow<FormWindow>().Show();
        }

        void BindData()
        {
            formDB = ServiceLocator.instance.formDB;
        }
        
        void InitVisualTree()
        {
            if (visualTree == null)
            {
                visualTree =
                    AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI/FormWindow/FormWindow.uxml");
            }
            visualTree.CloneTree(rootVisualElement);
        }

        void SubscribeEvent()
        {
            ServiceLocator.instance.events.onProtoFormChange += Render;
            disposeBag.Add(()=>ServiceLocator.instance.events.onProtoFormChange -= Render);
        }
        
        private void OnDestroy()
        {
            foreach (var disposeFunc in disposeBag)
            {
                disposeFunc();
            }
        }

        private void CreateGUI()
        {
            InitVisualTree();
            BindData();
            SubscribeEvent();
            categoryTree = new CategoryTree(formDB, rootVisualElement.Q<TreeView>("CategoryTree"), (path) =>
            {
                filterPath = path;
                formTable.Render(filterPath);
            });
            formTable = new FormTable(formDB, rootVisualElement.Q<MultiColumnListView>("FormTable"));
        }

        private void Render()
        {
            categoryTree.Render();
            formTable.Render(filterPath);
        }
    }
}