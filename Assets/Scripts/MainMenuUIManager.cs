using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
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
    private GssKeyManager _gssKeyManager;

    private void Awake()
    {
        UpdateGssKeyRelatedUI();
    }

    public void UpdateGssKeyRelatedUI()
    {
        if (_gssKeyManager.IsGssKeyAssigned())
        {
            _savedGssKeyExistsText.text = "Saved GSS Key exists";
            _savedGssKeyExistsText.color = _plusColor;
            _savedGssKey.text = "\nSaved GSS Key:\n" + _gssKeyManager.GetGssKey();
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
