using System;
using System.Collections;
using UnityEngine;

public class GameField : MonoBehaviour
{
    [SerializeField] private GameObject _cellPrefab;

    [SerializeField] private float _accelerationTime = 1f; 

    private FieldRow[] _rows;
    private float _curScrollSpeed;

    public event Action OnScrollContinued;
    public event Action OnScrollStopped;
    public event Action OnLineMoved;
    
    [HideInInspector] public bool Scroling = true;
    [HideInInspector] public float Scroll = 0f;
    [HideInInspector] public int ScrolledLines = 0;
    public float ScrollSpeed = .1f;

    private void Start()
    {
        transform.DestroyAllChildren();
        _curScrollSpeed = ScrollSpeed;
        _rows = new FieldRow[Settings.Height];
        for (int i = 0; i < Settings.Height; i++)
            _rows[i] = AddNewLine(Settings.Height - i);
    }

    private void Update()
    {
        _curScrollSpeed = ScrollSpeed;
        if(Scroling){
            Scroll += _curScrollSpeed * Time.deltaTime;
            if (Scroll > 1f)
            {
                Scroll--;
                ScrolledLines++;
                MoveTopRowToBottom();
            }
        }
    }
    
    private FieldRow AddNewLine(int y)
    {
        var rowObj = new GameObject("Field Row");
        var row = rowObj.AddComponent<FieldRow>();
        row.transform.parent = gameObject.transform;
        row.Init(Settings.Width, y, _cellPrefab);
        return row;
    }

    private void MoveTopRowToBottom()
    {
        int i = (ScrolledLines - 1) % Settings.Height;
        _rows[i].MoveTo(-ScrolledLines+1);
        
        OnLineMoved?.Invoke();
    }

    public bool IsInsideField(Vector2 playerPosition)
    {
        var field = new Rect(0, -ScrolledLines+1, Settings.Width, Settings.Height);
        return field.Contains(playerPosition);
    }
    
    public void StopScrolling()
    {
        _curScrollSpeed = 0;
        Scroling = false;
        OnScrollStopped?.Invoke();
    }
    
    public void ContinueScrolling()
    {
        StartCoroutine(IncreasingSpeed());
        Scroling = true;
        OnScrollContinued?.Invoke();
    }

    private IEnumerator IncreasingSpeed()
    {
        for (float i = 0; i < 1; i += Time.deltaTime/_accelerationTime)
        {
            _curScrollSpeed = Mathf.Lerp(0, ScrollSpeed, i);
            yield return null;
        }

        _curScrollSpeed = ScrollSpeed;
    }

    [ContextMenu("GenerateField")]
    private void GenerateField()
	{
        for (int i = 0; i < Settings.Height; i++)
            AddNewLine(Settings.Height - i);
    }
    [ContextMenu("DestroyField")]
    private void DestroyFieldImmediate()
    {
		for (int i = transform.childCount - 1; i >= 0; i--)
		{
            var child = transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }
    }
}