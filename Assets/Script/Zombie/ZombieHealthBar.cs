using UnityEngine;
using UnityEngine.UI;

namespace Enemies
{
    /// <summary>
    /// Thanh máu (World Space Canvas) hiển thị phía trên đầu zombie, luôn quay
    /// mặt về phía camera. Được EnemyBase tự động gắn (AddComponent) ở Awake,
    /// nên không cần chỉnh sửa prefab Zombie theo cách thủ công.
    /// </summary>
    [DisallowMultipleComponent]
    public class ZombieHealthBar : MonoBehaviour
    {
        [Header("Vị trí & kích thước")]
        public float heightOffset = 2.3f;
        public float barWidth = 1f; // đơn vị mét trong thế giới game

        [Header("Màu sắc")]
        public Color highColor = new Color(0.2f, 0.85f, 0.2f);
        public Color lowColor = new Color(0.85f, 0.15f, 0.15f);

        private EnemyBase enemy;
        private Transform canvasTransform;
        private Image fillImage;
        private Camera cam;

        private const float CanvasWidthUnits = 100f;
        private const float CanvasHeightUnits = 14f;

        void Awake()
        {
            enemy = GetComponent<EnemyBase>();
            cam = Camera.main;
            BuildHealthBar();
        }

        void LateUpdate()
        {
            if (enemy == null || canvasTransform == null) return;

            if (enemy.IsDead)
            {
                if (canvasTransform.gameObject.activeSelf)
                    canvasTransform.gameObject.SetActive(false);
                return;
            }

            if (cam == null) cam = Camera.main;

            canvasTransform.position = transform.position + Vector3.up * heightOffset;

            if (cam != null)
                canvasTransform.rotation = Quaternion.LookRotation(canvasTransform.position - cam.transform.position);

            float ratio = enemy.MaxHealth > 0 ? Mathf.Clamp01(enemy.CurrentHealth / enemy.MaxHealth) : 0f;
            fillImage.fillAmount = ratio;
            fillImage.color = Color.Lerp(lowColor, highColor, ratio);
        }

        private void BuildHealthBar()
        {
            GameObject canvasObj = new GameObject("HealthBarCanvas");
            canvasObj.transform.SetParent(transform, false);
            canvasObj.transform.position = transform.position + Vector3.up * heightOffset;

            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasObj.AddComponent<CanvasScaler>();

            RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(CanvasWidthUnits, CanvasHeightUnits);
            canvasObj.transform.localScale = Vector3.one * (barWidth / CanvasWidthUnits);

            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(canvasObj.transform, false);
            Image bg = bgObj.AddComponent<Image>();
            bg.color = new Color(0f, 0f, 0f, 0.6f);
            RectTransform bgRect = bg.rectTransform;
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            GameObject fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(canvasObj.transform, false);
            fillImage = fillObj.AddComponent<Image>();
            fillImage.color = highColor;
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
            fillImage.fillAmount = 1f;
            RectTransform fillRect = fillImage.rectTransform;
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = new Vector2(3, 3);
            fillRect.offsetMax = new Vector2(-3, -3);

            canvasTransform = canvasObj.transform;
        }
    }
}
