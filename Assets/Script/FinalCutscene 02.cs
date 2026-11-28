using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class FinalCutscene02 : MonoBehaviour
{
    [SerializeField] TimelineClip timelineAnimacao;
    [SerializeField] GameObject _GO_Da_Animacao;
    void Start()
    {
        if (_GO_Da_Animacao != null) 
        { 
            if (!_GO_Da_Animacao.activeSelf)
            { 
                _GO_Da_Animacao.SetActive(true); 
            } 
        }
    }
}
