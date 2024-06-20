using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    UIController uIController;
    CountDownTImer countdownTimer;
    private Level level;
    private List<List<CellItem>> playground;

    [SerializeField] private GameObject framePrefab;
    [SerializeField] private GameObject itemPrefab;

    private Vector3 prevInput;
    private Vector3 startInput;

    private Swipe swipeControls;
    private Vector2Int lastMove;

    private bool isCompleteLevel = false;

    private GameObject holder;

    public Vector2Int LastMove
    {
        get { return lastMove; }
    }


    private void Awake()
    {
        swipeControls = gameObject.AddComponent<Swipe>();
        uIController = GetComponent<UIController>();
        countdownTimer = GetComponent<CountDownTImer>();
    }


    public void StartGame(Level level)
    {
        this.level = level;
        uIController.ShowUI(uIController.inGameUI);
        CreateGrid(level);
        countdownTimer.StartCountdownTimer();
    }

    public void Restart()
    {
        RemoveLevel();
        CreateGrid(level);
        uIController.ShowUI(uIController.inGameUI);
        countdownTimer.StartCountdownTimer();
    }

    public void CreateGrid(Level level)
    {
        holder = new GameObject("Holder");
        playground= new List<List<CellItem>>();

        int width = level.width;
        int height = level.height;
        

        for (int i = 0; i < width; i++)
        {
            List<CellItem> currentLine = new List<CellItem>();
            for (int j = 0; j < height; j++)
            {
                var newCellItem = SpawnObject(itemPrefab).GetComponent<CellItem>();
                var itemData = level.listItemType[i * height + j];

                newCellItem.SetItem(itemData);
                newCellItem.transform.localPosition = new Vector3(i * 1.93f, j * 1.93f - 3.5f);
                newCellItem.transform.name = "Cell Item" + "(" + i + "," + j + ")";
                newCellItem.SetCoordinates(i, j);
                newCellItem.transform.parent = holder.transform;

                currentLine.Add(newCellItem);
                var frameObj = SpawnObject(framePrefab);
                frameObj.transform.localPosition = new Vector3(i * 1.93f, j * 1.93f - 3.5f);
                frameObj.transform.parent = holder.transform;
            }
            playground.Add(currentLine);
        }
    }

    public GameObject SpawnObject(GameObject prefab)
    {

        return Instantiate(prefab);
    }

    #region Touch and Move
    void Update()
    {
        if (swipeControls.SwipeRight)
        {
            OnSwipe(Vector2Int.right);
        }
        else if (swipeControls.SwipeLeft)
        {
            OnSwipe(Vector2Int.left);
        }
        else if (swipeControls.SwipeTop)
        {
            OnSwipe(Vector2Int.up);
        }
        else if (swipeControls.SwipeBottom)
        {
            OnSwipe(Vector2Int.down);
        }

//#if UNITY_EDITOR

//        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
//        {
//            OnSwipe(Vector2Int.up);
//        }
//        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
//        {
//            OnSwipe(Vector2Int.right);
//        }
//        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
//        {
//            OnSwipe(Vector2Int.down);
//        }
//        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
//        {
//            OnSwipe(Vector2Int.left);
//        }
//#endif
    }

    private void ResetStartTouchPostion()
    {
        startInput = Input.mousePosition;
        prevInput = Input.mousePosition;
    }

    private void OnSwipe(Vector2Int direction)
    {
        ResetStartTouchPostion();
        StartCoroutine(Move(direction));
    }

    IEnumerator Move(Vector2Int direction)
    {
        lastMove = direction;

        int lineAmount = GetLinesAmount(direction);
        int itemsInLine = GetItemsInLineAmount(direction);

        for (int line = 0; line < lineAmount; line++)
        {
            int pathLength = 0;
            ItemType prevItem = ItemType.None;
            for (int itemOnLine = 0; itemOnLine < itemsInLine; itemOnLine++)
            {
                CellItem currentItem = GetCell(direction, line, itemOnLine);
                
                if (currentItem.ItemType == ItemType.Candy)
                {
                    pathLength = 0;
                }
                
                if (pathLength> 0 && currentItem.ItemType != ItemType.None)
                {
                    if (prevItem != ItemType.None && prevItem != ItemType.GiftBox)
                    {
                        pathLength--;
                    }

                    if (currentItem.ItemType != ItemType.None)
                    {
                        prevItem = currentItem.ItemType;
                    }

                    CellItem finalItem = GetFinalCellOnPath(direction, line, itemOnLine, pathLength);
                    if (finalItem.ItemType == ItemType.GiftBox && currentItem.ItemType == ItemType.Cake && direction != Vector2Int.down)
                    {
                        finalItem = GetFinalCellOnPath(direction, line, itemOnLine, pathLength -= 1);
                    }
                    Debug.Log(finalItem);

                    MoveCell(new Vector2Int(currentItem.ColumnIndex, currentItem.RowIndex), finalItem, direction);
                    
                }
                else
                {
                    if (currentItem.ItemType != ItemType.None)
                    {
                        prevItem = currentItem.ItemType;
                    }
                }

                pathLength++;
            }
        }

        yield return new WaitForSeconds(0.04f);

        CheckForLevelComplete();
    }

    private int GetLinesAmount(Vector2Int direction)
    {
        if (direction == Vector2Int.up || direction == Vector2Int.down)
        {
            return level.width;
        }
        else
        {
            return level.height;
        }
    }

 
    private int GetItemsInLineAmount(Vector2Int direction)
    {
        if (direction == Vector2Int.up || direction == Vector2Int.down)
        {
            return level.width;
        }
        else
        {
            return level.height;
        }
    }

    private CellItem GetCell(Vector2Int direction, int line, int item)
    {
        if (direction == Vector2Int.up || direction == Vector2Int.down)
        {
            return playground[line][direction.y == -1 ? item : level.height - 1 - item];
        }
        else
        {
            return playground[direction.x == -1 ? item : level.width - 1 - item][line];
        }
    }

    private CellItem GetFinalCellOnPath(Vector2Int direction, int line, int item, int pathLength)
    {
        if (direction == Vector2Int.up || direction == Vector2Int.down)
        {
            return playground[line][direction.y == -1 ? item - pathLength : level.height - 1 - item + pathLength];
        }
        else
        {
            return playground[direction.x == -1 ? item - pathLength : level.width - 1 - item + pathLength][line];
        }
    }

    private void MoveCell(Vector2Int cellIndex, CellItem finalItem, Vector2Int direction)
    {
        
        if (finalItem.ItemType == ItemType.GiftBox && direction == Vector2Int.down)
        {
            playground[cellIndex.x][cellIndex.y].Move(finalItem);
            if (playground[cellIndex.x][cellIndex.y].transform.position == finalItem.transform.position)
            {
                Debug.Log("Win");
                isCompleteLevel = true;
            }
        }
        else if (finalItem.ItemType != ItemType.GiftBox)
        {
            var temp = finalItem.ItemType;
            finalItem.ItemType = playground[cellIndex.x][cellIndex.y].ItemType;
            playground[cellIndex.x][cellIndex.y].ItemType = temp;
            finalItem.SetView();
            playground[cellIndex.x][cellIndex.y].SetView();
        }
    }

    private void CheckForLevelComplete()
    {
        if (isCompleteLevel)
        {
            OnLevelComplete();
        }

    }

    public void OnLevelComplete()
    {
        RemoveLevel();
        uIController.ShowUI(uIController.levelCompleteUI);
    }

    public void RemoveLevel()
    {
        Destroy(holder);
    }
    
    #endregion

}
