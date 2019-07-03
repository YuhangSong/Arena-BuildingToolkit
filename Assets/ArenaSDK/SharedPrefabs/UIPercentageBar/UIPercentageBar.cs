using UnityEngine;
using UnityEngine.UI;

public class UIPercentageBar : MonoBehaviour
{
    public string ID = "ID";

    public Text TextOnDisplay;
    public Image ImageOnDisplay;

    float CurrentPercentage = 0f;
    bool Enabled = true;

    void
    Start()
    {
        // by default, it is disabled
        Disable();
    }

    /// <summary>
    /// Enable this bar, this will cause:
    ///   text display: "ID: Enabled"
    /// </summary>
    public void
    Enable()
    {
        if (!Enabled) {
            Enabled = true;
            TextOnDisplay.text        = string.Format("{0}: {1} %", ID, "Enabled");
            ImageOnDisplay.fillAmount = 0f;
        }
    }

    /// <summary>
    /// Disable this bar, this will cause:
    ///   text display: "ID: Disabled"
    /// </summary>
    public void
    Disable()
    {
        if (Enabled) {
            Enabled = false;
            TextOnDisplay.text        = string.Format("{0}: {1} %", ID, "Disabled");
            ImageOnDisplay.fillAmount = 0f;
        }
    }

    /// <summary>
    /// Update the percentage to display.
    /// </summary>
    /// <param name="Percentage_">The percentage to be updated to.</param>
    public void
    UpdatePercentage(float Percentage_)
    {
        if (Enabled) {
            if (Percentage_ != CurrentPercentage) {
                CurrentPercentage = Percentage_;

                // update
                TextOnDisplay.text        = string.Format("{0}: {1} %", ID, Mathf.RoundToInt(CurrentPercentage * 100f));
                ImageOnDisplay.fillAmount = CurrentPercentage;
            }
        } else {
            Debug.LogWarning(
                "UIPercentageBar with ID " + ID
                + " is disabled, call Enable() to initialize before calling UpdatePercentage()");
        }
    }
}
