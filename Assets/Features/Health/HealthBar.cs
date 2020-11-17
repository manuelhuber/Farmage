using Grimity.Math;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Health {
[RequireComponent(typeof(Mortal))]
public class HealthBar : MonoBehaviour {
    public GameObject hpBarPrefab;
    public float hpBarOffset = 2f;
    public bool showWhenFull;
    private Bounds _bounds;

    private UnityEngine.Camera _camera;
    private GameObject _hpBar;
    private Mortal _mortal;
    private RectTransform _rectTransform;
    private bool _showBar;
    private Image _slider;

    private void Awake() {
        _camera = UnityEngine.Camera.main;
    }

    private void Start() {
        _bounds = GetComponent<Collider>().bounds;

        _mortal = GetComponent<Mortal>();
        _hpBar = Instantiate(hpBarPrefab, GameObject.FindWithTag("HitpointCanvas").transform);
        _slider = _hpBar.transform.GetChild(1).GetComponent<Image>();
        _slider.fillAmount = 1;

        _mortal.Hitpoints.OnChange(OnHpChange);
        _mortal.onDeath.AddListener(() => Destroy(_rectTransform.gameObject));

        SetHpBarSize();
    }

    private void Update() {
        var position = transform.position;
        var hpBarPosition = position;
        hpBarPosition.y += _bounds.size.y * hpBarOffset;
        var hpBarPositionScreenSpace = _camera.WorldToScreenPoint(hpBarPosition);
        var screenPointCenter = _camera.WorldToScreenPoint(position);
        _rectTransform.position = new Vector2(screenPointCenter.x, hpBarPositionScreenSpace.y);
    }

    private void OnHpChange(int hp) {
        _hpBar.SetActive(_mortal.MaxHitpoints != hp || showWhenFull);
        _slider.fillAmount = (float) hp / _mortal.MaxHitpoints;
    }

    private void SetHpBarSize() {
        _rectTransform = _hpBar.GetComponent<RectTransform>();
        var width = _mortal.MaxHitpoints.MinMax(50, 200);
        _rectTransform.sizeDelta = new Vector2(width, _rectTransform.sizeDelta.y);
    }
}
}