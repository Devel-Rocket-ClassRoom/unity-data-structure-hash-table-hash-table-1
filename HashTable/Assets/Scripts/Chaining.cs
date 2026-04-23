using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using UnityEngine;
using static UnityEditor.Progress;

public class Chaining<TKey, TValue> : IDictionary<TKey, TValue>
{
    public List<(TKey key, TValue value)> chainList;

    public List<(TKey key, TValue value)>[] hashTable = new List<(TKey key, TValue value)>[16];

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

    public int Count => CountValues();

    private IComparer comparer;

    public Chaining()
    {
        comparer = Comparer<TValue>.Default;
    }

    private int CountValues()
    {
        int amount = 0;

        foreach(var value in hashTable)
        {
            if(value != null)
            {
                amount++;
            }
        }

        return amount;
    }

    public bool IsReadOnly => false;

    private readonly float LoadFactor = 0.75f;

    public void Add(TKey key, TValue value)
    {
        int hash = key.GetHashCode();
        int index = (hash & 0x7fffffff) % hashTable.Length;

        if (hashTable[index] == null)
        {
            hashTable[index] = new List<(TKey key, TValue value)>();
        }

        hashTable[index].Add((key, value));

        if((float)Count/hashTable.Length > LoadFactor)
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
        var tempTable = hashTable;
        hashTable = new List<(TKey key, TValue value)>[hashTable.Length * 2];

        foreach(var value in tempTable)
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
        int index = (hash & 0x7fffffff) % hashTable.Length;

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
        int index = (hash & 0x7fffffff) % hashTable.Length;

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
        int hash = key.GetHashCode();
        int index = (hash & 0x7fffffff) % hashTable.Length;

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

    public string GetHashValues(int index)
    {
        if (hashTable[index] == null)
        {
            return string.Empty;
        }

        StringBuilder sb = new StringBuilder();
        
        for(int i = 0; i < hashTable[index].Count; i++)
        {
            if(i == 0)
            {
                sb.Append($"키: {hashTable[index][i].key}, 값: {hashTable[index][i].value}");
            }
            else
            {
                sb.Append($"-> 키: {hashTable[index][i].key}, 값: {hashTable[index][i].value}");
            }
        }

        return sb.ToString();
    }
}
