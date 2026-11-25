using UnityEngine;

public class ButtonAction : MonoBehaviour
{
    public enum ButtonMode { OpenDrawer, UnlockDoorDirectly }

    public ButtonMode mode;
    public int roomId;

    private bool alreadyUsed;

    private BoxCollider _ThisObjectBoxCollider;
    private void Start()
    {
        _ThisObjectBoxCollider = GetComponent<BoxCollider>();
    }
    public void Interact()
    {
        if (alreadyUsed) 
        {
            _ThisObjectBoxCollider.enabled = false;
            return; }
        alreadyUsed = true;

        switch (mode)
        {
            case ButtonMode.OpenDrawer:
                GameEvents.CallDrawerOpened(roomId);
                //_ThisObjectBoxCollider.enabled = false;
                break;
            case ButtonMode.UnlockDoorDirectly:
                GameEvents.CallDoorUnlocked(roomId);
                //_ThisObjectBoxCollider.enabled = false;
                break;
        }
    }
}