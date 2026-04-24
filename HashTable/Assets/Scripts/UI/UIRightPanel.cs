using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIRightPanel : MonoBehaviour
{
    public enum HashTableType
    {
        Simple,
        Chaining,
        OpenAdressing,
    }

    public TMP_Dropdown HashTable;
    public TMP_Dropdown Adress;

    private HashTableType currentType;

    private IDictionary<string, string> hashTable;

    private Coroutine coroutine;

    public TextMeshProUGUI Key;
    public TextMeshProUGUI Value;

    public UIIndexScrollView scrollView;

    public TextMeshProUGUI logText;

    private void OnEnable()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    private void Awake()
    {
        currentType = HashTableType.Simple;
        HashTable.value = (int)currentType;
        hashTable = new SimpleHashTable<string, string>();
        scrollView.InstantiateIndex(hashTable);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        coroutine = StartCoroutine(CoSetHype());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator CoSetHype()
    {
        yield return new WaitForSeconds(0.1f);

        Adress.interactable = false;
        Adress.captionText.text = "-";
    }

    private void OnDisable()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    public void OnHashTableChange(int index)
    {
        currentType = (HashTableType)index;
        if(currentType == HashTableType.OpenAdressing)
        {
            Adress.interactable = true;
            Adress.RefreshShownValue();
        }
        else
        {
            Adress.interactable = false;
            Adress.captionText.text = "-";
        }

        switch (currentType)
        {
            case HashTableType.Simple:
                hashTable = new SimpleHashTable<string, string>();
                break;
            case HashTableType.Chaining:
                hashTable = new Chaining<string, string>();
                break;
            case HashTableType.OpenAdressing:
                hashTable = new OpenAddressingHashTable<string, string>();
                break;
        }

        scrollView.UpdateLines(hashTable);
    }

    public void OnAdd()
    {
        try
        {
            hashTable.Add(Key.text, Value.text);
            logText.text = $"{logText.text}\nADD: {Key.text} -> {Value.text}";
            scrollView.UpdateLines(hashTable);
        }
        catch (Exception)
        {
            logText.text = $"{logText.text}\nADD 실패: 키 중복";
        }
    }

    public void OnRemove()
    {
        try
        {
            hashTable.Remove(Key.text);
            logText.text = $"{logText.text}\nREMOVE: {Key.text}";
            scrollView.UpdateLines(hashTable);
        }
        catch (Exception)
        {
            logText.text = $"{logText.text}\nREMOVE 실패: 키 없음";
        }
    }

    public void OnClear()
    {
        hashTable.Clear();
        logText.text = $"{logText.text}\nCLEAR: 모든 항목 삭제됨";
        scrollView.UpdateLines(hashTable);
    }
}
