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
    private TKey[] keys;
    private TValue[] values;
    private bool[] occupied;
    private bool[] deleted; 

    private int count;
    private readonly ProbingStrategy _strategy;

    private const int InitialCapacity = 16;
    private const double LoadFactor = 0.6;

    public int size => keys.Length;
    public OpenAddressingHashTable(ProbingStrategy strategy = ProbingStrategy.Linear)
    {
        _strategy = strategy;
        InitializeArrays(InitialCapacity);
    }

    private void InitializeArrays(int capacity)
    {
        keys = new TKey[capacity];
        values = new TValue[capacity];
        occupied = new bool[capacity];
        deleted = new bool[capacity];
        count = 0;
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
        var oldKeys = keys;
        var oldValues = values;
        var oldOccupied = occupied;
        var oldDeleted = deleted;

        InitializeArrays(resize);

        for( int i = 0; i < oldOccupied.Length;  i++ )
        {
            if (oldOccupied[i] && !oldDeleted[i])
            {
                Add(oldKeys[i], oldValues[i]);
            }
        }
    }
    public TValue this[TKey key]
    {
        get
        {
            if (TryGetValue(key, out var value))
            {
                return value;
            }
            else
            {
                throw new KeyNotFoundException($"{key}를 찾을 수 없음");
            }
        }
        set
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if(ContainsKey(key))
            {
                for (int attempt = 0; attempt < size; attempt++)
                {
                    int index = GetIndex(key, attempt);

                    if (!occupied[index] && !deleted[index])
                    {
                        break;
                    }

                    if (occupied[index] && EqualityComparer<TKey>.Default.Equals(keys[index], key))
                    {
                        values[index] = value;
                        return;
                    }
                }
            }
            else
            {
                Add(key, value);
            }
        }
    }

    public ICollection<TKey> Keys
    {
        get
        {
            TKey[] temp = new TKey[size];

            int index = 0;

            for(int i = 0; i <size; i++)
            {
                if (occupied[i] && !deleted[i])
                {
                    temp[index++] = keys[i];
                }
            }

            return temp;
        }
    }

    public ICollection<TValue> Values
    {
        get
        {
            TValue[] temp = new TValue[size];

            int index = 0;

            for (int i = 0; i < size; i++)
            {
                if (occupied[i] && !deleted[i])
                {
                    temp[index++] = values[i];
                }
            }

            return temp;
        }
    }
    public int Count => count;

    public bool IsReadOnly => false;

    public void Add(TKey key, TValue value)
    {
        if(key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        if ((double)(Count) / size > LoadFactor)
        {
            Resize();
        }

        int Tombstone = -1;

        for (int attempt = 0; attempt < size; attempt++)
        {
            int index = GetIndex(key, attempt);

            if (!occupied[index] && !deleted[index])
            {
                int insertAt = Tombstone != -1 ? Tombstone : index;

                keys[insertAt] = key;
                values[insertAt] = value;
                occupied[insertAt] = true;
                deleted[insertAt] = false;
                count++;

                return;
            }
            else if (deleted[index])
            {
                if (Tombstone == -1)
                {
                    Tombstone = index;
                }
            }
            else if (occupied[index] && EqualityComparer<TKey>.Default.Equals(keys[index], key))
            {
                 throw new ArgumentException("이미 동일한 키가 존재합니다.");
            }
        }

        throw new InvalidOperationException("해시 테이블이 가득 찼습니다.");
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        Array.Clear(keys, 0, size);
        Array.Clear(values, 0, size);
        Array.Clear(occupied, 0, size);
        Array.Clear(deleted, 0, size);
        count = 0;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        if (TryGetValue(item.Key, out TValue value))
        {
            return EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }

        return false;
    }

    public bool ContainsKey(TKey key)
    {
        if(key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        return TryGetValue(key, out _);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if(array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        if( arrayIndex < 0 )
        {
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        }

        if(array.Length - arrayIndex < count)
        {
            throw new ArgumentException(nameof(array));
        }

        for (int i = 0; i < size; i++)
        {
            if (occupied[i] && !deleted[i])
            {
                array[arrayIndex++] = new KeyValuePair<TKey, TValue>(keys[i], values[i]);
            }
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        for (int i = 0; i < size; i++)
        {
            if(occupied[i] && !deleted[i])
            {
                yield return new KeyValuePair<TKey, TValue>(keys[i], values[i]);
            }
        }
    }

    public bool Remove(TKey key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        for (int attempt = 0; attempt < size; attempt++)
        {
            int index = GetIndex(key, attempt);

            if (!occupied[index] && !deleted[index])
            {
                return false;
            }

            if (occupied[index] && EqualityComparer<TKey>.Default.Equals(keys[index], key))
            {
                occupied[index] = false;
                deleted[index] = true; 
                keys[index] = default; 
                values[index] = default;
                count--;

                return true;
            }
        }
        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        if (Contains(item))
        {
            return Remove(item.Key);
        }

        return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if(key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        for (int attempt = 0; attempt < size; attempt++)
        {
            int index = GetIndex(key, attempt);

            if (!occupied[index] && !deleted[index])
            {
                break;
            }

            if (occupied[index] && EqualityComparer<TKey>.Default.Equals(keys[index], key))
            {
                value = values[index];

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


}
