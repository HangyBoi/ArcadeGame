using GXPEngine;
using GXPEngine.Core;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public class SoundManager : GameObject
{
    private Sound backgroundMusic;
    public SoundChannel musicChannel;
    private SoundChannel spellCastChannel;
    private Sound playerHurt;
    private Sound gameOverMusic;
    private List<Sound> enemySounds;
    private Sound zappingCastSound;
    private Sound spellCastSound;
    private Sound bombCastSound;

    private float initialPan = -0.75f;
    private float finalPan = 0.75f;
    private float panIncrement = 0.015f;
    private float currentPan;
    private bool isPlaying = false;

    private float frequencyOnDifficulty;

    public SoundManager()
    {
        backgroundMusic = new Sound("sounds/backMusic.mp3", true, false);

        gameOverMusic = new Sound("sounds/game_over.wav", false, false);

        enemySounds = new List<Sound>();
        Sound enemyHurt1 = new Sound("sounds/enemy_hurt1.wav", false, false);
        enemySounds.Add(enemyHurt1);
        Sound enemyHurt2 = new Sound("sounds/enemy_hurt2.wav", false, false);
        enemySounds.Add(enemyHurt2);
        Sound enemyHurt3 = new Sound("sounds/enemy_hurt3.wav", false, false);
        enemySounds.Add(enemyHurt3);

        playerHurt = new Sound("sounds/hurt_player.wav", false, false);

        zappingCastSound = new Sound("sounds/zap.mp3", false, false);
        spellCastSound = new Sound("sounds/spellCast.wav", false, false);
        bombCastSound = new Sound("sounds/bombSound.wav", false, false);

    }

    public void StartBackMusic()
    {
        musicChannel = backgroundMusic.Play();
    }

    public void UpdateMusicFrequency()
    {
        frequencyOnDifficulty = Mathf.Clamp(35000 + Mathf.Log(MyGame.self.difficulty - 4) * 3500, 35000, 50000);
        musicChannel.Frequency = frequencyOnDifficulty;
    }

    public void StopBackMusic()
    {
        musicChannel.Stop();
    }

    public void GameOverSoundPlay()
    {
        gameOverMusic.Play();
    }

    public void ZappingCastSoundPlay()
    {
        zappingCastSound.Play();
    }

    public void BombCastSoundPlay()
    {
        bombCastSound.Play();
    }

    public void SpellCastSoundPlay()
    {
        currentPan = initialPan;
        spellCastChannel = spellCastSound.Play();
        spellCastChannel.Frequency = Utils.Random(25000, 40000);
        spellCastChannel.Volume = 0.4f;

        isPlaying = true;
    }

    public void EnemySoundPlay()
    {
        if (enemySounds.Count > 0)
        {
            int randomIndex = Utils.Random(0, enemySounds.Count);
            Sound randomEnemySound = enemySounds[randomIndex];
            randomEnemySound.Play();
        }
    }

    public void PlayerHurtSoundPlay()
    {
        playerHurt.Play();
    }

    void Update()
    {
        if (isPlaying)
        {
            if (currentPan <= finalPan)
            {
                spellCastChannel.Pan = currentPan;
                currentPan += panIncrement;
                Console.WriteLine(frequencyOnDifficulty);
            }
            else
            {
                isPlaying = false;
            }
        }
        UpdateMusicFrequency();
    }

}
