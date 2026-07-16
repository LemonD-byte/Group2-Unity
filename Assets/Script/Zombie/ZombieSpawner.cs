using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Enemies; // Gọi namespace Enemies của bộ não zombie mới

/// <summary>
/// 1 đợt (wave) zombie: số lượng, tốc độ ra quân, và thoại (tuỳ chọn) hiện TRƯỚC khi đợt này bắt đầu.
/// Nếu không cấu hình gì trong mảng "Waves" ở Inspector, Spawner sẽ tự tạo 1 đợt duy nhất
/// từ 3 ô cấu hình nhanh bên dưới (totalZombiesForThisMap / maxZombiesAliveAtOnce / spawnDelay)
/// -> Map Task 1 hiện tại KHÔNG CẦN CHỈNH GÌ THÊM, vẫn chạy y như cũ.
/// </summary>
[System.Serializable]
public class ZombieWave
{
    public string waveName = "Đợt 1";
    public int zombieCount = 10;
    public int maxZombiesAliveAtOnce = 5;
    public float spawnDelay = 2f;

}

// UnityEvent<T> generic thô không tự hiện được trong Inspector, cần class con serializable riêng.
[System.Serializable] public class ZombieCountEvent : UnityEvent<int, int> { }
[System.Serializable] public class WaveIndexEvent : UnityEvent<int> { }

public class ZombieSpawner : MonoBehaviour
{
    [Header("Zombie Prefab xịn")]
    public GameObject zombiePrefab; // Kéo file Prefab con Zombie (chứa NormalZombie.cs) vào đây

    [Header("Vị trí Spawn ngẫu nhiên")]
    public Transform[] spawnPoints; // Mảng chứa các vị trí spawn (Spawn Points)

    [Header("Nhiều đợt (Wave) - để trống nếu map chỉ có 1 đợt (VD Task 1)")]
    public List<ZombieWave> waves = new List<ZombieWave>();

    [Header("Cấu hình nhanh (chỉ dùng khi mảng Waves ở trên để trống)")]
    public int totalZombiesForThisMap = 10; // Tổng số zombie của màn này
    public int maxZombiesAliveAtOnce = 5;   // Số lượng zombie tối đa xuất hiện cùng lúc trên map
    public float spawnDelay = 2f;           // Thời gian giãn cách giữa mỗi lần spawn

    [Header("Khởi động")]
    [Tooltip("Bật sẵn = Spawner tự chạy khi vào scene (giống hành vi cũ). " +
             "LevelManager sẽ tự tắt cờ này nếu có thoại mở đầu màn, và tự gọi BeginWaves() sau khi thoại xong.")]
    public bool autoStart = true;

    [Header("Events (để HUD / LevelManager lắng nghe)")]
    [Tooltip("Bắn ra mỗi khi có zombie chết: (số đã diệt, tổng số toàn màn)")]
    public ZombieCountEvent onZombieCountChanged = new ZombieCountEvent();
    [Tooltip("Bắn ra khi 1 đợt bị diệt sạch: (chỉ số đợt, bắt đầu từ 0)")]
    public WaveIndexEvent onWaveCleared = new WaveIndexEvent();
    [Tooltip("Bắn ra khi TẤT CẢ các đợt đã bị diệt sạch -> dùng để mở Panel đánh giá sao")]
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

        // Nếu không cấu hình Waves thủ công -> tự tạo 1 đợt duy nhất từ 3 ô cấu hình nhanh
        // (giữ nguyên hành vi cũ cho map 1 đợt như Task 1)
        if (waves == null || waves.Count == 0)
        {
            effectiveWaves = new List<ZombieWave>
            {
                new ZombieWave
                {
                    waveName = "Đợt 1",
                    zombieCount = totalZombiesForThisMap,
                    maxZombiesAliveAtOnce = maxZombiesAliveAtOnce,
                    spawnDelay = spawnDelay
                }
            };
        }
        else
        {
            effectiveWaves = waves;
        }

        totalZombiesAllWaves = 0;
        foreach (var w in effectiveWaves) totalZombiesAllWaves += w.zombieCount;

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
    /// (VD: LevelManager gọi sau khi thoại mở đầu màn kết thúc).
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
        int spawnedThisWave = 0;

        while (spawnedThisWave < wave.zombieCount)
        {
            activeZombies.RemoveAll(zombie => zombie == null);

            if (activeZombies.Count < wave.maxZombiesAliveAtOnce)
            {
                SpawnOneZombie();
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

    void SpawnOneZombie()
    {
        // 1. Lấy ngẫu nhiên một vị trí trong mảng Spawn Points
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform selectedPoint = spawnPoints[randomIndex];

        // 2. Tạo bản sao Zombie ngay tại vị trí ngẫu nhiên đó
        GameObject newZombie = Instantiate(zombiePrefab, selectedPoint.position, selectedPoint.rotation);

        // 3. Đưa vào danh sách quản lý
        activeZombies.Add(newZombie);
    }

    /// <summary>Còn giữ lại để tương thích code cũ: true khi toàn bộ các đợt đã bị diệt sạch.</summary>
    public bool IsWaveCleared()
    {
        return totalZombiesKilled >= totalZombiesAllWaves && activeZombies.Count == 0;
    }
}