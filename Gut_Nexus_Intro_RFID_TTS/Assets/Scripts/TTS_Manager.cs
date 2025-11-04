using System.Speech.Synthesis;
using UnityEngine;
public class TTS_Manager : MonoBehaviour
{
    private SpeechSynthesizer synth;
    void Start()
    {
        synth = new SpeechSynthesizer();
        synth.Volume = 100;  // 0...100
        synth.Rate = 0;      // -10...10
        
    }

    public void Speak(string text)
    {
        synth.SpeakAsync(text);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Speak("Hello World");
        }
    }
}
