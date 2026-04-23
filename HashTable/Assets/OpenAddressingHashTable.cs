using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.Port;
using static UnityEngine.Rendering.DebugUI;

public enum ProbingStrategy 
{ 
    Linear,
    Quadratic,
    DoubleHash,
}
public class OpenAddressingHashTable<TKey, TValue> : IDictionary<TKey, TValue>
{
    private TKey[] _keys;
    private TValue[] _values;
    private bool[] _occupied;
    private bool[] _deleted; 

    private int _count;
    private readonly ProbingStrategy _strategy;

    private const int InitialCapacity = 16;
    private const double LoadFactorThreshold = 0.6;

    public int size => _keys.Length;
    public OpenAddressingHashTable(ProbingStrategy strategy = ProbingStrategy.Linear)
    {
        _strategy = strategy;
        InitializeArrays(InitialCapacity);
    }

    private void InitializeArrays(int capacity)
    {
        _keys = new TKey[capacity];
        _values = new TValue[capacity];
        _occupied = new bool[capacity];
        _deleted = new bool[capacity];
        _count = 0;
    }

    public int GetHash(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int hash = key.GetHashCode();
        return (hash & 0x7fffffff) % size;
    }

    public int GetSecondaryHash(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int hash = key.GetHashCode();
        return 1 + ((hash & 0x7fffffff) % (size - 1));
    }

    public int GetIndex(TKey key, int attempts)
    {
        switch (_strategy)
        {
            case ProbingStrategy.Linear:
                return (GetHash(key) + attempts) % size;
            case ProbingStrategy.Quadratic:
                return (GetHash(key) + (attempts * attempts)) % size;
            case ProbingStrategy.DoubleHash:
                return (GetHash(key) + attempts * GetSecondaryHash(key)) % size;
            default:
                throw new NotImplementedException();
        }
    }

    public void Resize()
    {
        int resize = size * 2;
        var oldKeys = _keys;
        var oldValues = _values;
        var oldOccupied = _occupied;

        InitializeArrays(resize);

        for( int i = 0; i < oldOccupied.Length;  i++ )
        {
            if (oldOccupied[i] == true)
            {
                Add(oldKeys[i], oldValues[i]);
            }
        }
    }
    public TValue this[TKey key] 
    { 
        get => throw new System.NotImplementedException(); 
        set => throw new System.NotImplementedException(); 
    }

    public ICollection<TKey> Keys => throw new System.NotImplementedException();

    public ICollection<TValue> Values => throw new System.NotImplementedException();

    public int Count => throw new System.NotImplementedException();

    public bool IsReadOnly => throw new System.NotImplementedException();

    public void Add(TKey key, TValue value)
    {
        throw new System.NotImplementedException();
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        throw new System.NotImplementedException();
    }

    public void Clear()
    {
        Array.Clear(_keys, 0, size);
        Array.Clear(_values, 0, size);
        Array.Clear(_occupied, 0, size);
        Array.Clear(_deleted, 0, size);
        _count = 0;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        throw new System.NotImplementedException();
    }

    public bool ContainsKey(TKey key)
    {
        throw new System.NotImplementedException();
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        throw new System.NotImplementedException();
    }

    public bool Remove(TKey key)
    {
        throw new System.NotImplementedException();
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        throw new System.NotImplementedException();
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        throw new System.NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


}
