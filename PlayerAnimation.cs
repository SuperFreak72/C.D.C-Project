using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateMovementAnimation(bool isGrounded, float vAxis, float hAxis, bool isRunning)
    {
        if (isGrounded)
        {
            // �ȱ� �� �޸��� �ִϸ��̼� ����
            anim.SetBool("isWalkingForward", vAxis > 0 && !isRunning);
            anim.SetBool("isWalkingBackward", vAxis < 0 && !isRunning);
            anim.SetBool("isWalkingRight", hAxis > 0 && !isRunning);
            anim.SetBool("isWalkingLeft", hAxis < 0 && !isRunning);

            // �޸��� �ִϸ��̼� ����
            anim.SetBool("isRunning", isRunning && vAxis != 0);
            anim.SetBool("isRunningSL", isRunning && hAxis != 0);
        }
        else
        {
            // ���߿� ���� ���� �ȱ�/�޸��� �ִϸ��̼��� ���� ���� �ִϸ��̼��� ����
            anim.SetBool("isWalkingForward", false);
            anim.SetBool("isWalkingBackward", false);
            anim.SetBool("isWalkingRight", false);
            anim.SetBool("isWalkingLeft", false);
            anim.SetBool("isRunning", false);
            anim.SetBool("isRunningSL", false);
        }
    }

    public void SetJumping(bool isJumping)
    {
        anim.SetBool("isJumping", isJumping);
    }

    // Drop �ִϸ��̼� Ʈ���� ����
    public void TriggerDrop()
    {
        anim.SetTrigger("Drop");
    }

    // Throw �ִϸ��̼� Ʈ���� ����
    public void TriggerThrow()
    {
        anim.SetTrigger("Throw");
    }

}
