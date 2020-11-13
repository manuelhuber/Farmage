using JetBrains.Annotations;
using UnityEngine;

namespace Features.Animations {
/// <summary>
/// Light wrapper around units Animator to make it easier to handle callbacks
/// </summary>
[RequireComponent(typeof(Animator))]
public class AnimationHandler : MonoBehaviour {
    private Animator _animator;
    private string _lastTrigger;

    private void Awake() {
        _animator = gameObject.GetComponent<Animator>();
    }

    public void SetTrigger(string trigger) {
        _lastTrigger = trigger;
        _animator.SetTrigger(trigger);
    }

    [UsedImplicitly]
    public void AnimationCallback() {
        transform.parent.transform.SendMessage("AnimationCallback", _lastTrigger);
    }

    // ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ delegate functions to _animator without any additional logic ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓

    public float GetFloat(string propertyName) {
        return _animator.GetFloat(propertyName);
    }

    public float GetFloat(int id) {
        return _animator.GetFloat(id);
    }

    public void SetFloat(string propertyName, float value) {
        _animator.SetFloat(propertyName, value);
    }

    public void SetFloat(string propertyName, float value, float dampTime, float deltaTime) {
        _animator.SetFloat(propertyName, value, dampTime, deltaTime);
    }

    public void SetFloat(int id, float value) {
        _animator.SetFloat(id, value);
    }

    public void SetFloat(int id, float value, float dampTime, float deltaTime) {
        _animator.SetFloat(id, value, dampTime, deltaTime);
    }

    public bool GetBool(string propertyName) {
        return _animator.GetBool(propertyName);
    }

    public bool GetBool(int id) {
        return _animator.GetBool(id);
    }

    public void SetBool(string propertyName, bool value) {
        _animator.SetBool(propertyName, value);
    }

    public void SetBool(int id, bool value) {
        _animator.SetBool(id, value);
    }

    public int GetInteger(string propertyName) {
        return _animator.GetInteger(propertyName);
    }

    public int GetInteger(int id) {
        return _animator.GetInteger(id);
    }

    public void SetInteger(string propertyName, int value) {
        _animator.SetInteger(propertyName, value);
    }

    public void SetInteger(int id, int value) {
        _animator.SetInteger(id, value);
    }

    public void SetTrigger(int id) {
        _animator.SetTrigger(id);
    }

    public void ResetTrigger(string propertyName) {
        _animator.ResetTrigger(propertyName);
    }

    public void ResetTrigger(int id) {
        _animator.ResetTrigger(id);
    }

    public bool IsParameterControlledByCurve(string propertyName) {
        return _animator.IsParameterControlledByCurve(propertyName);
    }

    public bool IsParameterControlledByCurve(int id) {
        return _animator.IsParameterControlledByCurve(id);
    }
}
}