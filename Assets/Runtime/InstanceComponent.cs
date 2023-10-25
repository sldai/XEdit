using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class InstanceComponent : MonoBehaviour
{
    public string formID;
    public event Action<Transform> onTransformChange;
    
    // Start is called before the first frame update
    void Start()
    {
        GetComponent()
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.hasChanged)
        {
            onTransformChange?.Invoke(transform);
            transform.hasChanged = false; // Reset the flag
        }
    }
}

