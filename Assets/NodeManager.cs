using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;


class NodeManager: MonoBehaviour
{
    public GameManager gameManager;

    public PlayerMovement playerMovement;
    public GameObject refToEngine;
    public GameObject nodePrefab,
    linePrefab, enemyPrefab;

    public int nodeLevel=0;

    public GameObject refToAskedNode;

    public Node root; // 트리의 루트 노드
    float nodeSpacing = 2.0f; // 노드 간 간격 (X축, Y축 기준)
    float nodeHeight;

    public bool gameHasStarted = false;
    //적 참고용


    public Transform objectChild;
    public Transform objectParent;

    private Rigidbody rb;

    //적 material

    public Material refToScoreShader;
    public Material refToGold;

    //적 speed

    public float speedA=0.3f;

    // 하트 참고
    public Sprite refToHeart;

    //노드 클래스 
    public class Node
    {
        public Transform position;
        public int index;
        public Node left; // 왼쪽 자식
        public Node right; // 오른쪽 자식
        public GameObject nodeObject; // 이 노드와 연결된 GameObject
        public LineRenderer leftLine;
        public LineRenderer rightLine;
        public int experiment;
        // 생성자
        public Node(int index, GameObject nodeObject)
        {
            this.index = index;
            this.nodeObject = nodeObject;
            this.left = null;
            this.right = null;
            this.leftLine = null;
            this.rightLine = null;
        }
    }

    void Start()
    {
        
        // 초기 트리 생성 (루트 노드를 화면 중앙에 배치)
        playerMovement = GetComponent<PlayerMovement>();
        gameManager = GetComponent<GameManager>();

        rb = GetComponent<Rigidbody>();

    }

    private void Update()
    {
        if (gameManager.isGameRunning == true&&gameHasStarted==false)
        {
            gameHasStarted = true;
            gameStart();
        }
    }

    void gameStart()
    {
        root = CreateNode(1, new Vector3(0, -4.5f, 0));
        SplitTree(root, nodeSpacing, 2.3f);
        SplitTree(root, nodeSpacing, 2.3f);
    }
    // Update is called once per frame


    public void GStart()
    {
         SplitTree(root, nodeSpacing, 2.3f);
         SplitTree(root, nodeSpacing, 2.3f); 
    }
    public void EnemyRespone()
    {
        SplitTree(root, nodeSpacing, 2.3f);
        SplitTree(root, nodeSpacing, 2.3f);
        SplitTree(root, nodeSpacing, 2.3f);
    }

    private Dictionary<int, Node> deletedNodes = new Dictionary<int, Node>();
    public Dictionary<int, GameObject> nodeMap = new Dictionary<int, GameObject>();
    public Dictionary<int, Node> nodeMap2 = new Dictionary<int, Node>();


    Dictionary<int,GameObject> enemyMap = new Dictionary<int, GameObject>();
    Node CreateNode(int index, Vector2 position)
    {
        
        GameObject nodeObject = Instantiate(nodePrefab, position, Quaternion.identity);
        nodeObject.name = $"Node {index}";

        Node newNode = new Node(index, nodeObject);
        nodeMap[index] = newNode.nodeObject;//플레이어 위치 참고용
        nodeMap2[index] = newNode;// 노드 삭제용 + 노드 거리유지용
     
        return newNode;
    }
    



    // 트리를 분열시키는 메서드
    (float, float) IndexSpaing(int index)
    {
        if(index == 1) return (4.4f, 1.1f);
        if (index >= 2 && index <= 3) return (2.2f, 1.3f);
        if (index >= 4 && index <= 7) return (1.1f, 1.5f);
        if (index >= 8 && index <= 15) return (0.6f, 1.8f);
        if (index >= 16 && index <= 31) return (0.28f, 1.8f);
        if (index >= 32 && index <= 63) return (0.1f, 1.9f);
        if (index >= 64 && index <= 127) return (0, 0);
        if (index >= 128 && index <= 255) return (0, 0);

       
        return (0, 0);
    }
    void SplitTree(Node node, float spacing, float depth)
    {
        if (node == null || node.index >= 64) return;


            (float horizentalSpace, float nodeHeight) = IndexSpaing(node.index);

        int randomInt = UnityEngine.Random.Range(0, 2);

        if(randomInt == 0)
        {
            // 왼쪽 자식이 없으면 생성
            if (node.left == null)
            {
                Vector2 leftPosition = (Vector2)node.nodeObject.transform.position + new Vector2(-horizentalSpace, nodeHeight);
                node.left = CreateNode(node.index * 2, leftPosition);
                node.leftLine = CreateLine(node.nodeObject.transform.position, node.left.nodeObject.transform.position);


                //적의 위치생성

                CreateEnemy(node.index * 2, node.left.nodeObject.transform.position);
            }
            else
            {
                // 왼쪽 자식이 이미 있다면 재귀적으로 분열
                SplitTree(node.left, spacing, depth * 2);
               // DestroyEnemy(node.index * 2);
            }
        }

        if (randomInt == 1)
        {
            // 오른쪽 자식이 없으면 생성
            if (node.right == null)
            {
                Vector2 rightPosition = (Vector2)node.nodeObject.transform.position + new Vector2(horizentalSpace, nodeHeight);
                node.right = CreateNode(node.index * 2 + 1, rightPosition);
                node.rightLine = CreateLine(node.nodeObject.transform.position, node.right.nodeObject.transform.position);


                //적의 위치생성
                CreateEnemy(node.index * 2+1, node.right.nodeObject.transform.position);
            }
            
          
            else
            {
                // 오른쪽 자식이 이미 있다면 재귀적으로 분열
                SplitTree(node.right, spacing, depth * 2);
              //  DestroyEnemy(node.index*2+1);
            }
        }
            

        void CreateEnemy(int index, Vector3 position)
        {

            GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
            enemy.name = $"Enemy {index}";
            enemyMap[index] = enemy;

           
            if (enemyMap.TryGetValue(index, out enemy))
            {
                StartCoroutine(EnemyAnimation(enemy, index));
            }

           
            int randomChance = UnityEngine.Random.Range(0, 91); 

            if (randomChance < 60) 
            {
                enemy.tag = ("Score");
                Renderer renderer = enemy.GetComponent<Renderer>();    
                renderer.material = refToScoreShader;

            }
            else if(randomChance >= 60&&randomChance<90)
            {
                enemy.tag = ("Enemy");
            }

            
            if (randomChance==90)
            {
                enemy.tag = ("Heart");


                SpriteRenderer spriteRenderer = enemy.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.sprite = refToHeart;
                }
            }

        }     
    }
    IEnumerator EnemyAnimation(GameObject enemy, int index)
    {
        
            if (enemy == null)
        {
            yield break;
        }

       


        float moveSpeed = UnityEngine.Random.Range(speedA, speedA+0.1f);
        float t = 0f;
        bool isMoving = false;

        Vector3 pointA = enemy.transform.position;
        Vector3 pointB = pointA;

        int targetIndex = index / 2;

        // 타겟 노드를 찾을 수 있는지 확인
        if (nodeMap2.TryGetValue(targetIndex, out NodeManager.Node targetNode))
        {
            if (targetNode.nodeObject != null&&pointB!=null)
            {
                pointB = targetNode.nodeObject.transform.position;
            }
           
        }

        // root가 null이거나 enemy가 root 위치에 있으면 더 이상 재귀 호출하지 않음
        if (root == null || enemy.transform.position == root.nodeObject.transform.position)
        {
            DestroyEnemy(index);  // 적을 제거하고
            yield break;  // 재귀 호출을 중지
        }

        while (t < 1f)
        {
            isMoving = true;
            t += moveSpeed * Time.deltaTime;
            if ((enemy!=null))
            {
                enemy.transform.position = Vector3.Lerp(pointA, pointB, t);
            }
            

            // 만약 targetNode가 삭제되었거나, pointB가 null이 되면 이동을 멈추고 종료
            if (targetNode == null || pointB == Vector3.zero)
            {
                DestroyEnemy(index);
                yield break;
            }
            
            yield return null;
        }

        if (enemy != null)
        {
            enemy.transform.position = pointB;
        }
        
        isMoving = false;

        // 더 이상 이동할 노드가 없다면, 재귀 호출을 멈추고 적을 제거
        if (targetNode != null)
        {
            StartCoroutine(EnemyAnimation(enemy, targetIndex)); // 재귀 호출
        }

        if(enemy != null)
        {
            if (enemy.transform.position.y < -4.0f)
            {
                if (enemy.CompareTag("Score"))
                {
                    gameManager.ScoreUp();
                }
                else if (enemy.CompareTag("Enemy"))
                {
                    gameManager.ScoreDown();
                }
                else if (enemy.CompareTag("Heart"))
                {
                    gameManager.Gold();
                  
                }






                    Destroy(enemy);
            }
        }
        
    }


    void DestroyEnemy(int index)
    {
        if (enemyMap.TryGetValue(index, out GameObject enemy))
        {
            // 적이 존재하면 삭제 조건 검사
            if (enemy != null)
            {
                // 적의 현재 위치
                Vector3 enemyPosition = enemy.transform.position;

                // 생성된 노드의 부모 인덱스를 계산
                int parentIndex = index / 2;

                // 부모 노드의 위치 확인
                if (nodeMap2.TryGetValue(parentIndex, out Node parentNode))
                {
                    Vector3 parentNodePosition = parentNode.nodeObject.transform.position;

                    // 적이 부모 노드보다 아래로 내려갔는지 확인
                    if (enemyPosition.y > parentNodePosition.y)
                    {
                        Debug.Log($"Enemy {index} has moved below its parent node.");
                        Destroy(enemy);  // 적 삭제
                        enemyMap.Remove(index);  // 적을 맵에서 삭제
                    }
                    else
                    {
                        Debug.Log($"Enemy {index} is above its parent node.");
                    }
                }
                else
                {
                    // 부모 노드가 없을 경우 적을 삭제
                    Debug.LogWarning($"Parent node for Enemy {index} not found. Destroying enemy.");
                    Destroy(enemy);
                    enemyMap.Remove(index);
                }
            }
        }
    }





    public void DeleteNode(int index)
    {
        if (index == 1) return;
        // 1. 현재 노드를 가져옵니다.
        if (!nodeMap2.TryGetValue(index, out Node node))
        {
            Debug.LogWarning($"Node {index} does not exist.");
            return;
        }

        // 2. 부모 노드와의 연결 해제 (부모 노드에서 연결 제거)
        int parentIndex = index / 2;
        if (nodeMap2.TryGetValue(parentIndex, out Node parentNode))
        {
            if (parentNode.left != null && parentNode.left.index == index)
            {
                parentNode.left = null;
                if (parentNode.leftLine != null)
                {
                    Destroy(parentNode.leftLine.gameObject);
                    parentNode.leftLine = null;
                }
            }
            else if (parentNode.right != null && parentNode.right.index == index)
            {
                parentNode.right = null;
                if (parentNode.rightLine != null)
                {
                    Destroy(parentNode.rightLine.gameObject);
                    parentNode.rightLine = null;
                }
            }
        }

        // 3. 연결된 적 삭제 (enemyMap을 통해)
        if (enemyMap.TryGetValue(index, out GameObject enemy))
        {
            if (enemy != null)
            {
                Destroy(enemy);  // 적 삭제
                enemyMap.Remove(index);  // 적을 맵에서 삭제
            }
        }

        // 4. 자식 노드 삭제 (재귀 호출) - 먼저 자식들을 삭제
        if (node.left != null)
        {
            DeleteNode(node.left.index);  // 왼쪽 자식 삭제
        }

        if (node.right != null)
        {
            DeleteNode(node.right.index);  // 오른쪽 자식 삭제
        }

        // 5. 노드 저장 후 삭제
        deletedNodes[index] = node; // 노드를 삭제 맵에 저장
        nodeMap2.Remove(index);      // 삭제된 노드 참조를 제거
        nodeMap.Remove(index);       // 삭제된 노드 GameObject 참조를 제거

        // 6. GameObject 삭제
        if (node.nodeObject != null)
        {
            Destroy(node.nodeObject); // 노드 GameObject 삭제
        }



        playerMovement.player.currentIndex = index / 2;
        
    }




    //    Parent(1, parent);
    //        if (node.index != parent)/* && 만약 node 가 부모가 아니라면*/
    //        {
    //            Destroy(node.nodeObject); // GameObject 삭제
    //    Debug.Log("굳");
    //            DeleteNode(index);
    //}

    public int Parent(byte key, int parentIndex)
    {
        int ParentIndex = 0;

        if(key==0)
        {
            ParentIndex = parentIndex;
        }
        else if(key==1)
        {
            return ParentIndex;
        }
        return 0;
    }


    //byte[] EdgeNode(byte[] edgeNode)
    //{
    //    return edgeNode;
    //}

    LineRenderer CreateLine(Vector3 start, Vector3 end)
    {
        // 라인 프리팹을 생성하고 LineRenderer 컴포넌트를 가져옴
        GameObject lineObject = Instantiate(linePrefab);
        LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();

        // 라인의 시작점과 끝점을 설정
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        // 라인 스타일 설정 (필요에 따라)
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

      //  Material lineMaterial = new Material(Shader.Find("Sprites/Default"));
       // lineMaterial.color = Color.green;
        return lineRenderer;
    }
  

    // 노드 클래스 정의
    
}
