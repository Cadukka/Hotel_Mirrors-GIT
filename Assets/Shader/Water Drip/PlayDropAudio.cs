using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

[ExecuteAlways]
[RequireComponent(typeof(VisualEffect))]
public class PlayWaterAudio : VFXOutputEventAbstractHandler
{
    public override bool canExecuteInEditor => true;
    public AudioSource audioSource;
    public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute)
    {
        if(audioSource != null) 
        { audioSource.pitch = (Random.Range(0.6f, .7f));
            audioSource.Play(); }
    }
}
