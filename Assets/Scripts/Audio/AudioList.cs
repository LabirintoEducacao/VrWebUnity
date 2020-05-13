using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*TODO: Artes para menu de configuração e musicas
* TODO: Colocar o HUD do menu de config em todas as cenas em que precisa ser acessado
* TODO: Manter o volume após mudar de cena
* TODO: Colocar objetos de Audio (3) em todas as cenas
* TODO: Para cada audio, criar e vincular objeto de audio, e adicionar função (Para BGM apenas função de play, para SFX função de play e stop)
*/
public class AudioList : MonoBehaviour
{
    //BGM
    [SerializeField] private AudioClip menuBGM;
    [SerializeField] private AudioClip gameGeloBGM;
    [SerializeField] private AudioClip gameFlorestaBGM;
    [SerializeField] private AudioClip gameCasaBGM;
    [SerializeField] private AudioClip gameUrbanoBGM;
    [SerializeField] private AudioClip gameCavernaBGM;
    [SerializeField] private AudioClip gameDesertoBGM;
    //SFX
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip playerPassos;
    [SerializeField] private AudioClip playerAlavanca;
    [SerializeField] private AudioClip playerChave;
    
    //Funções BGM
    public void PararMusica(){ AudioMixerSource.instance.PararBGM(); }
    public void PlayMenuBGM(){ AudioMixerSource.instance.TocarBGM(menuBGM); }
    public void PlayGameGeloBGM(){ AudioMixerSource.instance.TocarBGM(gameGeloBGM); }
    
    //Funções SFX
    public void PararEfeitos(){ AudioMixerSource.instance.PararSFX(); }
    public void PlayButtonClick(){ AudioMixerSource.instance.TocarSFX(buttonClick); }
    public void PlayPlayerPassos(){ AudioMixerSource.instance.TocarSFX(playerPassos); }
    public void PlayPlayerAlavanca(){ AudioMixerSource.instance.TocarSFX(playerAlavanca); }
    public void PlayPlayerChave(){ AudioMixerSource.instance.TocarSFX(playerChave); }
    
}
