using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Lane Position X")]
    public float posLeftLane;
    public float posMiddleLane;
    public float posRightLane;

    [Header("Jauge")]
    public float increaseJaugeEnergyRefill;
    public float shockObstacleDecreaser;
    public float decreaseMultiplier;
    public float mediumValue;
    public float criticValue;
    public Color positiveColor;
    public Color mediumColor;
    public Color criticColor;

    [Header("Shake")]
    public CameraShake cameraShake;
    public float duration;
    public float magnitude;

    [Header("Player")]
    public CharacterControl player;
    public float obstacleSpeedMalus;
    public float minSpeedValue;
    public float maxSpeedValue;
    public float boostSpeedValue;
    public float secondsBeforeIncrease;
    [HideInInspector]
    public bool isCold = false;
    [HideInInspector]
    public bool isDead = false;
    [HideInInspector]
    public bool asWin = false;
    [HideInInspector]
    public bool deadByFuel = false;

    [Header("Camera")]
    public Transform mainCamera;

    [Header("Canvas Manager")]
    public CanvasGroup GameCanvas;
    public Image freezingCanvas; 
    [SerializeField]
    private CanvasGroup StartMenuCanvas;
    [SerializeField]
    private CanvasGroup MainMenuCanvas;
    [SerializeField]
    private CanvasGroup HowToPlayCanvas;
    public CanvasGroup WinCanvas;
    public CanvasGroup DeathCanvas;

    [Header("Main Menu")]
    [HideInInspector]
    public bool isInStartMenu = true;

    [Header("How To Play Meu")]
    [SerializeField]
    private Image previousButtonImage;
    [SerializeField]
    private Image nextButtonImage;
    [SerializeField]
    private Transform page1;
    [SerializeField]
    private Transform page2;

    [Header("Death Zone")]
    public DeathZone_Script deathZone;
    [SerializeField]
    private float secondsBeforeStartMove;

    [Header("Sound")]
    public AudioSource audioManager;
    public AudioSource audioPlayer;
    [SerializeField]
    private AudioSource audioBlizzard;
    [SerializeField]
    private AudioSource audioThunder;
    [SerializeField]
    private AudioSource audioWin;

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        mainCamera.localPosition = new Vector3(0.25f, 0.2f, 5f);

        InitCanvas();
    }

    void InitCanvas()
    {
        GameCanvas.DOFade(0, 0.1f);
        GameCanvas.gameObject.SetActive(false);

        WinCanvas.DOFade(0, 0.1f);
        WinCanvas.gameObject.SetActive(false);

        DeathCanvas.DOFade(0, 0.1f);
        DeathCanvas.gameObject.SetActive(false);

        HowToPlayCanvas.DOFade(0, 0.1f);
        HowToPlayCanvas.gameObject.SetActive(false);

        StartMenuCanvas.DOFade(1, 0.1f);

    }

    public void StartGame()
    {
        StartMenuCanvas.DOFade(0, 1f).OnPlay(()=> 
        { 
            mainCamera.DOLocalMove(Vector3.zero, 1f);
            mainCamera.DOLocalRotate(Vector3.zero, 1f);
        }).OnComplete(()=> {
            GameCanvas.gameObject.SetActive(true);
            GameCanvas.DOFade(1, 1f).OnComplete(() =>
            {
                isInStartMenu = false;
                player.canIncreaseSpeed = true;
                player.playerSpeed = minSpeedValue;
                StartCoroutine(moveDeathZone());
            });
        });
    }

    IEnumerator moveDeathZone()
    {
        yield return new WaitForSeconds(secondsBeforeStartMove);
        deathZone.canMove = true;
    }

    public void BackMainMenu()
    {
        if (asWin || isDead)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        else
        {
            HowToPlayCanvas.DOFade(0, 1f).OnComplete(() => { 
                HowToPlayCanvas.gameObject.SetActive(false);
                MainMenuCanvas.gameObject.SetActive(true);
                MainMenuCanvas.DOFade(1, 1f);
            });
        }
    }

    public void HowToPlayMenu()
    {
        MainMenuCanvas.DOFade(0, 1f).OnComplete(() => {
            HowToPlayCanvas.gameObject.SetActive(true);
            MainMenuCanvas.gameObject.SetActive(false); 
            HowToPlayCanvas.DOFade(1, 1f); 
        });
    }

    public void ChangePageOptions(int index)
    {
        switch (index)
        {
            case 1:
                previousButtonImage.DOFade(0, 0.5f).OnComplete(() =>
                {
                    page2.DOLocalMoveX(1920f, 1f);
                    page1.DOLocalMoveX(0f, 1f).OnComplete(() => {
                        nextButtonImage.DOFade(1f, 0.5f);
                    });
                });
                break;

            case 2:
                nextButtonImage.DOFade(0, 0.5f).OnComplete(() =>
                {
                    page1.DOLocalMoveX(-1920f, 1f);
                    page2.DOLocalMoveX(0f, 1f).OnComplete(()=> {
                        previousButtonImage.DOFade(1f, 0.5f);
                    });
                });
                break;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public IEnumerator winZone()
    {
        player.transform.DOMoveX(posMiddleLane, 0.5f);
        player.Lane = CharacterControl.currentLane.MiddleLane;

        asWin = true;
        player.cameraHolder.parent = null;
        player.stormParticles.parent = null;

        GameCanvas.DOFade(0, 1f);

        audioManager.DOFade(0, 1f);
        audioPlayer.DOFade(0, 1f);
        audioBlizzard.DOFade(0, 1f);
        audioThunder.DOFade(0, 1f);

        yield return new WaitForSeconds(1);

        audioWin.DOFade(1f, 1f);

        WinCanvas.gameObject.SetActive(true);
        WinCanvas.DOFade(1, 1f);
    }

    public IEnumerator deathZoneCoroutine()
    {
        isDead = true;

        GameCanvas.DOFade(0, 1f);

        yield return new WaitForSeconds(1);

        DeathCanvas.gameObject.SetActive(true);
        DeathCanvas.DOFade(1, 1f);
    }

    public void ChangeAmbiantSounds(bool value)
    {
        if (value)
        {
            audioPlayer.volume = 0;
            audioManager.DOFade(0, 0.5f);
            audioPlayer.DOFade(1, 0.5f);
        }
        else if(!value)
        {
            audioManager.DOFade(1, 0.5f);
            audioPlayer.DOFade(0, 0.5f);
        }
    }

    public void ButtonSound()
    {
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Return))
            return;

        GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
    }
}
