using System.Collections.Generic;
using System.Linq;
using Form;
using Schema;
using UnityEngine;
using UnityEngine.Assertions;

namespace Mod
{
    /**
     * ActiveMod is used when editing one mod.
     * The mod can add new forms, edit and delete existing forms.
     * ActiveMod's forms can override previous forms.
     */
    public class ActiveMod
    {
        private Mod targetMod;
        private FormDB formDB;
        private Dictionary<string, BaseForm> id2Form;
        private Dictionary<string, BaseForm> formOverrided = new();

        public string fname;
        public bool unsaveChanges = false;
        /**
         * build id2Form index, fill formDB and records overrided
         */
        public ActiveMod(Mod mod, FormDB formDB, string fname)
        {
            this.targetMod = mod;
            this.formDB = formDB;
            id2Form = mod.forms.Select(form => new { form.formID, form })
                .ToDictionary(pair => pair.formID, pair => pair.form);
            foreach (var form in targetMod.forms)
            {
                if (formDB.Contains(form.formID)) formOverrided[form.formID] = formDB.Get(form.formID);
                formDB.Set(form);
            }

            this.fname = fname;
        }

        public bool Contains(string formID)
        {
            return id2Form.ContainsKey(formID);
        }
        
        public void AddForm(BaseForm form)
        {
            Assert.IsFalse(id2Form.ContainsKey(form.formID));
            unsaveChanges = true;
            // add to mod
            targetMod.forms.Add(form);
            
            // update index
            id2Form[form.formID] = form;
            
            // check override
            if (formDB.Contains(form.formID))
            {
                formOverrided[form.formID] = formDB.Get(form.formID);
            }
            
            // set formDB
            formDB.Set(form);
        }

        public void EditForm(BaseForm form)
        {
            Assert.IsTrue(id2Form.ContainsKey(form.formID));
            unsaveChanges = true;
            // set mod
            var ind = targetMod.forms.IndexOf(form);
            targetMod.forms[ind] = form;
            
            // update index
            id2Form[form.formID] = form;
            
            // no need to check override
            
            // set formDB
            formDB.Set(form);
        }

        /**
         * Delete form from mod and formDB.
         * If this form override other, add other to formDB
         */
        public void DeleteForm(BaseForm form)
        {
            Assert.IsTrue(id2Form.ContainsKey(form.formID));
            unsaveChanges = true;
            // delete mod
            targetMod.forms.Remove(form);
            
            // update index
            id2Form.Remove(form.formID);
            
            // if override exists, add it to formDB
            if (formOverrided.ContainsKey(form.formID))
            {
                formDB.Set(formOverrided[form.formID]);
                formOverrided.Remove(form.formID);
            }
            else
            {
                formDB.Delete(form.formID);
            }
        }
    }
}