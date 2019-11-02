using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Example : MonoBehaviour
{
    // These are the floats for the x, y, and z components of the quaternion
    public float m_MyX, m_MyY, m_MyZ, m_MyW;
    // These are the Sliders that set the rotation. Remember to assign these in the Inspector
    public Slider m_SliderX, m_SliderY, m_SliderZ;
    // These are the Texts that output the current value of the rotations. Remember to assign these in the Inspector
    public Text m_TextX, m_TextY, m_TextZ;

    // Use this for initialization
    void
    Start()
    {
        // Initialise the x, y, and z components of the future Quaternion
        m_MyX = 0;
        m_MyY = 0;
        m_MyZ = 0;

        // Set all the sliders max values to 1 so the Quaternion values don't go over 1
        m_SliderX.maxValue = 1;
        m_SliderY.maxValue = 1;
        m_SliderZ.maxValue = 1;

        // Set all the sliders min values to -1 so the Quaternion values don't go under 1
        m_SliderX.minValue = -1;
        m_SliderY.minValue = -1;
        m_SliderZ.minValue = -1;
    }

    // Change the Quaternion values depending on the values of the Sliders
    private static Quaternion
    Change(float x, float y, float z, float w)
    {
        // Return the new Quaternion
        return new Quaternion(x, y, z, w);
    }

    void
    Update()
    {
        // // Update the x, y and z values to that of the sliders
        // m_MyX = m_SliderX.value;
        // m_MyY = m_SliderY.value;
        // m_MyZ = m_SliderZ.value;
        // Output the current values of x, y, and z
        m_TextX.text = " X : " + m_MyX;
        m_TextY.text = " Y : " + m_MyY;
        m_TextZ.text = " Z : " + m_MyZ;

        // Rotate the GameObject by the new Quaternion
        transform.rotation = Change(m_MyX, m_MyY, m_MyZ, m_MyW);
    }
}
