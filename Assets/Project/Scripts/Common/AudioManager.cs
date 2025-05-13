using System;
using UnityEngine;

namespace IdleTycoon
{
    [System.Serializable]
    public struct SoundParameters
    {
        [Range(0, 1)]
        public float Volume;
        [Range(-3, 3)]
        public float Pitch;
        public bool Loop;
    }

    [System.Serializable]
    public class Sound
    {
        [SerializeField] String name = String.Empty;
        public String Name => name;

        [SerializeField] AudioClip clip = null;
        public AudioClip Clip => clip;

        [SerializeField] SoundParameters parameters = new SoundParameters();
        public SoundParameters Parameters => parameters;

        [HideInInspector]
        public AudioSource Source = null;

        public void Play()
        {
            Source.clip = Clip;

            Source.volume = Parameters.Volume;
            Source.pitch = Parameters.Pitch;
            Source.loop = Parameters.Loop;

            Source.Play();
        }

        public void Stop()
        {
            Source.Stop();
        }
    }

    public class AudioManager : MonoBehaviour
    {
        [SerializeField] Sound[] sounds = null;
        [SerializeField] AudioSource sourcePrefab = null;

        [SerializeField] String startupTrack = String.Empty;

        public bool isSoundActive;

        void Awake()
        {
            if (Service<AudioManager>.Get() != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Service<AudioManager>.Set(this);
                DontDestroyOnLoad(gameObject);
            }
        }

        //void Start()
        //{
        //    if (string.IsNullOrEmpty(startupTrack) != true)
        //    {
        //        PlaySound(startupTrack);
        //    }
        //}

        void PlayStartupTrack()
        {
            if (string.IsNullOrEmpty(startupTrack) != true)
            {
                PlaySound(startupTrack);
            }
        }

        public void InitSounds()
        {
            foreach (var sound in sounds)
            {
                AudioSource source = Instantiate<AudioSource>(sourcePrefab, gameObject.transform);
                source.name = sound.Name;
                sound.Source = source;
            }

            PlayStartupTrack();
        }

        public void PlaySound(string name)
        {
            if (!isSoundActive) return;

            var sound = GetSound(name);
            if (sound != null)
            {
                sound.Play();
            }
            else
            {
                Debug.LogWarning("Sound by the name " + name + " is not found! Issues occured at AudioManager.PlaySound()");
            }
        }

        public void PlayMusic(string name)
        {
            var sound = GetSound(name);
            if (sound != null)
            {
                sound.Play();
            }
            else
            {
                Debug.LogWarning("Sound by the name " + name + " is not found! Issues occured at AudioManager.PlaySound()");
            }
        }

        public void StopSound(string name)
        {
            var sound = GetSound(name);
            if (sound != null)
            {
                sound.Stop();
            }
            else
            {
                Debug.LogWarning("Sound by the name " + name + " is not found! Issues occured at AudioManager.StopSound()");
            }
        }

        public void SetActiveSounds(bool value)
        {
            isSoundActive = value;
        }

        public void SetActiveStartupTrack(bool value)
        {
            if (value)
            {
                PlayMusic(startupTrack);
            }
            else
            {
                StopSound(startupTrack);
            }
        }

        Sound GetSound(string name)
        {
            foreach (var sound in sounds)
            {
                if (sound.Name == name)
                {
                    return sound;
                }
            }
            return null;
        }
    }
}