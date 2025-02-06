using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineTracker : MonoBehaviour
{
    [SerializeField] private List<string> runningCoroutinesNames = new List<string>();

    private HashSet<IEnumerator> runningCoroutines = new HashSet<IEnumerator>();

    public new Coroutine StartCoroutine(IEnumerator routine)
    {
        runningCoroutines.Add(routine);
        runningCoroutinesNames.Add(routine.ToString());
        return base.StartCoroutine(WrapCoroutine(routine));
    }

    public new void StopCoroutine(IEnumerator routine)
    {
        runningCoroutines.Remove(routine);
        runningCoroutinesNames.Remove(routine.ToString());
        base.StopCoroutine(routine);
    }

    public new void StopAllCoroutines()
    {
        runningCoroutines.Clear();
        runningCoroutinesNames.Clear();
        base.StopAllCoroutines();
    }

    private IEnumerator WrapCoroutine(IEnumerator routine)
    {
        yield return routine;
        runningCoroutines.Remove(routine);
        runningCoroutinesNames.Remove(routine.ToString());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PrintRunningCoroutines();
        }
    }

    public void PrintRunningCoroutines()
    {
        Debug.Log($"Running coroutines: {runningCoroutines.Count}");
        foreach (var coroutine in runningCoroutines)
        {
            Debug.Log($"Coroutine: {coroutine}");
        }
    }
}