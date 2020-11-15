﻿using UnityEngine;
using UnityEngine.UI;
using Vendor.Werewolf.StatusIndicators.Scripts.Components;

namespace Vendor.Werewolf.StatusIndicators.Demos.SpellIndicators.Scripts {
public class CharacterDemo1 : MonoBehaviour {
    public SplatManager Splats { get; set; }

    private int index;

    private void Start() {
        Splats = GetComponentInChildren<SplatManager>();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Splats.CancelSpellIndicator();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            index = (int) Mathf.Repeat(index - 1, Splats.SpellIndicators.Length);
            UpdateSelection();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            index = (int) Mathf.Repeat(index + 1, Splats.SpellIndicators.Length);
            UpdateSelection();
        }
    }

    private void UpdateSelection() {
        Splats.SelectSpellIndicator(Splats.SpellIndicators[index].name);
        FindObjectOfType<SplatName>().GetComponent<Text>().text =
            index + ": " + Splats.CurrentSpellIndicator.name;
    }
}
}