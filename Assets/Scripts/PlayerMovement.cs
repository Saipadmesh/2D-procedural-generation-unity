using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Camera cam;
    Vector2 movement;
    Vector2 mousePos;
    private bool _isDodging;
    private bool _canMove;
    public float dodgeSpeed = 10f;
    private float _defaultSpeed;
    public float dodgeDistance;

    public healthBar healthbar;
    public float maxHealth = 100f;
    public float currHealth;

    private void Start()
    {
        _defaultSpeed = moveSpeed;
        healthbar.SetMaxHealth(maxHealth);
        currHealth = maxHealth;
        _canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement.Normalize();

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    void FixedUpdate()
    {
     
        //movement
        if (_canMove)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
        
        //rotation
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y,lookDir.x)  * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;

        /*if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            moveSpeed = 10f;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed = _defaultSpeed;
        }*/

        if (Input.GetKey(KeyCode.Space))
        {
            
            Vector2 tempMovement = movement;
            if(tempMovement == Vector2.zero)
            {
                tempMovement.y = 1f;
            }
            if(_isDodging == false)
            {
                StartCoroutine(Dodge(tempMovement * dodgeSpeed, dodgeDistance));
            }
            
        }
    }

    /*void Dodge()
    {
        if(!_isDodging && Input.GetKey(KeyCode.Space))
        {
            _isDodging = true;
            Debug.Log("inside if");

        }
        if (_isDodging)
        {
            Debug.Log("inside second if");
            //rb.AddForce(movement * dodgeSpeed);
            rb.MovePosition(rb.position + movement * dodgeSpeed * Time.fixedDeltaTime);
            _isDodging = false;
        }*/

    IEnumerator Dodge(Vector2 speed, float distance)
    {
        float distanceDodged = 0;
        Vector3 startingPosition = this.transform.position;
        bool keepDodging = true;
        _isDodging = true;
        _canMove = false;

        while (distanceDodged < distance && keepDodging)
        {
            distanceDodged = Vector3.Distance(startingPosition, this.transform.position);

            // stop the dodge if we encounter a wall since we can't travel any farther
            /*if (CollidingOnSide)
            {
                keepDodging = false;
            }*/

            //moveSpeed = speed;
            rb.MovePosition(rb.position + speed * Time.deltaTime);

            yield return 0;
        }

        _canMove = true;
        yield return new WaitForSeconds(0.5f);
        _isDodging = false;
        //Player.State = PlayerStates.Idle;
    }

    private void TakeDamage(float val)
    {
        currHealth -= val;
        healthbar.SetHealth(currHealth);
    }

    private void Heal(float val)
    {
        currHealth += val;
        healthbar.SetHealth(currHealth);
    }
}

