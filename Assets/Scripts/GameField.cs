using System;
using UnityEngine;

public class GameField : MonoBehaviour
{ 
    [SerializeField] private int _width = 7;
    [SerializeField] private int _height = 6;

    [SerializeField] private GameObject _cellPrefab;
    [SerializeField] private CellLine _lineEmptyObjectPrefab;
    
    private CellLine[] _cellLines;
    private float _scrollSpeedConst;

    private Vector2 _leftTopPointOfField;
    private Vector2 _rightBottomPointOfField;

    public static event Action ScrollStateHasContinued;
    public static event Action ScrollStateHasStopped;
    public static event Action OnLineMoved;
    
    [HideInInspector] public bool Scroling = true;
    [HideInInspector] public float Scroll = 0f;
    [HideInInspector] public int ScrollCount = 0;
    public float ScrollSpeed = .1f;

    private void Start()
    {
        _leftTopPointOfField.y = _height;
        _leftTopPointOfField.x = 0;
        _rightBottomPointOfField.y = 0;
        _rightBottomPointOfField.x = _width;
        
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
                ScrollCount++;
                MoveLine(ScrollCount);
            }
        }
    }

    private void AddNewLine(int y, int i)
    {
        Vector2 position;
        position.y = y;
        position.x = 0;

        CellLine lineGameObject = _cellLines[i] = Instantiate<CellLine>(_lineEmptyObjectPrefab, gameObject.transform);
        lineGameObject.CreateLine(y, _width, _cellPrefab);
        lineGameObject.transform.localPosition = position;
    }

    private void MoveLine(int scroll)
    {
        scroll--;
        Vector2 position;
        position.x = 0;
        position.y = -scroll;
        
        _leftTopPointOfField.y--;
        _rightBottomPointOfField.y--;
        
        int i = scroll % _height;
        _cellLines[i].transform.position = position;
        
        print("MovedLine");
        OnLineMoved?.Invoke();
    }

    public bool IsInsideField(Vector2 playerPosition)
    {
        if (_leftTopPointOfField.y > playerPosition.y && playerPosition.y > _rightBottomPointOfField.y &&
            _leftTopPointOfField.x < playerPosition.x && playerPosition.x < _rightBottomPointOfField.x)
            return true;
        return false;
    }
    
    public void StopScrolling()
    {
        _scrollSpeedConst = ScrollSpeed;
        ScrollSpeed = 0;
        Scroling = false;
        ScrollStateHasStopped?.Invoke();
    }
    
    public void ContinueScrolling()
    {
        ScrollSpeed = _scrollSpeedConst;
        Scroling = true;
        ScrollStateHasContinued?.Invoke();
    }
}