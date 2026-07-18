using UnityEngine;

public class Level2Controller : MonoBehaviour
{
    public ZombieSpawner spawner; // Task 2 không có thoại mở đầu -> spawner tự chạy luôn

    void Start()
    {
        spawner.BeginWaves();
    }

    // Gắn hàm này vào ZombieSpawner.onAllWavesCleared trong Inspector
    public void OnAllZombiesCleared()
    {
        DialogueManager.Instance.StartDialogue(DialogueDatabase.GetLevel2(), null);
    }
}