using UnityEngine;

public class GestureSoundManager : MonoBehaviour
{
    [Header("Fire Sounds")]
    public AudioSource fireBall;
    public AudioSource fireWall;

    [Header("Water Sounds")]
    public AudioSource waterBall;
    public AudioSource waterWall;

    [Header("Earth Sounds")]
    public AudioSource earthBall;
    public AudioSource earthWall;

    [Header("Movement Sounds")]
    public AudioSource moveForward;
    public AudioSource moveBack;
    public AudioSource turnLeft;
    public AudioSource turnRight;

    // Methods to play sounds
    public void PlayFireBall() { PlayOnce(fireBall); }
    public void PlayFireWall() { PlayOnce(fireWall); }
    public void PlayWaterBall() { PlayOnce(waterBall); }
    public void PlayWaterWall() { PlayOnce(waterWall); }
    public void PlayEarthBall() { PlayOnce(earthBall); }
    public void PlayEarthWall() { PlayOnce(earthWall); }

    public void PlayMoveForward() { PlayOnce(moveForward); }
    public void PlayMoveBack() { PlayOnce(moveBack); }
    public void PlayTurnLeft() { PlayOnce(turnLeft); }
    public void PlayTurnRight() { PlayOnce(turnRight); }

    private void PlayOnce(AudioSource source)
    {
        if (source != null && !source.isPlaying)
            source.Play();
    }
}
