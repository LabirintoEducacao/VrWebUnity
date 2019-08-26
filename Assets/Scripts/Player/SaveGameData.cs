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
}

[Serializable]
public class RoomPlayerInfo {
	public int room_id;
	public ROOM_STATUS status;
	public int wrongs = 0;
	public bool right = false;
	public int answer_id = -1;
	public float enterTime;
	public float timeInside;
}

public enum ROOM_STATUS { NONE, RIGHT, WRONG };
