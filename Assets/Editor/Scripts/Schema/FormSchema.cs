using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Form;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;
using Quaternion = System.Numerics.Quaternion;

namespace Schema
{
    [Serializable]
    public class FormSchema
    {
        private static Dictionary<string, FieldInfo> FormSchemaFields =
            typeof(FormSchema).GetFields().ToDictionary(field => field.Name, field => field);

        public string TYPE;
        public string editorID;
        public string formID; // ulid
        public DateTime modifyTime;

        // cell
        public string name;
        public string worldSpace;
        public Vector3? origin;
        public List<string> instanceFormIDs;

        // instance
        public string protoID;
        public Vector3? position;
        public Quaternion? rotation;
        public Vector3? scale;
        public List<Tuple<string, object>> overrides;

        // Static
        public string model;

        public static BaseForm Schema2Form(FormSchema schema)
        {
            // Determine the form type using the TYPE field and FormUtils
            var formType = FormUtils.Str2Type[schema.TYPE];
            if (formType == null)
            {
                Debug.LogError($"Unknown form type: {schema.TYPE}");
                return null;
            }

            // Create a new instance of the form type
            var form = Activator.CreateInstance(formType) as BaseForm;
            if (form == null)
            {
                Debug.LogError($"Failed to create instance of form type: {formType}");
                return null;
            }

            // Use reflection to populate fields using TypeName2Fields
            var formFields = FormUtils.TypeName2Fields[schema.TYPE];
            foreach (var formField in formFields.Values)
            {
                if (FormSchemaFields.TryGetValue(formField.Name, out var schemaField))
                {
                    formField.SetValue(form, schemaField.GetValue(schema));
                }
            }

            return form;
        }

        public static FormSchema Form2Schema(BaseForm form)
        {
            var schema = new FormSchema();

            // Set the TYPE field directly
            schema.TYPE = form.GetTYPE;

            // Determine the form type and retrieve its fields using FormUtils
            var typeName = form.GetTYPE;
            var formFields = FormUtils.TypeName2Fields[typeName];

            foreach (var formField in formFields.Values)
            {
                if (FormSchemaFields.TryGetValue(formField.Name, out var schemaField))
                {
                    schemaField.SetValue(schema, formField.GetValue(form));
                }
            }

            return schema;
        }
    }
}