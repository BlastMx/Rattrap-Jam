using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterControl : MonoBehaviour
{
    GameManager gameManager;

    private Animator charAnimator;
    private Rigidbody charRigidbody;

    private bool canJump = false;
    private bool canIncreaseSpeed = false;
    private bool canBoost = false;
    private bool canSlow = false;

    [SerializeField]
    private float playerSpeed;
    [SerializeField]
    private Jauge_Script playerJauge;

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
        if (gameManager.isDead)
        {
            charAnimator.SetTrigger("Dying");
            return;
        }

        ControlCharacterMovement();
        CharacterMovement();
        IncreaseSpeed();
        EnterSlowZone();
        SpeedBoost();
    }

    void InitCharacter()
    {
        transform.position = Vector3.zero;
        Lane = currentLane.MiddleLane;

        playerSpeed = gameManager.minSpeedValue;
        canIncreaseSpeed = true;
    }

    void CharacterMovement()
    {
        transform.position += transform.forward * Time.deltaTime * playerSpeed;
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
            charRigidbody.AddForce(Vector3.up * 150);
            charAnimator.SetTrigger("Jump");
        }
    }

    void IncreaseSpeedBoost()
    {
        playerSpeed = gameManager.boostSpeedValue;
    }

    void DecreaseSpeedBoost()
    {
        playerSpeed -= Time.deltaTime * 2f;
    }

    void SpeedBoost()
    {
        if (canBoost)
            IncreaseSpeedBoost();
        else if (playerSpeed >= gameManager.maxSpeedValue && !canBoost)
            DecreaseSpeedBoost();
    }

    void EnterSlowZone()
    {
        if (canSlow)
            playerSpeed = gameManager.minSpeedValue;

    }

    void IncreaseSpeed()
    {
        if (!canIncreaseSpeed || canSlow || canBoost || playerSpeed >= gameManager.maxSpeedValue)
            return;

        if (canIncreaseSpeed)
        {
            playerSpeed += Time.deltaTime / 2;
        }
    }

    IEnumerator increaseSpeedAfterShock()
    {
        yield return new WaitForSeconds(gameManager.secondsBeforeIncrease);
        canIncreaseSpeed = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Floor":
                charRigidbody.constraints = RigidbodyConstraints.FreezePosition;
                canJump = true;
                break;

            case "Obstacle":
                if (canJump)
                {
                    StartCoroutine(gameManager.cameraShake.Shake(gameManager.duration, gameManager.magnitude));
                    playerJauge.SpecialDecreaseJauge(gameManager.shockObstacleDecreaser / 100);
                    playerSpeed = gameManager.minSpeedValue;
                    StartCoroutine(increaseSpeedAfterShock());
                }
                break;

            case "EnergyRefill":
                playerJauge.IncreaseJauge(gameManager.increaseJaugeEnergyRefill / 100);
                //Play particle effect
                //Play sound effect
                Destroy(other.gameObject);
                break;

            case "SpeedUpZone":
                canBoost = true;
                break;

            case "SlowZone":
                canSlow = true;
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Floor":
                canJump = false;
                break;

            case "SpeedUpZone":
                canBoost = false;
                break;

            case "SlowZone":
                canSlow = false;
                break;
        }
    }
}
