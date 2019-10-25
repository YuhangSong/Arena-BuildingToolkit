using UnityEngine;
using UnityEngine.UI;

namespace Arena
{
    public class UIText : MonoBehaviour
    {
        public string ID = "ID";

        public Text TextOnDisplay;

        public void
        setText(string Text_)
        {
            TextOnDisplay.text = Text_;
        }

        public void
        setColor(Color Color_)
        {
            TextOnDisplay.color = Color_;
        }
    }
}
