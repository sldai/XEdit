using System;
using System.Collections.Generic;
using System.IO;
using Editor.Scripts;
using Mod;
using UnityEngine.UIElements;

namespace Editor.UI.ModFileWindow
{
    public class ModFileTable
    {
        private List<ModManager.ModEntry> modEntryList;
        private MultiColumnListView table;
        private Action<int> setModSelectInd;
        public ModFileTable(List<ModManager.ModEntry> modEntryList, MultiColumnListView table, Action<int> setModSelectInd)
        {
            this.modEntryList = modEntryList;
            this.table = table;
            this.setModSelectInd = setModSelectInd;
            SetupTableController();
        }

        private void SetupTableController()
        {
            table.itemsSource = modEntryList;
            table.selectionChanged += _ => setModSelectInd(table.selectedIndex);
            var enableCol = table.columns["Enable"];
            enableCol.makeCell = () =>
            {
                var toggle = new Toggle();
                toggle.RegisterValueChangedCallback((evt) =>
                {
                    var index = (int)toggle.userData; 
                    modEntryList[index].enable = evt.newValue;
                    table.RefreshItems();
                });
                return toggle;
            };
            enableCol.bindCell = (element, i) =>
            {
                element.userData = i;
                (element as Toggle).value = modEntryList[i].enable;
            };
            
            table.columns["File"].makeCell = () => new Label();
            table.columns["File"].bindCell =
                (element, i) => (element as Label).text = modEntryList[i].fileName;
            
            table.columns["Status"].makeCell = () => new Label();
            table.columns["Status"].bindCell = (element, i) => (element as Label).text = modEntryList[i].active? "Active" : "Not Active";
        }

        public void Render()
        {
            table.RefreshItems();
        }
    }
}