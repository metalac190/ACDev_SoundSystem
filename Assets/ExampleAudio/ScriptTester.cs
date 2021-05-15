using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundSystem;

public class ScriptTester : MonoBehaviour
{
    [SerializeField] MusicEvent _songA;
    [SerializeField] MusicEvent _songB;
    [SerializeField] MusicEvent _songC;
    [SerializeField] MusicEvent _songD;

    [SerializeField] SFXOneShot _soundA;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _songA.Play(5f);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            _songB.Play(5f);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            _songC.Play(5f);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            _songD.Play(5f);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            _soundA.PlayOneShot(transform.position);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MusicManager.Instance.IncreaseLayerLevel(5f);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MusicManager.Instance.DecreaseLayerLevel(5f);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MusicManager.Instance.StopMusic(2f);
        }
    }
}
