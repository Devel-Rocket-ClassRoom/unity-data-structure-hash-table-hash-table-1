using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRightPanel : MonoBehaviour
{
    public enum HashTableType
    {
        Simple,
        Chaining,
        OpenAdressing,
    }

    public TMP_Dropdown HashTableDropDown;
    public TMP_Dropdown AdressDropDown;

    private HashTableType currentType;

    private Coroutine coroutine;

    private IDictionary<string, string> hashTable;
    private Chaining<string, string> chaining;

    public TextMeshProUGUI Key;
    public TextMeshProUGUI Value;

    public UIIndexScrollView scrollView;

    private void Awake()
    {
        hashTable = new SimpleHashTable<string, string>();
        currentType = HashTableType.Simple;
        scrollView.UpdateContent(hashTable);
    }

    public void OnTypeChange(int index)
    {
        currentType = (HashTableType)index;

        switch (currentType)
        {
            case HashTableType.Simple:
                hashTable = new SimpleHashTable<string, string>();
                currentType = HashTableType.Simple;
                scrollView.UpdateContent(hashTable);
                break;
            case HashTableType.Chaining:
                chaining = new Chaining<string, string>();
                currentType = HashTableType.Chaining;
                scrollView.ChainUpdate(chaining);
                break;
            case HashTableType.OpenAdressing:
                hashTable = new OpenAddressingHashTable<string, string>();
                currentType = HashTableType.OpenAdressing;
                scrollView.UpdateContent(hashTable);
                break;
        }
    }

    public void OnAdd()
    {
        
        switch (currentType)
        {
            case HashTableType.Simple:
            case HashTableType.OpenAdressing:
                hashTable.Add(Key.text, Value.text);
                scrollView.UpdateContent(hashTable);
                break;
            case HashTableType.Chaining:
                chaining.Add(Key.text, Value.text);
                scrollView.ChainUpdate(chaining);
                break;
        }
    }

    public void OnRemove()
    {
        switch (currentType)
        {
            case HashTableType.Simple:
            case HashTableType.OpenAdressing:
                hashTable.Remove(new KeyValuePair<string, string>(Key.text, Value.text));
                scrollView.UpdateContent(hashTable);
                break;
            case HashTableType.Chaining:
                chaining.Remove(new KeyValuePair<string, string>(Key.text, Value.text));
                scrollView.ChainUpdate(chaining);
                break;
        }
    }

    public void OnClear()
    {
        hashTable.Clear();
        switch (currentType)
        {
            case HashTableType.Simple:
            case HashTableType.OpenAdressing:
                scrollView.UpdateContent(hashTable);
                break;
            case HashTableType.Chaining:
                scrollView.ChainUpdate(chaining);
                break;
        }
    }
}
