using UnityEngine;
using UnityEngine.AI;

public class ItemSpawner : MonoBehaviour
{
    public GameObject itemPrefab; // 소환할 아이템의 프리팹
    public int itemCount = 10; // 소환할 아이템의 개수
    public float spawnAreaRadius = 20f; // 소환할 영역의 반경
    public float yHeightOffset = 0.5f; // 아이템의 Y축 오프셋 (높이 조정)

    void Start()
    {
        for (int i = 0; i < itemCount; i++)
        {
            SpawnItem();
        }
    }

    void SpawnItem()
    {
        Vector3 randomPosition = RandomNavMeshPosition(transform.position, spawnAreaRadius);

        if (randomPosition != Vector3.zero)
        {
            // Y축 높이를 조정하여 아이템 소환
            Instantiate(itemPrefab, randomPosition + new Vector3(0, yHeightOffset, 0), Quaternion.identity);
        }
    }

    // NavMesh에서 랜덤한 위치 찾기
    public Vector3 RandomNavMeshPosition(Vector3 origin, float radius)
    {
        Vector3 randomPosition = Vector3.zero;
        bool foundPosition = false;

        // 여러 번 시도하여 랜덤한 위치를 찾기
        for (int i = 0; i < 10; i++) // 최대 10번 시도
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius; // 랜덤한 방향 생성
            randomDirection += origin; // 시작 위치에 더함

            NavMeshHit navHit;
            // NavMesh에서 유효한 위치를 찾기
            if (NavMesh.SamplePosition(randomDirection, out navHit, radius, NavMesh.AllAreas))
            {
                randomPosition = new Vector3(navHit.position.x, navHit.position.y, navHit.position.z);
                foundPosition = true;
                break; // 유효한 위치를 찾으면 종료
            }
        }

        // 유효한 위치를 찾았으면 반환, 없으면 Vector3.zero 반환
        return foundPosition ? randomPosition : Vector3.zero;
    }
}
