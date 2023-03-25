using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileUI : MonoBehaviour
{
    
    public static TileUI instance;
    private void Awake()
    {
        if (instance != null)
            Destroy(instance);
        instance = this;
        Clear();
    }
    
    private Node _node;
    private float _offsetY = 1f;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUp(Vector3 position, Node node)
    {
        _node = node;
        transform.position = position + Vector3.up * _offsetY;
        
        gameObject.SetActive(true);
    }
    public void Clear()
    {
        // 노드 초기화
        _node = null;
        gameObject.SetActive(false);

    }
    
}
