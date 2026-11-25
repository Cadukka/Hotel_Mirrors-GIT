using UnityEngine;

public class CopiarAnimacaoDoPlayer : MonoBehaviour
{
    //Feito com o GPT

    [SerializeField] private GameObject _ObjectPlayer;
    [SerializeField] private GameObject _MirrorCharacterGameObject;

    private Animator _AnimatorPlayer;
    private Animator _MirrorAnimator;

    void Start()
    {
        _AnimatorPlayer = _ObjectPlayer.GetComponent<Animator>(); //Get Component pra definir as referencias
        _MirrorAnimator = _MirrorCharacterGameObject.GetComponent<Animator>();

        //.runtimeAnimatorController define o Controller que ele cria ao dar "play"
        _MirrorAnimator.runtimeAnimatorController =
            _AnimatorPlayer.runtimeAnimatorController;
    }

    void LateUpdate()
    {
        if (_AnimatorPlayer == null || _MirrorAnimator == null) //Se for nulo, para
            return;

        //AnimatorStateInfo é usado para identificar qual a animação está tocando
        AnimatorStateInfo state =
            _AnimatorPlayer.GetCurrentAnimatorStateInfo(0);

        //Define o estado animator "mirror" para o do "state" (O do player)
        _MirrorAnimator.Play(
            state.fullPathHash,
            0,
            state.normalizedTime
        );

        _MirrorAnimator.speed = _AnimatorPlayer.speed;
    }
}
