using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Enemies; // Gọi namespace Enemies của bộ não zombie mới

public class ZombieSpawner : MonoBehaviour
{
    [Header("Zombie Prefab xịn")]
    public GameObject zombiePrefab; // Kéo file Prefab con Zombie (chứa NormalZombie.cs) vào đây

    [Header("Vị trí Spawn ngẫu nhiên")]
    public Transform[] spawnPoints; // Mảng chứa các vị trí spawn (Spawn Points)

    [Header("Cấu hình Wave")]
    public int totalZombiesForThisMap = 10; // Tổng số zombie của màn này
    public int maxZombiesAliveAtOnce = 5;   // Số lượng zombie tối đa xuất hiện cùng lúc trên map
    public float spawnDelay = 2f;           // Thời gian giãn cách giữa mỗi lần spawn

    private int zombiesSpawnedSoFar = 0;    // Số zombie đã sinh ra nãy giờ
    private List<GameObject> activeZombies = new List<GameObject>(); // Danh sách quản lý zombie đang sống

    void Start()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Mạnh ơi! Chưa tạo hoặc chưa gán các SpawnPoint vào Spawner kìa!");
            return;
        }
        
        // Bắt đầu vòng lặp chạy ngầm để spawn zombie
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (zombiesSpawnedSoFar < totalZombiesForThisMap)
        {
            // Tự động dọn dẹp danh sách: Xóa những con zombie đã bị Mạnh bắn chết (bị Destroy phá hủy xác)
            activeZombies.RemoveAll(zombie => zombie == null);

            // Nếu số zombie đang chạy trên map ít hơn giới hạn cho phép
            if (activeZombies.Count < maxZombiesAliveAtOnce)
            {
                SpawnOneZombie();
            }

            // Chờ một khoảng thời gian cấu hình rồi mới check tiếp
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void SpawnOneZombie()
    {
        // 1. Lấy ngẫu nhiên một vị trí trong mảng Spawn Points
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform selectedPoint = spawnPoints[randomIndex];

        // 2. Tạo bản sao Zombie ngay tại vị trí ngẫu nhiên đó
        GameObject newZombie = Instantiate(zombiePrefab, selectedPoint.position, selectedPoint.rotation);

        // 3. Đưa vào danh sách quản lý và tăng bộ đếm số lượng
        activeZombies.Add(newZombie);
        zombiesSpawnedSoFar++;
    }

    // Hàm kiểm tra xem Mạnh đã dọn sạch map chưa (Dùng để GameManager check Win màn)
    public bool IsWaveCleared()
    {
        activeZombies.RemoveAll(zombie => zombie == null);
        return zombiesSpawnedSoFar >= totalZombiesForThisMap && activeZombies.Count == 0;
    }
}