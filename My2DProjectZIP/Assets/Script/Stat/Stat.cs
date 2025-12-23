using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField] private int baseValue;

    public List<int> modifiers;

    public int GetValue()
    {
        int fineValue = baseValue;
        foreach (var modifier in modifiers)
        {
            fineValue += modifier;
        }
        return fineValue;
    }

    public void SetDefaultValue(int _value)
    {
        baseValue = _value;
    }

    public void AddModifier(int _modifier)
    {
        modifiers.Add(_modifier);
    }

    public void MoveModifier(int _modifier)
    {
        modifiers.Remove(_modifier);
    }

}
