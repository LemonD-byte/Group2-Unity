using UnityEngine;

public class Level1Controller : MonoBehaviour
{
    public ZombieSpawner spawner; // Nhớ tắt autoStart của spawner trong Inspector

    void Start()
    {
        DialogueManager.Instance.StartDialogue(DialogueDatabase.GetLevel1(), () =>
        {
            spawner.BeginWaves();
        });
    }
}