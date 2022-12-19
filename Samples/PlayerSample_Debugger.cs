using uDebugger.Attributes;
using UnityEngine;

public class PlayerSample_Debugger : MonoBehaviour
{
    public static PlayerSample_Debugger Instance { get; private set; } = null;

    public int health = 100;

    [DebugMethod(alias: "player.takeDamage")]
    private static void TakeDamage(int amount)
    {
        if (Instance.health <= 0)
            return;

        Instance.health -= amount;

        if (Instance.health <= 0)
        {
            Instance.health = 0;
            Destroy(Instance.gameObject);
        }
    }
}
