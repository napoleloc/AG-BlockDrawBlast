using System;
using EncosyTower.PageFlows.MonoPages;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlockDrawBlast.UI
{
    public class MonoPagePlay : MonoPageBase
    {
        [SerializeField] private Button _buttonSetting;
        [SerializeField] private Button _buttonRefresh;

        [SerializeField] private TMP_Text _labelLevel;
        [SerializeField] private TMP_Text _labelScore;

        private void Awake()
        {
            _buttonSetting.onClick.AddListener(OnButtonSettingClick);
            _buttonRefresh.onClick.AddListener(OnButtonRefreshClick);
        }

        private void OnDestroy()
        {
            _buttonRefresh.onClick.RemoveListener(OnButtonRefreshClick);
            _buttonSetting.onClick.RemoveListener(OnButtonSettingClick);
        }

        private void OnButtonSettingClick()
        {
            
        }

        private void OnButtonRefreshClick()
        {
            
        }
        
    }
    
}

