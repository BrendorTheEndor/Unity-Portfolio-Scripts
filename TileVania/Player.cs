using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

// This script handles everything about the player, like movement, animations, etc
public class Player : MonoBehaviour {

    // Const variables used for animation states
    const string RUNNING_BOOL = "isRunning";
    const string CLIMBING_BOOL = "isClimbing";
    const string JUMPING_BOOL = "isJumping";
    const string DEAD_TRIGGER = "Die";
    const string GROUND_LAYER = "Ground";
    const string CLIMBING_LAYER = "Climbing";
    const string ENEMY_LAYER = "Enemy";
    const string HAZARDS_LAYER = "Hazards";

    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float climbSpeed = 1f;
    [SerializeField] float jumpHeight = 1f;
    [SerializeField] int groundlayer = 8;
    [SerializeField] Vector2 deathKick = new Vector2(14f, 23f);

    bool isAlive = true;

    Rigidbody2D myRigidbody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider2D;
    BoxCollider2D myFeetCollider2D;
    float startingGravity;
    GameSession myGameSession;

    void Start() {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider2D = GetComponent<CapsuleCollider2D>();
        myFeetCollider2D = GetComponent<BoxCollider2D>();
        myGameSession = FindObjectOfType<GameSession>();
        startingGravity = myRigidbody.gravityScale;
    }

    // Resets animation states where needed, resets gravity (it's altered in a later method)
    // and performs all the collision checks and input handling
    void Update() {

        myAnimator.SetBool(RUNNING_BOOL, false);
        myAnimator.SetBool(CLIMBING_BOOL, false);
        myAnimator.SetBool(JUMPING_BOOL, false);
        myRigidbody.gravityScale = startingGravity;

        if(!isAlive) {
            return;
        }

        HazardsCheck();
        EnemyCheck();
        Run();
        ClimbLadder();
        Jump();
        Animations();
    }

    // Checks if either of the player's colliders are touching a hazard, and kills the player if so
    private void HazardsCheck() {
        if((myBodyCollider2D.IsTouchingLayers(LayerMask.GetMask(HAZARDS_LAYER)))
    || (myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask(HAZARDS_LAYER)))) {
            Die();
        }
    }

    // Same as above but for enemies
    private void EnemyCheck() {
        if((myBodyCollider2D.IsTouchingLayers(LayerMask.GetMask(ENEMY_LAYER)))
            || (myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask(ENEMY_LAYER)))) {
            Die();
        }
    }

    // Kills the player and has the GameSession (handles scene loading and things of that nature) process this
    private void Die() {
        myRigidbody.velocity = deathKick;
        myAnimator.SetTrigger(DEAD_TRIGGER);
        isAlive = false;
        myGameSession.ProcessPlayerDeath();
    }

    // Player movement
    private void Run() {
        // Gonna be from -1 to +1; it's the amount to "throw" each frame; this is the actual movement input
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal");
        Vector2 playerVelocity = new Vector2(controlThrow * moveSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;
    }

    // Allows player to climb up and down ladders without sliding down (by setting gravity to 0)
    private void ClimbLadder() {
        if(!myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask(CLIMBING_LAYER))) { return; }

        myRigidbody.gravityScale = 0f;
        float controlThrow = CrossPlatformInputManager.GetAxis("Vertical");
        Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, controlThrow * climbSpeed);
        myRigidbody.velocity = climbVelocity;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool(CLIMBING_BOOL, playerHasVerticalSpeed);
    }

    // processes player jumping by adding a certain amount to their y velocity
    private void Jump() {

        if(!myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask(GROUND_LAYER))) { return; }

        if(CrossPlatformInputManager.GetButtonDown("Jump")) {
            Vector2 jumpVelocityToAdd = new Vector2(myRigidbody.velocity.x, jumpHeight);
            myRigidbody.velocity = jumpVelocityToAdd;
        }
    }

    // Handles player animation states
    private void Animations() {

        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;

        if(playerHasHorizontalSpeed) {
            myAnimator.SetBool(RUNNING_BOOL, true);
            // Flips the sprite if need be
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), transform.localScale.y);
        }

        string[] layers = { GROUND_LAYER, CLIMBING_LAYER };

        if(!myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask(layers))) {
            myAnimator.SetBool(JUMPING_BOOL, true);
        }
    }
}
