using System;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using Holoville.HOTween.Core;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
	public class Sfx
	{
		private AudioSource source;

		private Tweener fader;

		public Sfx(AudioSource source, float volume, float duration)
		{
			this.source = source;
			if (duration != 0f)
			{
				source.volume = 0f;
				fader = HOTween.To(source, duration, new TweenParms().NewProp("volume", volume).OnComplete((TweenDelegate.TweenCallback)delegate
				{
					fader = null;
				}));
			}
			else
			{
				source.volume = volume;
			}
			source.Play();
		}

		public void UpdateVolume()
		{
			source.volume = Singleton<AudioManager>.instance.SfxVolume;
		}

		public void Stop(float duration)
		{
			if (fader != null)
			{
				fader.Kill();
				fader = null;
			}
			HOTween.To(source, duration, new TweenParms().NewProp("volume", 0).OnComplete((TweenDelegate.TweenCallback)delegate
			{
				source.Stop();
				source.clip = null;
			}));
		}
	}

	public class RepeatingSfx
	{
		private AudioSource source;

		private Tweener fader;

		private Action<AudioSource> freeAction;

		public RepeatingSfx(AudioSource source, float volume, float duration, Action<AudioSource> freeAction)
		{
			this.source = source;
			this.freeAction = freeAction;
			if (duration != 0f)
			{
				source.volume = 0f;
				fader = HOTween.To(source, duration, new TweenParms().NewProp("volume", volume).OnComplete((TweenDelegate.TweenCallback)delegate
				{
					fader = null;
				}));
			}
			else
			{
				source.volume = volume;
			}
			source.Play();
		}

		public void UpdateVolume()
		{
			source.volume = Singleton<AudioManager>.instance.SfxVolume;
		}

		public void Stop(float duration = 0f)
		{
			if (fader != null)
			{
				fader.Kill();
				fader = null;
			}
			if (duration > 0f)
			{
				HOTween.To(source, duration, new TweenParms().NewProp("volume", 0).OnComplete((TweenDelegate.TweenCallback)delegate
				{
					freeAction(source);
				}));
			}
			else
			{
				freeAction(source);
			}
		}
	}

	private const string musicVolumeKey = "musicVolume";

	private const string sfxVolumeKey = "sfxVolume";

	private AudioSource musicSource;

	private AudioClip musicClip;

	private Tweener musicFader;

	private bool ready;

	private float musicVolume;

	private float sfxVolume;

	private KeyValueStorage kvs;

	private Queue<AudioSource> availableSources = new Queue<AudioSource>();

	public float MusicVolume
	{
		get
		{
			return musicVolume;
		}
		set
		{
			value = Mathf.Round(value * 100f) / 100f;
			if (musicVolume != value)
			{
				musicVolume = value;
				kvs.SetValueAsync("musicVolume", value);
				if (musicSource.isPlaying)
				{
					musicSource.volume = musicVolume;
				}
			}
		}
	}

	public float SfxVolume
	{
		get
		{
			return sfxVolume;
		}
		set
		{
			value = Mathf.Round(value * 100f) / 100f;
			if (sfxVolume != value)
			{
				sfxVolume = value;
				kvs.SetValueAsync("sfxVolume", value);
			}
		}
	}

	private void Awake()
	{
		base.gameObject.AddComponent<AudioListener>();
		musicSource = base.gameObject.AddComponent<AudioSource>();
		musicSource.loop = true;
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.BootReady, delegate
		{
			WorkQueue.Do(delegate
			{
				kvs = KeyValueStorage.Instance(KeyValueStorage.Storage.PREFERENCES);
				musicVolume = kvs.GetValue<float>("musicVolume");
				sfxVolume = kvs.GetValue<float>("sfxVolume");
				return (object)null;
			}, delegate
			{
				ready = true;
				musicSource.volume = musicVolume;
				if (musicClip != null)
				{
					musicSource.Play();
				}
			});
		});
	}

	private AudioSource GetAudioSource()
	{
		if (availableSources.Count > 0)
		{
			return availableSources.Dequeue();
		}
		return base.gameObject.AddComponent<AudioSource>();
	}

	private void FreeAudioSource(AudioSource source)
	{
		source.Stop();
		source.clip = null;
		availableSources.Enqueue(source);
	}

	private void QueueMusicFade()
	{
		if (musicFader == null)
		{
			musicFader = HOTween.To(musicSource, 1f, new TweenParms().NewProp("volume", 0).OnComplete((TweenDelegate.TweenCallback)delegate
			{
				musicSource.Stop();
				musicSource.clip = musicClip;
				musicSource.volume = musicVolume;
				musicSource.Play();
				musicFader = null;
			}));
		}
	}

	public void PlayMusic(AudioClip music)
	{
		if (music == musicClip)
		{
			return;
		}
		if (musicClip == null)
		{
			musicClip = music;
			musicSource.clip = music;
			musicSource.volume = musicVolume;
			if (ready)
			{
				musicSource.Play();
			}
		}
		else
		{
			musicClip = music;
			QueueMusicFade();
		}
	}

	public void StopMusic(float duration = 0f)
	{
		if (musicSource.isPlaying)
		{
			HOTween.To(musicSource, duration, new TweenParms().NewProp("volume", 0).OnComplete((TweenDelegate.TweenCallback)delegate
			{
				musicSource.Stop();
				musicSource.clip = null;
				musicClip = null;
			}));
		}
	}

	public AudioClip CurrentMusic()
	{
		return musicClip;
	}

	public Sfx PlaySfx(AudioClip sfx, Action finishedCallback = null)
	{
		if (sfx == null)
		{
			return null;
		}
		AudioSource audioSource = GetAudioSource();
		audioSource.clip = sfx;
		audioSource.loop = false;
		audioSource.volume = sfxVolume;
		Sfx result = new Sfx(audioSource, sfxVolume, 0f);
		audioSource.Play();
		StartCoroutine(CleanupSfx(audioSource, finishedCallback));
		return result;
	}

	private IEnumerator CleanupSfx(AudioSource source, Action finishedCallback)
	{
		while (source.isPlaying)
		{
			yield return 0;
		}
		FreeAudioSource(source);
		if (finishedCallback != null)
		{
			finishedCallback();
		}
	}

	public RepeatingSfx PlayRepeatingSfx(AudioClip sfx, float fadeInDuration)
	{
		AudioSource audioSource = GetAudioSource();
		audioSource.clip = sfx;
		audioSource.loop = true;
		audioSource.volume = sfxVolume;
		return new RepeatingSfx(audioSource, sfxVolume, fadeInDuration, FreeAudioSource);
	}
}
