using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveGameData {
	public int mazeID = -1;
	public int score = 0;
	public int currentRoomID = -1;
	public float timeElapsed = 0f;
	public float startTime;
	public bool playing = false;
	public bool finished = false;
	public RoomPlayerInfo[] rooms;

	public int rightAnswers = 0;
	public int wrongAnswers = 0;

	public void setRoomEnd(int room_id, bool correct) {
		for (int i = 0; i < rooms.Length; i++) {
			if (rooms[i].room_id == room_id) {
				rooms[i].timeInside = timeElapsed - rooms[i].enterTime;
				rooms[i].status = correct ? ROOM_STATUS.RIGHT : ROOM_STATUS.WRONG;
                if (correct)
                {
                    rightAnswers++;
					rooms[i].right++;
                }
                else
                {
                    wrongAnswers++;
                    rooms[i].wrongs++;
                }
				return;
			}
		}
		Debug.LogWarning("[End] Room " + room_id + " not found in save.");
	}

	/*public void setAnswer(int room_id, int answer_id) {
		for (int i = 0; i < rooms.Length; i++) {
			if (rooms[i].room_id == room_id) {
				rooms[i].answer_id = answer_id;
				return;
			}
		}
		Debug.LogWarning("[Answer] Room " + room_id + " not found in save.");
	}*/

	public void setRoomStart(int room_id) {
		for(int i = 0; i < rooms.Length; i++) {
			if (rooms[i].room_id == room_id) {
				rooms[i].enterTime = timeElapsed;
				return;
			}
		}
		Debug.LogWarning("[Start] Room " + room_id + " not found in save.");
	}
}

[Serializable]
public class RoomPlayerInfo {
	public int room_id;
	public ROOM_STATUS status;
	public int wrongs = 0;
	public int right = 0;
	public int answer_id = -1;
	public float enterTime;
	public float timeInside;

	public RoomPlayerInfo(int id) {
		status = ROOM_STATUS.NONE;
		enterTime = 0f;
		timeInside = 0f;
		room_id = id;
	}
}

public enum ROOM_STATUS { NONE, RIGHT, WRONG };
