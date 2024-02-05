using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chessBoardMove : MonoBehaviour
{
    public Transform board;
    public Transform bot;
    public void moveBoardToggle(bool m) {
        if (m) {
            board.parent = transform;
            bot.parent = board;
        }
        else {
            board.parent = null;
            bot.parent = null;

        }
    }
}
