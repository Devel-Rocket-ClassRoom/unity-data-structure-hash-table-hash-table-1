using TMPro;
using UnityEngine;

public class UILine : MonoBehaviour
{
    public TextMeshProUGUI Text;

    public void SetText(string text)
    {
        Text.text = text;
    }
}
