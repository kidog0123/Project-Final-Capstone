using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SFXScripts : MonoBehaviour
{
    public Slider _sfxSlider;
    public Sprite newImageSFX; // Ảnh khi tắt âm thanh 
    public Sprite oldImageSFX;     // Ảnh khi mở âm thanh
                                    // 
    private Image buttonImageSFX; // Biến lưu trữ component Image của nút Nhạc

   /* private void Awake()
    {
        buttonImageSFX = GetComponent<Image>();
        _sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.5f); // Tải giá trị âm lượng đã lưu hoặc đặt mặc định là 0.5
        SFXVolume(); // Áp dụng giá trị âm lượng
    }*/

    // Start is called before the first frame update
    void Start()
    {
        bool isMuted = PlayerPrefs.GetInt("SfxMuted", 0) == 1;
        AudioManager.Instance.sfxSource.mute = isMuted;
        buttonImageSFX = GetComponent<Image>();
        _sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.5f); // Tải giá trị âm lượng đã lưu hoặc đặt mặc định là 0.5
        SFXVolume(); // Áp dụng giá trị âm lượng
        UpdateButtonImage();
    }
    public void ToggleSFX()
    {
        bool isMuted = !AudioManager.Instance.sfxSource.mute;
        AudioManager.Instance.sfxSource.mute = isMuted;
        PlayerPrefs.SetInt("SfxMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
        AudioManager.Instance.PlaySFX("ClickButton");
        UpdateButtonImage();
    }
    private void UpdateButtonImage()
    {
        // Cập nhật hình ảnh của button dựa trên trạng thái mute
        buttonImageSFX.sprite = AudioManager.Instance.sfxSource.mute ? newImageSFX : oldImageSFX;
    }
    public void SFXVolume()
    {
        float volume = _sfxSlider.value;
        AudioManager.Instance.SFXVolume(volume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }
}

