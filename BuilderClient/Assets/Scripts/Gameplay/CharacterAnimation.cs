using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private const string isFlyName = "isFly";
    private const string isRunName = "isRun";

    [SerializeField] private Animator _animator;
    [SerializeField] private CheckFly _checkFly;
    [SerializeField] private Character _character;

    private void Update()
    {
        bool isFly = _checkFly.IsFly;
        bool isRun = _character.velocity.magnitude > 0.01f; 

        _animator.SetBool(isRunName, isRun);
        if (isFly) _animator.SetTrigger(isFlyName);
    }
}
