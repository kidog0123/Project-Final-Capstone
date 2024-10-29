using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputField : MonoBehaviour
{
    public TMP_InputField tmpInputField;
    // Start is called before the first frame update
    void Start()
    {
        tmpInputField.onEndEdit.AddListener(ValidateInput);
    }
    void ValidateInput(string input)
    {
        if (input.Length > 1)
        {
            tmpInputField.text = input[input.Length - 1].ToString();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
