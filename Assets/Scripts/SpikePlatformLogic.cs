using UnityEngine;

public class SpikePlatformLogic : MonoBehaviour
{
    public int spikePlatformDamage = 10;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (this.transform.parent != collision.gameObject.GetComponent<PlayerMovement>().previousDamageSource) {
                collision.gameObject.GetComponent<PlayerMovement>().TakeDamage(spikePlatformDamage);
                Debug.Log("Player landed on spike platform and took 10 damage");
                collision.gameObject.GetComponent<PlayerMovement>().previousDamageSource = this.transform.parent;
            }
            else {
                Debug.Log("Player was already hit by this spike platform");
            }
        }
    }
}
