using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DeathZone_Script : MonoBehaviour
{
    [HideInInspector]
    public bool canMove = false;
    [SerializeField]
    private float speed;

    [SerializeField]
    private AudioSource blizzardSource;
    [SerializeField]
    private AudioSource thunderSource;
    private float maxDistance = 150f;

    [SerializeField]
    private Transform player;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.asWin || GameManager.Instance.isDead)
            return;

        if (canMove)
        {
            transform.position += transform.forward * Time.deltaTime * speed;
        }
        ChangeVolumeStorm();
    }

    void ChangeVolumeStorm()
    {
        if (GameManager.Instance.isDead || GameManager.Instance.asWin)
            return;

        float dist = Vector3.Distance(player.position, transform.position);
        float volume = 0f;

        if(dist < maxDistance)
            volume = 1f - (dist - maxDistance);

        Debug.Log(volume / 100f);
        blizzardSource.volume = volume / 100f;
        thunderSource.volume = volume / 100f;
    }
}
