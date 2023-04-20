using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSettingButton : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    
    public void ChangeActivity()
    {
        _panel.SetActive(!_panel.activeSelf);
    }
}
