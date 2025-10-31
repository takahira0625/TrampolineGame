using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManualTextENGManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Text ENG1;
    [SerializeField] private Text ENG2;
    [SerializeField] private Text ENG3;
    void Start()
    {
        if (!InputManager.LeftClickSlow) {
            ENG1.text = "Right Click　　　   Game Start";
            ENG2.text = "Right Click　　　　Activate Slow Zone";
            ENG3.text = "Left Click　　　   Hit Ball";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
