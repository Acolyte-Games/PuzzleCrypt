using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float speed; // Player movement speed

    void Awake()
    {
        speed = 5f; // Set the movement speed
    }

    void Update()
    {
        //Controls movement in all four directions
        if (Input.GetKey(KeyCode.W)) 
        { 
            transform.Translate(Vector2.up * testVert(Vector2.up));
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector2.left * testHor(Vector2.left));
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector2.down * testVert(Vector2.down));
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector2.right *testHor(Vector2.right));
        }
    }

    private float testHor(Vector2 dir)
    {
        Vector2 origin = transform.position;
        float offset;
        if (dir.Equals(Vector2.left))
        {
            offset = -0.125f; //Offset for the left side of the character
        } else
        {
            offset = 0.125f; //Offset for the right side of the character
        }

        origin.x += offset;
        RaycastHit2D raycast = Physics2D.Raycast(origin, dir, speed * Time.deltaTime);

        if (raycast.collider != null && raycast.collider.gameObject.tag == "Collidable") //If raycast hits something tagged "Collideable" 
        {
            float distance = Math.Abs(raycast.point.x - origin.x);
            return distance;
        }

        return speed * Time.deltaTime;
    }

    private float testVert(Vector2 dir)
    {
        Vector2 origin = transform.position;
        float offset;
        if (dir.Equals(Vector2.up))
        {
            offset = .1f; //Offset for top side of character
        } else
        {
            offset = -0.5f; //Offset for bottom side of character
        }
        origin.y += offset;
        RaycastHit2D raycast = Physics2D.Raycast(origin, dir, speed * Time.deltaTime);

        if (raycast.collider != null && raycast.collider.gameObject.tag == "Collidable") //If raycast hits something tagged "Collideable" 
        {
            float distance = Math.Abs(raycast.point.y - origin.y);
            return distance;
        }

        return speed * Time.deltaTime;
    }
}