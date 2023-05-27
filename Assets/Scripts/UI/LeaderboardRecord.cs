using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LeaderboardRecord : MonoBehaviour
{
    public LeaderboardDataRecord Data { get; private set; }
    [SerializeField] private Image _back;
    [SerializeField] private Color _playerColor;
    [SerializeField] private TMP_Text _rank;
    [SerializeField] private RawImage _image;
    [SerializeField] private Texture _imageDef;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _score;

    public void Init(LeaderboardDataRecord data)
	{
        Data = data;
        _rank.text = data.Rank.ToString();
        _name.text = data.Name;
        _score.text = data.Score.ToString();
        if (data.IsPlayer)
            _back.color = _playerColor;
        _image.texture = _imageDef;
        if (data.Avatar != "")
            StartCoroutine(Downloadlmage(data.Avatar));
    }

    public void SetRank(int rank)
	{
        _rank.text = rank.ToString();
    }

    public void SetScore(int score)
    {
        _score.text = score.ToString();
    }

    IEnumerator Downloadlmage(string mediaUri)
	{
        var request = UnityWebRequestTexture.GetTexture(mediaUri);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            Debug.Log(request.error);
        else
            _image.texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
	}
}
