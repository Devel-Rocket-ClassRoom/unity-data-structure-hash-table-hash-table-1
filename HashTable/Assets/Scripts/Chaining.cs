using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;
using static UnityEngine.Rendering.DebugUI;

public class Chaining<TKey, TValue> : IDictionary<TKey, TValue>
{
    private int size = 16;

    public List<List<(TKey key, TValue value)>> hashTable = new();

    public TValue this[TKey key]
    {
        get
        {
            if (TryGetValue(key, out TValue value))
            {
                return value;
            }
            else
            {
                throw new KeyNotFoundException($"키 {key} 없음");
            }
        }

        set => Add(key, value);
    }

    public ICollection<TKey> Keys => throw new System.NotImplementedException();

    public ICollection<TValue> Values => throw new System.NotImplementedException();

    public int Count => size;

    private IComparer comparer;

    private int count = 0;

    public Chaining()
    {
        for(int i = 0; i < size; i++)
        {
            hashTable.Add(new List<(TKey key, TValue value)>());
        }
        comparer = Comparer<TValue>.Default;
    }

    public bool IsReadOnly => false;

    private readonly float LoadFactor = 0.75f;

    public void Add(TKey key, TValue value)
    {
        int hash = key.GetHashCode();
        int index = (hash & 0x7fffffff) % size;

        hashTable[index].Add((key, value));
        count++;

        if ((float)count / size > LoadFactor)
        {
            Resize();
        }
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    private void Resize()
    {
        Debug.Log("리사이즈");

        size *= 2;

        var tempTable = hashTable.ToList();
        hashTable = new();

        for (int i = 0; i < size; i++)
        {
            hashTable.Add(new List<(TKey key, TValue value)>());
            count = 0;
        }

        foreach (var value in tempTable)
        {
            if(value != null)
            {
                foreach(var temp in value)
                {
                    Add(temp.key, temp.value);
                }
            }
        }
    }

    public void Clear()
    {
        hashTable = null;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        int hash = item.Key.GetHashCode();
        int index = (hash & 0x7fffffff) % size;

        if(hashTable[index] == null)
        {
            return false;
        }

        foreach(var value in hashTable[index])
        {
            if(comparer.Compare(value.value, item.Value) == 0)
            {
                return true;
            }
        }

        return false;
    }

    public bool ContainsKey(TKey key)
    {
        int hash = key.GetHashCode();
        int index = (hash & 0x7fffffff) % size;

        if (hashTable[index] == null)
        {
            return false;
        }

        foreach (var value in hashTable[index])
        {
            if (comparer.Compare(value.key, key) == 0)
            {
                return true;
            }
        }

        return false;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        for(int i = 0; i < hashTable.Count; i++)
        {
            foreach(var value in hashTable[i])
            {
                yield return new KeyValuePair<TKey, TValue>(value.key, value.value);
            }
        }
    }

    public bool Remove(TKey key)
    {
        int hash = key.GetHashCode();
        int index = (hash & 0x7fffffff) % size;

        foreach(var temp in hashTable[index])
        {
            if(comparer.Compare(temp.key, key) == 0)
            {
                hashTable[index].Clear();
                return true;
            }
        }

        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        int hash = item.Key.GetHashCode();
        int index = (hash & 0x7fffffff) % size;

        foreach (var temp in hashTable[index])
        {
            if (comparer.Compare(temp.value, item.Value) == 0)
            {
                hashTable[index].Remove((item.Key, item.Value));
                return true;
            }
        }

        return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        int hash = key.GetHashCode();
        int index = (hash & 0x7fffffff) % size;

        if (hashTable[index] == null)
        {
            value = default;
            return false;
        }

        foreach (var item in hashTable[index])
        {
            if (comparer.Compare(item.key, key) == 0)
            {
                value = item.value;
                return true;
            }
        }

        value = default;
        return false;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public List<(TKey key, TValue value)> GetList(int index)
    {
        return hashTable[index];
    }
}
