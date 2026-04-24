using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIIndexScrollView : MonoBehaviour
{
    public UILine IndexInfo;
    public ScrollRect ScrollRect;

    private List<UILine> lines = new();

    public void InstantiateIndex(IDictionary<string, string> table)
    {
        int index = 0;
        foreach (var value in table)
        {
            var line = Instantiate(IndexInfo, ScrollRect.content);
            lines.Add(line);

            StringBuilder sb = new StringBuilder();
            sb.Append($"l: {index} ");
            if (value.Key != null)
            {
                sb.Append($"key:{value.Key}, value: {value.Value}");
                line.GetComponent<Image>().color = Color.green;
            }
            else
            {
                line.GetComponent<Image>().color = Color.white;
            }

            line.SetText(sb.ToString());

            index++;
        }
    }

    public void UpdateLines(IDictionary<string, string> table)
    {
        if(table.Count > lines.Count)
        {
            var lines = GameObject.FindGameObjectsWithTag("line");
            foreach(var line in lines)
            {
                Destroy(line);
            }

            InstantiateIndex(table);
            return;
        }

        int index = 0;
        foreach (var value in table)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"I: {index} ");
            if(value.Key != null)
            {
                sb.Append($"key: {value.Key}, value: {value.Value}");
                lines[index].GetComponent<Image>().color = Color.green;
            }
            else
            {
                lines[index].GetComponent<Image>().color = Color.white;
            }

            lines[index].SetText(sb.ToString());
            index++;
        }

    }

    public void ChainUpdate(Chaining<string, string> table)
    {
        if (lines.Count < table.Count)
        {
            for (int i = lines.Count; i < table.Count; i++)
            {
                var newLine = Instantiate(IndexInfo, ScrollRect.content);
                lines.Add(newLine);
            }
        }

        for(int i = 0; i < lines.Count; i++)
        {
            var temp = ((Chaining<string, string>)table).GetList(i);
            if(temp.Count != 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"I: {i} ");
                for(int j = 0; j < temp.Count; j++)
                {
                    if(j == 0)
                    {
                        sb.Append($"key: {temp[j].key}, value: {temp[j].value}");
                    }
                    else
                    {
                        sb.Append($" -> key: {temp[j].key}, value: {temp[j].value}");
                    }
                }
                lines[i].SetText(sb.ToString());
                lines[i].GetComponent<Image>().color = Color.green;
            }
            else
            {
                lines[i].SetText($"I: {i}");
                lines[i].GetComponent<Image>().color = Color.white;
            }
        }
    }
}
