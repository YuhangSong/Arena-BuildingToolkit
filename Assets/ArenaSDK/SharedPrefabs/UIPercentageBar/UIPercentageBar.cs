using UnityEngine;
using UnityEngine.UI;

public class UIPercentageBar : MonoBehaviour
{
    public string ID = "Value";

    public Text TextOnDisplay;
    public Image ImageOnDisplay;

    float CurrentPercentage = 0f;

    void
    Start()
    {
        UpdatePercentage(0.323f);
    }

    /// <summary>
    /// Update the percentage to display.
    /// </summary>
    /// <param name="Percentage_">The percentage to be updated to.</param>
    public void
    UpdatePercentage(float Percentage_)
    {
        if (Percentage_ != CurrentPercentage) {
            CurrentPercentage = Percentage_;

            // update
            TextOnDisplay.text        = string.Format("{0}: {1} %", ID, Mathf.RoundToInt(CurrentPercentage * 100f));
            ImageOnDisplay.fillAmount = CurrentPercentage;
        }
    }
}
