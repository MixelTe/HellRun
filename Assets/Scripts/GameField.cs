using System;
using System.Collections;
using UnityEngine;

public class GameField : MonoBehaviour
{ 
    [SerializeField] private int _width = 7;
    [SerializeField] private int _height = 6;

    [SerializeField] private CellLine _lineEmptyObjectPrefab;
    
    private CellLine[] _cellLines;
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
        
        _cellLines = new CellLine[_height];
        for (int y = _height, i = 0; y > 0; y--, i++)
            AddNewLine(y, i);
    }

    private void Update()
    {
        //For testing Stop and ContinueScrolling
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Scroling)
                StopScrolling();
            else
                ContinueScrolling();
        }
        
        if(Scroling){
            Scroll += ScrollSpeed * Time.deltaTime;
            if (Scroll > 1f)
            {
                Scroll--;
                ScrolledLines++;
                MoveTopRowToBottom();
            }
        }
    }

    private void AddNewLine(int y, int i)
    {
        Vector2 position;
        position.y = y;
        position.x = 0;

        CellLine lineGameObject = _cellLines[i] = Instantiate<CellLine>(_lineEmptyObjectPrefab, gameObject.transform);
        lineGameObject.CreateLine(y, _width);
        lineGameObject.transform.localPosition = position;
    }

    private void MoveTopRowToBottom()
    {
        int scroll = ScrolledLines;
        scroll--;
        Vector2 position;
        position.x = 0;
        position.y = -scroll;
        
        
        int i = scroll % _height;
        _cellLines[i].transform.position = position;
        
        print("MovedLine");
        OnLineMoved?.Invoke();
    }

    public bool IsInsideField(Vector2 playerPosition)
    {
        var field = new Rect(0, -ScrolledLines, _width, _height);
        return field.Contains(playerPosition);
    }
    
    public void StopScrolling()
    {
        _curScrollSpeed = ScrollSpeed;
        ScrollSpeed = 0;
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
        for (float i = 0; i < _curScrollSpeed; i += Time.deltaTime * .5f)
        {
            ScrollSpeed = i;
            yield return null;
        }

        ScrollSpeed = _curScrollSpeed;
    }
}