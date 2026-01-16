using UnityEngine;
using System.Collections;
using TMPro;
public class PlayerMovement : MonoBehaviour
{
    private NodeManager nodeComponent; // NodeManager MonoBehaviour 참조
    public GameObject playerPrefab;
    public Player player;

    //Player Movement
    public Transform pointA;
    public Transform pointB;

    bool isMoving;

    private bool playerReady = false;
    public class Player
    {
        public int currentIndex;
        public GameObject playerObject;

        public Player(GameObject prefab)
        {
            currentIndex = 1;
            playerObject = prefab;
        }
    }

    void Start()
    {
        player = new Player(playerPrefab);
        nodeComponent = GetComponent<NodeManager>();

        if (nodeComponent != null && nodeComponent.root != null)
        {
            player.currentIndex = 1;
            player.playerObject.transform.position = nodeComponent.root.nodeObject.transform.position;
        }

    }
    
    public void Update()
    {
        if(nodeComponent.gameHasStarted==true&&playerReady==false)
        {
            PlayerReady();
            playerReady = true;

        }
        HandleMovementInput();
    }
 
    void PlayerReady()
    {
        player = new Player(playerPrefab);
        nodeComponent = GetComponent<NodeManager>();

        if (nodeComponent != null && nodeComponent.root != null)
        {
            player.currentIndex = 1;
            player.playerObject.transform.position = nodeComponent.root.nodeObject.transform.position;
        }
    }
    void HandleMovementInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MovePlayer(-1); // 왼쪽 이동
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MovePlayer(1); // 오른쪽 이동
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MovePlayer(0); // 뒤로 이동
        }
        else if (Input.GetKeyDown(KeyCode.Delete))
        {
            MovePlayer(4); // 뒤로 이동
        }
    }

    void MovePlayer(int direction)
    {
        if (isMoving) return;  // isMoving이 false일 때만 이동을 시작

        
        if (nodeComponent.nodeMap2.TryGetValue(player.currentIndex, out NodeManager.Node targetNode))
        {
            if(targetNode.left != null)
            {
                if (direction == -1)
                {
                    player.currentIndex = movingLeft(player.currentIndex);
                }

            }
            if (targetNode.right != null)
            {
                if (direction == 1)
                {
                    player.currentIndex = movingRight(player.currentIndex);
                }
            }
              

            if (direction == 0)
            {
                player.currentIndex = movingBack(player.currentIndex);
            }


            else if (direction == 4)
            {
                nodeDelete();
            }

        }
        MovePlayerToCurrentIndex();
        
    }

    void MovePlayerToCurrentIndex()
    {
     // PlayerAnimation 코루틴 실행
            if (nodeComponent.nodeMap2.TryGetValue(player.currentIndex, out NodeManager.Node targetNode))
            {
               
                    StartCoroutine(PlayerAnimation(targetNode));
                

            }
    }

    IEnumerator PlayerAnimation(NodeManager.Node targetNode)
    {
        float moveSpeed = 7f;
        float t = 0f;

        Vector2 pointA = player.playerObject.transform.position;
        Vector2 pointB = targetNode.nodeObject.transform.position;

        player.playerObject.transform.position = Vector2.Lerp(player.playerObject.transform.position, targetNode.nodeObject.transform.position, t);

        while (t < 1f)
        {
            isMoving = true;
            t += (moveSpeed * Time.deltaTime);
            player.playerObject.transform.position = Vector2.Lerp(pointA, pointB, t);

            yield return null;
        }
        player.playerObject.transform.position = pointB;
        isMoving = false;

        yield break;
    }
    int movingLeft(int currentIndex)
    {
        return currentIndex * 2;
    }

    int movingRight(int currentIndex)
    {
        return (currentIndex * 2) + 1;
    }

    int movingBack(int currentIndex)
    {
        if (currentIndex > 1)
        {
            if (currentIndex % 2 == 0)
                return currentIndex / 2;
            else
                return (currentIndex - 1) / 2;
        }
        return 1; // 루트 노드로 되돌아감
    }

    void nodeDelete()
    {
        nodeComponent.DeleteNode(player.currentIndex);
    }
}
