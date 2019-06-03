using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    /* Quando o jogador  selecionar um item do puzzle, o mesmo deve ser anexado ao slot,
     * slot deve receber a referencia do tipo de script : AnswerReference.
     * 
     * Se o jogador quiser trocar o item que foi coletado, o mesmo deve substituir o item
     * atual salvo no slot, para que o mesmo não consiga carregar mais de uma "Resposta".
     * 
     * Quando o jogador for interagir com a porta de saida, o script player deve verificar
     * primeiramente se o slot é vázio, caso não seja, verificar a resposta, para assim ver
     * (em caso de ser tres portas na sala) se aquela resposta é a que faz aquela porta abrir,
     * caso não, ele terá o feedback disso.
     * 
     * Se o item for utilizado(Caso o jogador tenha conseguido abrir a porta), o slot deve ser
     * esvaziado.
     * 
     * *Obs*
     * 1- Se o jogador trocar de item o antigo deve ser devolvido ao local de origem do mesmo
     * (Talvez devo salvar a posição dele para fazer a devolução dele no local desejado).
     * 2- Terá 2 tipos de salas:
     *      1- Uma porta de saída, no qual somente uma das resposta irá abri-la;
     *      2- Tres portas de saída, na qual somente uma das portas(a que abre com a resposta certa)
     *      da continuidade ao labirinto, as outras duas levam as corredores que direcionam para salas
     *      de reforço.
     */



    public AnswerReference item;
    public GameObject ItemObject;
}
