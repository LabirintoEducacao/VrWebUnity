using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    
    public Transform[] spawnAnswer;
    public AnswerReference[] answerReference;
    public GameObject answerPrefab;

    [Header("room data")]
    public int starting_question_id;
    public float time_limit;
    public int theme;
    public Question question;
    public Answer[] answers;

    private void Start()
    {
        GameObject[] answerTemp = new GameObject[spawnAnswer.Length];
        answerReference = new AnswerReference[spawnAnswer.Length];
        answers = question.answers;
        for (int i = 0; i < spawnAnswer.Length; i++)
        {
            answerTemp[i] = Instantiate(answerPrefab, spawnAnswer[i].position, spawnAnswer[i].rotation);
        }

        for (int i = 0; i < answerTemp.Length; i++)
        {
            answerReference[i] = answerTemp[i].GetComponent<AnswerReference>();
        }

        Invoke("AnswerText", 0.1f);
    }

    public void AnswerText()
    {
        for (int i = 0; i < question.answers.Length; i++)
        {
            Debug.Log(question.answers[i].answer);
            answerReference[i].textPanel.text = question.answers[i].answer;
        }
    }
}
