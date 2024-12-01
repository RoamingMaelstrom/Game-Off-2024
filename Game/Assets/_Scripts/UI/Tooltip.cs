using UnityEngine;

public class Tooltip : MonoBehaviour
{
    [TextArea(3, 8)] public string content;
    public int suggestedWidth;
    public int suggestedHeight;
}
