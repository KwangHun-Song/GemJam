using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Record;

namespace Utility {

    [CreateAssetMenu]
    public class SimpleSound : ScriptableObject {
        public List<AudioClip> bgm;
        public List<AudioClip> sfx;
        private Dictionary<string, AudioClip> bgmDict = new Dictionary<string, AudioClip>();
        private Dictionary<string, AudioClip> sfxDict = new Dictionary<string, AudioClip>();
        private Dictionary<string, float> lastPlayTime = new Dictionary<string, float>();
        
        private static SimpleSound _instance;
        public static SimpleSound Instance {
            get {
                if (_instance == null)
                    _instance = FindObjectOfType<SimpleSound>();
                if (_instance == null)
                    Debug.LogError("SimpleSound instance not found!");
                return _instance;
            }
        }
        
        private static GameObject BGMSource;
        private static GameObject SFXSource;
        private static List<AudioSource> bgmAudios = new List<AudioSource>();
        private static List<AudioSource> sfxAudios = new List<AudioSource>();

        // [RuntimeInitializeOnLoadMethod]
        private static void Initialize() {
            return;
            _instance = Resources.Load<SimpleSound>(nameof(SimpleSound));
            BGMSource = new GameObject("BGM Audio");
            SFXSource = new GameObject("SFX Audio");
            foreach (var bgm in Instance.bgm) Instance.bgmDict[bgm.name] = bgm;
            foreach (var sfx in Instance.sfx) Instance.sfxDict[sfx.name] = sfx;
            foreach (var pair in Instance.bgmDict) Instance.lastPlayTime[pair.Key] = 0f;
            foreach (var pair in Instance.sfxDict) Instance.lastPlayTime[pair.Key] = 0f;

            DontDestroyOnLoad(BGMSource);
            DontDestroyOnLoad(SFXSource);
        }

        public static void Play(SoundName soundName,
            bool loop = false,
            float minInterval = 0.05f,
            float volume = 1f,
            float pitch = 1f) //파일이름 / 반복여부 / 최소 재생 간격
        {
            return;
            if (soundName == SoundName.None) return;
            var name = soundName.ToString();
            if (Instance.sfxDict.TryGetValue(name, out var sfx)) {
                if (!PlayerInfo.EnableSfx) return;
                // 동일한 효과음이 주어진 최소 재생 간격 이내에 재생된 적이 있다면, 리턴
                if (Time.time - Instance.lastPlayTime[name] < minInterval) return;

                var audio = GetOrCreateAudioSource(SFXSource, sfxAudios); // 플레이 중이지 않은 오디오를 가져온다.

                Instance.lastPlayTime[name] = Time.time; // 마지막 재생 시간을 저장한다.
                audio.clip = sfx;
                audio.loop = loop;
                audio.volume = volume;
                audio.pitch = pitch;
                audio.Play();
            } else {
                Debug.LogError($"해당 사운드를 찾을 수 없음: {name}");
            }
        }

        public static void PlayBGM(SoundName soundName, bool loop = true, float volume = 1f) {
            return;
            if (soundName == SoundName.None) return;
            var name = soundName.ToString();
            if (!Instance.bgmDict.TryGetValue(name, out var bgm)) {
                Debug.LogError($"해당 사운드를 찾을 수 없음: {name}");
                return;
            }

            if (PlayerInfo.EnableBgm == false) return;
            if (bgmAudios.Any(IsBgmPlayingNow)) return;

            var audio = GetOrCreateAudioSource(BGMSource, bgmAudios); // 플레이 중이지 않은 오디오를 가져온다.

            audio.clip = bgm;
            audio.loop = loop;
            audio.volume = volume;
            audio.Play();

            bool IsBgmPlayingNow(AudioSource source) {
                var isFadeOutNow = DOTween.IsTweening(source.GetInstanceID());
                return source.clip == bgm && source.isPlaying && !isFadeOutNow;
            }
        }
        
        private static AudioSource GetOrCreateAudioSource(GameObject sourceObject, List<AudioSource> audioList) {
            var audio = audioList.Find(a => !a.isPlaying);
            if (audio != null) return audio;
            audio = sourceObject.AddComponent<AudioSource>();
            audioList.Add(audio);
            return audio;
        }

        public static void Stop(SoundName soundName) {
            if (soundName == SoundName.None) return;
            var name = soundName.ToString();
            if (TryStop(name, Instance.bgmDict, bgmAudios)) return;
            if (TryStop(name, Instance.sfxDict, sfxAudios)) return;
            Debug.LogError($"해당 사운드를 찾을 수 없음: {name}");

            bool TryStop(string soundName, Dictionary<string, AudioClip> dict, List<AudioSource> audios) {
                if (dict.TryGetValue(soundName, out var clip)) {
                    var audio = audios.Find(a => a.clip == clip);
                    if (audio != null) {
                        audio.Stop();
                        return true;
                    } else {
                        Debug.LogError($"해당 사운드가 재생 중이 아님: {soundName}");
                        return true;
                    }
                }
                return false;
            }
        }

        public static void StopBGM(float fadeSec = 0f) {
            return;
            fadeSec = Mathf.Abs(fadeSec);
            foreach (var a in bgmAudios) {
                DOTween.Kill(a.GetInstanceID());
                if (!a.isPlaying) continue;
                if (fadeSec > Mathf.Epsilon)
                    a.DOFade(0f, fadeSec).SetId(a.GetInstanceID()).OnComplete(a.Stop);
                else
                    a.Stop();
            }
        }

        public static AudioClip GetClip(string name) {
            if (Instance.sfxDict.TryGetValue(name, out var sfx))
                return sfx;

            Debug.Log($"해당 클립을 찾을 수 없음: {name}");
            return null;
        }
    }
}