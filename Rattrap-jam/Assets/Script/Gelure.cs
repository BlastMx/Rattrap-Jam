using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Gelure : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (GameManager.Instance.isCold)
            transform.DOScale(Vector3.one, 20f);
        else
            transform.DOScale(Vector3.zero, 2f);
    }
}
