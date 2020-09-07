using Features.Ui.Selection;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Building.Production {
public class ProductionQueueGui : MonoBehaviour, ISingleSelectionDetailGui {
    public GameObject iconPrefab;
    public Image currentProductionIcon;
    public Text currentProductionText;
    public Transform queueSection;

    private Production _production;
    private Image[] _queueIcons;

    private void Update() {
        UpdateQueueIcons();
        UpdateCurrentProduction();
    }

    public void Init(GameObject selectedUnit) {
        _production = selectedUnit.GetComponent<Production>();
        CreateQueuePrefabs(_production.queueSize);
    }

    private void UpdateCurrentProduction() {
        if (_production.Current == null) return;
        var target = _production.Current.productionTimeInSeconds;
        var progress = _production.Progress;
        var percentage = progress / target * 100;
        currentProductionText.text = $"Progress: {percentage:00}%";
        currentProductionIcon.sprite = _production.Current.icon;
    }

    private void UpdateQueueIcons() {
        var queue = _production.Queue.ToArray();
        for (var i = 0; i < _queueIcons.Length; i++) {
            var queueIcon = _queueIcons[i];

            if (queue.Length <= i) {
                queueIcon.enabled = false;
            } else {
                queueIcon.enabled = true;
                queueIcon.sprite = queue[i].icon;
            }
        }
    }

    private void CreateQueuePrefabs(int queueSize) {
        _queueIcons = new Image[queueSize];
        for (var i = 0; i < queueSize; i++) {
            var icon = Instantiate(iconPrefab, queueSection);
            _queueIcons[i] = icon.GetComponent<Image>();
        }
    }
}
}