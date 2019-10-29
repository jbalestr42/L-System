namespace Oisif
{

public class DeterministicRule : ARule
{
    string _result;

    public DeterministicRule(char sign, string result)
        :base(sign)
    {
        _result = result;
    }

    public override string Evaluate(LSystem.Token token)
    {
        return _result;
    }
}

}