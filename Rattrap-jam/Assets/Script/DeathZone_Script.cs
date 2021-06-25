using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone_Script : MonoBehaviour
{
    [HideInInspector]
    public bool canMove = false;
    [SerializeField]
    private float speed;

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            transform.position += transform.forward * Time.deltaTime * speed;
        }
    }
}
