using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Jauge_Script : MonoBehaviour
{
    GameManager gameManager;
    private Image imageJauge;

    [SerializeField]
    private Image freezeImage;

    private float alpha;

    [SerializeField]
    private AudioSource jaugeAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        imageJauge = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckDeath();
        DecreaseJauge();
        ChangeColorJauge();
        FreezeScreen();
    }

    void DecreaseJauge()
    {
        imageJauge.fillAmount -= Time.deltaTime/gameManager.decreaseMultiplier;
    }

    public void SpecialDecreaseJauge(float value)
    {
        imageJauge.fillAmount -= value;
    }

    public void IncreaseJauge(float value)
    {
        imageJauge.fillAmount += value;
    }

    public void CheckDeath()
    {
        if (imageJauge.fillAmount <= 0) 
            gameManager.isDead = true;
    }

    void FreezeScreen()
    {
        if(imageJauge.fillAmount > gameManager.mediumValue / 100)
        {
            freezeImage.DOFade(0f, 2f);
            alpha = 0;
        }
        else if(imageJauge.fillAmount <= gameManager.mediumValue / 100)
        {
            alpha += Time.deltaTime / 50;
            freezeImage.color = new Color(1, 1, 1, alpha);
        }
    }

    void ChangeColorJauge()
    {
        if (imageJauge.fillAmount <= (gameManager.criticValue / 100))
        {
            if (imageJauge.color != gameManager.criticColor)
            {
                jaugeAudioSource.DOFade(1, 1f);
                imageJauge.DOColor(gameManager.criticColor, 0.5f);
                gameManager.isCold = true;
            }
        }
        else if (imageJauge.fillAmount <= (gameManager.mediumValue / 100))
        {
            if(imageJauge.color != gameManager.mediumColor)
            {
                jaugeAudioSource.DOFade(0, 1f);
                imageJauge.DOColor(gameManager.mediumColor, 0.5f);
                gameManager.isCold = false;
            }
        }
        else
        {
            if (imageJauge.color != gameManager.positiveColor)
            {
                jaugeAudioSource.DOFade(0, 1f);
                imageJauge.DOColor(gameManager.positiveColor, 0.5f);
                gameManager.isCold = false;
            }
        }
    }
}
