using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    //���ӸŴ��� ���� ����
    public Text timeText;
    // �̵� �� ȸ�� ���� ����
    public float speed = 5f;         // �⺻ �̵� �ӵ�
    public float runSpeed = 10f;     // �޸��� �ӵ�
    public float lookSpeed = 4f;     // ���콺 ȸ�� �ӵ�
    public float maxLookAngle = 80f; // ī�޶� ���� ȸ�� �ִ� ����
    public float jumpForce = 5f;    // ���� ��

    // ���¹̳� ���� ����
    public float maxStamina = 300f;  // �ִ� ���¹̳�
    private float stamina;           // ���� ���¹̳�
    public float staminaDrainRate = 10f;  // �޸� �� ���¹̳� �Ҹ� �ӵ�
    public float staminaRecoveryRate = 20f; // ������ �� ���¹̳� ȸ�� �ӵ�
    public Image staminaBar;         // ���¹̳� UI ��
    public Text staminaText;         // ���¹̳� �ؽ�Ʈ

    public GameObject sweat;
    public GameObject lackhealth;
    public Fadeinout fadeInOutScript;
    public Fadeinout fadeInOutScript2;
         

    private AudioSource audioSource;
    public AudioClip footstepClip; // 걸음소리 클립
    public AudioClip jumpClip; // 점프소리 클립
    public AudioClip breatheClip;//숨소리 클립
    public AudioClip damageClip;
    private float stepCooldown = 0.5f; // 걸음소리 간격 
    private float nextStepTime = 0f; // 다음 걸음소리 재생 시간
    private bool isJumping = false; // 점프 중 여부
    public float footstepVolume = 0.5f; // 걸음 소리 볼륨
    public float jumpVolume = 0.7f;     // 점프 소리 볼륨
    public float breatheVolume = 0.5f;  // 숨소리 볼륨


    // ������Ʈ ��� �� ������ ���� ����
    public Transform holdPoint;      // ���� ������Ʈ�� ��ġ
    public float grabRange = 3f;     // ���� �� �ִ� ����
    public float throwForce = 10f;   // ���� �� ��

    // ���� ����
    private bool isGrounded = true;  // �÷��̾ ���� �ִ��� ����
    private bool isRunning = false;  // �޸��� �ִ��� ����
    private bool isMoving = false;   // �����̰� �ִ��� ����

    private bool isHoldingObject = false;  // ������Ʈ�� ��� �ִ��� ����
    private GameObject heldObject;         // ��� �ִ� ������Ʈ

    // ī�޶�� ���� ó�� ���� ����
    private Transform playerCamera;  // �÷��̾� ī�޶�
    private Rigidbody playerRb;      // �÷��̾� ������ٵ�
    private PlayerAnimation playerAnimation; // �÷��̾� �ִϸ��̼�

    private float pitch = 0f;        // ī�޶� ���� ȸ�� ����
    private float yaw = 0f;          // �÷��̾� �¿� ȸ�� ����

    public GameObject smell2Object;
    public GameObject smell;
    public GameObject Grab;
    public GameObject blind;

    public int playerHealth = 100;
    public float damageCooldown = 3.0f; // ������ ��Ÿ�� (3��)
    private float lastDamageTime; // ������ �������� ���� �ð�
    public Text healthText; // ü�� �ؽ�Ʈ UI
    public Text itemValueText; // ���� �����۰�ġ UI

    void Start()
    {
        // �ʱ�ȭ �۾�
        playerCamera = Camera.main.transform; // ���� ī�޶� ����
        playerRb = GetComponent<Rigidbody>(); // ������ٵ� ������Ʈ ��������
        playerAnimation = GetComponent<PlayerAnimation>(); // �ִϸ��̼� ��ũ��Ʈ ��������

        stamina = maxStamina; // ���¹̳� �ʱ�ȭ
        Cursor.lockState = CursorLockMode.Locked; // Ŀ�� ��� ����
        Cursor.visible = false; // Ŀ�� �����

        UpdateStaminaUI(); // ���¹̳� UI ������Ʈ
        UpdateHealthUI();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = footstepClip; // 걸음소리 클립 설정
        audioSource.loop = false; // 소리 반복하지 않기
       
    }
    void Update()
    {
        HandleInput();          // �Է� ó��
        HandleMovement();       // �̵� ó��
        HandleJump();           // ���� ó��
        HandleStamina();        // ���¹̳� ó��
        HandleObjectInteractions(); // ������Ʈ ��ȣ�ۿ� ó��

        if (isHoldingObject) HoldObject(); // ������Ʈ�� ��� ������ ��ġ ����

        HandleMouseLook();      // ���콺�� �ü� �̵� ó��

        UpdateTimeUI();
        UpdateItemValueUI();

        if (IsMoving() && Time.time >= nextStepTime)
        {
            PlayFootstepSound();
            // 달리기 상태에 따라 소리 재생 주기를 조절
            nextStepTime = Time.time + (isRunning ? stepCooldown / 2 : stepCooldown);
        }


        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            Jump();
        }
        UpdateHealthWarning();
    }

    // �Է� ó�� �Լ�
    private void HandleInput()
    {
        isMoving = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0; // �̵� ������ Ȯ��
        isRunning = Input.GetKey(KeyCode.LeftShift) && stamina > 0; // �޸��� �ִ��� Ȯ��
    }

    // �÷��̾� �̵� �� ȸ�� ó�� �Լ�
    private void HandleMovement()
    {
        // �̵� ���� ���
        Vector3 moveDirection = GetMoveDirection();
        float moveSpeed = isRunning ? runSpeed : speed; // �޸� ���� ���� �� �ӵ� ����
        transform.position += moveDirection * moveSpeed * Time.deltaTime; // �̵� ó��

        // �ִϸ��̼� ������Ʈ
        playerAnimation.UpdateMovementAnimation(isGrounded, Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal"), isRunning);
    }

    // ī�޶� ������ �������� �̵� ���� ���
    private Vector3 GetMoveDirection()
    {
        Vector3 forward = playerCamera.forward;
        Vector3 right = playerCamera.right;
        forward.y = right.y = 0; // Y�� ����

        return (forward.normalized * Input.GetAxisRaw("Vertical") + right.normalized * Input.GetAxisRaw("Horizontal")).normalized;
    }

    // ���� ó�� �Լ�
    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded) // ���� ��ư�� ������ ��
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // ���� ���� �� �߰�
            playerAnimation.SetJumping(true); // ���� �ִϸ��̼� ����
            isGrounded = false; // ���� ���·� ��ȯ
        }
    }


    // ���¹̳� ó�� �Լ�
    private void HandleStamina()
    {
        if (isMoving)
        {
            if (isRunning) // �޸��� ���̶��
            {
                stamina -= staminaDrainRate * Time.deltaTime; // ���¹̳� �Ҹ�
                stamina = Mathf.Clamp(stamina, 0, maxStamina); // ���¹̳� �� ����
            }
            else
            {
                // 달리지 않지만 움직이고 있을 경우 스테미나 회복
                stamina += 5f * Time.deltaTime; // 초당 5만큼 스테미나 회복
                stamina = Mathf.Clamp(stamina, 0, maxStamina); // 스테미나를 0과 최대 스테미나 사이로 제한
            }
        }
        else if (stamina < maxStamina) // ���� ���� �� ���¹̳� ȸ��
        {
            stamina += staminaRecoveryRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }

        UpdateStaminaUI(); // ���¹̳� UI ������Ʈ

        if (fadeInOutScript != null)
        {
            if (stamina < 150 && !sweat.activeSelf)
            {
                sweat.SetActive(true);
                fadeInOutScript.OnFade(FadeState.FadeLoop); // 페이드 효과 시작
            }
            else if (stamina >= 150 && sweat.activeSelf)
            {
                sweat.SetActive(false);
                fadeInOutScript.OnFade(FadeState.FadeOut); // 페이드 효과 중지
            }
        }
    }

    // ���¹̳� UI ������Ʈ
    private void UpdateStaminaUI()
    {
        if (staminaBar != null) staminaBar.fillAmount = stamina / maxStamina; // ���¹̳� �� ä���
        if (staminaText != null) staminaText.text = $"stamina = {Mathf.RoundToInt(stamina)}"; // ���¹̳� �ؽ�Ʈ ������Ʈ
    }

    public void ReduceStamina(float amount)
    {
        stamina -= amount;
        stamina = Mathf.Clamp(stamina, 0, maxStamina); // ���¹̳ʴ� 0 �̸����� �������� �ʰ�
        UpdateStaminaUI(); // ���¹̳� UI ������Ʈ
    }


    // ������Ʈ ��ȣ�ۿ� ó�� �Լ�
    private void HandleObjectInteractions()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isHoldingObject) TryGrabObject(); // ������Ʈ ��� �õ�
        else if (Input.GetKeyDown(KeyCode.G) && isHoldingObject) DropObject(); // ������Ʈ ����
        else if (Input.GetKeyDown(KeyCode.Q) && isHoldingObject) ThrowObject(); // ������Ʈ ������
        else if (Input.GetKeyDown(KeyCode.E) && isHoldingObject) UseHeldObject(); // ������Ʈ ���
    }



    // ���콺 �ü� �̵� ó�� �Լ�
    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed; // ���콺 �¿� ������
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed; // ���콺 ���� ������

        // �÷��̾� �¿� ȸ�� ó��
        yaw += mouseX;
        transform.localRotation = Quaternion.Euler(0, yaw, 0);

        // ī�޶� ���� ȸ�� ó�� (pitch ����)
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle); // ���� ȸ�� ���� ����
        playerCamera.localRotation = Quaternion.Euler(pitch, 0, 0);
    }

    // ������Ʈ ��� �õ� �Լ�
    private void TryGrabObject()
    {
        Ray ray = playerCamera.GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2)); // ȭ�� �߾ӿ��� ���� �߻�
        if (Physics.Raycast(ray, out RaycastHit hit, grabRange) &&
            (hit.collider.CompareTag("Grabbable") ||
            hit.collider.CompareTag("Potion") ||
            hit.collider.CompareTag("Target") ||
            hit.collider.CompareTag("Treasure1")||
             hit.collider.CompareTag("Treasure2")||
              hit.collider.CompareTag("Treasure3"))) // Grabbable �±װ� �ִ� ������Ʈ ����
        {
            playerAnimation.TriggerDrop();

            heldObject = hit.collider.gameObject; // ������Ʈ ����
            SetObjectPhysics(heldObject, true); // ������Ʈ ���� ó�� ��Ȱ��ȭ

            // ������Ʈ�� �ݶ��̴� ��Ȱ��ȭ
            Collider objectCollider = heldObject.GetComponent<Collider>();
            if (objectCollider != null)
            {
                objectCollider.enabled = false; // �ݶ��̴� ��Ȱ��ȭ
            }

            isHoldingObject = true; // ������Ʈ�� ���� ���·� ��ȯ
        }
    }

    // ���� ������Ʈ�� ������Ű�� �Լ�
    private void HoldObject()
    {
        heldObject.transform.position = holdPoint.position; // holdPoint�� ������Ʈ ��ġ ����
        heldObject.transform.rotation = holdPoint.rotation; // ������Ʈ ȸ���� holdPoint�� ����
    }

    // ������Ʈ ���� �Լ�
    private void DropObject()
    {
        SetObjectPhysics(heldObject, false); // ������Ʈ ���� ó�� Ȱ��ȭ

        // ������Ʈ�� �ݶ��̴� Ȱ��ȭ
        Collider objectCollider = heldObject.GetComponent<Collider>();
        if (objectCollider != null)
        {
            objectCollider.enabled = true; // �ݶ��̴� Ȱ��ȭ
        }

        heldObject = null; // ������Ʈ ���� ����
        isHoldingObject = false; // ���� ���� ����
    }

    // ������Ʈ ������ �Լ�
    private void ThrowObject()
    {
        Rigidbody rb = heldObject.GetComponent<Rigidbody>(); // ������Ʈ�� ������ٵ� ����
        rb.isKinematic = false; // ���� ó�� Ȱ��ȭ
        heldObject.transform.SetParent(null); // �θ� ���� ����
        rb.AddForce(holdPoint.forward * throwForce, ForceMode.Impulse); // ���� �� �� ���ϱ�

        playerAnimation.TriggerThrow();

        // ������Ʈ�� �ݶ��̴� Ȱ��ȭ
        Collider objectCollider = heldObject.GetComponent<Collider>();
        if (objectCollider != null)
        {
            objectCollider.enabled = true; // �ݶ��̴� Ȱ��ȭ
        }

        heldObject = null; // ������Ʈ ���� ����
        isHoldingObject = false; // ���� ���� ����
    }

    // ���� ������Ʈ ��� �Լ�
    private void UseHeldObject()
    {
        if (heldObject != null)
        {
            if (heldObject.CompareTag("Potion"))
            {
                if (smell2Object != null)
                {
                    smell2Object.gameObject.SetActive(false);
                    smell.gameObject.SetActive(false);

                }

                heldObject.SetActive(false); // Potion �±װ� ������ ��Ȱ��ȭ

                DropObject(); // ������Ʈ ����
            }
        }
    }

    // ������Ʈ ���� ó�� ���� �Լ�
    private void SetObjectPhysics(GameObject obj, bool isKinematic)
    {
        obj.GetComponent<Rigidbody>().isKinematic = isKinematic; // ���� ó�� Ȱ��ȭ/��Ȱ��ȭ ����
        if (!isKinematic) obj.transform.SetParent(null); // ���� ó���� Ȱ��ȭ�Ǹ� �θ� ���� ����
    }

    // �浹 ó�� �Լ�
    void OnCollisionEnter(Collision collision)
    {
        isGrounded = true; // ���� ������ ���� ���� ����
        playerAnimation.SetJumping(false); // ���� �ִϸ��̼� ����

        // �±װ� "Bomber"�� �� Smell2 ������Ʈ Ȱ��ȭ
        if (collision.gameObject.CompareTag("Bomber"))
        {
            if (smell2Object != null)
            {
                // Smell2 ������Ʈ�� Ȱ��ȭ
                smell2Object.SetActive(true);
            }
        }

    }

    public void TakeDamage(int damage)
    {
        // 데미지 쿨다운 확인
        if (Time.time >= lastDamageTime + damageCooldown)
        {
            playerHealth -= damage;
            playerHealth = Mathf.Max(playerHealth, 0); // HP가 0 이하로 떨어지지 않도록 제한
            Debug.Log($"Player took {damage} damage. Current health: {playerHealth}");

            UpdateHealthUI(); // HP UI 업데이트
            lastDamageTime = Time.time; // 마지막 데미지 시간 기록

            if (damageClip != null)
            {
                audioSource.PlayOneShot(damageClip);
            }

            if (playerHealth <= 0)
            {
                Die(); // 플레이어 사망 처리
                GameManager.instance.DestroyAndLoadFailScene(); // 게임 매니저에서 실패 씬으로 이동
            }
        }
        else
        {
            Debug.Log("Damage is on cooldown.");
        }
    }

    private void UpdateHealthWarning()
    {
        if (fadeInOutScript2 != null)
        {
            if (playerHealth < 20)
            {
                if (!lackhealth.activeSelf) // 비활성 상태일 때만 활성화
                {
                    lackhealth.SetActive(true);
                    Debug.Log("LackHP 활성화됨");
                }
                fadeInOutScript2.OnFade(FadeState.FadeLoop); // 페이드 효과 시작
            }
            else if (playerHealth >= 20 && lackhealth.activeSelf)
            {
                fadeInOutScript2.OnFade(FadeState.FadeOut); // 페이드 효과 중지
                lackhealth.SetActive(false); // 페이드가 끝난 후 비활성화
            }
        }
    }


    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {playerHealth}"; // ü�� �ؽ�Ʈ ������Ʈ
        }
    }

    private void UpdateItemValueUI()
    {
        int itemValue = GameManager.instance.itemValue;
        if (itemValueText != null)
        {
            itemValueText.text = $"ItemValue : {itemValue}"; // �����۰�ġ ������Ʈ
        }
    }

    public void UpdateTimeUI()
    {
        if (timeText != null)
        {
            timeText.text = $"Time : {GameManager.instance.countdownTime}"; // ī��Ʈ�ٿ�
        }
    }


    private void Die()
    {
        Debug.Log("Player has died.");
        // �÷��̾� ��� ó�� ���� �߰�
    }

    private bool IsMoving()
    {
        // 이동을 확인하는 로직 (예: 키 입력이나 Rigidbody 사용)
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
    }

    private void PlayFootstepSound()
    {
        // 걸음소리 재생
        audioSource.PlayOneShot(footstepClip, footstepVolume);
    }

    private void Jump()
    {
        isJumping = true;
        PlayJumpSound();

        StartCoroutine(ResetJump());
    }

    private void PlayJumpSound()
    {

        audioSource.PlayOneShot(jumpClip, jumpVolume);
        audioSource.PlayOneShot(breatheClip, breatheVolume);
    }

    private IEnumerator ResetJump()
    {

        yield return new WaitForSeconds(0.5f);
        isJumping = false;
    }

    // 블라인드를 비활성화하는 메서드 추가
    public void DisableBlindAfterDelay(float delay)
    {
        StartCoroutine(DisableBlindCoroutine(delay));
    }

    private IEnumerator DisableBlindCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay); // 지정된 시간 동안 대기
        if (blind != null && blind.activeSelf) // 블라인드가 존재하고 활성화 상태일 경우
        {
            blind.SetActive(false); // 블라인드 비활성화
        }
    }

}