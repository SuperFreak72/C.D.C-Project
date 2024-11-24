using UnityEngine;
using UnityEngine.AI;

public class ItemSpawner : MonoBehaviour
{
    public GameObject itemPrefab; // ��ȯ�� �������� ������
    public int itemCount = 10; // ��ȯ�� �������� ����
    public float spawnAreaRadius = 20f; // ��ȯ�� ������ �ݰ�
    public float yHeightOffset = 0.5f; // �������� Y�� ������ (���� ����)

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
            // Y�� ���̸� �����Ͽ� ������ ��ȯ
            Instantiate(itemPrefab, randomPosition + new Vector3(0, yHeightOffset, 0), Quaternion.identity);
        }
    }

    // NavMesh���� ������ ��ġ ã��
    public Vector3 RandomNavMeshPosition(Vector3 origin, float radius)
    {
        Vector3 randomPosition = Vector3.zero;
        bool foundPosition = false;

        // ���� �� �õ��Ͽ� ������ ��ġ�� ã��
        for (int i = 0; i < 10; i++) // �ִ� 10�� �õ�
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius; // ������ ���� ����
            randomDirection += origin; // ���� ��ġ�� ����

            NavMeshHit navHit;
            // NavMesh���� ��ȿ�� ��ġ�� ã��
            if (NavMesh.SamplePosition(randomDirection, out navHit, radius, NavMesh.AllAreas))
            {
                randomPosition = new Vector3(navHit.position.x, navHit.position.y, navHit.position.z);
                foundPosition = true;
                break; // ��ȿ�� ��ġ�� ã���� ����
            }
        }

        // ��ȿ�� ��ġ�� ã������ ��ȯ, ������ Vector3.zero ��ȯ
        return foundPosition ? randomPosition : Vector3.zero;
    }
}
