using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*TODO: Artes para menu de configuração e musicas
* TODO: Colocar objetos de Audio em todas as cenas
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
    [SerializeField] private AudioClip buttonVR;
    [SerializeField] private AudioClip playerPassos;
    [SerializeField] private AudioClip playerAlavanca;
    [SerializeField] private AudioClip playerChave;
    [SerializeField] private AudioClip playerObjeto;
    [SerializeField] private AudioClip playerBotao;
    [SerializeField] private AudioClip portaAbrindo;
    [SerializeField] private AudioClip erro;

    public static AudioList instance;
    
    void Awake()
    {
        instance = this;
    }

    //Funções BGM
    public void PararMusica(){ AudioMixerSource.instance.PararBGM(); }
    public void PlayMenuBGM(){ AudioMixerSource.instance.TocarBGM(menuBGM); }
    public void PlayGameGeloBGM(){ AudioMixerSource.instance.TocarBGM(gameGeloBGM); }
    public void PlayGameFlorestaBGM(){ AudioMixerSource.instance.TocarBGM(gameFlorestaBGM); }
    public void PlayGameCasaBGM(){ AudioMixerSource.instance.TocarBGM(gameCasaBGM); }
    public void PlayGameUrbanoBGM(){ AudioMixerSource.instance.TocarBGM(gameUrbanoBGM); }
    public void PlayGameCavernaBGM(){ AudioMixerSource.instance.TocarBGM(gameCavernaBGM); }
    public void PlayGameDesertoBGM(){ AudioMixerSource.instance.TocarBGM(gameDesertoBGM); }
    
    //Funções SFX
    public void PararEfeitos(){ AudioMixerSource.instance.PararSFX(); }
    public void PlayButtonClick(){ AudioMixerSource.instance.TocarSFX(buttonClick); }
    public void PlayButtonVR(){ AudioMixerSource.instance.TocarSFX(buttonVR); }
    public void PlayPlayerPassos(){ AudioMixerSource.instance.TocarSFX(playerPassos); }
    public void PlayPlayerAlavanca(){ AudioMixerSource.instance.TocarSFX(playerAlavanca); }
    public void PlayPlayerChave(){ AudioMixerSource.instance.TocarSFX(playerChave); }
    public void PlayPlayerObjeto(){ AudioMixerSource.instance.TocarSFX(playerObjeto); }
    public void PlayPlayerBotao(){ AudioMixerSource.instance.TocarSFX(playerBotao); }
    public void PlayPortaAbrindo(){ AudioMixerSource.instance.TocarSFX(portaAbrindo); }
    public void PlayErro(){ AudioMixerSource.instance.TocarSFX(erro); }
    
}
