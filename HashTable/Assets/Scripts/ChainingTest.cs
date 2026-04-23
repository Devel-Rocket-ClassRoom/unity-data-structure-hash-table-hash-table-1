using UnityEngine;

public class ChainingTest : MonoBehaviour
{
    private Chaining<int, string> chain = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chain.Add(1, "asd");
        chain.Add(2, "afc");
        chain.Add(3, "qer");
        chain.Add(4, "erg");
        chain.Add(1, "ntw");
        chain.Add(1, "fbg");
        chain.Add(1234, "fbg");
        chain.Add(432, "fbg");
        chain.Add(245, "fbg");
        chain.Add(23545, "fbg");
        chain.Add(234, "fbg");
        chain.Add(52343, "fbg");
        chain.Add(521, "fbg");
        chain.Add(2352, "fbg");
        chain.Add(23523, "fbg");

        for(int i = 0; i < chain.Count; i++)
        {
            Debug.Log(chain.GetHashValues(i));
        }
    }
}
