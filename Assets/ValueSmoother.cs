using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class ValueSmoother
{
    int Values;
    Vector3 [] ValueArray;
    Vector3 Sum;
    int CurrentValue;

    public ValueSmoother(int values)
    {
        Values = values;
        ValueArray = new Vector3[values];
        for (int i = 0; i < values; ++i)
        {
            ValueArray[i] = new Vector3();
        }
        Sum = new Vector3();
        CurrentValue = 0;
    }

    public void Add(Vector3 value)
    {
        Sum -= ValueArray[CurrentValue];
        Sum += value;
        ValueArray[CurrentValue] = value;

        CurrentValue++;
        CurrentValue = CurrentValue % Values;
    }

    public void Reset(Vector3 value)
    {
        for (int i = 0; i < Values; ++i)
        {
            ValueArray[i] = value;
        }
        Sum = value * Values;
        CurrentValue = 0;
    }

    public Vector3 GetValue()
    {
        return Sum / Values;
    }
}
