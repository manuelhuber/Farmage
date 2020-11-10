using JetBrains.Annotations;
using UnityEngine;

namespace Features.Animations {
public class AnimationHandler : MonoBehaviour {
    private Animator _animator;
    private string _lastTrigger;

    private void Start() {
        _animator = gameObject.GetComponent<Animator>();
    }

    public void Trigger(string trigger) {
        _lastTrigger = trigger;
        _animator.SetTrigger(trigger);
    }

    [UsedImplicitly]
    public void AnimationCallback() {
        transform.parent.transform.SendMessage("AnimationCallback", _lastTrigger);
    }
}
}