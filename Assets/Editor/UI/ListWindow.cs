using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Editor.Scripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ListWindow : EditorWindow
{

    [MenuItem("Window/ListViewExampleWindow")]
    public static void penDemoManual()
    {
        GetWindow<ListWindow>().Show();
    }

    public void OnEnable()
    {
        // Create a list of data. In this case, numbers from 1 to 1000.
        const int itemCount = 10;
        var items = new List<string>(itemCount);
        for (int i = 1; i <= itemCount; i++)
            items.Add(i.ToString());

        // The "makeItem" function is called when the
        // ListView needs more items to render.
        Func<VisualElement> makeItem = () => new Label();

        // As the user scrolls through the list, the ListView object
        // recycles elements created by the "makeItem" function,
        // and invoke the "bindItem" callback to associate
        // the element with the matching data item (specified as an index in the list).
        Action<VisualElement, int> bindItem = (e, i) => (e as Label).text = items[i];

        // Provide the list view with an explicit height for every row
        // so it can calculate how many items to actually display
        const int itemHeight = 16;

        var listView = new ListView(items, itemHeight, makeItem, bindItem)
        {
            // Enables multiple selection using shift or ctrl/cmd keys.
            selectionType = SelectionType.Single
        };

        listView.RefreshItems();
        // Single click triggers "selectionChanged" with the selected items. (f.k.a. "onSelectionChange")
        // Use "selectedIndicesChanged" to get the indices of the selected items instead. (f.k.a. "onSelectedIndicesChange")
        listView.selectionChanged += objects => Debug.Log($"Selected: {string.Join(", ", objects)}");

        // Double-click triggers "itemsChosen" with the selected items. (f.k.a. "onItemsChosen")
        listView.itemsChosen += objects => Debug.Log($"Double-clicked: {string.Join(", ", objects)}");

        listView.style.flexGrow = 1.0f;

        rootVisualElement.Add(listView);
        var button = new Button();
        rootVisualElement.Add(button);
        button.clicked += () =>
        {
            items.RemoveAt(0);
            listView.RefreshItems();
        };
// Registering the callback for the drag-and-drop functionality
        listView.RegisterCallback<MouseDownEvent>(evt =>
        {
            if (evt.clickCount == 1 && evt.button == 0 && listView.selectedIndex >= 0)
            {
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.SetGenericData("ItemIndex", listView.selectedIndex);
                DragAndDrop.StartDrag("Dragging Item");
                evt.StopPropagation();
            }
        });

        listView.RegisterCallback<MouseUpEvent>(evt => DragAndDrop.AcceptDrag());
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        var currentEvent = Event.current;

        if (currentEvent.type == EventType.DragUpdated || currentEvent.type == EventType.DragPerform)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (currentEvent.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();

                int itemIndex = (int)DragAndDrop.GetGenericData("ItemIndex");
                if (itemIndex >= 0)
                {
                    // Create GameObject in the Scene
                    GameObject newGameObject = new GameObject(itemIndex.ToString());
                    Selection.activeObject = newGameObject;
                }
            }

            currentEvent.Use();
        }
    }
}