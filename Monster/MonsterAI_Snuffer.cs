using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAI_Snuffer : MonoBehaviour
{
    public float wanderSpeed = 2f;
    public float chaseSpeed = 4f;
    public float detectionRange = 10f;
    public GameObject smellObject;
    public float rotationSpeed = 5f;
    public float wanderRadius = 20f;
    public float wanderInterval = 5f;
    public float chaseDelay = 4f;
    public float sniffDuration = 4f;

    private NavMeshAgent agent;
    private Transform player;
    private bool isChasing;
    private bool isWaitingToChase;
    private bool isSniffing;
    private float wanderTimer;

    public Animator animator;

    private Monster_S monsterManager;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>(); // NavMeshAgent ������Ʈ ��������
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        wanderTimer = wanderInterval; // �ʱ� Ÿ�̸� �� ����

        // Animator ������Ʈ ��������
        animator = GetComponent<Animator>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            PlayerMove playerController = playerObject.GetComponent<PlayerMove>();
            if (playerController != null)
            {
                smellObject = playerController.smell; // �÷��̾��� smellObject�� ������
            }
        }

        monsterManager = FindObjectOfType<Monster_S>();
    }

    private void Update()
    {
        if (smellObject != null && smellObject.activeSelf && !isWaitingToChase && !isChasing && !isSniffing)
        {
            StartSniffAnimation();
        }

        if (smellObject != null && !smellObject.activeSelf && isChasing)
        {
            StopChasing();
        }

        if (isChasing)
        {
            ChaseSmell();
        }
        else if (!isWaitingToChase && !isSniffing)
        {
            Wander();
        }
    }

    private void StartSniffAnimation()
    {
        isSniffing = true;
        agent.isStopped = true;
        animator.SetBool("Sniff", true);
        StartCoroutine(WaitForSniffToEnd());
    }

    private IEnumerator WaitForSniffToEnd()
    {
        yield return new WaitForSeconds(sniffDuration);
        animator.SetBool("Sniff", false);
        StartPoseAnimation();
    }

    private void StartPoseAnimation()
    {
        animator.SetBool("Pose", true);
        StartCoroutine(WaitBeforeChase());
    }

    private IEnumerator WaitBeforeChase()
    {
        isWaitingToChase = true;
        yield return new WaitForSeconds(chaseDelay);
        isChasing = true;
        animator.SetBool("Pose", false);
        animator.SetBool("Chase", true);
        agent.isStopped = false;
        isWaitingToChase = false;
        isSniffing = false;
    }

    private void StopChasing()
    {
        isChasing = false;
        animator.SetBool("Chase", false);
        agent.isStopped = false;
        Wander();
    }

    private void Wander()
    {
        wanderTimer += Time.deltaTime;

        if (wanderTimer >= wanderInterval)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            agent.speed = wanderSpeed;
            wanderTimer = 0f;
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);

        return navHit.position;
    }

    private void ChaseSmell()
    {
        agent.speed = chaseSpeed;
        agent.destination = smellObject.transform.position;

        Vector3 directionToSmell = smellObject.transform.position - transform.position;
        directionToSmell.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToSmell);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // �÷��̾�� �浹�� �� Attack �ִϸ��̼� ���
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && isChasing)  // ���� ���� ��쿡�� ����
        {
            agent.isStopped = true;  // �浹 �� �̵� ����
            animator.SetBool("Attack", true);  // Attack �ִϸ��̼� Ȱ��ȭ

            // �ִϸ��̼��� ������ �ٽ� �̵� �簳 (�ʿ��� ��� ���� �ð� ���)
            StartCoroutine(ResumeAfterAttack());
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            // �÷��̾��� ��ġ�� �����ͼ� ���� ����
            Vector3 directionToPlayer = (collision.transform.position - transform.position).normalized;

            // ���Ͱ� �÷��̾� ���� �ٶ󺸰� ȸ��
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1f);

            animator.SetTrigger("Attack2");
        }

        if (collision.gameObject.CompareTag("Player")) // �÷��̾� �±׿� ��
        {
            int damage = 10; // ���Ͱ� �÷��̾�� �ִ� ������
            Debug.Log("Collision with player detected.");

            if (gameObject.layer == 8) // "Monster" ���̾� Ȯ��
            {
                // ������ �ε���
                int monsterIndex = 0;
                monsterManager.M_Struct[monsterIndex].HandleCollision(collision.gameObject, damage); // �ش� ���Ϳ� ���� �ֱ�
            }
        }

    }

    // ���� �� �̵� �簳
    private IEnumerator ResumeAfterAttack()
    {
        yield return new WaitForSeconds(1.5f);  // ���� �ִϸ��̼��� ���� ������ ���
        animator.SetBool("Attack", false);  // Attack �ִϸ��̼� ��Ȱ��ȭ
        agent.isStopped = false;  // �̵� �簳
    }
}