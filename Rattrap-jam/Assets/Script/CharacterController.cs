using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        ControlCharacterMovement();
    }

    void ControlCharacterMovement()
    {
        if (transform.position.x != gameManager.posLeftLane && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position = new Vector3(transform.position.x + gameManager.posLeftLane, transform.position.y);
        }
        else if (transform.position.x != gameManager.posRightLane && Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position = new Vector3(transform.position.x + gameManager.posRightLane, transform.position.y);
        }
    }
}
