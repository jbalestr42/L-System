namespace Oisif
{

public class SimpleRule : ARule
{
    string _result;

    public SimpleRule(char sign, string result)
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