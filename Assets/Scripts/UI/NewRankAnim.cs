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

	public void Show(LeaderboardDataRecord recordNext1,
		LeaderboardDataRecord recordNext2,
		LeaderboardDataRecord recordPlayer,
		LeaderboardDataRecord recordPast1,
		LeaderboardDataRecord recordPast2,
		LeaderboardDataRecord recordPast3,
        int pastRank)
    {
        _showed = true;
        gameObject.SetActive(true);

        if (recordNext1 == null) _recordNext1.gameObject.SetActive(false); 
        else _recordNext1.Init(recordNext1);

        if (recordNext2 == null) _recordNext2.gameObject.SetActive(false); 
        else _recordNext2.Init(recordNext2);

        if (recordPlayer == null) _recordPlayer.gameObject.SetActive(false);
        else
        {
            _rank = recordPlayer.Rank;
            _pastRank = pastRank;
            _recordPlayer.Init(recordPlayer);
            _recordPlayer.SetRank(pastRank);
        }

        if (recordPast1 == null) _recordPast1.gameObject.SetActive(false); 
        else _recordPast1.Init(recordPast1);

        if (recordPast2 == null) _recordPast2.gameObject.SetActive(false); 
        else _recordPast2.Init(recordPast2);

        if (recordPast3 == null) _recordPast3.gameObject.SetActive(false); 
        else _recordPast3.Init(recordPast3);

        _animator.SetTrigger("Start");
    }

    public void AnimStart(float time)
    {
        _particles.Play();
        _recordPlayer.SetRank(_pastRank);
        StartCoroutine(AnimScore(_recordPlayer, time));
    }

    public void AnimEvent()
	{
        _recordPlayer.SetRank(_rank);
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
