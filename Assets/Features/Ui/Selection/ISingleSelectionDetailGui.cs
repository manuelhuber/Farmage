using UnityEngine;

namespace Features.Ui.Selection {
/// This is the UI that lands in the center of the GUI, right below the Selectable's name / HP
public interface ISingleSelectionDetailGui {
    /// <summary>
    ///     This is called when the Selection is made
    /// </summary>
    /// <param name="selectedUnit"></param>
    void Init(GameObject selectedUnit);
}
}