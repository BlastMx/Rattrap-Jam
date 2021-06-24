using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float minSpeedValue;
    public float maxSpeedValue;
    public float boostSpeedValue;
    public float secondsBeforeIncrease;
    [HideInInspector]
    public bool isCold = false;
    [HideInInspector]
    public bool isDead = false;

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
    }
}
