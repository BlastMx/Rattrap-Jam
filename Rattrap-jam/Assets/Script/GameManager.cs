using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    public float minSpeedValue;
    public float maxSpeedValue;
    public float boostSpeedValue;
    public float secondsBeforeIncrease;
    [HideInInspector]
    public bool isCold = false;
    [HideInInspector]
    public bool isDead = false;

    [Header("Camera")]
    public Transform mainCamera;

    [Header("Start Menu")]
    [SerializeField]
    private CanvasGroup GameCanvas;
    [SerializeField]
    private CanvasGroup StartMenuCanvas;
    [HideInInspector]
    public bool isInStartMenu = true;

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

        GameCanvas.DOFade(0, 0.1f);
        StartMenuCanvas.DOFade(1, 0.1f);
    }

    public void StartGame()
    {
        StartMenuCanvas.DOFade(0, 1f).OnPlay(()=> 
        { 
            mainCamera.DOLocalMove(Vector3.zero, 1f);
            mainCamera.DOLocalRotate(Vector3.zero, 1f);
        }).OnComplete(()=> {
            GameCanvas.DOFade(1, 1f).OnComplete(() =>
            {
                isInStartMenu = false;
                player.canIncreaseSpeed = true;
                player.playerSpeed = minSpeedValue;
            });
        });
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
