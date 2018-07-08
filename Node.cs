using System.Collections.Generic;

/// A Node holding a value of type T
/// connected to other Nodes by Edges which hold values of type E.
/// E.g., A Node<Song, Word> could be a Node holding a Song that is connected to other Nodes
/// by Edges that represent individual lyric Words.
public class Node<T, E>
{

    public T Value { get; private set; }

    public List<Edge<E, T>> Edges { get; private set; } = new List<Edge<E, T>>();

    public Node(T value)
    {
        this.Value = value;
    }

    public void AddEdge(Edge<E, T> edge)
    {
        this.Edges.Add(edge);
    }

    public void AddEdge(E value, Node<T, E> otherEnd)
    {
        var edge = new Edge<E, T>(value, otherEnd);
        this.Edges.Add(edge);
    }
}

public class Edge<E, N>
{
    public E Value { get; private set; }
    public Node<N, E> OtherEnd { get; private set; }

    public Edge(E value, Node<N, E> otherEnd)
    {
        this.Value = value;
        this.OtherEnd = otherEnd;
    }
}