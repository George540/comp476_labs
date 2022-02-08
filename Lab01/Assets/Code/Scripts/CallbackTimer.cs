using UnityEngine;
using UnityEngine.Events;

public class CallbackTimer : MonoBehaviour
{
    [SerializeField]
    private float timeInterval = 1;
    [SerializeField]
    private UnityEvent callback;

    private float elapsedTime;

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime > timeInterval)
        {
            callback?.Invoke(); //invokes all the callbacks attached to the UnityEvent, if there are any (i.e. when callback != null)
            elapsedTime -= timeInterval;
        }
    }
}
