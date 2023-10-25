using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Form;
using Schema;
using UnityEngine;
using UnityEngine.Assertions;

namespace Mod
{
    /**
     * Allow mod override form in other, dont allow mod delete form in other
     */
    public class ModManager
    {
        public class ModEntry
        {
            public bool enable = false;
            public bool active = false;
            public int size = 0; // [kb]
            public string fileName;
            public Mod mod;
        }

        public static string modDirectory = Path.Combine(Directory.GetParent(Application.dataPath).ToString(), "Mod");
        public List<ModEntry> modEntryList = new(); // scanned mod, only loaded mods have forms
        private AssetDB assetDB;
        private FormDB formDB;
        public ActiveMod activeMod;
        
        public ModManager(AssetDB assetDB, FormDB formDB)
        {
            this.assetDB = assetDB;
            this.formDB = formDB;
        }

        private List<string> FindModFileNames()
        {
            var modFilePaths = new List<string>();

            if (Directory.Exists(modDirectory))
            {
                // List all json files in modPath.
                var files = Directory.GetFiles(modDirectory, "*.json");
                modFilePaths.AddRange(files);

                Debug.Log("Found " + files.Length + " mods.");
            }
            else
            {
                Debug.LogWarning("Mod path does not exist: " + modDirectory);
            }

            return modFilePaths.Select(Path.GetFileName).ToList();
        }

        public void ScanModFiles()
        {
            var modFileNames = FindModFileNames();
            modEntryList.Clear();
            foreach (var modFileName in modFileNames)
            {
                try
                {
                    var modSchema = ModSchema.LoadFile(Path.Combine(modDirectory, modFileName));
                    modSchema.forms.Clear();
                    modEntryList.Add(new ModEntry { mod = ModSchema.Schema2Mod(modSchema), fileName = modFileName});
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
        }
        
        public void LoadEnabledMods()
        {
            // unload all forms to ensure only loaded mod have forms
            foreach (var modEntry in modEntryList)
            {
                modEntry.mod.forms.Clear();
            }
            
            foreach (var modEntry in modEntryList.Where(m => m.enable))
            {
                var modSchema = ModSchema.LoadFile(Path.Combine(modDirectory, modEntry.fileName));
                modEntry.mod = ModSchema.Schema2Mod(modSchema);
            }
        }

        private List<string> FindBundles()
        {
            var bundleNames = new List<string>();
            foreach (var mod in from modEntry in modEntryList where modEntry.enable select modEntry.mod)
            {
                bundleNames.AddRange(mod.bundles.Select(bundleName => Path.Combine(modDirectory, bundleName)));
            }
            return bundleNames.Distinct().ToList();
        }

        public async UniTask InitAssetDB()
        {
            assetDB.UnloadAll();
            var bundles = FindBundles();
            Debug.Log(JsonHelper.PrettyString(bundles));
            await assetDB.LoadBundleList(bundles);
        }

        /**
         * add forms from enabled mods
         * finally create active mod, let it override mods in formDB 
         */
        public void InitFormDB()
        {
            // add forms from enabled mods
            formDB.Clear();
            foreach (var modEntry in modEntryList)
            {
                if (modEntry.active) break;
                if (!modEntry.enable) continue;
                foreach (var form in modEntry.mod.forms)
                {
                    formDB.Set(form);
                }
            }
            
            // create active mod, it own init formDB
            var modEntryActive = modEntryList.FirstOrDefault(modEntry => modEntry.active);
            if (modEntryActive is null) return;
            activeMod = new ActiveMod(mod: modEntryActive.mod, formDB: formDB, fname: modEntryActive.fileName);
        }

        /**
         * Set enabled mods and the active mod, then load content from them.
         * If no active mod, you cannot modify forms. 
         */
        public async UniTask Load(HashSet<int> enableInds, int activeInd=-1)
        {
            if (activeInd >= 0)
            {
                enableInds.Add(activeInd);
                Assert.IsTrue(enableInds.Max()==activeInd); // active mod must be the last loaded
            }

            foreach (var (ind, modEntry) in modEntryList.Select((value, ind) => (ind, value)))
            {
                modEntry.enable = enableInds.Contains(ind);
            }

            foreach (var (ind, modEntry) in modEntryList.Select((value, ind) => (ind, value)))
            {
                modEntry.active = ind == activeInd;
            }
            
            LoadEnabledMods();
            await InitAssetDB();
            InitFormDB();
        }

        public void SaveMod(int modInd)
        {
            if (modInd<0) return;
            var modEntry = modEntryList[modInd];
            ModSchema.SaveFile(Path.Combine(modDirectory, modEntry.fileName), ModSchema.Mod2Schema(modEntry.mod));
        }

        public void SaveActiveMod()
        {
            var activeInd = -1;
            for (int i = 0; i < modEntryList.Count; i++)
            {
                if (modEntryList[i].active)
                {
                    activeInd = i;
                    break;
                }
            }
            if (activeInd==-1) return;
            SaveMod(activeInd);
            activeMod.unsaveChanges = false;
        }
        public void SaveModMetadata(int modInd)
        {
            if (modInd<0) return;
            var modEntry = modEntryList[modInd];
            ModSchema.SaveWithoutForms(Path.Combine(modDirectory, modEntry.fileName), ModSchema.Mod2Schema(modEntry.mod));
        }

        public void NewMod()
        {
            Func<string> GenNewModFileName = () =>
            {
                const string baseName = "new_mod";
                var modName = baseName;
                var cnt = 1;
                while (modEntryList.Any(entry => Path.GetFileNameWithoutExtension(entry.fileName) == modName))
                {
                    modName = $"{baseName}_{cnt}";
                    cnt++;
                }
                return modName;
            };
            
            // create new mod
            var modName = GenNewModFileName();
            var mod = new Mod()
            {
                metadata = new Mod.Metadata()
                {
                    name = "Mod",
                    author = "Unknown",
                    description = "Description of Mod",
                },
            };
            
            modEntryList.Add(new ModEntry(){active = false, enable = false, fileName = modName+".json", mod=mod, size = 0});
            SaveMod(modEntryList.Count-1);
        }
    }
}