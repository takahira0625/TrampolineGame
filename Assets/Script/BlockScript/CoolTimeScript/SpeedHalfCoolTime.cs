using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedHalfCoolTime : MonoBehaviour
{
    [SerializeField] private float cooldownTime = 5.0f; // �N�[���^�C���i�b�j
    private bool isOnCooldown = false;
    private Collider2D col;
    private PhysicsMaterial2D physicsMaterial;

    void Awake()
    {
        // Collider���擾
        col = GetComponent<Collider2D>();
        if (col != null && col.sharedMaterial != null)
        {
            // PhysicsMaterial��Bounciness��2�ɐݒ�
            physicsMaterial = col.sharedMaterial;
            physicsMaterial.bounciness = 0.5f;
        }
        else
        {
            Debug.LogWarning("Collider2D �܂��� PhysicsMaterial2D ��������܂���");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Player���G�ꂽ�ꍇ�̏���
        if (collision.gameObject.CompareTag("Player") && !isOnCooldown)
        {
            // PhysicsMaterial��Bounciness��1�ɕύX
            if (physicsMaterial != null)
            {
                physicsMaterial.bounciness = 1.0f;
            }

            // �N�[���^�C���̃J�E���g�J�n
            StartCoroutine(CooldownTimer());

            // �}�e���A���ύX�֐����Ăяo��
            ChangeMaterial();
        }
    }

    // �}�e���A����ύX����֐�
    private void ChangeMaterial()
    {
        // ��FRenderer�̐F��ύX
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.gray; // �N�[���^�C�����͊D�F��
        }

        Debug.Log("�M�~�b�N���N�[���^�C����ԂɂȂ�܂���");
    }

    // �N�[���^�C���Ǘ��p�̃R���[�`��
    private IEnumerator CooldownTimer()
    {
        isOnCooldown = true;

        yield return new WaitForSeconds(cooldownTime);

        // �N�[���^�C���I����̏���
        isOnCooldown = false;

        // PhysicsMaterial��Bounciness�����ɖ߂�
        if (physicsMaterial != null)
        {
            physicsMaterial.bounciness = 0.5f;
        }

        // �����ڂ����ɖ߂�
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.white; // ���̐F�ɖ߂�
        }

        Debug.Log("�M�~�b�N�̃N�[���^�C�����I�����܂���");
    }
}