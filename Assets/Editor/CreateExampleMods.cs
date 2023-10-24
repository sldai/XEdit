using System;
using System.Collections.Generic;
using Form;
using Form.WorldObjects;
using Mod;
using Schema;
using UnityEditor;

namespace Editor
{
    public class CreateExampleMods
    {
        static ModSchema modA = new ModSchema()
        {
            metadata = new ModSchema.Metadata()
            {
                author = "sldai",
                coverUrl = "",
                modifyTime = DateTime.Now,
                description = "asdasda",
                name = "My Mod A",
            },
            bundles = new List<string>(),
            forms = new List<FormSchema>()
            {
                    new FormSchema()
                    {
                        TYPE = StaticForm.TYPE,
                        editorID = "a",
                        formID = "1",
                        model = "asd.fbx",
                        modifyTime = DateTime.Now
                    },
                    new FormSchema()
                    {
                        TYPE = StaticForm.TYPE,
                        editorID = "b",
                        formID = "2",
                        model = "basd.fbx",
                        modifyTime = DateTime.Now
                    }
            }
        };
        
        static ModSchema modB = new ModSchema()
        {
            metadata = new ModSchema.Metadata()
            {
                author = "sldai",
                coverUrl = "",
                modifyTime = DateTime.Now,
                description = "asdasda",
                name = "My Mod B",
            },
            bundles = new List<string>(),
                forms = new()
                {
                    new FormSchema()
                    {
                        TYPE = StaticForm.TYPE,
                        editorID = "c",
                        formID = "3",
                        model = "asd.fbx",
                        modifyTime = DateTime.Now
                    },
                    new FormSchema()
                    {
                        TYPE = StaticForm.TYPE,
                        editorID = "d",
                        formID = "4",
                        model = "basd.fbx",
                        modifyTime = DateTime.Now
                    }
                }
        };

        [MenuItem("Create/ExampleMods")]
        static void Create()
        {
            ModSchema.SaveFile("Mod/mod_a.json", modA);
            ModSchema.SaveFile("Mod/mod_b.json", modB);
        }
    }
}