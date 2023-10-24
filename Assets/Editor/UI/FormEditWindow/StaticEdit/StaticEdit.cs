using System;
using Editor.Scripts;
using Editor.UI;
using Form;
using Form.WorldObjects;
using Mod;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

public class StaticEdit : EditorWindow
{
    [SerializeField] private VisualTreeAsset visualTreeAsset;

    private ActiveMod activeMod;
    private FormDB formDB;
    private StaticForm form;

    private TextField editorIDField;
    private Button apply;
    private Button cancel;

    public static void ShowWindow(BaseForm form)
    {
        if (form is null) return;
        var window = GetWindow<StaticEdit>();
        window.form = form as StaticForm;
        window.Render();
    }

    private void BindData()
    {
        activeMod = ServiceLocator.instance.modManager.activeMod;
        formDB = ServiceLocator.instance.formDB;
    }

    public void CreateGUI()
    {
        BindData();
        InitVisualTree();
        apply = rootVisualElement.Q<Button>("Apply");
        cancel = rootVisualElement.Q<Button>("Cancel");

        apply.clicked += ApplyChange;
        cancel.clicked += CancelChange;

        editorIDField = rootVisualElement.Q<TextField>("EditorIDField");
        editorIDField.RegisterValueChangedCallback(evt => form.editorID = evt.newValue);
    }

    private void InitVisualTree()
    {
        var root = rootVisualElement;
        root.Add(visualTreeAsset.Instantiate());
    }

    private void ApplyChange()
    {
        if (!EditorUIUtils.ValidateEditorID(form.editorID)) return;

        Assert.IsNotNull(activeMod);
        form.modifyTime = DateTime.Now;
        if (!activeMod.Contains(form.formID)) activeMod.AddForm(form);
        else activeMod.EditForm(form);
        ServiceLocator.instance.events.TriggerProtoFormChange();
        Close();
    }

    private void CancelChange()
    {
        Close();
    }

    public void Render()
    {
        editorIDField.SetValueWithoutNotify(form.editorID);
    }
}