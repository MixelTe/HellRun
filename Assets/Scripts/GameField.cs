using System;
using System.Collections;
using UnityEngine;

public class GameField : MonoBehaviour
{ 
    [SerializeField] private int _width = 7;
    [SerializeField] private int _height = 6;
    
    [SerializeField] private GameObject _cellPrefab;

    [SerializeField] private float _accelerationTime = 1f; 

    private FieldRow[] _rows;
    private float _curScrollSpeed;
    
    public static event Action ScrollContinued;
    public static event Action ScrollStopped;
    public static event Action OnLineMoved;
    
    [HideInInspector] public bool Scroling = true;
    [HideInInspector] public float Scroll = 0f;
    [HideInInspector] public int ScrolledLines = 0;
    public float ScrollSpeed = .1f;

    private void Start()
    {
        _curScrollSpeed = ScrollSpeed;
        _rows = new FieldRow[_height];
        for (int i = 0; i < _height; i++)
            _rows[i] = AddNewLine(_height - i);
    }

    private void Update()
    {
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
        var rowObj = new GameObject();
        var row = rowObj.AddComponent<FieldRow>();
        row.transform.parent = gameObject.transform;
        row.Init(_width, y, _cellPrefab);
        return row;
    }

    private void MoveTopRowToBottom()
    {
        int i = (ScrolledLines - 1) % _height;
        _rows[i].MoveTo(-ScrolledLines+1);
        
        OnLineMoved?.Invoke();
    }

    public bool IsInsideField(Vector2 playerPosition)
    {
        var field = new Rect(0, -ScrolledLines+1, _width, _height);
        return field.Contains(playerPosition);
    }
    
    public void StopScrolling()
    {
        _curScrollSpeed = 0;
        Scroling = false;
        ScrollStopped?.Invoke();
    }
    
    public void ContinueScrolling()
    {
        StartCoroutine(IncreasingSpeed());
        Scroling = true;
        ScrollContinued?.Invoke();
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
}