using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterControl : MonoBehaviour
{
    GameManager gameManager;

    private Animator charAnimator;
    private Rigidbody charRigidbody;

    [HideInInspector]
    public bool canJump = false;
    [HideInInspector]
    public bool canIncreaseSpeed = false;
    private bool canBoost = false;
    [HideInInspector]
    public bool canSlow = false;
    private bool lookBehind = false;

    public float playerSpeed;
    [SerializeField]
    private Jauge_Script playerJauge;

    public Transform cameraHolder;

    [Header("Energy refill Particles")]
    [SerializeField]
    private ParticleSystem energyRefillParticle;
    [SerializeField]
    private ParticleSystem twinkleParticle;

    [HideInInspector]
    public enum currentLane
    {
        LeftLane,
        MiddleLane,
        RightLane
    }

    [HideInInspector]
    public currentLane Lane;

    [Header("SpeedUp Particles")]
    [SerializeField]
    private Transform speedUp;

    [Header("Explosion particles")]
    public ParticleSystem explosion;

    [Header("Oil particle")]
    public ParticleSystem lightOilDripping;
    public ParticleSystem HeavyOilDripping;
    [SerializeField]
    private ParticleSystem oilSplat;

    [Header("Jump particles")]
    [SerializeField]
    private ParticleSystem jumpSmoke;
    [SerializeField]
    private ParticleSystem landingParticles;

    private AudioSource audioSourcePlayer;
    [SerializeField]
    private AudioSource tempestHeavy;

    [Header("Sounds")]
    [SerializeField]
    private AudioClip bonusGetRefill;
    [SerializeField]
    private AudioClip changingLane;
    [SerializeField]
    private AudioClip impactWithSnowball;
    [SerializeField]
    private AudioClip jumping;
    [SerializeField]
    private AudioClip landingOnSnow;
    [SerializeField]
    private AudioClip pushedByWind;
    [SerializeField]
    private AudioClip deafeatThunder;
    [SerializeField]
    private AudioClip runningOnSnow;
    [SerializeField]
    private AudioClip runningOnGround;
    [SerializeField]
    private AudioClip freezingIce;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        charAnimator = GetComponent<Animator>();
        charRigidbody = GetComponent<Rigidbody>();
        audioSourcePlayer = GetComponent<AudioSource>();

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
        transform.position = new Vector3(0, 0.55f, 3.92f);
        Lane = currentLane.MiddleLane;
    }

    void CharacterMovement()
    {
        transform.position += transform.forward * Time.deltaTime * playerSpeed;
        charAnimator.SetFloat("SpeedPlayer", playerSpeed);
    }

    void ControlCharacterMovement()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            lookBehind = true;
            charAnimator.SetBool("LookBehind", lookBehind);
            changeSpeedPlayer(gameManager.minSpeedValue);
            gameManager.mainCamera.DOLocalMove(new Vector3(0.5f, 1.28f, 7.57f), 0.5f);
            gameManager.mainCamera.DOLocalRotate(new Vector3(0, 194f, 0), 0.5f);
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            lookBehind = false;
            charAnimator.SetBool("LookBehind", lookBehind);
            changeSpeedPlayer(gameManager.maxSpeedValue);
            gameManager.mainCamera.DOLocalMove(Vector3.zero, 1f);
            gameManager.mainCamera.DOLocalRotate(Vector3.zero, 1f);
        }

        if (!lookBehind)
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
                ChangeClip(changingLane);
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
                ChangeClip(changingLane);
            }
            else if (canJump && Input.GetKeyDown(KeyCode.Space))
            {
                jumpSmoke.Play();
                charRigidbody.constraints = RigidbodyConstraints.None;
                charRigidbody.AddForce(Vector3.up * 150);
                charAnimator.SetTrigger("Jump");
                if (!canSlow)
                    ChangeClip(jumping);
            }
        }
    }

    void IncreaseSpeedBoost()
    {
        //playerSpeed = gameManager.boostSpeedValue;
        changeSpeedPlayer(gameManager.boostSpeedValue);
    }

    void DecreaseSpeedBoost()
    {
        playerSpeed -= Time.deltaTime * 2f;
        changeSpeedPlayer(gameManager.maxSpeedValue);
    }

    void SpeedBoost()
    {
        if (canBoost && !lookBehind)
            IncreaseSpeedBoost();
        else if (playerSpeed >= gameManager.maxSpeedValue && !canBoost)
            DecreaseSpeedBoost();
    }

    void EnterSlowZone()
    {
        if (canSlow)
            changeSpeedPlayer(gameManager.obstacleSpeedMalus);
            //playerSpeed = gameManager.obstacleSpeedMalus;
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

    void changeSpeedPlayer(float value)
    {
        DOTween.To(() => playerSpeed, x => playerSpeed = x, value, 2);
    }

    void ChangeClip(AudioClip audioClip)
    {
        audioSourcePlayer.volume = 1;
        audioSourcePlayer.PlayOneShot(audioClip);
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
                        if (canBoost)
                        {
                            canBoost = false;
                            speedUp.DOScale(Vector3.zero, 1f);
                        }

                        StartCoroutine(gameManager.cameraShake.Shake(gameManager.duration, gameManager.magnitude));
                        other.transform.parent.GetChild(1).gameObject.GetComponent<ParticleSystem>().Play();
                        oilSplat.Play();
                        ChangeClip(impactWithSnowball);
                        Destroy(other.gameObject);
                        playerJauge.SpecialDecreaseJauge(gameManager.shockObstacleDecreaser / 100);
                        changeSpeedPlayer(gameManager.obstacleSpeedMalus);
                        StartCoroutine(increaseSpeedAfterShock());
                    }
                    break;

                case "EnergyRefill":
                    playerJauge.IncreaseJauge(gameManager.increaseJaugeEnergyRefill / 100);
                    //Play particle effect
                    energyRefillParticle.Play();
                    twinkleParticle.Play();
                    //Play sound effect
                    ChangeClip(bonusGetRefill);
                    other.gameObject.transform.DOScale(Vector3.zero, 0.1f);
                    //Destroy(other.gameObject);
                    break;

                case "SpeedUpZone":
                    canBoost = true;
                    speedUp.DOScale(new Vector3(0.2f, 0.2f, 0.2f), 1f);
                    break;

                case "SlowZone":
                    canSlow = true;
                    ChangeClip(freezingIce);
                    gameManager.freezingCanvas.DOFade(1, 5f);
                    tempestHeavy.DOFade(1, 1f);
                    charAnimator.SetBool("SlowZone", canSlow);
                    break;

                case "WindZone":
                    StartCoroutine(gameManager.cameraShake.Shake(gameManager.duration, gameManager.magnitude));
                    switch (other.gameObject.GetComponent<WindZone_Script>().windPos)
                    {
                        case WindZone_Script.windZonePos.LeftLane: //Wind Zone sur la left lane -> d�place le joueur au milieu
                            transform.DOMoveX(gameManager.posMiddleLane, 0.5f);
                            Lane = currentLane.MiddleLane;
                            break;

                        case WindZone_Script.windZonePos.MiddleLane: //Wind Zone sur la middle lane -> d�place le joueur al�atoirement � gauche ou � droite
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

                        case WindZone_Script.windZonePos.RightLane: //Wind Zone sur la right lane -> d�place le joueur au milieu
                            transform.DOMoveX(gameManager.posMiddleLane, 0.5f);
                            Lane = currentLane.MiddleLane;
                            break;
                    }
                    ChangeClip(pushedByWind);
                    break;

                case "WinZone":
                    StartCoroutine(gameManager.winZone());
                    break;

                case "DeathZone":
                    StartCoroutine(gameManager.deathZoneCoroutine());
                    ChangeClip(deafeatThunder);
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
                if(canBoost)
                    canBoost = false;
                speedUp.DOScale(Vector3.zero, 1f);
                break;

            case "SlowZone":
                canSlow = false;
                gameManager.freezingCanvas.DOFade(0, 2f);
                tempestHeavy.DOFade(0, 1f);
                charAnimator.SetBool("SlowZone", canSlow);
                changeSpeedPlayer(gameManager.minSpeedValue);
                break;
        }
    }

    public void PlayLandingOnSnow()
    {
        if (!canSlow)
        {
            ChangeClip(landingOnSnow);
            landingParticles.Play();
        }
    }

    public void PlayRunningOnSnow()
    {
        audioSourcePlayer.PlayOneShot(canBoost ? runningOnGround : runningOnSnow);
    }

    public void PlayExplosion()
    {
        if(gameManager.deadByFuel)
            explosion.Play();
    }
}
