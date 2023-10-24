using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Form.WorldObjects;

namespace Form
{
    public static class FormUtils
    {
        private static List<Type> FormTypes = new()
        {
            typeof(BaseForm),
            typeof(InstanceForm),
            typeof(StaticForm),
        };

        public static Dictionary<string, Type> Str2Type = BuildStr2Type();
        public static Dictionary<Type, string> Type2Str = BuildType2Str();
        public static Dictionary<string, List<string>> CategoryTypes = BuildCategoryTypes();
        public static Dictionary<string, Dictionary<string, FieldInfo>> TypeName2Fields = BuildType2Fields();

        private static Dictionary<string, Type> BuildStr2Type()
        {
            var result = new Dictionary<string, Type>();
            foreach (var formType in FormTypes)
            {
                var typeField = formType.GetField("TYPE", BindingFlags.Public | BindingFlags.Static);
                if (typeField != null && typeField.FieldType == typeof(string))
                {
                    result[(string)typeField.GetValue(null)] = formType;
                }
            }

            return result;
        }

        private static Dictionary<Type, string> BuildType2Str()
        {
            return Str2Type.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        }

        private static Dictionary<string, List<string>> BuildCategoryTypes()
        {
            var result = new Dictionary<string, List<string>>();
            foreach (var formType in FormTypes)
            {
                var categoryField = formType.GetField("CATEGORY", BindingFlags.Public | BindingFlags.Static);
                var typeField = formType.GetField("TYPE", BindingFlags.Public | BindingFlags.Static);
                if (categoryField == null || categoryField.FieldType != typeof(string) ||
                    typeField == null || typeField.FieldType != typeof(string)) continue;
                var category = (string)categoryField.GetValue(null);
                var typeStr = (string)typeField.GetValue(null);
                if (!result.ContainsKey(category))
                {
                    result[category] = new List<string>();
                }

                result[category].Add(typeStr);
            }

            return result;
        }

        private static Dictionary<string, Dictionary<string, FieldInfo>> BuildType2Fields()
        {
            var result = new Dictionary<string, Dictionary<string, FieldInfo>>();
            foreach (var formType in FormTypes)
            {
                var fields = formType.GetFields(BindingFlags.Instance|BindingFlags.Public);
                result[Type2Str[formType]] = fields.ToDictionary(field => field.Name, field => field);
            }

            return result;
        }
    }
}