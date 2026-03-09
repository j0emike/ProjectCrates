using UnityEngine;

public class ScoreZone : MonoBehaviour
{
    public ItemData.ColorType zoneColor; 

    private void OnTriggerEnter(Collider other)
    {
        ItemData item = other.GetComponent<ItemData>();

        if (item != null && item.boxColor == zoneColor)
        {
            GameManager.instance.AddScore(1);

            Destroy(other.gameObject);
          
            Debug.Log("Caja {item.boxColor} entregada");
        }
        else if (item != null)
        {
            Debug.Log("Color incorrecto.");
        }
    }
}