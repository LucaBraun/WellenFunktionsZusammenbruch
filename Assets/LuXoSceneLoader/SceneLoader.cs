using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
public class SceneLoader : MonoBehaviour
{
    private Animator _animator;
    private static readonly int TriggerWipeOut = Animator.StringToHash("TriggerWipeOut");

    public bool Wiping { get; private set; }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void StartLoadScene(int index)
    {
        if (Wiping) return;
        
        StartCoroutine(LoadSceneAsync(index));
    }

    private IEnumerator LoadSceneAsync(int index)
    {
        _animator.SetTrigger(TriggerWipeOut);
        Wiping = true;
        while (Wiping)
        {
            yield return null;
        }
        
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);

        while (!operation.isDone)
        {
            Debug.Log(operation.progress);
            yield return null;
        }
    }

    public void WipeCompleteCallback()
    {
        Wiping = false;
    }

    public void WipeStartedCallback()
    {
        Wiping = true;
    }
}
