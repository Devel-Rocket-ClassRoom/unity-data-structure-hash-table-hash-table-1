using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIIndexScrollView : MonoBehaviour
{
    public UILine IndexInfo;
    public ScrollRect ScrollRect;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InstantiateIndex(IDictionary<string, string> table)
    {
        var keys = table.Keys.ToList();
        var values = table.Values.ToList();



        StringBuilder sb = new StringBuilder();
        for(int i = 0; i < table.Count; i++)
        {
            Instantiate(IndexInfo, ScrollRect.content);
        }
    }
}
