using UnityEngine;
using UnityEngine.UI;

public class TestingInput : MonoBehaviour
{
    public InputField Field;
    private bool wasFocused;

    private void Update()
    {
        if (wasFocused && Input.GetKeyDown(KeyCode.Return)) {
            Submit(Field.text);
        }

        wasFocused = Field.isFocused;
    }

    private void Submit(string text)
    {
        Debug.Log("Submit=" + text);
    }
}