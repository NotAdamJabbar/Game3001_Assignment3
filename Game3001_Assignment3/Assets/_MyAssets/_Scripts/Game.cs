using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; } // Static object of the class.
    public SoundManager SOMA;

    private void Awake() // Ensure there is only one instance.
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Will persist between scenes.
            Initialize();
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances.
        }
    }

    private void Initialize()
    {
        SOMA = new SoundManager();
        SOMA.Initialize(gameObject);
        SOMA.AddSound("Mutara", Resources.Load<AudioClip>("Audio/Mutara"), SoundManager.SoundType.SOUND_MUSIC);
        SOMA.AddSound("Klingon", Resources.Load<AudioClip>("Audio/Klingon"), SoundManager.SoundType.SOUND_MUSIC);
        SOMA.AddSound("Torpedo_k", Resources.Load<AudioClip>("Audio/torpedo_k"), SoundManager.SoundType.SOUND_SFX);
        SOMA.AddSound("Torpedo", Resources.Load<AudioClip>("Audio/torpedo"), SoundManager.SoundType.SOUND_SFX);

        SOMA.PlayMusic("Klingon");
    }
}
