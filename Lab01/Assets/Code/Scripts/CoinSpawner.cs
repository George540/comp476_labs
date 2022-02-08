using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    // TODO

    [SerializeField] private GameObject coinPrefab;

    private void Start()
    {
        GetComponent<PositionRandomizer>()?.Randomize();
        SpawnCoin();
    }

    public void SpawnCoin()
    {
        GameObject coin = Instantiate(coinPrefab, transform.position, Quaternion.identity);
        Destroy(coin, 10);
    }
}
