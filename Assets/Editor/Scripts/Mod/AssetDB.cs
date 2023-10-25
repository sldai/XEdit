using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace Mod
{
    // in memory asset db,  
    public class AssetDB
    {
        private class AssetMetadata
        {
            public string key;
            public AssetBundle bundle;
            public object assetData;
        }


        private Dictionary<string, AssetMetadata> assets = new();

        private List<AssetBundle> bundles = new();

        // bundles are LIFO, i.e. last loaded bundle overrides
        public async UniTask LoadBundle(string path)
        {
            var bundle = await AssetBundle.LoadFromFileAsync(path);
            bundles.Add(bundle);
            GetMetaDataFromBundle(bundle);
        }

        void GetMetaDataFromBundle(AssetBundle bundle)
        {
            var names = bundle.GetAllAssetNames();
            foreach (var name in names)
            {
                assets[name] = new AssetMetadata
                {
                    key = name,
                    bundle = bundle,
                };
            }
        }

        public async UniTask LoadBundleList(List<string> bundleList)
        {
            foreach (var bundlePath in bundleList)
            {
                await LoadBundle(bundlePath);
            }
        }

        public async UniTask<T> GetAsset<T>(string key) where T : UnityEngine.Object
        {
            foreach (var k in assets.Keys)
            {
                Debug.Log(k);
            }

            Assert.IsTrue(assets.ContainsKey(key), $"Asset {key} not exist");
            var metadata = assets[key];
            metadata.assetData ??= await metadata.bundle.LoadAssetAsync<T>(key);
            Assert.IsTrue(typeof(T) == metadata.assetData.GetType());
            return metadata.assetData as T;
        }

        public async UniTask UnLoadAsset(string key)
        {
            await UniTask.NextFrame();
        }

        public void UnloadAll()
        {
            assets.Clear();
            bundles.Clear();
            AssetBundle.UnloadAllAssetBundles(true);
        }
    }
    
}
