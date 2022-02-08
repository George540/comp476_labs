using System;
using UnityEngine;

public class SineMotion : MonoBehaviour
{
    private enum TrigFunction : byte {Sin, Cos, Tan}
    private static readonly Func<float, float>[] trigFunctions = {Mathf.Sin, Mathf.Cos, Mathf.Tan};

    [SerializeField] 
    private TrigFunction selectedTrigFunction;

    [SerializeField] 
    private float amplitude = 1;
    [SerializeField]
    private float stepPerSecond = 1;

    private Func<float, float> trigFunction;
    private static readonly Vector3 Up = Vector3.up;
    private float currentAngle = 0;
    private float previousUnitDelta = 0;

    private void Awake() => OnValidate();
    private void OnValidate() => UpdateTrigFunction();
    private void UpdateTrigFunction() => trigFunction = trigFunctions[(int)selectedTrigFunction];
    private void Update()
    {
        currentAngle += stepPerSecond * Time.deltaTime;
        float currentUnitDelta = trigFunction(currentAngle);
        transform.Translate(amplitude * (currentUnitDelta - previousUnitDelta) * Up);
        previousUnitDelta = currentUnitDelta;
    }
}
