using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

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

    [Header("Camera")]
    public Transform mainCamera;

    [Header("Canvas Manager")]
    public CanvasGroup GameCanvas;
    [SerializeField]
    private CanvasGroup StartMenuCanvas;
    public CanvasGroup WinCanvas;
    public CanvasGroup DeathCanvas;

    [Header("Main Menu")]
    [HideInInspector]
    public bool isInStartMenu = true;

    [Header("Death Zone")]
    public DeathZone_Script deathZone;
    [SerializeField]
    private float secondsBeforeStartMove;

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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
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

        GameCanvas.DOFade(0, 1f);

        yield return new WaitForSeconds(1);

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
}
