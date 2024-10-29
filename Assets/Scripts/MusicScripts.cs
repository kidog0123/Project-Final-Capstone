using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicScripts : MonoBehaviour
{
    public Slider _musicSlider;
    public Sprite newImageNhac; // Ảnh khi tắt âm thanh 
    public Sprite oldImageNhac;     // Ảnh khi mở âm thanh
                                    // 
    private Image buttonImageNhac; // Biến lưu trữ component Image của nút Nhạc
    // Start is called before the first frame update
  /*  private void Awake()
    {
        _musicSlider.value = PlayerPrefs.GetFloat("musicVolume", 0.5f); // Tải giá trị âm lượng đã lưu hoặc đặt mặc định là 0.5
        MusicVolume(); // Áp dụng giá trị âm lượng
        buttonImageNhac = GetComponent<Image>(); // Lấy component Image của nút

        UpdateButtonImage(); // Cập nhật hình ảnh nút
    }*/
    void Start()
    {
        bool isMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
        AudioManager.Instance.musicSource.mute = isMuted;
        buttonImageNhac = GetComponent<Image>(); // Lấy component Image của nút
        UpdateButtonImage();
        _musicSlider.value = PlayerPrefs.GetFloat("musicVolume", 0.5f);
        MusicVolume();
    }
    public void ToggleMusic()
    {
       // AudioManager.Instance.PlaySFX("ClickButton");
        bool isMuted = !AudioManager.Instance.musicSource.mute;
        AudioManager.Instance.musicSource.mute = isMuted;
        PlayerPrefs.SetInt("MusicMuted", isMuted ? 1 : 0);
        AudioManager.Instance.PlaySFX("ClickButton");
        PlayerPrefs.Save();
        UpdateButtonImage();
    }
    private void UpdateButtonImage()
    {
        // Cập nhật hình ảnh của button dựa trên trạng thái mute
        buttonImageNhac.sprite = AudioManager.Instance.musicSource.mute ? newImageNhac : oldImageNhac;
    }
    public void MusicVolume()
    {
        float volume = _musicSlider.value;
        AudioManager.Instance.MusicVolume(volume);
        PlayerPrefs.SetFloat("musicVolume", volume);
        PlayerPrefs.Save();
    }
}
