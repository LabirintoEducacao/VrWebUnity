using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckLevers : CheckBase
{
    public List<Lever> listLevers;

    public override bool checkAnswer(Answer ans)
    {

        bool isCorrect = true;

        // Verifica as alavancas
        foreach (Lever lever in listLevers)
        {
            if (lever.isActivate != lever.properties.correct)
            {
                isCorrect = false;
            }
        }

        // Se tudo estiver certo
        if (isCorrect)
        {
            // Esconde as alavancas
            foreach (ItemBase item in rmanager.answerReference)
            {
                item.gameObject.SetActive(false);
            }

            // Coloca o mesmo item para ser verificado na porta da próxima sala
            Inventory.instance.item = listLevers[0];

            door.anim.SetTrigger("openning");
            Player.instance.currentRoom.PositionNextRoom("DoorAnswer", true);
        }

        anim.SetBool("isRight", isCorrect);
        anim.SetBool("isWait", false);

        return isCorrect;

    }
}
