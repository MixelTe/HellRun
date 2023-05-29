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
    [SerializeField] private Image _crown;
    [SerializeField] private Image _crownSmall;
    [SerializeField] private Image _crownSmallWreath;
    [SerializeField] private Image _wreath;
    [SerializeField] private Image _wreathSmall;
    [SerializeField] private Image _start;
    [SerializeField] private Image _startWreath;
    [SerializeField] private Image _gear;
    [SerializeField] private Texture _imageDef;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _score;
    [SerializeField] private TMP_Text _gamesPlayed;

    public void Init(LeaderboardDataRecord data)
	{
        Data = data;
        SetRank(data.Rank);
        SetScore(data.Score);
        _name.text = data.Name;
        _gamesPlayed.text = data.GamesPlayed.ToString();
        _gear.gameObject.SetActive(Data.HasGear);
        if (data.IsPlayer)
            _back.color = _playerColor;
        _image.texture = _imageDef;
        if (data.Avatar != "")
            StartCoroutine(Downloadlmage(data.Avatar));
    }

    public void SetRank(int rank)
	{
        _rank.text = rank.ToString();

        // Top 5
        _crown.gameObject.SetActive(rank == 1);
        _wreath.gameObject.SetActive(rank <= 5);
        
        _crownSmallWreath.gameObject.SetActive(rank > 1 && rank <= 5 && Data.WasFirst);
        _startWreath.gameObject.SetActive(rank <= 5 && Data.RatedGame);

        // Rest
        _crownSmall.gameObject.SetActive(rank > 5 && Data.WasFirst);
        _wreathSmall.gameObject.SetActive(rank > 5 && Data.WasTop && !Data.WasFirst);
        _start.gameObject.SetActive(rank > 5 && Data.RatedGame);
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
