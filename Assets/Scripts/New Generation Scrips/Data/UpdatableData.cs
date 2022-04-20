using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatableData : ScriptableObject
{
    public event System.Action OnValuesUpdated;
    public bool autoUpdate;

    #if UNITY_EDITOR
    protected virtual void OnValidate(){
        if(autoUpdate){
            UnityEditor.EditorApplication.update+= NotifyOfUpdatedvalues;
        }
    }

    public void NotifyOfUpdatedvalues()
    {
        UnityEditor.EditorApplication.update-= NotifyOfUpdatedvalues;
        if(OnValuesUpdated!=null){
            OnValuesUpdated();
        }
    }
    #endif
}
