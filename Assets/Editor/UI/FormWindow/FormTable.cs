using System;
using System.Collections.Generic;
using System.Linq;
using Editor.Scripts;
using Force.DeepCloner;
using Form;
using Mod;
using UI.FormEditWindow;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.UI.FormWindow
{
    public class FormTable
    {
        private FormDB formDB;
        private ActiveMod activeMod => ServiceLocator.instance.modManager.activeMod;
        private List<BaseForm> forms = new(); // forms filtered by type and name, notice sync with formDB
        private string curFormType = ""; // current displayed form type, if no specific type, it is empty 
        private readonly MultiColumnListView table;

        public FormTable(FormDB formDB, MultiColumnListView table)
        {
            this.formDB = formDB;
            this.table = table;
            SetupTableController();
        }

        private void SetupTableController()
        {
            table.itemsSource = forms;
            table.selectionChanged += objects =>
            {
                if (table.selectedIndex<0) return;
                var form = forms[table.selectedIndex];
                Debug.Log(form.editorID);
            };

            // right click pop up context menu
            table.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("New", _ => NewForm(), EnableIfHasActiveModAndType);
                evt.menu.AppendAction("Edit", _ => EditForm(), EnableIfHasActiveModAndSelection);
                evt.menu.AppendAction("Duplicate", _ => DuplicateForm(), EnableIfHasActiveModAndSelection);
                evt.menu.AppendAction("Delete", _ => DeleteForm(), EnableIfHasActiveModAndSelection);
            }));
            return;

            DropdownMenuAction.Status EnableIfHasActiveModAndType(DropdownMenuAction action)
            {
                if (activeMod is null) return DropdownMenuAction.Status.Disabled;
                return !string.IsNullOrEmpty(curFormType) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled;
            }

            DropdownMenuAction.Status EnableIfHasActiveModAndSelection(DropdownMenuAction action)
            {
                if (activeMod is null) return DropdownMenuAction.Status.Disabled;
                return table.selectedIndex < 0 ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Normal;
            }
        }

        /**
         * Create new form and pass it to edit window
         */
        private void NewForm()
        {
            if (string.IsNullOrEmpty(curFormType)) return;
            var formType = FormUtils.Str2Type[curFormType];
            var newForm = Activator.CreateInstance(formType) as BaseForm;
            FormEditWindow.ShowWindow(newForm);
        }

        /**
         * Clone the form and pass it to edit window
         */
        private void EditForm()
        {
            if (table.selectedIndex < 0) return;
            var form = forms[table.selectedIndex];
            FormEditWindow.ShowWindow(form.DeepClone());
        }

        private string GenNewFormName(string basename)
        {
            int cnt = 1;
            string newName = basename;
            while (formDB.ContainsEditorID(newName))
            {
                newName = $"{basename}_${cnt}";
                cnt++;
            }

            return newName;
        }

        private void DuplicateForm()
        {
            if (table.selectedIndex < 0) return;
            var form = forms[table.selectedIndex];
            var dupForm = form.DeepClone();

            dupForm.formID = BaseForm.GenID();
            dupForm.editorID = GenNewFormName($"{dupForm.editorID}_COPY");
            dupForm.createTime = DateTime.Now;
            dupForm.modifyTime = DateTime.Now;
            activeMod?.AddForm(dupForm);
            ServiceLocator.instance.events.TriggerProtoFormChange();
        }

        private void DeleteForm()
        {
            if (table.selectedIndex < 0) return;
            var form = forms[table.selectedIndex];
            activeMod?.DeleteForm(form);
            ServiceLocator.instance.events.TriggerProtoFormChange();
        }

        /**
         * Sync form from formDB,
         * Create table columns based on form type
         * Bind form to table
         */
        public void Render(string path)
        {
            // set displayed form type
            var keys = path.Split(':');
            curFormType = keys.Length >= 2 ? keys[1] : "";
            forms = QueryByPath(formDB, path);
            table.itemsSource = forms;
            var formType = TypeNameFromPath(path);
            CreateColumnsForFormType(formType);
            BindDataToColumns();
            table.RefreshItems();
        }

        private static List<Column> formColumns = new()
        {
            new Column { name = "editorID", title = "EditorID", width = 80 },
            new Column { name = "formID", title = "FormID", width = 60 },
            new Column { name = "model", title = "Model", width = 80 },
            new Column { name = "modifyTime", title = "Modified", width = 70 }
        };

        // select column based type property
        private void CreateColumnsForFormType(string formTypeName)
        {
            table.columns.Clear();
            var fields = FormUtils.TypeName2Fields[formTypeName];
            foreach (var column in formColumns.Where(column => fields.ContainsKey(column.name)))
            {
                table.columns.Add(column);
            }
        }

        private void BindDataToColumns()
        {
            foreach (var column in table.columns)
            {
                column.makeCell = () =>
                {
                    var label = new Label();

                    // right click selection
                    label.RegisterCallback<MouseDownEvent>(evt =>
                    {
                        if (evt.button != (int)MouseButton.RightMouse) return;
                        table.SetSelection((int)label.userData);
                        table.Focus();
                    });

                    return label;
                };

                column.bindCell = (element, i) =>
                {
                    element.userData = i;
                    var form = forms[i];
                    var propertyValue = GetFieldValue(form, column.name);
                    if (column.name == "editorID" && activeMod is not null && activeMod.Contains(form.formID))
                    {
                        (element as Label).text = $"{propertyValue}*";
                    }
                    else
                    {
                        (element as Label).text = propertyValue;
                    }
                };
            }
        }

        private static string GetFieldValue(BaseForm form, string fieldName)
        {
            var fields = FormUtils.TypeName2Fields[form.GetTYPE];
            var field = fields[fieldName];
            return field?.GetValue(form)?.ToString() ?? string.Empty;
        }


        /**
         * Given path like [category, type, ...type specific tag], return type
         */
        private static string TypeNameFromPath(string path)
        {
            var keys = path.Split(':');
            return keys.Length < 2 ? BaseForm.TYPE : keys[1];
        }

        private static List<BaseForm> QueryByPath(FormDB formDB, string path)
        {
            if (path == "All") return formDB.GetAll();
            var keys = path.Split(':');
            if (keys.Length == 1) return formDB.FilterByCategory(keys[0]);
            return formDB.FilterByType(keys[1]);
        }
    }
}