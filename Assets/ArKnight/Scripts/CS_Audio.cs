using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Audio : MonoBehaviour
{
    public AudioClip put;
    public AudioClip die;
    public AudioClip hpDecrease;
    public AudioClip win;
    public AudioClip skill;
    public AudioSource audioSource;
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
}
