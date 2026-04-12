using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
    {
        if (GameManager.Instance == null)
        {
            GameObject gmObj = new GameObject("GameManager");
            gmObj.AddComponent<GameManager>();
        }

        if (AudioManager.Instance == null)
        {
            GameObject audioObj = new GameObject("AudioManager");
            AudioSource bgm = audioObj.AddComponent<AudioSource>();
            AudioSource sfx = audioObj.AddComponent<AudioSource>();
            AudioManager am = audioObj.AddComponent<AudioManager>();
            am.bgmSource = bgm;
            am.sfxSource = sfx;

            am.menuBGM = ProceduralAudio.CreateMenuBGM();
            am.level1BGM = ProceduralAudio.CreateLevelBGM(1);
            am.level2BGM = ProceduralAudio.CreateLevelBGM(2);
            am.shootSFX = ProceduralAudio.CreateShootSFX();
            am.hitSFX = ProceduralAudio.CreateHitSFX();
            am.explosionSFX = ProceduralAudio.CreateExplosionSFX();
            am.pickupSFX = ProceduralAudio.CreatePickupSFX();
            am.hurtSFX = ProceduralAudio.CreateHurtSFX();
            am.victorySFX = ProceduralAudio.CreateVictorySFX();
            am.defeatSFX = ProceduralAudio.CreateDefeatSFX();
            am.buttonClickSFX = ProceduralAudio.CreateButtonClickSFX();
        }

        if (LeaderboardManager.Instance == null)
        {
            GameObject lbObj = new GameObject("LeaderboardManager");
            lbObj.AddComponent<LeaderboardManager>();
        }

        if (GameSetup.Instance == null)
        {
            GameObject setupObj = new GameObject("GameSetup");
            setupObj.AddComponent<GameSetup>();
        }
    }
}
