using System.Collections.Generic;
using static System.Array;

public class Song
{
    public string Title { get; private set; }
    public HashSet<Word> LyricSet { get; private set; }

    public Song(string title, string lyrics)
    {
        this.Title = title;
        this.LyricSet = new HashSet<Word>();

        ForEach(lyrics.Split(' '), s => this.LyricSet.Add(Word.From(s)));
    }

    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is Song))
        {
            return false;
        }

        Song other = (Song)obj;

        return other.Title.Equals(this.Title)
            && other.LyricSet.Equals(this.LyricSet);
    }

    public override int GetHashCode()
    {
        return this.Title.GetHashCode() * 17 + this.LyricSet.GetHashCode();
    }
}