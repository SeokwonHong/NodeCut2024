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

    public Node root; 
    float nodeSpacing = 2.0f; 
    float nodeHeight;

    public bool gameHasStarted = false;



    public Transform objectChild;
    public Transform objectParent;

    private Rigidbody rb;



    public Material refToScoreShader;
    public Material refToGold;



    public float speedA=0.3f;


    public Sprite refToHeart;

   
    public class Node
    {
        public Transform position;
        public int index;
        public Node left;
        public Node right; 
        public GameObject nodeObject;
        public LineRenderer leftLine;
        public LineRenderer rightLine;
        public int experiment;

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
        nodeMap[index] = newNode.nodeObject;
        nodeMap2[index] = newNode;
     
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
           
            if (node.left == null)
            {
                Vector2 leftPosition = (Vector2)node.nodeObject.transform.position + new Vector2(-horizentalSpace, nodeHeight);
                node.left = CreateNode(node.index * 2, leftPosition);
                node.leftLine = CreateLine(node.nodeObject.transform.position, node.left.nodeObject.transform.position);


                

                CreateEnemy(node.index * 2, node.left.nodeObject.transform.position);
            }
            else
            {
            
                SplitTree(node.left, spacing, depth * 2);
            
            }
        }

        if (randomInt == 1)
        {
         
            if (node.right == null)
            {
                Vector2 rightPosition = (Vector2)node.nodeObject.transform.position + new Vector2(horizentalSpace, nodeHeight);
                node.right = CreateNode(node.index * 2 + 1, rightPosition);
                node.rightLine = CreateLine(node.nodeObject.transform.position, node.right.nodeObject.transform.position);


             
                CreateEnemy(node.index * 2+1, node.right.nodeObject.transform.position);
            }
            
          
            else
            {
                
                SplitTree(node.right, spacing, depth * 2);
          
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

      
        if (nodeMap2.TryGetValue(targetIndex, out NodeManager.Node targetNode))
        {
            if (targetNode.nodeObject != null&&pointB!=null)
            {
                pointB = targetNode.nodeObject.transform.position;
            }
           
        }


        if (root == null || enemy.transform.position == root.nodeObject.transform.position)
        {
            DestroyEnemy(index);  
            yield break;  
        }

        while (t < 1f)
        {
            isMoving = true;
            t += moveSpeed * Time.deltaTime;
            if ((enemy!=null))
            {
                enemy.transform.position = Vector3.Lerp(pointA, pointB, t);
            }
            

     
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

       
        if (targetNode != null)
        {
            StartCoroutine(EnemyAnimation(enemy, targetIndex)); 
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
  
            if (enemy != null)
            {
           
                Vector3 enemyPosition = enemy.transform.position;

               
                int parentIndex = index / 2;

            
                if (nodeMap2.TryGetValue(parentIndex, out Node parentNode))
                {
                    Vector3 parentNodePosition = parentNode.nodeObject.transform.position;

                 
                    if (enemyPosition.y > parentNodePosition.y)
                    {
                        Debug.Log($"Enemy {index} has moved below its parent node.");
                        Destroy(enemy); 
                        enemyMap.Remove(index); 
                    }
                    else
                    {
                        Debug.Log($"Enemy {index} is above its parent node.");
                    }
                }
                else
                {
              
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
   
        if (!nodeMap2.TryGetValue(index, out Node node))
        {
            Debug.LogWarning($"Node {index} does not exist.");
            return;
        }

    
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

     
        if (enemyMap.TryGetValue(index, out GameObject enemy))
        {
            if (enemy != null)
            {
                Destroy(enemy);  
                enemyMap.Remove(index);
            }
        }

        
        if (node.left != null)
        {
            DeleteNode(node.left.index);  
        }

        if (node.right != null)
        {
            DeleteNode(node.right.index); 
        }

        
        deletedNodes[index] = node; 
        nodeMap2.Remove(index);     
        nodeMap.Remove(index);      


        if (node.nodeObject != null)
        {
            Destroy(node.nodeObject);
        }



        playerMovement.player.currentIndex = index / 2;
        
    }



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





    LineRenderer CreateLine(Vector3 start, Vector3 end)
    {

        GameObject lineObject = Instantiate(linePrefab);
        LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();

 
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);


        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        return lineRenderer;
    }
  


    
}
