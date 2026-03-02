using UnityEngine;

public class ItemData : MonoBehaviour
{
    // Tipos posibles
    public enum ColorType { Red, Blue, Green, Yellow }

    [Header("Settings")]
    public ColorType boxColor;
}