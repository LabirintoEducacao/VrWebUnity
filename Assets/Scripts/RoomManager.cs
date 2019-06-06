﻿using System;
using System.Collections;
using System.Collections.Generic;
using larcom.MazeGenerator.Support;
using UnityEngine;

public class RoomManager : MonoBehaviour {
    public int entranceDirection = Constants.DIRECTION_NONE;
    public Transform[] spawnAnswer;
    public List<AnswerReference> answerReference;
    public GameObject answerPrefab;
    public Door door;
    [Header ("room data")]
    public Question question;
    public int id { get => question.question_id; }

    private void Start ( ) {
        
    }

    public void generateAnswers ( ) {
        Answer[] answers = question.answers;
        GameObject[] answerTemp = new GameObject[answers.Length];
        answerReference = new List<AnswerReference> ( );

        List<Transform> molds = new List<Transform> (spawnAnswer);
        Tools.Shuffle (molds);

        for (int i = 0; i < answers.Length; i++) {
            GameObject go = Instantiate (answerPrefab, molds[i].position, molds[i].rotation, molds[i]);
            AnswerReference ansRef = go.GetComponent<AnswerReference> ( );
            ansRef.properties = answers[i];
            answerReference.Add (ansRef);
        }
        door.room = this;
    }
}
