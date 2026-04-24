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

    public TMP_Dropdown HashTableDropDown;
    public TMP_Dropdown AdressDropDown;
    public TextMeshProUGUI logText;

    private HashTableType currentType;
    private ProbingStrategy ProbingStrategyType;

    private Coroutine coroutine;

    private IDictionary<string, string> hashTable;
    private Chaining<string, string> chaining;

    public TextMeshProUGUI Key;
    public TextMeshProUGUI Value;

    public UIIndexScrollView scrollView;

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
        hashTable = new SimpleHashTable<string, string>();
        currentType = HashTableType.Simple;
        scrollView.InstantiateIndex(hashTable);
    }

    private void Start()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        coroutine = StartCoroutine(CoAdress());
    }

    private IEnumerator CoAdress()
    {
        yield return null;
        AdressDropDown.interactable = false;
        AdressDropDown.captionText.text = "-";
    }

    public void OnTypeChange(int index)
    {
        OnClear();

        currentType = (HashTableType)index;

        if(currentType == HashTableType.OpenAdressing)
        {
            AdressDropDown.interactable = true;
            AdressDropDown.value = 0;
        }
        else
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
            AdressDropDown.interactable = false;
            coroutine = StartCoroutine(CoAdress());
        }

        switch (currentType)
        {
            case HashTableType.Simple:
                hashTable = new SimpleHashTable<string, string>();
                currentType = HashTableType.Simple;
                scrollView.UpdateLines(hashTable);
                break;
            case HashTableType.Chaining:
                chaining = new Chaining<string, string>();
                currentType = HashTableType.Chaining;
                scrollView.ChainUpdate(chaining);
                break;
            case HashTableType.OpenAdressing:
                hashTable = new OpenAddressingHashTable<string, string>();
                currentType = HashTableType.OpenAdressing;
                scrollView.UpdateLines(hashTable);
                break;
        }
    }

    public void OnProbingStrategyChange(int index)
    {
        OnClear();

        ProbingStrategyType = (ProbingStrategy)index;

        switch (ProbingStrategyType)
        {
            case ProbingStrategy.Linear:
                hashTable = new OpenAddressingHashTable<string, string>(ProbingStrategyType);
                currentType = HashTableType.Simple;
                scrollView.UpdateLines(hashTable);
                break;
            case ProbingStrategy.Quadratic:
                hashTable = new OpenAddressingHashTable<string, string>(ProbingStrategyType);
                currentType = HashTableType.Chaining;
                scrollView.ChainUpdate(chaining);
                break;
            case ProbingStrategy.DoubleHash:
                hashTable = new OpenAddressingHashTable<string, string>(ProbingStrategyType);
                currentType = HashTableType.OpenAdressing;
                scrollView.UpdateLines(hashTable);
                break;
        }
    }

    public void OnAdd()
    {
        try
        {
            Debug.Log(Key.text);

            if (string.IsNullOrEmpty(Key.text))
            {
                logText.text = $"{logText.text}\nADD 실패: 빈 값";
                return;
            }

            switch (currentType)
            {
                case HashTableType.Simple:
                case HashTableType.OpenAdressing:
                    hashTable.Add(Key.text, Value.text);
                    scrollView.UpdateLines(hashTable);
                    break;
                case HashTableType.Chaining:
                    chaining.Add(Key.text, Value.text);
                    scrollView.ChainUpdate(chaining);
                    break;
            }
            logText.text = $"{logText.text}\nADD {Key.text} -> {Value.text}";
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
            switch (currentType)
        {
            case HashTableType.Simple:
            case HashTableType.OpenAdressing:
                hashTable.Remove(Key.text);
                scrollView.UpdateLines(hashTable);
                break;
            case HashTableType.Chaining:
                chaining.Remove(Key.text);
                scrollView.ChainUpdate(chaining);
                break;
        }
        }
        catch (Exception)
        {
            logText.text = $"{logText.text}\nREMOVE 실패: 키 없음";
        }
    }

    public void OnClear()
    {
        switch (currentType)
        {
            case HashTableType.Simple:
            case HashTableType.OpenAdressing:
                hashTable.Clear();
                scrollView.UpdateLines(hashTable);
                break;
            case HashTableType.Chaining:
                chaining.Clear();
                scrollView.ChainUpdate(chaining);
                break;
        }
        logText.text = $"{logText.text}\nCLEAR: 모든 항목 삭제됨";
        scrollView.UpdateLines(hashTable);
    }
}
