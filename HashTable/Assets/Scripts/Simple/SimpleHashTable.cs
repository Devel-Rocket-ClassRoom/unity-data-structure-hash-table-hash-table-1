using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleHashTable<TKey, TValue> : IDictionary<TKey, TValue>
{
    private int capacity = 16;
    public int Count => capacity;

    private int count = 0;
    public int FactorCount => count;

    private List<KeyValuePair<TKey, TValue>> hashTable = new();
    private List<bool> occupied = new();

    public SimpleHashTable()
    {
        for (int i = 0; i < capacity; i++)
        {
            hashTable.Add(new KeyValuePair<TKey, TValue>());
            occupied.Add(false);
        }
    }

    public TValue this[TKey key]
    {
        get
        {
            int index = GetHash(key);
            return hashTable[index].Value;
        }

        set => throw new NotImplementedException();
    }

    public ICollection<TKey> Keys => throw new NotImplementedException();
    public ICollection<TValue> Values => throw new NotImplementedException();

    public bool IsReadOnly => true;

    public void Add(TKey key, TValue value)
    {
        Add(new KeyValuePair<TKey, TValue>(key, value));
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        if ((double)count / capacity > 0.75)
        {
            Resize();
        }

        int index = GetHash(item.Key);
        Debug.Log(index);

        if (occupied[index])    // 이미 키 있을 때
        {
            throw new Exception("충돌");  // 충돌 처리
        }

        hashTable[index] = item;
        occupied[index] = true;
        count++;
    }

    public void Clear()
    {
        hashTable.Clear();
        occupied.Clear();

        count = 0;

        for (int i = 0; i < capacity; i++)
        {
            hashTable[i] = new KeyValuePair<TKey, TValue>();
            occupied[i] = false;
        }
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        throw new NotImplementedException();
    }

    public bool ContainsKey(TKey key)
    {
        throw new NotImplementedException();
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return hashTable.GetEnumerator();
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }

    public bool Remove(TKey key)
    {
        int index = GetHash(key);
        hashTable[index] = new KeyValuePair<TKey, TValue>();
        occupied[index] = false;
        count--;

        return true;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private int GetHash(TKey key)
    {
        int hash = key.GetHashCode();
        return (hash & 0x7fffffff) % hashTable.Capacity;
    }

    private void Resize()
    {
        Debug.Log("Resize()");
        
        capacity *= 2;
        List<KeyValuePair<TKey, TValue>> newHashTable = new();
        List<bool> newOccupied = new();

        for (int i = 0; i < capacity; i++)
        {
            newHashTable.Add(new KeyValuePair<TKey, TValue>());
            newOccupied.Add(false);
        }

        // 모든 원소를 새 크기 기준으로 해싱해 재배치
        for (int i = 0; i < hashTable.Count; i++)
        {
            if (!occupied[i])
            {
                continue;
            }
            
            int newIndex = GetHash(hashTable[i].Key);
            newHashTable[newIndex] = hashTable[i];
            newOccupied[newIndex] = true;
        }

        hashTable = newHashTable;
        occupied = newOccupied;

        foreach (var item in hashTable)
        {
            Debug.Log(item);
        }
    }
}