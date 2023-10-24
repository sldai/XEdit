using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Editor.Scripts;
using Mod;
using UnityEditor;
using UnityEngine.UIElements;

namespace Editor.UI.ModFileWindow
{
    public class ModFileWindow : EditorWindow
    {
        private ModManager modManager;

        // modification of modEntry should first reflect on UI state
        // only after click Apply button, the modification sync to modManager
        private List<ModManager.ModEntry> modEntryList = new();
        private AssetDB assetDB;
        private int modSelectInd = -1;

        private static VisualTreeAsset visualTree;
        private ModFileTable modFileTable;
        private Button setActive;
        private Button newMod;
        private TextField authorField;
        private Label create;
        private Label lastUpdate;
        private TextField descriptionField;
        private Button apply;
        private Button cancel;

        [MenuItem("ModFile/Mod")]
        public static void OpenWindow()
        {
            GetWindow<ModFileWindow>().Show();
        }

        void BindData()
        {
            modManager = ServiceLocator.instance.modManager;
            assetDB = ServiceLocator.instance.assetDB;
            SyncModEntryList();
        }

        private void SyncModEntryList()
        {
            modEntryList.Clear();
            foreach (var modEntry in modManager.modEntryList)
            {
                modEntryList.Add(new ModManager.ModEntry()
                {
                    active = modEntry.active, enable = modEntry.enable, fileName = modEntry.fileName,
                    mod = modEntry.mod, size = modEntry.size
                });
            }
        }

        void InitVisualTree()
        {
            if (visualTree == null)
            {
                visualTree =
                    AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI/ModFileWindow/ModFileWindow.uxml");
            }

            rootVisualElement.Add(visualTree.Instantiate());
        }

        private void CreateGUI()
        {
            BindData();
            InitVisualTree();
            modFileTable = new ModFileTable(modEntryList, rootVisualElement.Q<MultiColumnListView>("ModTable"),
                SetModSelectIndex);
            setActive = rootVisualElement.Q<Button>("SetActive");
            setActive.clicked += () =>
            {
                if (modSelectInd < 0) return;
                foreach (var modEntry in modEntryList)
                {
                    modEntry.active = false;
                }

                modEntryList[modSelectInd].active = true;
                modEntryList[modSelectInd].enable = true;
                modFileTable.Render();
            };
            newMod = rootVisualElement.Q<Button>("NewMod");
            newMod.clicked += () =>
            {
                modManager.NewMod();
                SyncModEntryList();
                modFileTable.Render();
            };
            authorField = rootVisualElement.Q<TextField>("AuthorField");
            authorField.RegisterValueChangedCallback(evt =>
            {
                modEntryList[modSelectInd].mod.metadata.author = evt.newValue;
                modManager.SaveModMetadata(modSelectInd);
            });
            create = rootVisualElement.Q<Label>("Create");
            lastUpdate = rootVisualElement.Q<Label>("LastUpdate");
            descriptionField = rootVisualElement.Q<TextField>("DescriptionField");
            descriptionField.RegisterValueChangedCallback(evt =>
            {
                modEntryList[modSelectInd].mod.metadata.description = evt.newValue;
                modManager.SaveModMetadata(modSelectInd);
            });

            apply = rootVisualElement.Q<Button>("Apply");
            cancel = rootVisualElement.Q<Button>("Cancel");
            apply.clicked += () =>
            {
                if (!EditorUIUtils.ValidateSave()) return;
                var enableInds = new HashSet<int>();
                for (int i = 0; i < modEntryList.Count; i++)
                {
                    if (modEntryList[i].enable) enableInds.Add(i);
                }

                var activeInd = -1;
                for (int i = 0; i < modEntryList.Count; i++)
                {
                    if (modEntryList[i].active) activeInd = i;
                }

                modManager.Load(enableInds, activeInd).Forget();
                modEntryList = null;
                Close();
            };
            cancel.clicked += Close;
        }

        private void SetModSelectIndex(int ind)
        {
            modSelectInd = ind;
            RenderSelectedMod();
        }

        public void Render()
        {
            modFileTable.Render();
            RenderSelectedMod();
        }

        private void RenderSelectedMod()
        {
            if (modSelectInd < 0) return;
            var mod = modEntryList[modSelectInd].mod;
            authorField.SetValueWithoutNotify(mod.metadata?.author ?? "");
            create.text = mod.metadata?.modifyTime.ToString() ?? "";
            lastUpdate.text = mod.metadata?.modifyTime.ToString() ?? "";
            descriptionField.SetValueWithoutNotify(mod.metadata?.description ?? "");
        }
    }
}