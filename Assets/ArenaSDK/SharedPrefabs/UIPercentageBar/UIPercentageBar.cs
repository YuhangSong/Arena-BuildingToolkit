using UnityEngine;
using UnityEngine.UI;

public class UIPercentageBar : MonoBehaviour
{
    public string ID = "ID";

    public Text TextOnDisplay;
    public Image ImageOnDisplay;

    // for percentage
    float CurrentPercentage = 0f;

    // for value
    float CurrentValue    = 0f;
    bool IsSettingByValue = false;
    float MinValue        = 0f;
    float MaxValue        = 0f;

    // by default, it is disabled
    bool Enabled = false;

    void
    Start()
    {
        // by default, it is disabled
        TextOnDisplay.text        = string.Format("{0}: {1}", ID, "Disabled");
        ImageOnDisplay.fillAmount = 0f;
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
            TextOnDisplay.text        = string.Format("{0}: {1}", ID, "Enabled");
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
        UpdatePercentage(Percentage_, false);
    }

    /// <summary>
    /// Update the percentage to display.
    /// </summary>
    /// <param name="Percentage_">The percentage to be updated to.</param>
    /// <param name="IsForceUpdate_">Update no matter if Percentage_ has updated.</param>
    protected void
    UpdatePercentage(float Percentage_, bool IsForceUpdate_)
    {
        if (Enabled) {
            if ((Percentage_ != CurrentPercentage) || IsForceUpdate_) {
                CurrentPercentage = Percentage_;

                // update

                string TextToDisplay = string.Format("{0}: {1} %", ID, Mathf.RoundToInt(CurrentPercentage * 100f));
                if (IsSettingByValue) {
                    // is setting percentage by value
                    TextToDisplay +=
                      string.Format(" [{0},{1}]", Mathf.RoundToInt(MinValue), Mathf.RoundToInt(MaxValue));
                }

                TextOnDisplay.text        = TextToDisplay;
                ImageOnDisplay.fillAmount = CurrentPercentage;
            }
        } else {
            Debug.LogWarning(
                "UIPercentageBar with ID " + ID
                + " is disabled, call Enable() to initialize before calling UpdatePercentage()");
        }
    }

    /// <summary>
    /// Update the percentage to display by value, the value is automatically normalized to a percentage. The min and max of the value will also be displayed.
    /// </summary>
    /// <param name="Value_">The value to be updated to.</param>
    public void
    UpdateValue(float Value_)
    {
        if (!IsSettingByValue) {
            IsSettingByValue = true;
            CurrentValue     = Value_;
            MinValue         = Value_;
            MaxValue         = Value_;
        } else {
            if (Value_ != CurrentValue) {
                CurrentValue = Value_;
                if (CurrentValue > MaxValue) {
                    MaxValue = CurrentValue;
                } else if (CurrentValue < MinValue) {
                    MinValue = CurrentValue;
                }
                UpdatePercentage(
                    Mathf.Clamp(((CurrentValue - MinValue) / (MaxValue - MinValue)), 0f, 1f),
                    true
                );
            }
        }
    }
}
