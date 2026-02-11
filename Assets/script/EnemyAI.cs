using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private List<GameObject> paths = new List<GameObject>();
    private Transform pathsParentTransform;
    private GameObject closestPath;
    [SerializeField] private float enemySpeed;
    [SerializeField] private float howCloseToAPointShouldTheEnemyBeToSwitch;
    private int index = 0;
    void Awake()
    {
        GameObject pathsParent = GameObject.FindGameObjectWithTag("Paths");
        Debug.Log(pathsParent);
        
        foreach (Transform child in pathsParent.transform)
        {
            paths.Add(child.gameObject);
        }
        
        closestPath = FindClosestPath(paths);
    }

    // Update is called once per frame
    void Update()
    {
        FollowPath(closestPath);
    }

    GameObject FindClosestPath(List<GameObject> paths)
    {
        float closestPathDis = float.MaxValue;
        GameObject closestPathGameObject = null;

        for (int i = 0; i < paths.Count; i++)
        {
            if (paths[i] == null)continue;
            
            float pathDisSqr = (gameObject.transform.position - paths[i].transform.position).sqrMagnitude;
            
            if (pathDisSqr < closestPathDis)
            {
                closestPathDis = pathDisSqr;
                closestPathGameObject = paths[i];
                Debug.Log(paths[i]);
            }
        }
        Debug.Log(closestPathGameObject);
        return closestPathGameObject;
    }

    void FollowPath(GameObject path)
    {
        
        Transform p = path.transform;
        
        if(p == null || p.childCount == 0)return;
        
        Transform target = p.GetChild(index);
        
        Vector3 direction = target.position - transform.position;
        if (direction.sqrMagnitude <= howCloseToAPointShouldTheEnemyBeToSwitch * howCloseToAPointShouldTheEnemyBeToSwitch)
        {
            index++;
            
        }
        transform.position += direction.normalized * enemySpeed * Time.deltaTime;
        
    }
}
