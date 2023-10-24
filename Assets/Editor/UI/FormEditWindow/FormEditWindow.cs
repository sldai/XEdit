using System;
using System.Collections.Generic;
using Form;
using UnityEditor;

namespace UI.FormEditWindow
{
    public class FormEditWindow : EditorWindow
    {
        private static Dictionary<string, Action<BaseForm>> Type2ShowWindow = new()
        {
            { Form.WorldObjects.StaticForm.TYPE, StaticEdit.ShowWindow }
        };
        
        public static void ShowWindow(BaseForm form)
        {
            Type2ShowWindow[form.GetTYPE](form);
        }
    }
}