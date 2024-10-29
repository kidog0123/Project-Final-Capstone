using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject Menus;
    public GameObject settings;
    public GameObject sound;
    public GameObject ContronlSetting;
    public Button soundButton;
    public Button settingButton;
    //public TMP_InputField tmpInputField;
    // Start is called before the first frame update
    void Start()
    {
        Menus.SetActive(true);
        settings.SetActive(false);
    }

   /* void ValidateInput(string input)
    {
        if (input.Length > 1)
        {
            tmpInputField.text = input.Substring(0, 1);
        }
    } */
    void Settings()
    {
       int setting = PlayerPrefs.GetInt("Setting");
        if(setting==1)
        {
            soundButton.interactable = false;
            settingButton.interactable=true;
            ContronlSetting.SetActive(false);
            sound.SetActive(true);
        }
        if(setting==2)
        {
            soundButton.interactable = true;
            settingButton.interactable=false;
            ContronlSetting.SetActive(true);
            sound.SetActive(false);
          //  tmpInputField.onEndEdit.AddListener(ValidateInput);
        }
    }
    public void Sound()
    {
        PlayerPrefs.SetInt("Setting", 1);
    }
    public void Conttroler()
    {
        PlayerPrefs.SetInt("Setting", 2);
    }
    public void Settingpanal()
    {
        Menus.SetActive(false);
        settings.SetActive(true);
        PlayerPrefs.SetInt("Setting", 1);
    }
    public void Trolai()
    {
        Menus.SetActive(true);
        settings.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        Settings();
    }
}
