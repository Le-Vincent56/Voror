using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    #region FIELDS
    public Vector3 playerPosition = new Vector3(0, 0, 0);
    public Vector3 direction = new Vector3(0, 0, 0);
    Vector3 velocity = new Vector3(0, 0, 0);

    public float speed = 3f;

    public bool moving = false;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Set the player position
        playerPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        moving = false;

        // Calculate velocity
        velocity = direction * speed * Time.deltaTime;

        if(velocity != Vector3.zero)
        {
            moving = true;
        }

        if (moving)
        {
            // Add the velocity to the player position
            playerPosition += velocity;

            // Draw the player at the position
            transform.position = playerPosition;
        }
    }

    /// <summary>
    /// Get movement input
    /// </summary>
    /// <param name="context">The Callback Context from the Move event</param>
    public void OnMove(InputAction.CallbackContext context)
    {
        // Get the direction vector based on player input
        direction = context.ReadValue<Vector2>();

        // Flip the sprite depending on the direction
        if(direction.x > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        } else if(direction.x < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
    }
}
