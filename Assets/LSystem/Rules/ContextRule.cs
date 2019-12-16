namespace Oisif
{

public class ContextRule : ARule
{
    string _result;

    public ContextRule(char sign, string result)
        :base(sign)
    {
        _result = result;
    }

    public override string Evaluate(LSystem.Token token)
    {
        // check prev / next
        return _result;
    }
}

}