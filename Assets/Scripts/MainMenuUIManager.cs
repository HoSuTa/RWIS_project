using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField]
    private InputField _gssKeyField;
    [SerializeField]
    private InputField _gasKeyField;
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
        UpdateGssKeyRelatedUI();
    }

    private void Update()
    {
        if (_runSaveGssKey)
        {
            if (!string.IsNullOrEmpty(_gssKeyField.text))
            {
                _dbHub.CheckIfGssUrlValid(_gssKeyField.text, () => GssUrlManager.SaveGssUrl(_gssKeyField.text), UpdateGssKeyRelatedUI);
            }
            _runSaveGssKey = false;
        }

        if (_runSaveGasKey)
        {
            if (!string.IsNullOrEmpty(_gasKeyField.text))
            {
                //_dbHub.CheckIfGssUrlValid(_gasKeyField.text, () => GasUrlManager.SaveGasUrl(_gasKeyField.text), UpdateGssKeyRelatedUI);
            }
            _runSaveGasKey = false;
        }
    }



    public void UpdateGssKeyRelatedUI()
    {
        if (GssUrlManager.IsGssUrlAssigned())
        {
            _savedGssKeyExistsText.text = "Saved GSS Key exists";
            _savedGssKeyExistsText.color = _plusColor;
            _savedGssKey.text = "\nSaved GSS Key:\n" + GssUrlManager.GetGssUrl();
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
        if (!string.IsNullOrEmpty(_gssKeyField.text))
        {
            _runSaveGssKey = true;
        }
    }
    public void SaveGasUrl()
    {
        if (!string.IsNullOrEmpty(_gasKeyField.text))
        {
            _runSaveGasKey = true;
        }
    }

    public void ResetGssUrl()
    {
        GssUrlManager.ResetGssUrl();
        UpdateGssKeyRelatedUI();
    }

    public void ResetGasUrl()
    {
        GasUrlManager.ResetGasUrl();
        UpdateGssKeyRelatedUI();
    }
}
