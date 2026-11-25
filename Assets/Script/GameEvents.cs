using System;

public static class GameEvents
{
    public static event Action<int> DrawerOpened;
    public static event Action<int> KeyCollected;
    public static event Action<int> DoorUnlocked;

    public static void CallDrawerOpened(int roomId) { DrawerOpened?.Invoke(roomId); }
    public static void CallKeyCollected(int roomId) { KeyCollected?.Invoke(roomId); }
    public static void CallDoorUnlocked(int roomId) { DoorUnlocked?.Invoke(roomId); }
}