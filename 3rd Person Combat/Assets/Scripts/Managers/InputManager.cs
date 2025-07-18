using UnityEngine;
using UnityEngine.InputSystem; 

public class InputManager : Singleton<InputManager>
{
    public InputActionAsset InputActions; 

    public void EnableActionMap(string actionMap)
    {
        DisableAllActionMaps();

        InputActions.FindActionMap(actionMap)?.Enable(); 
    }

    public void DisableAllActionMaps()
    {
        foreach (var actionMap in InputActions.actionMaps)
        {
            actionMap.Disable();
        }
    }



}
