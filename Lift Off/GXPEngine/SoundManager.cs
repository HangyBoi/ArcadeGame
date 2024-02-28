using GXPEngine;
using GXPEngine.Core;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class SoundManager : GameObject
{
    private Sound backgroundMusic;
    private SoundChannel musicChannel;
    private List<Sound> spells;

    public SoundManager() {
        spells = new List<Sound>();
        backgroundMusic = new Sound("sounds/backMusic.wav", true, true);

        Sound zappingSound = new Sound("sounds/zapping.mp3", false, true);
        spells.Add(zappingSound);
    }

    public void StartBackMusic()
    {
        musicChannel = backgroundMusic.Play();
        //soundChannel.Frequency = 30000;
    }

    public void StopBackMusic()
    {
        musicChannel.Stop();
    }

    public void Zapping()
    {
        PlaySpellSound(0);
    }

    private void PlaySpellSound(int index)
    {
        if (index >= 0 && index < spells.Count)
        {
            spells[index].Play();
        }
    }
}
