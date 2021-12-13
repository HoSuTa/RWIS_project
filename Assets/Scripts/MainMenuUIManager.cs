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
    private Text _isKeyValidText;
    [SerializeField]
    private Text _savedKeyExistsText;
    [SerializeField]
    private Color _plusColor;
    [SerializeField]
    private Color _minusColor;
    [SerializeField]
    private Text _savedKey;
    [SerializeField]
    private GameObject _playButton;
    [SerializeField]
    private Dropdown _keySelectDropDown;
    [SerializeField]
    private GssDbManageWrapper.GssDbHub _dbHub;

    private List<string> _keyList = new List<string>() { "Edit Gss Url", "Edit Gas Url" };



    private bool _runSaveKey = false;
    private bool _runResetKey = false;
    private bool _runUpdateUI = false;

    private void Awake()
    {
        if (_dbHub == null)
        {
            _dbHub = GetComponent<GssDbManageWrapper.GssDbHub>();
        }
        _isKeyValidText.text = "";

        _keySelectDropDown.ClearOptions();
        _keySelectDropDown.AddOptions(_keyList);

        if (_keySelectDropDown.options[_keySelectDropDown.value].text == _keyList[0])
        {
            UpdateKeyRelatedUI(GssUrlManager.IsUrlAssigned, GssUrlManager.GetUrl);
        }
        else if (_keySelectDropDown.options[_keySelectDropDown.value].text == _keyList[1])
        {
            UpdateKeyRelatedUI(GasUrlManager.IsUrlAssigned, GasUrlManager.GetUrl);
        }
    }

    private void Update()
    {
        DecideEdittingKey(_keySelectDropDown);

        if (_runUpdateUI)
        {
            if (_keySelectDropDown.options[_keySelectDropDown.value].text == _keyList[0])
            {
                UpdateKeyRelatedUI(GssUrlManager.IsUrlAssigned, GssUrlManager.GetUrl);
            }
            else if (_keySelectDropDown.options[_keySelectDropDown.value].text == _keyList[1])
            {
                UpdateKeyRelatedUI(GasUrlManager.IsUrlAssigned, GasUrlManager.GetUrl);
            }
            _runUpdateUI = false;
        }
    }

    private void DecideEdittingKey(Dropdown keySelectDropDown)
    {
        if (keySelectDropDown.options[keySelectDropDown.value].text == _keyList[0])
        {
            if (_runSaveKey)
            {
                SaveKey(
                    _dbHub.CheckIfGssUrlValid,
                    GssUrlManager.SaveUrl,
                    () => UpdateKeyRelatedUI(GssUrlManager.IsUrlAssigned, GssUrlManager.GetUrl)
                    );
            }
            if (_runResetKey)
            {
                ResetKey(
                    GssUrlManager.ResetUrl,
                    () => UpdateKeyRelatedUI(GssUrlManager.IsUrlAssigned, GssUrlManager.GetUrl)
                    );
            }
        }
        else if (keySelectDropDown.options[keySelectDropDown.value].text == _keyList[1])
        {
            if (_runSaveKey)
            {
                SaveKey(
                    _dbHub.CheckIfGasUrlValid,
                    GasUrlManager.SaveUrl,
                    () => UpdateKeyRelatedUI(GasUrlManager.IsUrlAssigned, GasUrlManager.GetUrl)
                    );
            }
            if (_runResetKey)
            {
                ResetKey(
                    GasUrlManager.ResetUrl,
                    () => UpdateKeyRelatedUI(GasUrlManager.IsUrlAssigned, GasUrlManager.GetUrl)
                    );
            }
        }
    }

    private void KeyNotValidFeedBack()
    {
        _isKeyValidText.text = "The key was invalid";
    }

    public void RunUpdateUI()
    {
        _runUpdateUI = true;
    }

    private void UpdateKeyRelatedUI(Func<bool> isKeyAssigned, Func<string> getKey)
    {
        if (isKeyAssigned())
        {
            _savedKeyExistsText.text = "Saved Key exists";
            _savedKeyExistsText.color = _plusColor;
            _savedKey.text = "\nSaved Key:\n" + getKey();
            _isKeyValidText.text = "";
        }
        else
        {
            _savedKeyExistsText.text = "Saved Key does not exist.";
            _savedKeyExistsText.color = _minusColor;
            _savedKey.text = "\nSaved Key does not exist.";
        }

        if (GssUrlManager.IsUrlAssigned() && GasUrlManager.IsUrlAssigned())
        {
            _playButton.SetActive(true);
        }
        else
        {
            _playButton.SetActive(false);
        }
    }

    public void RunSaveKey()
    {
        if (!string.IsNullOrEmpty(_keyField.text))
        {
            _runSaveKey = true;
        }
    }

    private void SaveKey(Action<string, Action, Action, Action> checkIfKeyIsValid, Action<string> saveKey, Action updateUI)
    {
        if (!string.IsNullOrEmpty(_keyField.text))
        {
            checkIfKeyIsValid
            (
                _keyField.text,
                () => saveKey(_keyField.text),
                updateUI,
                KeyNotValidFeedBack
            );
        }
        _runSaveKey = false;
    }

    public void RunResetKey()
    {
        _runResetKey = true;
    }

    private void ResetKey(Action resetKey, Action resetUI)
    {
        resetKey();
        resetUI();
        _runResetKey = false;
    }

}
