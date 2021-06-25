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
    [HideInInspector]
    public bool canIncreaseSpeed = false;
    private bool canBoost = false;
    private bool canSlow = false;

    public float playerSpeed;
    [SerializeField]
    private Jauge_Script playerJauge;

    public Transform cameraHolder;

    [SerializeField]
    private ParticleSystem energyRefillParticle;

    [HideInInspector]
    public enum currentLane
    {
        LeftLane,
        MiddleLane,
        RightLane
    }

    [HideInInspector]
    public currentLane Lane;

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
        if (gameManager.isInStartMenu)
            return;

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
    }

    void CharacterMovement()
    {
        transform.position += transform.forward * Time.deltaTime * playerSpeed;
        charAnimator.SetFloat("SpeedPlayer", playerSpeed);
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
        if (!gameManager.asWin || !gameManager.isDead)
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
                        other.transform.parent.GetChild(1).gameObject.GetComponent<ParticleSystem>().Play();
                        Destroy(other.gameObject);
                        playerJauge.SpecialDecreaseJauge(gameManager.shockObstacleDecreaser / 100);
                        playerSpeed = gameManager.obstacleSpeedMalus;
                        StartCoroutine(increaseSpeedAfterShock());
                    }
                    break;

                case "EnergyRefill":
                    playerJauge.IncreaseJauge(gameManager.increaseJaugeEnergyRefill / 100);
                    //Play particle effect
                    energyRefillParticle.Play();
                    //Play sound effect
                    Destroy(other.gameObject);
                    break;

                case "SpeedUpZone":
                    canBoost = true;
                    break;

                case "SlowZone":
                    canSlow = true;
                    charAnimator.SetBool("SlowZone", canSlow);
                    break;

                case "WindZone":
                    StartCoroutine(gameManager.cameraShake.Shake(gameManager.duration, gameManager.magnitude));
                    switch (other.gameObject.GetComponent<WindZone_Script>().windPos)
                    {
                        case WindZone_Script.windZonePos.LeftLane: //Wind Zone sur la left lane -> déplace le joueur au milieu
                            transform.DOMoveX(gameManager.posMiddleLane, 0.5f);
                            Lane = currentLane.MiddleLane;
                            break;

                        case WindZone_Script.windZonePos.MiddleLane: //Wind Zone sur la middle lane -> déplace le joueur aléatoirement à gauche ou à droite
                            int randomValue = Random.Range(0, 2);
                            switch (randomValue)
                            {
                                case 0:
                                    transform.DOMoveX(gameManager.posLeftLane, 0.5f);
                                    Lane = currentLane.LeftLane;
                                    break;

                                case 1:
                                    transform.DOMoveX(gameManager.posRightLane, 0.5f);
                                    Lane = currentLane.RightLane;
                                    break;
                            }
                            break;

                        case WindZone_Script.windZonePos.RightLane: //Wind Zone sur la right lane -> déplace le joueur au milieu
                            transform.DOMoveX(gameManager.posMiddleLane, 0.5f);
                            Lane = currentLane.MiddleLane;
                            break;
                    }
                    break;

                case "WinZone":
                    StartCoroutine(gameManager.winZone());
                    break;

                case "DeathZone":
                    StartCoroutine(gameManager.deathZoneCoroutine());
                    break;
            }
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
                charAnimator.SetBool("SlowZone", canSlow);
                break;
        }
    }
}
