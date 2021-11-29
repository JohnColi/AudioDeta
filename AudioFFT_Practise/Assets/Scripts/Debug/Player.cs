using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public AudioSource PlayerAttack;
    public AudioSource EnemyExplosion;
    public AudioSource BGM;
    public AudioSource ObjAudio;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            PlayerAttack.Play();

        if (Input.GetKeyDown(KeyCode.E))
            EnemyExplosion.Play();

        if (Input.GetKeyDown(KeyCode.B))
            BGM.Play();

        if (Input.GetKeyDown(KeyCode.O))
            ObjAudio.Play();
    }
}
