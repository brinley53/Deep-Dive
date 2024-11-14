using UnityEngine;

public class ShopCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Entered shop trigger");
    }
}
