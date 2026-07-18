using UnityEngine;

public class Level3Controller : MonoBehaviour
{
    public ZombieSpawner wave1Spawner;
    public ZombieSpawner wave2Spawner; // để autoStart=true nhưng GameObject TẮT sẵn trong scene

    void Start()
    {
        if (wave2Spawner != null) wave2Spawner.gameObject.SetActive(false);

        DialogueManager.Instance.StartDialogue(DialogueDatabase.GetLevel3_Wave1(), () =>
        {
            wave1Spawner.BeginWaves();
        });
    }

    // Gắn vào MissionManager.onMaterialsCompleted trong Inspector
    public void OnMaterialsCollected()
    {
        DialogueManager.Instance.StartDialogue(DialogueDatabase.GetLevel3_Wave2(), () =>
        {
            if (wave2Spawner != null) wave2Spawner.gameObject.SetActive(true); // Start() của nó tự BeginWaves()
        });
    }

    // Gắn vào MissionManager.onMissionCompleted trong Inspector
    public void OnMissionCompleted()
    {
        DialogueManager.Instance.StartDialogue(DialogueDatabase.GetLevel3_End(), () =>
        {
            MissionManager.Instance.ShowVictoryPanel();
        });
    }
}