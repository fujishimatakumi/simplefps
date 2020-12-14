using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootSound : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Sound()
    {
        audioSource.PlayOneShot(audioSource.clip);
    }
}
