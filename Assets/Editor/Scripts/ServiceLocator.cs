using System;
using Form;
using Mod;
using UnityEditor;

namespace Editor.Scripts
{
    [InitializeOnLoad]
    public class ServiceLocator
    {
        public static ServiceLocator instance;
        static ServiceLocator()
        {
            instance = new ServiceLocator();
            instance.assetDB = new AssetDB();

            instance.formDB = new FormDB();
            
            instance.modManager = new ModManager(instance.assetDB, instance.formDB);
            instance.modManager.ScanModFiles();
        }

        public ModManager modManager;
        public AssetDB assetDB;
        public FormDB formDB;


        public class Events
        {
            public event Action onProtoFormChange;
            public void TriggerProtoFormChange() => onProtoFormChange?.Invoke();
        }
        public Events events = new Events();
    }
}