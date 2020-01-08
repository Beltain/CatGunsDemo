using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Status
{

    public float maxValue;
    public float minValue;
    public float currentValue;
    public float smoothedValue;
    public float regenRate;
    public float buffs;

    public Status()
    {
        maxValue = 100f;
        minValue = 0f;
        currentValue = 100f;
        smoothedValue = 100f;
        regenRate = 1f;
        buffs = 0f;
    }

    public Status(float valueMax, float valueMin, float valueCurrent, float _regenRate, float _buffs)
    {
        maxValue = valueMax;
        minValue = valueMin;
        currentValue = valueCurrent;
        smoothedValue = valueCurrent;
        regenRate = _regenRate;
        buffs = _buffs;
    }
}
