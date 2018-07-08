using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using SongNode = Node<Song, Word>;
using SongEdge = Edge<Word, Song>;

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
            var graph = BuildGraph(songs);

            Console.WriteLine("Graph generated.");
            Console.WriteLine("Traversing...");

            Stack<SongNode> path = Traverse(graph);

            ShowPathInfo(path);
        }

        private static void ShowPathInfo(Stack<SongNode> path)
        {
            if (path != null)
            {
                Console.WriteLine("Found a path!");
                while (path.TryPop(out SongNode current))
                {
                    Console.WriteLine($"{current.Value.Title} -->");
                }
            }
            else
            {
                Console.WriteLine("No path found.");
            }
        }

        private static void ShowHelpfulInfo(HashSet<SongNode> graphNodes)
        {
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

        /// Attempt traversing the entire graph
        /// with the constraint that we can only use each word once
        private static Stack<SongNode> Traverse(HashSet<SongNode> graph)
        {
            // once used, can't re-use these words
            var usedEdges = new HashSet<Word>();
            var visitedNodes = new HashSet<SongNode>();

            // Try each as our starting node
            foreach (SongNode startingNode in graph)
            {
                var resultingPath = TraverseHelper(startingNode,
                                                   graph,
                                                   new Stack<SongNode>(),
                                                   new HashSet<SongNode>(),
                                                   new HashSet<Word>());

                if (resultingPath != null)
                {
                    return resultingPath;
                }
                else
                {
                    continue;
                }
            }

            // didn't find a path :(
            return null;
        }

        private static Stack<SongNode> TraverseHelper(SongNode curNode,
                                                      HashSet<SongNode> graph,
                                                      Stack<SongNode> path,
                                                      HashSet<SongNode> visited,
                                                      HashSet<Word> usedWords)
        {
            // we're visiting this node right now
            visited.Add(curNode);
            path.Push(curNode);

            // does this conclude our travel?
            if (visited.Count == graph.Count)
            {
                // all done
                return path;
            }

            var usableEdges = curNode.Edges.Where(e => !usedWords.Contains(e.Value));

            // group edges using the same word
            // with the lowest count of neighbors first
            var groupedByWord = usableEdges.GroupBy(e => e.Value)
                                           .OrderBy(p => p.Count());

            foreach (var wordGroup in groupedByWord)
            {
                Word curWord = wordGroup.Key;
                usedWords.Add(curWord);

                foreach (SongNode nextNode in wordGroup.Select(e => e.OtherEnd)
                                                       .Where(n => !visited.Contains(n)))
                {
                    // visit the node
                    var result = TraverseHelper(nextNode, graph, path, visited, usedWords);

                    if (result != null)
                    {
                        return result;
                    }
                }

                usedWords.Remove(curWord);
            }

            // All paths through this node didn't work out,
            // so remove it from visited and the stack
            visited.Remove(curNode);
            path.Pop();

            return null;
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

            int limit = 10;

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
