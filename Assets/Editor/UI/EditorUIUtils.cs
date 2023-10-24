using Editor.Scripts;
using Form;
using Mod;
using UnityEditor;

namespace Editor.UI
{
    public static class EditorUIUtils
    {
        private static ActiveMod activeMod => ServiceLocator.instance.modManager.activeMod;
        private static FormDB formDB => ServiceLocator.instance.formDB;
        
        [MenuItem("ModFile/Save")]
        public static void Save()
        {
            if (activeMod.unsaveChanges) ServiceLocator.instance.modManager.SaveActiveMod();
        }
        
        /**
         * Return true means user make decisions, whether save or dont save, so caller can proceed operation.
         * false means user cancel the operation, caller should abort operation.
         */
        public static bool ValidateSave()
        {
            if (activeMod is null || !activeMod.unsaveChanges) return true;
            string title = "Save Changes?";
            string message = $"Do you want to save the changes you made in mod {activeMod.fname}?";
            string ok = "Save";
            string donTSave = "Don't Save";
            string cancel = "Cancel";

            int option = EditorUtility.DisplayDialogComplex(title, message, ok, donTSave, cancel);

            switch (option)
            {
                case 0: // Save
                    Save();
                    return true;
                case 1: // Don't Save
                    return true;
                case 2: // Cancel
                    return false;
                default:
                    return false;
            }
        }

        /**
         * check editorID valid, if not display warning
         */
        public static bool ValidateEditorID(string editorID)
        {
            var valid = !string.IsNullOrEmpty(editorID) && !formDB.ContainsEditorID(editorID);
            if (!valid)
            {
                EditorUtility.DisplayDialog("Error", "Invalid or existed editorID!", "OK");
            }

            return valid;
        }

        /**
         * check active mod exists, if not display warning
         */
        public static bool ValidateActiveMod()
        {
            var valid = activeMod is not null;
            if (!valid)
            {
                EditorUtility.DisplayDialog("Error", "Cannot edit without active mod!", "OK");
            }

            return valid;
        }
    }
}