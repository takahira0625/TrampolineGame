using UnityEngine;
using System.Collections.Generic;

// �v���W�F�N�g�E�B���h�E�́uCreate�v���j���[���炱�̃A�Z�b�g������悤�ɂ���
[CreateAssetMenu(fileName = "StageKeyConfig_New", menuName = "Game/Stage Key Config")]
public class StageKeyConfig : ScriptableObject
{
    [Header("UI�p�X�v���C�g")]
    [Tooltip("UI�ɕ\�����錮���i�̃X�v���C�g�B" +
             "���X�g�̐������̂܂܁A���̃X�e�[�W�̕K�v���i���ɂȂ�܂��B")]
    public List<Sprite> keyPartUISprites = new List<Sprite>();
}