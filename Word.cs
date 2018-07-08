public class Word
{
    public string Value { get; private set; }

    private Word(string value)
    {
        this.Value = value;
    }

    public static Word From(string value)
    {
        string simplified = value.ToLower();
        return new Word(simplified);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is Word))
        {
            return false;
        }

        Word other = (Word)obj;

        return other.Value.Equals(this.Value);
    }

    public override int GetHashCode()
    {
        return this.Value.GetHashCode();
    }
}