using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simple enemy patrol script, makes them walk back and forth
// on a platform (like red koopas in Mario) or between walls
public class EnemyMovement : MonoBehaviour {

    [SerializeField] float moveSpeed = 1f;

    Rigidbody2D myRigidbody2D;

    void Start() {
        myRigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Constantly sets the velocity to the proper move speed based on which
    // direction the enemy is facing, which is based on the sign of the enemy's x transform
    void Update() {
        if(IsFacingRight()) {
            myRigidbody2D.velocity = new Vector2(moveSpeed, 0);
        }
        else {
            myRigidbody2D.velocity = new Vector2(-moveSpeed, 0);
        }
    }

    private bool IsFacingRight() {
        return transform.localScale.x > 0;
    }

    // Inverts the enemy's x transform when a collider placed slightly below and in front of it
    // exits all other colliders (since the tilemap is an edge collider and does not have a body,
    // this works both for platforms and walls)
    private void OnTriggerExit2D(Collider2D collision) { // When hitting a wall, the thin collider technically isn't hitting the line of the wall anymore, so it exits
        transform.localScale = new Vector2(-(Mathf.Sign(myRigidbody2D.velocity.x)), transform.localScale.y);
    }
}
