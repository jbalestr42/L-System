namespace Oisif
{

public abstract class ARule
{
    public char Sign { get; }

    public ARule(char sign)
    {
        Sign = sign;
    }
    
    public abstract string Evaluate(LSystem.Token token);
}

}