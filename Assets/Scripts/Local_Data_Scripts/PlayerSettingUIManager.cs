using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GssDbManageWrapper;

[RequireComponent(typeof(GssDbHub))]
public class PlayerSettingUIManager : MonoBehaviour
{
    [SerializeField]
    public InputField _playerNameField;
    [SerializeField]
    private Text _playerNameVisualizeText;

    [SerializeField]
    private Text _playerValidation;
    [SerializeField]
    private GameObject _playerValidationBG;

    [SerializeField]
    private GameObject _nextSceneButton;

    [SerializeField]
    private Color _plusColor;
    [SerializeField]
    private Color _minusColor;

    private GssDbHub _gssDbHub;
    private bool _playerNameValidanceCheck = false;
    private bool _runValidation = false;

    private void Awake()
    {
        if (_gssDbHub == null) _gssDbHub = GetComponent<GssDbHub>();

        _playerNameField.text = "";
        _playerValidationBG.SetActive(false);
        //_nextSceneButton.SetActive(false);
    }

    private void Update()
    {
        /*
        if(_runValidation)
        {
            _gssDbHub.CheckIfPlayerNameValid(
                _playerNameField.text, UpdatePlayerNameRelatedUI);
            _runValidation = false;
        }


        if(_playerNameValidanceCheck)
        {
            StartCoroutine(PlayerNameValidationUIEffect());
            _playerNameValidanceCheck = false;
        }*/
    }


    IEnumerator PlayerNameValidationUIEffect()
    {
        yield return new WaitForSeconds(3.0f);
        _playerValidation.text = "";
        _playerValidationBG.SetActive(false);
    }

    private void UpdatePlayerNameRelatedUI(bool isPlayerNameValid, string playerName)
    {
        if (isPlayerNameValid)
        {
            _playerNameVisualizeText.text = "\n" + playerName;
            _nextSceneButton.SetActive(true);

            _playerValidationBG.SetActive(true);
            _playerValidation.text = $"The Player Name: {playerName} is valid";
            _playerValidation.color = _plusColor;
            _playerNameValidanceCheck = true;
        }
        else
        {
            if (_playerNameField.text == "")
            {
                _playerNameVisualizeText.text = "\nNo Player Name is set.";
            }

            _playerValidationBG.SetActive(true);
            _playerValidation.text = $"The Player Name: {playerName} is inValid";
            _playerValidation.color = _minusColor;
            _playerNameValidanceCheck = true;
        }
    }

    public void RunPlayerNameValidation()
    {
        if (!string.IsNullOrEmpty(_playerNameField.text))
        {
            _runValidation = true;
        }
    }
}
