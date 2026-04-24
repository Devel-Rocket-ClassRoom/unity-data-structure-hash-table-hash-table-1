using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
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

    public ICollection<TKey> Keys => GetKeys();

    public ICollection<TValue> Values => GetValues();

    public int Count => size;

    private IEqualityComparer comparer;

    private int count = 0;

    public Chaining()
    {
        for(int i = 0; i < size; i++)
        {
            hashTable.Add(new List<(TKey key, TValue value)>());
        }
        comparer = EqualityComparer<TKey>.Default;
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
        hashTable = new();
        for (int i = 0; i < size; i++)
        {
            hashTable.Add(new List<(TKey key, TValue value)>());
        }
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
            if(comparer.Equals(value.key, item.Key))
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
            if (comparer.Equals(value.key, key))
            {
                return true;
            }
        }

        return false;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        foreach(var item in hashTable)
        {
            if (item != null)
            {
                foreach (var temp in item)
                {
                    array[arrayIndex++] = new KeyValuePair<TKey, TValue>(temp.key, temp.value);
                }
            }
        }
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
            if(comparer.Equals(temp.key, key))
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
            if (comparer.Equals(temp.key, item.Key))
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
            if (comparer.Equals(item.key, key))
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

    private ICollection<TKey> GetKeys()
    {
        List<TKey> keys = new();

        foreach(var item in hashTable)
        {
            if(item != null)
            {
                foreach(var values in item)
                {
                    keys.Add(values.key);
                }
            }
        }

        return keys;
    }

    private ICollection<TValue> GetValues()
    {
        List<TValue> values = new();

        foreach (var item in hashTable)
        {
            if (item != null)
            {
                foreach (var value in item)
                {
                    values.Add(value.value);
                }
            }
        }

        return values;
    }
}
