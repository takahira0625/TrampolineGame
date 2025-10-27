using UnityEngine;

public class InputManager : MonoBehaviour
{
    // true = 左クリックがメイン, false = 右クリックがメイン
    public static bool LeftClickSlow= true;

    public static bool IsLeftClickPressed()
    {
        if (LeftClickSlow)
            return Input.GetMouseButton(0);
        else
            return Input.GetMouseButton(1);
    }
    public static bool IsRightClickPressed()
    {
        if (LeftClickSlow)
            return Input.GetMouseButton(1);
        else
            return Input.GetMouseButton(0);
    }
    public static bool IsLeftClickDown()
    {
        if (LeftClickSlow)
            return Input.GetMouseButtonDown(0);
        else
            return Input.GetMouseButtonDown(1);
    }
    public static bool IsRightClickDown()
    {
        if (LeftClickSlow)
            return Input.GetMouseButtonDown(1);
        else
            return Input.GetMouseButtonDown(0);
    }

    public static bool IsLeftClickUp()
    {
        if (LeftClickSlow)
            return Input.GetMouseButtonUp(0);
        else
            return Input.GetMouseButtonUp(1);
    }
    public static bool IsRightClickUp()
    {
        if (LeftClickSlow)
            return Input.GetMouseButtonUp(1);
        else
            return Input.GetMouseButtonUp(0);
    }

}
