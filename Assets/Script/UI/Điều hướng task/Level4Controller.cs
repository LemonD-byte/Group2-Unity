using UnityEngine;

public class Level4Controller : MonoBehaviour
{
    public ZombieSpawner wave1Spawner;
    public GameObject hulkBossObject; // Kéo Boss Hulk (đang tắt sẵn trong scene) vào đây

    void Start()
    {
        if (hulkBossObject != null) hulkBossObject.SetActive(false);

        DialogueManager.Instance.StartDialogue(DialogueDatabase.GetLevel4_Wave1(), () =>
        {
            wave1Spawner.BeginWaves();
        });
    }

    // Gắn vào ZombieSpawner (wave1Spawner).onAllWavesCleared trong Inspector
    public void OnWave1Cleared()
    {
        DialogueManager.Instance.StartDialogue(DialogueDatabase.GetLevel4_Wave2(), () =>
        {
            if (hulkBossObject != null) hulkBossObject.SetActive(true);
        });
    }

    // Gắn vào MissionManager.onMissionCompleted trong Inspector (bắn ra khi Boss chết)
    public void OnBossDefeated()
    {
        DialogueManager.Instance.StartDialogue(DialogueDatabase.GetLevel4_End(), () =>
        {
            MissionManager.Instance.ShowVictoryPanel();
        });
    }
}