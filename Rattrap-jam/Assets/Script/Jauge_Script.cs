using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Jauge_Script : MonoBehaviour
{
    GameManager gameManager;
    private Image imageJauge;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        imageJauge = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        DecreaseJauge();
        ChangeColorJauge();
    }

    void DecreaseJauge()
    {
        imageJauge.fillAmount -= Time.deltaTime/gameManager.decreaseMultiplier;
    }

    public void SpecialDecreaseJauge(float value)
    {
        imageJauge.fillAmount -= value;
    }

    void ChangeColorJauge()
    {
        if (imageJauge.fillAmount <= (gameManager.criticValue / 100))
        {
            if (imageJauge.color != gameManager.criticColor)
            {
                imageJauge.DOColor(gameManager.criticColor, 0.5f);
            }
        }
        else
        {
            if (imageJauge.color != gameManager.positiveColor)
            {
                imageJauge.DOColor(gameManager.positiveColor, 0.5f);
            }
        }
    }
}
