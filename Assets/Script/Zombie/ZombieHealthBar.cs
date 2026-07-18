using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Enemies
{
    /// <summary>
    /// Thanh máu World Space hiển thị phía trên đầu Zombie.
    /// Tự động hiển thị tên Zombie dựa vào tên Prefab/GameObject.
    /// </summary>
    [DisallowMultipleComponent]
    public class ZombieHealthBar : MonoBehaviour
    {
        [Header("Vị trí & kích thước")]
        public float heightOffset = 2.8f;   // Đã tăng thêm 0.5
        public float barWidth = 1f;

        [Header("Màu sắc")]
        public Color highColor = new Color(0.2f, 0.85f, 0.2f);
        public Color lowColor = new Color(0.85f, 0.15f, 0.15f);

        [Header("Tên Zombie")]
        public float nameFontSize = 8f;
        public Color nameColor = Color.red; // Đổi sang màu đỏ

        private EnemyBase enemy;
        private Transform canvasTransform;
        private Image fillImage;
        private TMP_Text nameText;
        private Camera cam;

        private const float CanvasWidthUnits = 100f;
        private const float CanvasHeightUnits = 30f;

        void Awake()
        {
            enemy = GetComponent<EnemyBase>();
            cam = Camera.main;

            BuildHealthBar();

            if (nameText != null)
                nameText.text = GetZombieName();
        }

        void LateUpdate()
        {
            if (enemy == null || canvasTransform == null)
                return;

            if (enemy.IsDead)
            {
                if (canvasTransform.gameObject.activeSelf)
                    canvasTransform.gameObject.SetActive(false);

                return;
            }

            if (cam == null)
                cam = Camera.main;

            canvasTransform.position = transform.position + Vector3.up * heightOffset;

            if (cam != null)
            {
                canvasTransform.rotation =
                    Quaternion.LookRotation(canvasTransform.position - cam.transform.position);
            }

            float ratio = enemy.MaxHealth > 0
                ? Mathf.Clamp01(enemy.CurrentHealth / enemy.MaxHealth)
                : 0f;

            fillImage.fillAmount = ratio;
            fillImage.color = Color.Lerp(lowColor, highColor, ratio);
        }

        private string GetZombieName()
        {
            return gameObject.name.Replace("(Clone)", "").Trim();
        }

        private void BuildHealthBar()
        {
            //================ Canvas ================
            GameObject canvasObj = new GameObject("HealthBarCanvas");
            canvasObj.transform.SetParent(transform, false);
            canvasObj.transform.position = transform.position + Vector3.up * heightOffset;

            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            canvasObj.AddComponent<CanvasScaler>();

            RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(CanvasWidthUnits, CanvasHeightUnits);

            canvasObj.transform.localScale =
                Vector3.one * (barWidth / CanvasWidthUnits);

            //================ Zombie Name ================
            GameObject textObj = new GameObject("ZombieName");
            textObj.transform.SetParent(canvasObj.transform, false);

            nameText = textObj.AddComponent<TextMeshProUGUI>();
            nameText.text = "";
            nameText.fontSize = nameFontSize;
            nameText.color = nameColor;
            nameText.alignment = TextAlignmentOptions.Center;
            nameText.fontStyle = FontStyles.Bold;
            nameText.enableAutoSizing = true;

            RectTransform textRect = nameText.rectTransform;
            textRect.anchorMin = new Vector2(0, 1);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.pivot = new Vector2(0.5f, 1);
            textRect.sizeDelta = new Vector2(0, 14);
            textRect.anchoredPosition = Vector2.zero;

            //================ Background ================
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(canvasObj.transform, false);

            Image bg = bgObj.AddComponent<Image>();
            bg.color = new Color(0f, 0f, 0f, 0.6f);

            RectTransform bgRect = bg.rectTransform;
            bgRect.anchorMin = new Vector2(0, 0);
            bgRect.anchorMax = new Vector2(1, 0);
            bgRect.pivot = new Vector2(0.5f, 0);
            bgRect.sizeDelta = new Vector2(0, 12);
            bgRect.anchoredPosition = new Vector2(0, 2);

            //================ Fill ================
            GameObject fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(bgObj.transform, false);

            fillImage = fillObj.AddComponent<Image>();
            fillImage.color = highColor;
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
            fillImage.fillAmount = 1f;

            RectTransform fillRect = fillImage.rectTransform;
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = new Vector2(2, 2);
            fillRect.offsetMax = new Vector2(-2, -2);

            canvasTransform = canvasObj.transform;
        }
    }
}