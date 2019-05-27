[System.Serializable]
public class MazeLDWrapper {
  public int maze_id = -1;
  public int starting_question_id;
  public int time_limit = -1;
  public int theme;
  public Question[] questions;
}

[System.Serializable]
public class Question {
  public int question_id = -1;
  public string answer_type = "text";
  public string question_type = "text";
  public string question;
  public string room_type;
  public MazePath[] paths;
  public Answer[] answers;
}

[System.Serializable]
public class MazePath {
  public int width = 1;
  public int height = 1;
  public string type = "maze";
  public int connected_question = -1;
  public bool end_game = false;
}

[System.Serializable]
public class Answer {
  public int answer_id = -1;
  public string answer;
  public bool correct = false;
}
