using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MarubatuGame : EditorWindow
{
    [SerializeField]
    private int buttonSize = 50;
    private int[,] marubatuMap = new int[3, 3];
    private string[] buttonText = { "　", "〇", "×" };
    private bool turn;
    private bool gameEnd;
    private string gameEndText = "引き分け";

    [MenuItem("Window/Game/MaruBatuGame")]
    private static void Init()
    {
        EditorWindow marubatuWindow = GetWindow(typeof(MarubatuGame));

        marubatuWindow.Show();
    }

    private void Awake()
    {
        turn = (Random.Range(0, 2) == 1);
    }

    // 縦ラインの走査
    private bool JudgeHeightLine(int[,] map, int maru_or_batu)
    {
        // 縦を走査
        for (int y = 0; y < map.GetLength(0); y++)
        {
            if (map[y, 0] == maru_or_batu &&
                map[y, 1] == maru_or_batu &&
                map[y, 2] == maru_or_batu)
            {
                // 全てあっていたらそいつの勝ち
                return true;
            }
            else
            {
                // どっか一つでも違ったら次のラインへ
                continue;
            }
        }
        // どこにも当たらなかったら縦のラインはどこも揃ってない
        return false;
    }

    // 横ラインの走査
    private bool JudgeWidthLine(int[,] map, int maru_or_batu)
    {
        // 縦を走査
        for (int x = 0; x < map.GetLength(0); x++)
        {
            if (map[0, x] == maru_or_batu &&
                map[1, x] == maru_or_batu &&
                map[2, x] == maru_or_batu)
            {
                // 全てあっていたらそいつの勝ち
                return true;
            }
            else
            {
                // どっか一つでも違ったら次のラインへ
                continue;
            }
        }
        // どこにも当たらなかったら縦のラインはどこも揃ってない
        return false;
    }

    private bool JudgeCrossLine(int[,] map, int maru_or_batu)
    {

        // 右斜め下に向かって走査
        if (map[0, 0] == maru_or_batu &&
            map[1, 1] == maru_or_batu &&
            map[2, 2] == maru_or_batu)
        {
            // 全てあっていたらそいつの勝ち
            return true;
        }

        // 左斜め下に向かって走査
        if (map[0, 2] == maru_or_batu &&
            map[1, 1] == maru_or_batu &&
            map[2, 0] == maru_or_batu)
        {
            //Debug.Log(index);
            // 全てあっていたらそいつの勝ち
            return true;
        }

        // ここまで来たら斜めは揃ってない
        return false;
    }

    // 判定
    private int Judge(int[,] map)
    {
        // 〇の縦ラインの走査
        if (JudgeHeightLine(map, 1)) return 1;
        // ×の縦ラインの走査
        if (JudgeHeightLine(map, 2)) return 2;

        //Debug.Log("縦は違う");

        // 〇の横ラインの走査
        if (JudgeWidthLine(map, 1)) return 1;
        // ×の横ラインの走査
        if (JudgeWidthLine(map, 2)) return 2;

        //Debug.Log("横も違う");

        // 〇のクロスラインの走査
        if (JudgeCrossLine(map, 1)) return 1;
        // ×のクロスラインの走査
        if (JudgeCrossLine(map, 2)) return 2;

        //Debug.Log("クロスも違う");

        // ここまで来たら引き分け
        return 0;
    }

    private void OnGUI()
    {
        GUI.skin.label.fontSize = buttonSize;
        GUI.skin.button.fontSize = buttonSize;

        GUILayout.Label((turn ? "〇" : "×") + "のターン");

        using (new GUILayout.VerticalScope())
        {
            for (int y = 0; y < marubatuMap.GetLength(0); y++)
            {
                using (new GUILayout.HorizontalScope())
                {
                    for (int x = 0; x < marubatuMap.GetLength(1); x++)
                    {
                        using (new EditorGUI.DisabledGroupScope(gameEnd))
                        {

                            if (GUILayout.Button(buttonText[marubatuMap[y, x]], GUILayout.Width(buttonSize * 1.25f), GUILayout.Height(buttonSize * 1.25f)))
                            {
                                if (marubatuMap[y, x] != 0) return;

                                if (turn)
                                {// 〇なら１を入れる
                                    marubatuMap[y, x] = 1;
                                }
                                else
                                {// ×なら２を入れる
                                    marubatuMap[y, x] = 2;
                                }

                                if (Judge(marubatuMap) == 1)
                                {// 1なら〇の勝ち
                                    gameEndText = "〇の勝ち";
                                    gameEnd = true;
                                }
                                else if (Judge(marubatuMap) == 2)
                                {// 2なら×の勝ち
                                    gameEndText = "×の勝ち";
                                    gameEnd = true;
                                }

                                // ターンを切り替える
                                turn = !turn;
                            }
                        }
                    }
                }
            }

            if (!gameEnd)
            {
                // 盤面が埋まっているか確認
                foreach (int element in marubatuMap)
                {
                    if (element == 0) return;
                }
            }

            // ここまで来れたらゲームエンド
            gameEnd = true;

            // ゲームが終了していたら
            if (gameEnd)
            {
                GUILayout.Label(gameEndText);
                if (GUILayout.Button("リセット"))
                {
                    // 初期化
                    turn = (Random.Range(0, 2) == 1);
                    gameEnd = false;
                    gameEndText = "引き分け";
                    marubatuMap = new int[marubatuMap.GetLength(0), marubatuMap.GetLength(1)];
                }
            }
        }

        GUI.skin.label.fontSize = 10;
        GUI.skin.button.fontSize = 11;
    }
}
