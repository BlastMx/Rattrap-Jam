using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterController : MonoBehaviour
{
    GameManager gameManager;

    private Animator charAnimator;
    private Rigidbody charRigidbody;

    private bool canJump = false;

    enum currentLane
    {
        LeftLane,
        MiddleLane,
        RightLane
    }

    currentLane Lane;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        charAnimator = GetComponent<Animator>();
        charRigidbody = GetComponent<Rigidbody>();

        InitCharacter();
    }

    // Update is called once per frame
    void Update()
    {
        ControlCharacterMovement();
    }

    void InitCharacter()
    {
        transform.position = Vector3.zero;
        Lane = currentLane.MiddleLane;
    }

    void ControlCharacterMovement()
    {
        if (Lane != currentLane.LeftLane && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            switch (Lane)
            {
                case currentLane.RightLane:
                    transform.DOMoveX(gameManager.posMiddleLane, 0.5f);
                    Lane = currentLane.MiddleLane;
                    break;

                case currentLane.MiddleLane:
                    transform.DOMoveX(gameManager.posLeftLane, 0.5f);
                    Lane = currentLane.LeftLane;
                    break;
            }
        }
        else if (Lane != currentLane.RightLane && Input.GetKeyDown(KeyCode.RightArrow))
        {
            switch (Lane)
            {
                case currentLane.LeftLane:
                    transform.DOMoveX(gameManager.posMiddleLane, 0.5f);
                    Lane = currentLane.MiddleLane;
                    break;

                case currentLane.MiddleLane:
                    transform.DOMoveX(gameManager.posRightLane, 0.5f);
                    Lane = currentLane.RightLane;
                    break;
            }
        }
        else if(canJump && Input.GetKeyDown(KeyCode.Space))
        {
            charRigidbody.constraints = RigidbodyConstraints.None;
            charRigidbody.AddForce(Vector3.up * 100);
            charAnimator.SetTrigger("Jump");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            charRigidbody.constraints = RigidbodyConstraints.FreezePosition;
            canJump = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        canJump = false;
    }
}
