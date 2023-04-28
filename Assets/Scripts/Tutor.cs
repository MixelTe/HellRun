using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tutor : MonoBehaviour
{
    [SerializeField] private Animator _controlsMobile1;
    [SerializeField] private Animator _controlsMobile2;
    [SerializeField] private Animator _controlsMobile3;
    [SerializeField] private GameObject _controlsDesktop;
    [SerializeField] private Notify _hint;
    [SerializeField] private TMP_Text _hintText;
    [SerializeField, TextArea] private string _hintTextDesktop;
    [SerializeField, TextArea] private string _hintTextMobile;
    [SerializeField] private float _hintShowTime;
    private bool _hidden = false;

    private void Start()
    {
        var mobile = YaApi.Mobile();
        _controlsDesktop.SetActive(!mobile);

		GameManager.PlayerInput.OnMoved += HideTutor;

        if (mobile) _hintText.text = _hintTextMobile;
        else _hintText.text = _hintTextDesktop;

        StartCoroutine(ShowHint());
    }

	private void HideTutor(Vector2Int _)
	{
		GameManager.PlayerInput.OnMoved -= HideTutor;
        _hidden = true;
        _controlsMobile1.SetTrigger("Disable");
        _controlsMobile2.SetTrigger("Disable");
        _controlsMobile3.SetTrigger("Disable");
        _controlsDesktop.SetActive(false);
        _hint.gameObject.SetActive(false);
    }

    private IEnumerator ShowHint()
	{
        yield return new WaitForSeconds(_hintShowTime);
        if (!_hidden)
            _hint.Show(false);
	}
}
