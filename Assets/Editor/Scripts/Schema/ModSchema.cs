using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Schema
{
    [Serializable]
    public class ModSchema
    {
        [Serializable]
        public class Metadata
        {
            public string name;
            public string version;
            public string author;
            public string description;
            public string coverUrl;
            public DateTime modifyTime;
        }

        public Metadata metadata;
        public List<string> bundles;
        public List<FormSchema> forms;
        
        public static ModSchema LoadFile(string path)
        {
            var result = JsonHelper.ReadFile<ModSchema>(path);
            if (result.metadata?.name is null) throw new InvalidDataException($"{path} mod file is not valid");
            return result;
        }
        
        public static void SaveFile(string path, ModSchema mod)
        {
            if (!File.Exists(path))
            {
                var f = File.Create(path);
                f.Close();
            }
            JsonHelper.WriteFile(path, mod);
        }

        public static void SaveWithoutForms(string path, ModSchema mod)
        {
            if (!File.Exists(path)) return;
            var oldModSchema = JsonHelper.ReadFile<ModSchema>(path);
            var newModSchema = new ModSchema()
            {
                metadata = mod.metadata,
                bundles = mod.bundles,
                forms = oldModSchema.forms
            };
            JsonHelper.WriteFile(path, newModSchema);
        }

        public static Mod.Mod Schema2Mod(ModSchema schema)
        {
            return new Mod.Mod
            {
                metadata = new Mod.Mod.Metadata
                {
                    name = schema.metadata.name,
                    version = schema.metadata.version,
                    author = schema.metadata.author,
                    description = schema.metadata.description,
                    coverUrl = schema.metadata.coverUrl,
                    modifyTime = schema.metadata.modifyTime
                },
                bundles = schema.bundles,
                forms = schema.forms.Select(FormSchema.Schema2Form).ToList()
            };
        }

        public static ModSchema Mod2Schema(Mod.Mod mod)
        {
            return new ModSchema()
            {
                metadata = new ModSchema.Metadata
                {
                    name = mod.metadata.name,
                    version = mod.metadata.version,
                    author = mod.metadata.author,
                    description = mod.metadata.description,
                    coverUrl = mod.metadata.coverUrl,
                    modifyTime = mod.metadata.modifyTime
                },
                bundles = mod.bundles,
                forms = mod.forms.Select(FormSchema.Form2Schema).ToList()
            };
        }
    }    
}
