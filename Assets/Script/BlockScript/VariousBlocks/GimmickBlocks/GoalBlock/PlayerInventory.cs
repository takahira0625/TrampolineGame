using UnityEngine;
using System; // �� �C�x���g���g�����߂ɕK�v

public class PlayerInventory : MonoBehaviour
{
    //���̐����ς�����Ƃ��ɒʒm����C�x���g
    public static event Action<int> OnKeyCountChanged;

    // �C�x���g���Ηp���\�b�h
    public static void RaiseKeyCountChanged(int keyCount)
    {
        OnKeyCountChanged?.Invoke(keyCount);
    }

    public void AddKey()
    {
        // ���̑�����GameManager�ɓo�^�i�S�v���C���[�ŋ��L�j
        GameManager.instance.AddKeyGlobal();
    }
}