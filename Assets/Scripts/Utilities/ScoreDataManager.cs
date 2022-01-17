using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GssDbManageWrapper;
[RequireComponent(typeof(UserDataManager))]
public class ScoreDataManager : MonoBehaviour
{
    UserDataManager     _userDataManager;
    PolyLineDataManager _polyLineDataManager;
    private Transform   _textBGTransform;
    Dictionary<string, GameObject>  _scoreObjects;
    // Start is called before the first frame update
    void Start()
    {
        if (_userDataManager==null)
        {
            _userDataManager = GetComponent<UserDataManager>();
        }
        if (_polyLineDataManager==null)
        {
            _polyLineDataManager = GetComponent<PolyLineDataManager>();
        }
        if (_textBGTransform == null)
        {
            _textBGTransform = GameObject.Find("TEXT_BG").transform;
        }
        _scoreObjects = new Dictionary<string, GameObject>();
    }
    // Update is called once per frame
    void Update()
    {
        var userNameSet = new HashSet<string>();
        foreach(var userName in _userDataManager._userNames)
        {
            userNameSet.Add(userName);
        }
        var removeUserNames = new List<string>();
        {
            foreach(var userName in _scoreObjects.Keys)
            {
                if (!userNameSet.Contains(userName))
                {
                    removeUserNames.Add(userName);
                }
            }
            foreach(var userName in removeUserNames)
            {
                var scoreObject = _scoreObjects[userName];
                _scoreObjects.Remove(userName);
                Destroy(scoreObject);
            }
        }
        foreach(var userName in _userDataManager._userNames)
        {
            if (!_scoreObjects.ContainsKey(userName))
            {
                var userData    = _userDataManager.GetUserData(userName);
                var scoreObject = new GameObject();
                scoreObject.transform.SetParent(_textBGTransform);
                scoreObject.transform.name = "scoreBoard " + userName;
                scoreObject.transform.localScale = new Vector3(1, 1, 1);
                var scoreText   = scoreObject.AddComponent<Text>();
                scoreText.text  = "Score: 0";
                scoreText.font  = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                scoreText.fontSize = 20;
                scoreText.color = userData._color;
                scoreText.rectTransform.position  = new Vector3(0,0,0);
                scoreText.rectTransform.sizeDelta = new Vector2(200.0f, 20.0f);
                _scoreObjects.Add(userName, scoreObject);
            }
        }
    }
}
