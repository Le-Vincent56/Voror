using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bullet : MonoBehaviour
{
    #region FIELDS
    public Vector3 bulletPosition = new Vector3(0, 0, 0);
    public Vector3 bulletDirection = new Vector3(1, 0, 0);
    Vector3 mousePos;
    Vector3 velocity = new Vector3(0, 0, 0);

    public float speed = 10f;
    public float damage = 10f;
    Camera cam;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        mousePos = cam.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0));
        bulletDirection = new Vector3(mousePos.x - bulletPosition.x, mousePos.y - bulletPosition.y, 0).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate velocity
        velocity = bulletDirection * speed * Time.deltaTime;

        // Add the velocity to the bullet position
        bulletPosition += velocity;

        // Draw the bullet at the position
        transform.position = bulletPosition;
    }
}
