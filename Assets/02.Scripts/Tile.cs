using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
    
public class Tile : MonoBehaviour
{
    private Collider2D _collider2D;
    public Image TileImage;

    public Tile(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    public int x, y;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void OnMoveTo(Vector3 end)
    {
        StartCoroutine("MoveTo", end);
    }

    private IEnumerator MoveTo(Vector3 end)
    {
        float	current  = 0;
        float	percent  = 0;
        float	moveTime = 1.0f;
        Vector3	start	 = GetComponent<RectTransform>().localPosition; // 본인 위치

        while ( percent < 1 )
        {
            current += Time.deltaTime;
            percent = current / moveTime;
            // 움직이는 코드
            GetComponent<RectTransform>().localPosition = Vector3.Lerp(start, end, percent);

            yield return null;
        }
    }
}
