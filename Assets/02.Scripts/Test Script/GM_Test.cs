using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM_Test : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;
    private Player_Test player;
    private bool player1Turn;

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
            else
            {
                yield return StartCoroutine(PlayerTurn(player2));
            }
            player1Turn = !player1Turn;
        }
    }

    private IEnumerator PlayerTurn(GameObject player)
    {
        yield return StartCoroutine(Act1(player));
        Debug.Log($"{player.name} Act1 끝");
        yield return StartCoroutine(Act2(player));
        Debug.Log($"{player.name} Act2 끝");

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
    }

    private IEnumerator Act1(GameObject player)
    {
        Debug.Log($"{player.name} Act1 시작");
        while (true)
        {
            // 이거 계속 호출함.
            if (player.GetComponent<Player_Test>().isCanMove())
            {
                player.GetComponent<Player_Test>().canMove = true;
                break;
            }
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
