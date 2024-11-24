using System.Collections;
using UnityEngine;

public class MonsterAI_Grab : MonoBehaviour
{
    public float wanderSpeed = 2f;
    public float chaseSpeed = 4f;
    public float detectionRange = 10f;
    public float viewAngle = 45f;
    public GameObject blindCollider;
    public GameObject grabCollider;
    public float grabSlowDuration = 3f;
    public float grabSlowAmount = 0.5f;
    public float loseSightDistance = 15f;
    public float rotationSpeed = 5f;
    public float jumpAttackChance = 0.2f;  // ���� ���� Ȯ�� (20%)
    public float jumpAttackCooldown = 5f;  // ���� ���� ��ٿ� �ð�

    private Transform player;
    private bool isChasing;
    private float wanderTimer;
    private Rigidbody rb;
    private Animator animator;
    private PlayerMove playerMovement;
    private Vector3 wanderDirection;
    private bool canJumpAttack = true;     // ���� ������ �������� ����
    private bool isAttacking = false;      // ���� ������ ����

    private Monster_S monsterManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerMovement = player.GetComponent<PlayerMove>();

        if (playerMovement == null)
        {
            Debug.LogWarning("PlayerMove ��ũ��Ʈ�� �÷��̾ �Ҵ���� �ʾҽ��ϴ�.");
        }

        SetRandomWanderDirection();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            PlayerMove playerController = playerObject.GetComponent<PlayerMove>();
            if (playerController != null)
            {
                grabCollider = playerController.Grab;
            }
        }

        if (playerObject != null)
        {
            PlayerMove playerController = playerObject.GetComponent<PlayerMove>();
            if (playerController != null)
            {
                blindCollider = playerController.blind; 
            }
        }

        monsterManager = FindObjectOfType<Monster_S>();
    }

    private void Update()
    {
        if (!isAttacking)  // ���� ���� �ƴ� ���� �߰��ϰų� �̵�
        {
            if (CanSeePlayer())
            {
                isChasing = true;
            }

            if (isChasing)
            {
                ChasePlayer();
            }
            else
            {
                Wander();
            }
        }

        // �̵� ���̸� �ȱ� �ִϸ��̼�
        if (rb.velocity.magnitude > 0.1f)
        {
            animator.SetBool("G_Walk", true);
        }
        else
        {
            animator.SetBool("G_Walk", false);
        }
    }

    // �÷��̾� ����
    private bool CanSeePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleToPlayer <= viewAngle)
            {
                return true;
            }
        }
        return false;
    }

    // ���� �̵�
    private void Wander()
    {
        wanderTimer += Time.deltaTime;

        if (wanderTimer >= 5f)
        {
            SetRandomWanderDirection();
        }

        Quaternion targetRotation = Quaternion.LookRotation(wanderDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        rb.velocity = wanderDirection * wanderSpeed;
    }

    private void SetRandomWanderDirection()
    {
        wanderTimer = 0f;
        wanderDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;

        float randomSpeedFactor = Random.Range(0.8f, 1.2f);
        rb.velocity = wanderDirection * wanderSpeed * randomSpeedFactor;
    }

    // �÷��̾� �߰�
    private void ChasePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        rb.velocity = directionToPlayer * chaseSpeed;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // ���� ���� �õ�
        if (distanceToPlayer <= loseSightDistance && canJumpAttack && Random.value <= jumpAttackChance)
        {
            StartCoroutine(PerformJumpAttack());  // ���� ���� ����
        }

        if (distanceToPlayer <= 0.5f)
        {
            ActivateRandomSkill();
            Destroy(gameObject);
        }
        else if (distanceToPlayer >= loseSightDistance)
        {
            isChasing = false;
        }
    }

    // ���� ���� �ڷ�ƾ
    private IEnumerator PerformJumpAttack()
    {
        isAttacking = true;
        canJumpAttack = false;  // ���� ���� ��ٿ� ����

        // ���� �ִϸ��̼� ����
        animator.SetTrigger("G_jump");

        // �ӵ��� ������Ŵ
        float originalSpeed = chaseSpeed;  // ���� �ӵ��� ����
        chaseSpeed *= 3f;  // �ӵ� ����
        rb.velocity = (player.position - transform.position).normalized * chaseSpeed; // �÷��̾� �������� �ӵ� ����

        yield return new WaitForSeconds(1f);  // ���� ��� �ð�

        // ���� �ִϸ��̼� ����
        animator.SetTrigger("G_Attack");

        // ���� �ִϸ��̼� ���� �ӵ� ����
        rb.velocity = Vector3.zero;  // ���� �� �̵��� ����
        yield return new WaitForSeconds(1f);  // ���� ��� �ð�

        // ���� �� idle�� ��ȯ
        animator.SetTrigger("G_idle");

        // ���� �ӵ��� ����
        chaseSpeed = originalSpeed; // �ӵ��� ������� ����
        rb.velocity = Vector3.zero;  // �̵��� ����

        yield return new WaitForSeconds(0.5f);  // idle ���·� ���ư��� �� �ణ�� �ð�

        isAttacking = false;

        // ���� ���� ��ٿ� Ÿ�̸�
        yield return new WaitForSeconds(jumpAttackCooldown);
        canJumpAttack = true;  // ��ٿ� ���� �� ���� ���� ����
    }


    // �÷��̾�� �浹 ��
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            ActivateRandomSkill();
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Player")) // �÷��̾� �±׿� ��
        {
            int damage = 10; // ���Ͱ� �÷��̾�� �ִ� ������
            Debug.Log("Collision with player detected.");

            if (gameObject.layer == 8) // "Monster" ���̾� Ȯ��
            {
                // ������ �ε����� ������ �����ؾ� �մϴ�.
                int monsterIndex = 2;
                monsterManager.M_Struct[monsterIndex].HandleCollision(collision.gameObject, damage); // �ش� ���Ϳ� ���� �ֱ�
            }
        }
    }

    private void ActivateRandomSkill()
    {
        float randomValue = Random.value;

        if (randomValue <= 0.5f)
        {
            ActivateBlind();
        }
        else
        {
            ActivateGrab();
        }
    }

    private void ActivateBlind()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            PlayerMove playerController = playerObject.GetComponent<PlayerMove>();
            if (playerController != null && !playerController.blind.activeSelf) // �÷��̾��� blindCollider�� ��Ȱ��ȭ ������ ��
            {
                playerController.blind.SetActive(true); // ����ε� Ȱ��ȭ
                playerController.DisableBlindAfterDelay(5f); // 5�� �� ��Ȱ��ȭ ȣ��
            }
        }
    }

    private IEnumerator DisableBlindAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // ������ �ð� ���� ���

        GameObject blindObject = GameObject.FindGameObjectWithTag("Blind"); // "Blind" �±׸� ���� ������Ʈ ã��
        if (blindObject != null)
        {
            blindObject.SetActive(false); // ����ε� ��Ȱ��ȭ
        }
    }


    private void ActivateGrab()
    {
        if (!grabCollider.activeSelf)
        {
            grabCollider.SetActive(true);
            StartCoroutine(SlowPlayer());
        }
    }

    private IEnumerator SlowPlayer()
    {
        if (playerMovement != null)
        {
            playerMovement.speed *= grabSlowAmount;
            yield return new WaitForSeconds(grabSlowDuration);
            playerMovement.speed /= grabSlowAmount;
        }
    }
}
