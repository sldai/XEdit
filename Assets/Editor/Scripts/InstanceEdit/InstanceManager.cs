using System.Collections.Generic;
using Editor.Scripts.Utility;
using Form;
using UnityEngine;
using UnityEngine.Assertions;

namespace Editor.Scripts.InstanceEdit
{
    public class InstanceManager
    {
        // Dictionaries to maintain relationships
        private readonly Dictionary<int, InstanceForm> componentIDToForm = new ();
        private readonly Dictionary<string, InstanceComponent> formIDToComponent = new ();

        public InstanceManager()
        {
            
        }

        private void Validate()
        {
            foreach (var formID in formIDToComponent.Keys)
            {
                Assert.IsTrue(componentIDToForm[formIDToComponent[formID].GetInstanceID()].formID == formID);
            }
        }
        
        // Add a new relationship
        public void Register(InstanceComponent component, InstanceForm form)
        {
            componentIDToForm[component.GetInstanceID()] = form;
            formIDToComponent[form.formID] = component;
            component.onTransformChange += transform =>  OnTransformChange(component.GetInstanceID(), transform);
        }

        // Remove a relationship
        public void Unregister(string formID)
        {
            var componentID = formIDToComponent[formID].GetInstanceID();
            formIDToComponent.Remove(formID);
            componentIDToForm.Remove(componentID);    
        }

        private void OnTransformChange(int componentID, Transform transform)
        {
            var form = componentIDToForm[componentID];
            form.position = Numerics.ConvertToSystemNumerics(transform.position);
            form.rotation = Numerics.ConvertToSystemNumerics(transform.rotation);
            form.scale = Numerics.ConvertToSystemNumerics(transform.localScale);
        }

        private void OnFormChange(string formID)
        {
            
        }

        /**
         * Given form, spawn a component in scene and register the association
         */
        public void SpawnComponent(InstanceForm form)
        {
            GameObject foo;
            var instanceObject = GameObject.Instantiate(foo);
            InstanceComponent instanceComponent = instanceObject.GetComponent<InstanceComponent>();
            Assert.IsNotNull(instanceComponent, "Prefab does not contain an InstanceComponent!");

            Register(instanceComponent, form);
        }
    }
}