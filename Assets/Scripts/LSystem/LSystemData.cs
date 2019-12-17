using System.Collections.Generic;
using UnityEngine;

namespace Oisif
{

public interface IValuable
{
    float Value();
}

public struct SimpleValue : IValuable
{
    float _value;

    public SimpleValue(float value)
    {
        _value = value;
    }

    public float Value()
    {
        return _value;
    }
}

public struct RangeValue : IValuable
{
    float _min;
    float _max;

    public RangeValue(float min, float max)
    {
        _min = min;
        _max = max;
    }

    public float Value()
    {
        return Random.Range(_min, _max);
    }
}


public class LSystemData
{
    public string Axiom { get; }
    public IValuable Angle { get; }
    public float DepthFactor { get; }
    public List<ARule> Rules { get; }

    public LSystemData(string axiom, IValuable angle, float depthFactor)
    {
        Rules = new List<ARule>();
        Axiom = axiom;
        Angle = angle;
        DepthFactor = depthFactor;
    }

    public void AddRule(ARule rule)
    {
        Rules.Add(rule);
    }
}

}