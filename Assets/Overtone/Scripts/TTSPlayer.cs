using System.Threading.Tasks;
using Assets.Overtone.Scripts;
using UnityEngine;

namespace LeastSquares.Overtone
{
    public class TTSPlayer : CustomSingleton<TTSPlayer>
    {
        public TTSEngine Engine;
        public TTSVoice Voice;
        public AudioSource source;
        public AudioSource speechSource;
        public async Task Speak(string text)
        {
            var audioClip = await Engine.Speak(text, Voice.VoiceModel);
            source.clip = audioClip;
            source.loop = false;
            source.Play();
            speechSource.clip = audioClip;
            speechSource.Play();
        }
    }
}