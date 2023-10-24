using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Form
{
    public class FormDB
    {
        private Dictionary<string, BaseForm> forms = new();
        private MultiItemIndex<string, BaseForm> typeIndex = new(form => form.GetTYPE);
        private Dictionary<string, BaseForm> editorID2Form = new();
        
        public bool Contains(string formID) => forms.ContainsKey(formID);
        public bool ContainsEditorID(string editorID) => editorID2Form.ContainsKey(editorID);
        public void Clear()
        {
            forms.Clear();
            typeIndex.Clear();
        }

        public void Set([NotNull] BaseForm form)
        {
            if (forms.ContainsKey(form.formID))
            {
                Debug.LogWarning($"Form with formID {form.formID} already exists");
                var old = forms[form.formID];
                forms[form.formID] = form;
                IndexRemoveItem(old);
                IndexAddItem(form);
                return;
            }
            
            forms.Add(form.formID, form);
            IndexAddItem(form);
        }
        
        public void Delete(string formID)
        {
            if (!forms.ContainsKey(formID))
            {
                Debug.LogWarning($"No form found with formID {formID}");
                return;
            }

            forms.Remove(formID, out var removed);
            IndexRemoveItem(removed);
        }

        public BaseForm Get(string formID)
        {
            if (!forms.ContainsKey(formID))
            {
                Debug.LogWarning($"No form found with formID {formID}");
                return null;
            }

            return forms[formID];
        }

        public List<BaseForm> GetAll()
        {
            return forms.Values.ToList();
        }

        public List<BaseForm> FilterByType<T>()
        {
            return FilterByType(typeof(T));
        }

        public List<BaseForm> FilterByType(Type type)
        {
            return FilterByType(FormUtils.Type2Str[type]);
        }

        public List<BaseForm> FilterByType(string TYPE)
        {
            return typeIndex.Get(TYPE);
        }

        public List<BaseForm> FilterByCategory(string category)
        {
            return FormUtils.CategoryTypes[category]
                .SelectMany(FilterByType)
                .ToList();
        }

        void IndexAddItem(BaseForm form)
        {
            editorID2Form[form.editorID] = form;
            typeIndex.Add(form);
        }

        void IndexRemoveItem(BaseForm form)
        {
            editorID2Form.Remove(form.editorID);
            typeIndex.Remove(form);
        }
    }

    class MultiItemIndex<TKey, TItem>
    {
        private readonly Func<TItem, TKey> _fieldValueGetter;
        private readonly Dictionary<TKey, HashSet<TItem>> _index = new();

        public MultiItemIndex(Func<TItem, TKey> fieldValueGetter)
        {
            _fieldValueGetter = fieldValueGetter;
        }

        public void Clear()
        {
            _index.Clear();
        }

        /**
         * If item already exists, Add do nothing
         */
        public void Add(TItem item)
        {
            var key = GetFieldValue(item);
            if (key == null) return;

            if (!_index.ContainsKey(key))
            {
                _index[key] = new HashSet<TItem>();
            }

            _index[key].Add(item);
        }

        public void AddRange(IEnumerable<TItem> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public bool Contains(TItem item)
        {
            if (!_index.ContainsKey(GetFieldValue(item))) return false;
            return _index[GetFieldValue(item)].Contains(item);
        }

        public void Remove(TItem item)
        {
            if (!Contains(item)) return;
            _index[GetFieldValue(item)].Remove(item);
        }

        public List<TItem> Get(TKey key)
        {
            return _index.TryGetValue(key, out var items) ? items.ToList() : new List<TItem>();
        }

        private TKey GetFieldValue(TItem item)
        {
            return _fieldValueGetter(item);
        }
    }
}