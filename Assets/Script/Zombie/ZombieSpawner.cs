using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Enemies; // Gọi namespace Enemies của bộ não zombie mới

/// <summary>
/// 1 loại quái + số lượng trong 1 đợt. VD: Normal x20, Runner x12, Spitter x6...
/// </summary>
[System.Serializable]
public class ZombieSpawnEntry
{
    public GameObject zombiePrefab; // Kéo đúng prefab loại quái (Normal/Runner/Spitter/Hulk...) vào đây
    public int count = 1;
}

/// <summary>
/// 1 đợt (wave) zombie: có thể TRỘN nhiều loại quái khác nhau trong cùng 1 đợt
/// (VD: Task 2 = 20 thường + 12 runner cùng lúc), tốc độ ra quân, và thoại (tuỳ chọn)
/// hiện TRƯỚC khi đợt này bắt đầu (do LevelController quản lý riêng, không nằm trong class này).
/// Nếu không cấu hình gì trong mảng "Waves" ở Inspector, Spawner sẽ tự tạo 1 đợt duy nhất
/// từ 3 ô cấu hình nhanh bên dưới (zombiePrefab / totalZombiesForThisMap / maxZombiesAliveAtOnce / spawnDelay)
/// -> Map chỉ có 1 loại quái duy nhất (VD Task 1) KHÔNG CẦN CHỈNH GÌ THÊM.
/// </summary>
[System.Serializable]
public class ZombieWave
{
    public string waveName = "Đợt 1";

    [Tooltip("Danh sách các loại quái + số lượng trong đợt này. Có thể trộn nhiều loại " +
             "(VD: 20 thường + 12 runner). Các loại sẽ ra ngẫu nhiên xen kẽ nhau, không dồn cục theo thứ tự.")]
    public List<ZombieSpawnEntry> spawnEntries = new List<ZombieSpawnEntry>();

    public int maxZombiesAliveAtOnce = 5;
    public float spawnDelay = 2f;

    /// <summary>Tổng số quái (mọi loại cộng lại) của đợt này.</summary>
    public int TotalCount()
    {
        int total = 0;
        foreach (var e in spawnEntries) total += e.count;
        return total;
    }
}

// UnityEvent<T> generic thô không tự hiện được trong Inspector, cần class con serializable riêng.
[System.Serializable] public class ZombieCountEvent : UnityEvent<int, int> { }
[System.Serializable] public class WaveIndexEvent : UnityEvent<int> { }

public class ZombieSpawner : MonoBehaviour
{
    [Header("Zombie Prefab xịn (CHỈ dùng cho map 1 loại quái duy nhất, khi mảng Waves để trống)")]
    public GameObject zombiePrefab; // Kéo file Prefab con Zombie (chứa NormalZombie.cs) vào đây

    [Header("Vị trí Spawn ngẫu nhiên")]
    public Transform[] spawnPoints; // Mảng chứa các vị trí spawn (Spawn Points)

    [Header("Nhiều đợt (Wave) - để trống nếu map chỉ có 1 đợt, 1 loại quái duy nhất (VD Task 1)")]
    public List<ZombieWave> waves = new List<ZombieWave>();

    [Header("Khởi động")]
    [Tooltip("Bật sẵn = Spawner tự chạy khi vào scene. LevelController sẽ tự tắt cờ này nếu " +
             "màn có thoại mở đầu, và tự gọi BeginWaves() sau khi thoại xong.")]
    public bool autoStart = true;

    [Header("Events (để HUD / LevelController lắng nghe)")]
    [Tooltip("Bắn ra mỗi khi có zombie chết: (số đã diệt, tổng số toàn màn)")]
    public ZombieCountEvent onZombieCountChanged = new ZombieCountEvent();
    [Tooltip("Bắn ra khi 1 đợt bị diệt sạch: (chỉ số đợt, bắt đầu từ 0)")]
    public WaveIndexEvent onWaveCleared = new WaveIndexEvent();
    [Tooltip("Bắn ra khi TẤT CẢ các đợt đã bị diệt sạch -> LevelController dùng để chèn thoại / kích hoạt bước tiếp theo")]
    public UnityEvent onAllWavesCleared;

    private List<ZombieWave> effectiveWaves;
    private int totalZombiesAllWaves;
    private int totalZombiesKilled;
    private int currentWaveIndex = -1;
    private bool isRunning;

    private List<GameObject> activeZombies = new List<GameObject>(); // Danh sách quản lý zombie đang sống

    public int TotalZombies => totalZombiesAllWaves;
    public int ZombiesKilled => totalZombiesKilled;
    public int CurrentWaveIndex => currentWaveIndex;

    void Start()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Mạnh ơi! Chưa tạo hoặc chưa gán các SpawnPoint vào Spawner kìa!");
            return;
        }

        if (waves == null || waves.Count == 0)
        {
            Debug.LogError("ZombieSpawner: Chưa cấu hình bất kỳ Wave nào!");
            return;
        }

        effectiveWaves = waves;

        totalZombiesAllWaves = 0;
        foreach (var w in effectiveWaves) totalZombiesAllWaves += w.TotalCount();

        if (autoStart)
        {
            BeginWaves();
        }
    }

    void Update()
    {
        // Dọn dẹp danh sách mỗi frame: xóa những con zombie đã bị bắn chết (bị Destroy phá hủy xác)
        // và cộng dồn số lượng đã diệt để bắn event cho HUD.
        int before = activeZombies.Count;
        if (before > 0)
        {
            activeZombies.RemoveAll(z => z == null);
            int killedThisFrame = before - activeZombies.Count;
            if (killedThisFrame > 0)
            {
                totalZombiesKilled += killedThisFrame;
                onZombieCountChanged?.Invoke(totalZombiesKilled, totalZombiesAllWaves);
            }
        }
    }

    /// <summary>
    /// Bắt đầu chạy toàn bộ chuỗi đợt (waves). Gọi hàm này thủ công nếu autoStart = false
    /// (VD: LevelController gọi sau khi thoại mở đầu màn kết thúc).
    /// </summary>
    public void BeginWaves()
    {
        if (isRunning) return;
        isRunning = true;
        StartCoroutine(RunAllWaves());
    }

    private IEnumerator RunAllWaves()
    {
        for (int i = 0; i < effectiveWaves.Count; i++)
        {
            currentWaveIndex = i;
            var wave = effectiveWaves[i];

            yield return StartCoroutine(SpawnWaveRoutine(wave));

            onWaveCleared?.Invoke(i);
        }

        onAllWavesCleared?.Invoke();
    }

    private IEnumerator SpawnWaveRoutine(ZombieWave wave)
    {
        // Gộp toàn bộ loại quái + số lượng của đợt này thành 1 danh sách phẳng
        List<GameObject> spawnQueue = new List<GameObject>();
        foreach (var entry in wave.spawnEntries)
        {
            for (int i = 0; i < entry.count; i++)
                spawnQueue.Add(entry.zombiePrefab);
        }

        // Xáo trộn ngẫu nhiên để các loại quái ra xen kẽ nhau, không dồn cục theo thứ tự khai báo
        for (int i = spawnQueue.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (spawnQueue[i], spawnQueue[j]) = (spawnQueue[j], spawnQueue[i]);
        }

        int spawnedThisWave = 0;

        while (spawnedThisWave < spawnQueue.Count)
        {
            activeZombies.RemoveAll(zombie => zombie == null);

            if (activeZombies.Count < wave.maxZombiesAliveAtOnce)
            {
                SpawnOneZombie(spawnQueue[spawnedThisWave]);
                spawnedThisWave++;
            }

            yield return new WaitForSeconds(wave.spawnDelay);
        }

        // Chờ đến khi những con cuối cùng của đợt này cũng bị diệt sạch trước khi qua đợt kế
        while (true)
        {
            activeZombies.RemoveAll(zombie => zombie == null);
            if (activeZombies.Count == 0) yield break;
            yield return null;
        }
    }

    void SpawnOneZombie(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("ZombieSpawner: có 1 Spawn Entry chưa gán Zombie Prefab!");
            return;
        }

        // 1. Lấy ngẫu nhiên một vị trí trong mảng Spawn Points
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform selectedPoint = spawnPoints[randomIndex];

        // 2. Tạo bản sao Zombie (đúng loại theo prefab truyền vào) ngay tại vị trí ngẫu nhiên đó
        GameObject newZombie = Instantiate(prefab, selectedPoint.position, selectedPoint.rotation);

        // 3. Đưa vào danh sách quản lý
        activeZombies.Add(newZombie);
    }

    /// <summary>Còn giữ lại để tương thích code cũ: true khi toàn bộ các đợt đã bị diệt sạch.</summary>
    public bool IsWaveCleared()
    {
        return totalZombiesKilled >= totalZombiesAllWaves && activeZombies.Count == 0;
    }
}