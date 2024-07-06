using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Stat 
{
    [SerializeField] private int baseValue;

    public List<int> modifiers;
    // lay gia tri
    public int GetValue()
    {
        int finalValue = baseValue;

        foreach (int modifier in modifiers)
        {
            finalValue += modifier;
        }

        return finalValue;
    }
    // tao ham luu suc manh co ban cua player
    public void SetDefaultValue(int _value)
    {
        baseValue = _value;
    }
    //Modifire them gia tri can xua doi sua doi
    public void AddModifier(int _modifier)
    {
        modifiers.Add(_modifier);
    }

    // loai bo gia tri
    public void RemoveModifier(int _modifier)
    {
        modifiers.Remove(_modifier);
    }
}
