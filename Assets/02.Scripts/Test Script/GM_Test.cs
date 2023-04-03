using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM_Test : MonoBehaviour
{
    public GameObject player1;
    // public GameObject player2;
    private Player_Test player;
    private bool player1Turn;
    private bool isPlayerMoving;
    private void Start()
    {
        player1Turn = true;
        player = GetComponent<Player_Test>();
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
            player1Turn = !player1Turn;
        }
    }

    private IEnumerator PlayerTurn(GameObject player)
    {
        yield return StartCoroutine(MovePlayer(player));
        yield return StartCoroutine(Act1(player));
        Debug.Log($"{player.name} Act1 끝");
        yield return StartCoroutine(Act2(player));
        Debug.Log($"{player.name} Act2 끝");

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
    }

    private IEnumerator MovePlayer(GameObject player)
    {
        // 이동 가능 상태로 변경
        Debug.Log($"{player.name} MovePlayer 시작");
        while (true)
        {
            player.GetComponent<Player_Test>().MoveController();
            if (Input.GetKeyDown(KeyCode.Space)) break;
            yield return null;
        }
    }
    
    private IEnumerator Act1(GameObject player)
    {
        Debug.Log($"{player.name} Act1 시작");
        while (true)
        {
            yield return null;
        }
    }

    private IEnumerator Act2(GameObject player)
    {
        Debug.Log($"{player.name} Act2 시작");
        while (true)
        {
            if(Input.GetKeyDown(KeyCode.Keypad2)) break;

            yield return null;
        }
    }
}
