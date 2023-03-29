using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM_Test : MonoBehaviour
{
    public GameObject[] tiles;
    public GameObject player1;
    public GameObject player2;

    private bool player1Turn;

    private void Start()
    {
        player1Turn = true;
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (true)
        {
            if (player1Turn)
            {
                yield return StartCoroutine(PlayerTurn(player1));
            }
            else
            {
                yield return StartCoroutine(PlayerTurn(player2));
            }
            player1Turn = !player1Turn;
        }
    }

    private IEnumerator PlayerTurn(GameObject player)
    {
        // 타일 옮기기 전에는 플레이어의 말을 이동할 수 없음
        player.GetComponent<Player_Test>().canMove = false;

        // 타일 옮기기
        bool tileMoved = false;
        while (!tileMoved)
        {
            // 타일 옮기기 
            yield return StartCoroutine(DragTile(player));
            // 타일을 옮겼는지 확인하는 함수. 이건 아마 박스들이 가지고 있을 듯...
            // if (IsTileInBox())
            // {
            //     // 함수통해서 GameManager가 변수 저장
            //     tileMoved = true;
            // }
        }

        // 플레이어 말 이동하기
        yield return StartCoroutine(MovePlayer(player));

        // 다음 턴을 위해 타일을 초기 위치로 돌려놓음
        // ResetTilePosition();

        // 타일 옮기기가 끝나면 플레이어의 말을 이동할 수 있음
        player.GetComponent<Player_Test>().canMove = true;

        // 플레이어 턴이 끝날 때까지 기다림
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
    }

    private IEnumerator DragTile(GameObject player)
    {
        // 마우스로 타일 드래그 드롭
        while (true)
        {
            // isSelecetdTile?
            // 타일을 클릭하고 있는 동안에만 마우스 위치에 따라 타일을 이동시킴
            if (Input.GetMouseButton(0))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0f;
                player.transform.position = mousePos;
            }

            yield return null;

            // 타일을 놓았을 때 타일 영역에 들어가면 타일 이동 종료
            // if (IsTileInBox())
            // {
            //     break;
            // }
        }
    }

    private IEnumerator MovePlayer(GameObject player)
    {
        // 플레이어 말 이동
        while (true)
        {
            // 마우스로 타일 클릭시 플레이어 말을 해당 타일 위치로 이동시킴
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && hit.collider.CompareTag("Tile"))
                {
                    // player.GetComponent<Player_Test>().MoveToTile(hit.collider);
                    break;
                }
            }

            yield return null;
        }
    }
}
