using UnityEngine;
using System.Collections.Generic;

// プロジェクトウィンドウの「Create」メニューからこのアセットを作れるようにする
[CreateAssetMenu(fileName = "StageKeyConfig_New", menuName = "Game/Stage Key Config")]
public class StageKeyConfig : ScriptableObject
{
    [Header("UI用スプライト")]
    [Tooltip("UIに表示する鍵部品のスプライト。" +
             "リストの数がそのまま、そのステージの必要部品数になります。")]
    public List<Sprite> keyPartUISprites = new List<Sprite>();
}