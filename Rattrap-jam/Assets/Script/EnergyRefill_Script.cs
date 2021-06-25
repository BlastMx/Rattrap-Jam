using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnergyRefill_Script : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.DOMoveY(1.5f, 2f).SetLoops(4, LoopType.Yoyo);
    }
}
