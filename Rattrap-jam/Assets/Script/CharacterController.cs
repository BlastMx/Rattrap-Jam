using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterController : MonoBehaviour
{
    GameManager gameManager;

    enum currentLane
    {
        LeftLane,
        MiddleLane,
        RightLane
    }

    [SerializeField]
    currentLane Lane;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;

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
    }
}
