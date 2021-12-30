using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GssDbManageWrapper.GssDbHub))]
public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField]
    private InputField _keyField;
    [SerializeField]
    private Text _keyValidationText;
    [SerializeField]
    private GameObject _keyValidationBG;
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
    private GssDbManageWrapper.GssDbHub _gssDbHub;

    private List<string> _keyList = new List<string>() { "Edit Gss Url", "Edit Gas Url" };



    private bool _runSaveKey = false;
    private bool _runResetKey = false;
    private bool _runUpdateUI = false;
    private bool _isKeyInvalid = false;

    private void Awake()
    {
        if (_gssDbHub == null)
        {
            _gssDbHub = GetComponent<GssDbManageWrapper.GssDbHub>();
        }

        _keyValidationBG.SetActive(false);
        _keyValidationText.text = "";

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
        if (_isKeyInvalid)
        {
            StartCoroutine(KeyValidationUIEffect());
            _isKeyInvalid = false;
        }
    }

    private void DecideEdittingKey(Dropdown keySelectDropDown)
    {
        if (!_runUpdateUI) return;

        if (keySelectDropDown.options[keySelectDropDown.value].text == _keyList[0])
        {
            if (_runSaveKey)
            {
                SaveKey(
                    _gssDbHub.CheckIfGssUrlValid,
                    GssUrlManager.SaveUrl,
                    () => UpdateKeyRelatedUI(GssUrlManager.IsUrlAssigned, GssUrlManager.GetUrl)
                    );
                _runSaveKey = false;

            }
            if (_runResetKey)
            {
                ResetKey(
                    GssUrlManager.ResetUrl,
                    () => UpdateKeyRelatedUI(GssUrlManager.IsUrlAssigned, GssUrlManager.GetUrl)
                    );
                _runResetKey = false;

            }
            UpdateKeyRelatedUI(GssUrlManager.IsUrlAssigned, GssUrlManager.GetUrl);

        }
        else if (keySelectDropDown.options[keySelectDropDown.value].text == _keyList[1])
        {
            if (_runSaveKey)
            {
                SaveKey(
                    _gssDbHub.CheckIfGasUrlValid,
                    GasUrlManager.SaveUrl,
                    () => UpdateKeyRelatedUI(GasUrlManager.IsUrlAssigned, GasUrlManager.GetUrl)
                    );
                _runSaveKey = false;
            }
            if (_runResetKey)
            {
                ResetKey(
                    GasUrlManager.ResetUrl,
                    () => UpdateKeyRelatedUI(GasUrlManager.IsUrlAssigned, GasUrlManager.GetUrl)
                    );
                _runResetKey = false;

            }
            UpdateKeyRelatedUI(GasUrlManager.IsUrlAssigned, GasUrlManager.GetUrl);
        }

        _runUpdateUI = false;
    }

    IEnumerator KeyValidationUIEffect()
    {
        yield return new WaitForSeconds(3.0f);
        _keyValidationText.text = "";
        _keyValidationBG.SetActive(false);

    }

    private void KeyValidFeedBack()
    {
        _keyValidationBG.SetActive(true);
        _keyValidationText.text = "The key is valid";
        _keyValidationText.color = _plusColor;
        _isKeyInvalid = true;
    }

    private void KeyInvalidFeedBack()
    {
        _keyValidationBG.SetActive(true);
        _keyValidationText.text = "The key is invalid";
        _keyValidationText.color = _minusColor;
        _isKeyInvalid = true;
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
            RunUpdateUI();
            _runSaveKey = true;
        }
    }

    private void SaveKey(Action<string, Action, Action, Action, Action> checkIfKeyIsValid, Action<string> saveKey, Action updateUI)
    {
        if (!string.IsNullOrEmpty(_keyField.text))
        {
            checkIfKeyIsValid
            (
                _keyField.text,
                () => saveKey(_keyField.text),
                updateUI,
                KeyValidFeedBack,
                KeyInvalidFeedBack
            );
        }
    }

    public void RunResetKey()
    {
        RunUpdateUI();
        _runResetKey = true;
    }

    private void ResetKey(Action resetKey, Action resetUI)
    {
        resetKey();
        resetUI();
    }

}
