public class Transition
{
    public int state, next;
    public string input;

    public override string ToString()
    {
        return $"({state},{input})->{next}";
    }

    public override bool Equals(object obj)
    {
        if (obj is Transition)
        {
            Transition o = obj as Transition;
            return (o.input == this.input && o.state == this.state && o.next == this.next);
        }
        return false;
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}