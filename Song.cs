using System.Collections.Generic;
using static System.Array;
using System;
using System.Linq;

public class Song
{
    public string Title { get; private set; }
    public HashSet<Word> LyricSet { get; private set; }

    public Song(string title, string lyrics)
    {
        this.Title = title;
        this.LyricSet = new HashSet<Word>();

        foreach (string splitWord in lyrics.Split(default(char[]), StringSplitOptions.RemoveEmptyEntries))
        {
            string sanitized = Sanitize(splitWord);
            this.LyricSet.Add(Word.From(sanitized));
        }
    }

    private static string Sanitize(string s)
    {
        var excludeChars = new HashSet<char>
        {
            ',',
            ';',
            '!',
            '?',
            '.',
            '\'',
            ')',
            '(',
        };

        var filteredEnum = s.Where(c => !excludeChars.Contains(c));

        return string.Concat(filteredEnum);
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