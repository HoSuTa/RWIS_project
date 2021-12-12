using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField]
    private InputField _keyField;
    [SerializeField]
    private Text _keyIsValidText;
    [SerializeField]
    private Text _savedGssKeyExistsText;
    [SerializeField]
    private Color _plusColor;
    [SerializeField]
    private Color _minusColor;
    [SerializeField]
    private Text _savedGssKey;
    [SerializeField]
    private GameObject _playButton;
    [SerializeField]
    private GssDbManageWrapper.GssDbHub _dbHub;

    private bool _runSaveGssKey = false;
    private bool _runSaveGasKey = false;

    private void Awake()
    {
        if(_dbHub == null)
        {
            _dbHub = GetComponent<GssDbManageWrapper.GssDbHub>();
        }
        UpdateKeyRelatedUI();
    }

    private void Update()
    {
        if (_runSaveGssKey)
        {
            if (!string.IsNullOrEmpty(_keyField.text))
            {
                _dbHub.CheckIfGssUrlValid
                (
                    _keyField.text, 
                    () => GssUrlManager.SaveUrl(_keyField.text), 
                    UpdateKeyRelatedUI,
                    KeyNotValidFeedBack
                );
            }
            _runSaveGssKey = false;
        }

        if (_runSaveGasKey)
        {
            if (!string.IsNullOrEmpty(_keyField.text))
            {
                _dbHub.CheckIfGssUrlValid
                 (
                     _keyField.text,
                     () => GasUrlManager.SaveUrl(_keyField.text),
                     UpdateKeyRelatedUI,
                     KeyNotValidFeedBack
                 );
            }
            _runSaveGasKey = false;
        }
    }

    private void KeyNotValidFeedBack()
    {
        _keyIsValidText.text = "The input was invalid";
    }

    public void UpdateKeyRelatedUI()
    {
        if (GssUrlManager.IsUrlAssigned())
        {
            _savedGssKeyExistsText.text = "Saved GSS Key exists";
            _savedGssKeyExistsText.color = _plusColor;
            _savedGssKey.text = "\nSaved GSS Key:\n" + GssUrlManager.GetUrl();
            _playButton.SetActive(true);
        }
        else
        {
            _savedGssKeyExistsText.text = "Saved GSS Key does not exist.";
            _savedGssKeyExistsText.color = _minusColor;
            _savedGssKey.text = "\nSaved Gss Key does not exist.";
            _playButton.SetActive(false);
        }
    }
    public void SaveGssUrl()
    {
        if (!string.IsNullOrEmpty(_keyField.text))
        {
            _runSaveGssKey = true;
        }
    }
    public void SaveGasUrl()
    {
        if (!string.IsNullOrEmpty(_keyField.text))
        {
            _runSaveGasKey = true;
        }
    }

    public void ResetGssUrl()
    {
        GssUrlManager.ResetUrl();
        UpdateKeyRelatedUI();
    }

    public void ResetGasUrl()
    {
        GasUrlManager.ResetUrl();
        UpdateKeyRelatedUI();
    }
}
