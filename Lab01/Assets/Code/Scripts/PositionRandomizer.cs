using UnityEngine;
using Random = UnityEngine.Random;

public class PositionRandomizer : MonoBehaviour
{
    [SerializeField]
    private Vector3 extents;

    private Vector3 halfExtents;
    private Vector3 center;

    private void Awake()
    {
        center = transform.position;
        UpdateExtents();
    }

    private void OnValidate() => UpdateExtents();

    private void UpdateExtents() => halfExtents = .5f * extents;

    public void Randomize()
    {
        transform.position = center + new Vector3(
            Random.Range(-halfExtents.x, halfExtents.x),
            Random.Range(-halfExtents.y, halfExtents.y),
            Random.Range(-halfExtents.z, halfExtents.z));
    }
}
