using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIIndexScrollView : MonoBehaviour
{
    public GameObject IndexInfo;
    public ScrollRect ScrollRect;

    void Start()
    {
        for(int i  = 0; i < 16; i++)
        {
            Instantiate(IndexInfo, ScrollRect.content);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
