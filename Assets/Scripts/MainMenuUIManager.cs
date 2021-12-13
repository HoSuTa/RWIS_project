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
        if (_dbHub == null)
        {
            _dbHub = GetComponent<GssDbManageWrapper.GssDbHub>();
        }

        if (_runSaveGssKey)
        {
            if (!string.IsNullOrEmpty(_gssKeyField.text))
            {
                Debug.Log(_gssKeyField.text);
                _dbHub.IsGssKeyValid(_gssKeyField.text, () => GssKeyManager.SaveGssKey(_gssKeyField.text), UpdateGssKeyRelatedUI);
            }
            _runSaveGssKey = false;
        }
    }

    public void SaveGssKey()
    {
        if (!string.IsNullOrEmpty(_gssKeyField.text))
        {
            _runSaveGssKey = true;
        }
    }

    public void UpdateGssKeyRelatedUI()
    {
        if (GssKeyManager.IsGssUrlAssigned())
        {
            _savedGssKeyExistsText.text = "Saved GSS Key exists";
            _savedGssKeyExistsText.color = _plusColor;
            _savedGssKey.text = "\nSaved GSS Key:\n" + GssKeyManager.GetGssKey();
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
}
