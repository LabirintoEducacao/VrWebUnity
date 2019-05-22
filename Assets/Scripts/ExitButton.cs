using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public AnswerReference answerReference;

    public void desactivePanel()
    {
        answerReference.DesactivePanel();
    }
}
