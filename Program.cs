using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using SongNode = Node<Song, Word>;

namespace traveling_beatles
{
    class Program
    {
        const string LYRIC_DIR = "lyrics_db";

        static void Main(string[] args)
        {
            // Build a graph where the nodes are songs and the edges connect songs that share a word in their lyrics
            Console.WriteLine("Starting...");

            List<Song> songs = GetSongs();
            var graphNodes = BuildGraph(songs);

            Console.WriteLine($"Created graph of size: {graphNodes.Count} nodes");

            foreach (SongNode node in graphNodes)
            {
                Console.WriteLine($"Song {node.Value.Title}:");
                var edgesByWord = node.Edges.GroupBy(e => e.Value);
                foreach (var wordGrouping in edgesByWord.OrderBy(w => w.Count()))
                {
                    Console.WriteLine($"\tconnected via '{wordGrouping.Key.Value}' to {wordGrouping.Count()} other songs");
                }
            }
        }

        private static HashSet<SongNode> BuildGraph(IList<Song> songs)
        {
            // create a mapping of ever Word that appears, to the set of Songs it is found in
            // (and at the same time make Nodes out of the Songs)
            var wordsToSongNodes = new Dictionary<Word, HashSet<SongNode>>();

            foreach (Song song in songs)
            {
                SongNode songNode = new SongNode(song);

                foreach (Word word in song.LyricSet)
                {
                    if (wordsToSongNodes.TryGetValue(word, out HashSet<SongNode> mappedSongs))
                    {
                        mappedSongs.Add(songNode);
                    }
                    else
                    {
                        wordsToSongNodes.Add(word, new HashSet<SongNode> { songNode });
                    }
                }
            }

            // now we use the dictionary to graph the nodes together

            HashSet<SongNode> graphNodes = GraphFromDict(wordsToSongNodes);
            return graphNodes;
        }

        private static HashSet<SongNode> GraphFromDict(Dictionary<Word, HashSet<SongNode>> dict)
        {
            var allNodes = new HashSet<SongNode>();

            foreach (var wordSongPair in dict)
            {
                // Connect all the SongNodes that share this word
                Word word = wordSongPair.Key;
                HashSet<SongNode> songsWithWord = wordSongPair.Value;

                foreach (SongNode songNode in songsWithWord)
                {
                    allNodes.Add(songNode);

                    foreach (SongNode songWithSameWord in songsWithWord)
                    {
                        if (songNode == songWithSameWord) { continue; }

                        songNode.AddEdge(new Edge<Word, Song>(word, songWithSameWord));
                    }
                }
            }

            return allNodes;
        }

        private static List<Song> GetSongs()
        {
            // stubbing this out, will implement at end
            // return new List<Song>
            // {
            //     new Song("Hey Jude", "Hey Jude, don't make it bad, take a sad song and make it better"),
            //     new Song("Let It Be", "Mother Mary comes to me, speaking words of wisdom, let it be"),
            //     new Song("She Loves You", "Well I saw her yesterday, it's you she's thinking of, and she told me what to say"),
            //     new Song("I'm Down", "I'm down, I'm down, how can you laugh when you know I'm down"),
            // };
            var songs = new List<Song>();

            var lyricFiles = Directory.GetFiles(LYRIC_DIR);

            int limit = 200;

            foreach (string lyricFileName in lyricFiles)
            {
                string content = File.ReadAllText(lyricFileName);
                Song song = new Song(lyricFileName, content);
                songs.Add(song);
                limit--;

                if (limit <= 0)
                {
                    break;
                }
            }

            return songs;
        }
    }
}
