using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAI_Bomber : MonoBehaviour
{
    public float speed = 3f;             // �⺻ ���� �̵� �ӵ�
    public float chaseSpeed = 6f;        // �÷��̾� ���� �ӵ�
    public Transform player;             // �÷��̾��� ��ġ
    public float detectionRange = 5f;    // ���Ͱ� �÷��̾ ������ �Ÿ�
    public float detectionAngle = 45f;   // �÷��̾ ������ ����
    public GameObject smellObject;       // �÷��̾��� Smell ������Ʈ
    public float rotationSpeed = 5f;     // ������ ȸ�� �ӵ�
    public float wanderRadius = 20f;     // AI�� �����Ӱ� ���ƴٴ� �� �ִ� �ݰ�
    public float wanderInterval = 5f;    // ���ο� ���� ��ġ�� �̵��ϴ� ����
    public Animator animator;            // ������ �ִϸ��̼��� ó���ϴ� Animator

    private NavMeshAgent navMeshAgent;   // NavMeshAgent ������Ʈ
    private float wanderTimer;           // �����Ӱ� �̵��ϴ� �ð� Ÿ�̸�
    private bool isWaiting = false;      // ��� ������ ���θ� ��Ÿ���� ����

    private PlayerMove playerMove; // �÷��̾� ��ũ��Ʈ ���� ����

    private Monster_S monsterManager;

    private void Start()
    {
        // NavMeshAgent�� �������� �⺻ �̵� �ӵ��� ����
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;

        // �ʱ� Ÿ�̸� �� ����
        wanderTimer = wanderInterval;

        // Animator ������Ʈ ��������
        animator = GetComponent<Animator>();

        // �ʱ� ���� ��ġ ����
        Wander();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            PlayerMove playerController = playerObject.GetComponent<PlayerMove>();
            if (playerController != null)
            {
                player = playerObject.transform; // �÷��̾��� Transform�� ������
                playerMove = playerObject.GetComponent<PlayerMove>(); // �÷��̾� Move ��ũ��Ʈ ��������
                smellObject = playerController.smell; // �÷��̾��� smellObject�� ������
            }
        }

        monsterManager = FindObjectOfType<Monster_S>();

        // NavMeshAgent�� ��Ȱ��ȭ�Ǿ� �ִٸ� Ȱ��ȭ
        if (navMeshAgent != null && !navMeshAgent.isActiveAndEnabled)
        {
            navMeshAgent.enabled = true;
        }
    }

    private void Update()
    {
        // NavMeshAgent�� Ȱ��ȭ�Ǿ� �ִ��� Ȯ��
        if (navMeshAgent == null || !navMeshAgent.isActiveAndEnabled)
        {
            Debug.LogWarning("NavMeshAgent is not active or not found!");
            return;
        }

        // �÷��̾���� �Ÿ� �� �þ� Ȯ��
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (IsPlayerInSight(distanceToPlayer) && !isWaiting)
        {
            // �÷��̾ �����ϸ� �ڷ�ƾ ���� (1�� ��� �� �ִϸ��̼�)
            StartCoroutine(DetectAndPause());
        }
        else if (!isWaiting)
        {
            // �����Ӱ� ���� ���ƴٴϱ�
            Wander();
        }
    }

    // �÷��̾ �����ϰ� 1�� ����ϴ� �ڷ�ƾ
    private IEnumerator DetectAndPause()
    {
        isWaiting = true; // ��� ���·� ��ȯ
        animator.SetBool("Pose", true);

        // 2�� ���
        yield return new WaitForSeconds(2f);
        animator.SetBool("Pose", false);

        // ���� �ӵ��� �ִϸ��̼� ����
        animator.SetBool("Run", true);
        navMeshAgent.speed = chaseSpeed;  // ������ �� �ӵ��� ������ ����

        // ��� ���¸� �����ϰ� �ٽ� �÷��̾� ����
        navMeshAgent.SetDestination(player.position);
        LookAtPlayer();

        isWaiting = false; // ��� ���� ����
    }

    // AI�� �����Ӱ� ���ƴٴϴ� ����
    private void Wander()
    {
        wanderTimer += Time.deltaTime;

        if (wanderTimer >= wanderInterval)
        {
            // ���ο� ���� ��ġ ����
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            navMeshAgent.SetDestination(newPos);
            wanderTimer = 0f;

            // �������� ���� �� �⺻ �ӵ��� ����
            navMeshAgent.speed = speed;
            animator.SetBool("Run", false); // AI�� �÷��̾ ��ġ�� �ٽ� idle ���°� ��
        }
    }

    // ������ �ݰ� ������ ������ NavMesh ���� ��ġ�� ã�� �Լ�
    public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance; // �ݰ� ������ ������ ���� ����
        randomDirection += origin; // ���� ��ġ�� ����

        NavMeshHit navHit;
        // NavMesh ���� ��ȿ�� ��ġ�� ã�� ��ȯ
        NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);

        return navHit.position;
    }

    // ������ �Ӹ��� �÷��̾ ���ϵ��� ȸ��
    private void LookAtPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // �÷��̾�� �浹 �� Smell ������Ʈ�� Ȱ��ȭ�ϰ� ���͸� ����
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (smellObject != null)
            {
                smellObject.SetActive(true);
            }

            // �÷��̾� ���¹̳ʸ� 40 ���ҽ�Ŵ
            if (playerMove != null)
            {
                playerMove.ReduceStamina(40f);
            }

            if (collision.gameObject.CompareTag("Player")) // �÷��̾� �±׿� ��
            {
                int damage = 10; // ���Ͱ� �÷��̾�� �ִ� ������
                Debug.Log("Collision with player detected.");

                if (gameObject.layer == 8) // "Monster" ���̾� Ȯ��
                {
                    // ������ �ε����� ������ ����
                    int monsterIndex = 1;
                    monsterManager.M_Struct[monsterIndex].HandleCollision(collision.gameObject, damage); // �ش� ���Ϳ� ���� �ֱ�
                }
            }

            Destroy(gameObject);
        }
    }

    // �÷��̾ ������ �þ߿� �ִ��� Ȯ���ϴ� �Լ�
    private bool IsPlayerInSight(float distanceToPlayer)
    {
        // �÷��̾ ���� ���� ���� �ִ��� Ȯ��
        if (distanceToPlayer > detectionRange) return false;

        // �÷��̾ ĳ������ ���� ���� ���� ���� �ִ��� Ȯ��
        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle < detectionAngle;
    }
}
