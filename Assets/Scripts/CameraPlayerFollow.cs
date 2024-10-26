using UnityEngine;

public class playerFollow : MonoBehaviour
{
    public GameObject player; // drag the player object from the hierarchy over to the the spot for this in the inspector

    // Update is called once per frame
    void Update()
    {
        // on every frame, set the camera's y position to be the player's y position
        transform.position = new Vector3(0, player.transform.position.y, -10);
    }
}
