using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Events;
[System.Serializable]
public class ValueHolder<T>
{
    [SerializeField] protected T heldValue = default(T);
    [SerializeField] protected bool changedSinceLastCheck = false;
    public UnityEvent OnValueChangedEvent = new UnityEvent();
    public void OnValueChanged()
    {
        changedSinceLastCheck = true;
        OnValueChangedEvent.Invoke();
    }
    public void RegisterEvent(UnityAction reciever) => OnValueChangedEvent.AddListener(reciever);
    public bool ChangedSinceLastCheck()
    {
        if (changedSinceLastCheck)
        {
            changedSinceLastCheck = false;
            return true;
        }
        return false;
    }
    public T Value { 
        get { return heldValue; }
        set
        {
            T oldValue = heldValue;
            heldValue = value;
            if (!oldValue.Equals(heldValue))
            {
                OnValueChanged();
            }
        }
    }
}
