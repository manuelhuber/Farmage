using Grimity.Math;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Health {
[RequireComponent(typeof(Mortal))]
public class Healthbar : MonoBehaviour {
    public GameObject hpBarPrefab;
    public float hpBarOffset = 2f;
    private Bounds _bounds;

    private UnityEngine.Camera _camera;
    private RectTransform _rectTransform;

    private void Awake() {
        _camera = UnityEngine.Camera.main;
    }

    private void Start() {
        var mortal = GetComponent<Mortal>();
        var hpBar = Instantiate(hpBarPrefab, GameObject.FindWithTag("HitpointCanvas").transform);
        var slider = hpBar.GetComponentInChildren<Slider>();
        _rectTransform = hpBar.GetComponent<RectTransform>();
        var width = mortal.MaxHitpoints.MinMax(50, 200);
        _rectTransform.sizeDelta = new Vector2(width, _rectTransform.sizeDelta.y);
        slider.maxValue = mortal.MaxHitpoints;
        mortal.Hitpoints.OnChange(hp => slider.value = hp);
        _bounds = GetComponent<Collider>().bounds;
        mortal.onDeath.AddListener(() => Destroy(_rectTransform.gameObject));
    }

    private void Update() {
        var position = transform.position;
        var hpBarPosition = position;
        hpBarPosition.y += _bounds.size.y * hpBarOffset;
        var hpBarPositionScreenSpace = _camera.WorldToScreenPoint(hpBarPosition);
        var screenPointCenter = _camera.WorldToScreenPoint(position);
        _rectTransform.position = new Vector2(screenPointCenter.x, hpBarPositionScreenSpace.y);
    }
}
}