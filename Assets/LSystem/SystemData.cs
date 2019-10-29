using System.Collections.Generic;

namespace Oisif
{

// TODO Scriptable Object?
public class SystemData
{
    public string Axiom { get; }
    public float Angle { get; }
    public float DepthFactor { get; }
    public List<ARule> Rules { get; }

    public SystemData(string axiom, float angle, float depthFactor)
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