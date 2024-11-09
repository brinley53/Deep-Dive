/**
Create date: 11/8/2024
**/

using System.Numerics;
using UnityEngine;

public class PlayerFlip : MonoBehaviour
{
    public float horizontalInput;
    public bool facingRight = true;

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        
        SetDirection();
    }

    private void SetDirection() {
        if (horizontalInput < 0 && facingRight || horizontalInput > 0 && !facingRight) {
            facingRight = !facingRight;
            transform.Rotate(new UnityEngine.Vector3(0,180,0));
        }
    }
}
