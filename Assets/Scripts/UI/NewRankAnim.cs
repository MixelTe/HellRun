using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRankAnim : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private Animator _animator;
    [SerializeField] private LeaderboardRecord _recordNext1;
    [SerializeField] private LeaderboardRecord _recordNext2;
    [SerializeField] private LeaderboardRecord _recordPlayer;
    [SerializeField] private LeaderboardRecord _recordPast1;
    [SerializeField] private LeaderboardRecord _recordPast2;
    [SerializeField] private LeaderboardRecord _recordPast3;

    private bool _showed = false;
    private int _pastRank;
    private int _rank;

    private void Awake()
	{
        if (!_showed)
            gameObject.SetActive(false);
	}

	public void Show(NewRankAnimData data)
    {
        _showed = true;
        gameObject.SetActive(true);

        if (data.RecordNext1 == null) _recordNext1.gameObject.SetActive(false); 
        else _recordNext1.Init(data.RecordNext1);

        if (data.RecordNext2 == null) _recordNext2.gameObject.SetActive(false); 
        else _recordNext2.Init(data.RecordNext2);

        if (data.RecordPlayer == null) _recordPlayer.gameObject.SetActive(false);
        else
        {
            _rank = data.RecordPlayer.Rank;
            _pastRank = data.PastRank;
            _recordPlayer.Init(data.RecordPlayer);
            _recordPlayer.SetRank(data.PastRank);
        }

        if (data.RecordPast1 == null) _recordPast1.gameObject.SetActive(false); 
        else _recordPast1.Init(data.RecordPast1);

        if (data.RecordPast2 == null) _recordPast2.gameObject.SetActive(false); 
        else _recordPast2.Init(data.RecordPast2);

        if (data.RecordPast3 == null) _recordPast3.gameObject.SetActive(false); 
        else _recordPast3.Init(data.RecordPast3);

        _animator.SetTrigger("Start");
    }

    public void AnimStart(float time)
    {
        _recordPlayer.SetRank(_pastRank);
        StartCoroutine(AnimScore(_recordPlayer, time));
    }

    public void AnimEvent()
	{
        _recordPlayer.SetRank(_rank);
    }

    public void AnimParticles()
    {
        _particles.Play();
    }

    public void AnimEnd()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator AnimScore(LeaderboardRecord record, float time)
	{
        var score = record.Data.Score;
        record.SetScore(0);
		for (float t = 0; t < 1; t += Time.deltaTime / time)
		{
            record.SetScore(Mathf.FloorToInt(Mathf.Lerp(0, score, t)));
            yield return new WaitForEndOfFrame();
		}
        record.SetScore(score);
    }
}

public struct NewRankAnimData
{
	public LeaderboardDataRecord RecordNext1;
	public LeaderboardDataRecord RecordNext2;
	public LeaderboardDataRecord RecordPlayer;
	public LeaderboardDataRecord RecordPast1;
	public LeaderboardDataRecord RecordPast2;
	public LeaderboardDataRecord RecordPast3;
    public int PastRank;

	public NewRankAnimData(LeaderboardDataRecord recordNext1, LeaderboardDataRecord recordNext2, LeaderboardDataRecord recordPlayer, LeaderboardDataRecord recordPast1, LeaderboardDataRecord recordPast2, LeaderboardDataRecord recordPast3, int pastRank)
	{
		RecordNext1 = recordNext1;
		RecordNext2 = recordNext2;
		RecordPlayer = recordPlayer;
		RecordPast1 = recordPast1;
		RecordPast2 = recordPast2;
		RecordPast3 = recordPast3;
		PastRank = pastRank;
	}
}
