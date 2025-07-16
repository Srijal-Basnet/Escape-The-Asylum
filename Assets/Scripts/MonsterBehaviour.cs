using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public class MonsterMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float fastMoveSpeed = 6f;
    public float detectionRange = 10f;
    public float stoppingDistance = 1.5f;
    public Transform playerTransform;
    public Animation walkAnimation;
    public AnimationClip walkingClip;
    public float mazeBoundary = 50f;

    [Header("Crouch Detection")]
    public float crouchDetectionMultiplier = 0f; // 0 means invisible while crouching

    [Header("Audio")]
    public List<AudioClip> growlSounds;
    public AudioClip roarSound;
    public AudioClip dangerMusic;
    public AudioSource audioSource;
    public AudioSource musicSource;

    private NavMeshAgent navMeshAgent;
    private bool isIdle = true;
    private bool isChasingPlayer = false;
    private Vector3 targetPosition;
    private float originalDetectionRange;
    private bool isPlayerCrouching = false;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = moveSpeed;
        originalDetectionRange = detectionRange;
        StartCoroutine(RandomBehavior());
        StartCoroutine(PlayRandomGrowls());
    }

    void Update()
    {
        float currentDetectionRange = isPlayerCrouching ? detectionRange * crouchDetectionMultiplier : detectionRange;

        if (PlayerInDetectionRange(currentDetectionRange) && !isPlayerCrouching)
        {
            if (!isChasingPlayer)
            {
                isChasingPlayer = true;
                navMeshAgent.speed = moveSpeed;
                PlayWalkingAnimation();
                PlaySound(roarSound);
                PlayDangerMusic();
            }
            navMeshAgent.SetDestination(playerTransform.position);

            if (Vector3.Distance(transform.position, playerTransform.position) <= stoppingDistance)
            {
                navMeshAgent.isStopped = true;
                StopWalkingAnimation();
            }
            else
            {
                navMeshAgent.isStopped = false;
            }
        }
        else
        {
            if (isChasingPlayer)
            {
                isChasingPlayer = false;
                navMeshAgent.speed = moveSpeed;
                FadeOutDangerMusic();
            }

            if (!isIdle)
            {
                if (Vector3.Distance(transform.position, targetPosition) >= 0.5f)
                {
                    navMeshAgent.SetDestination(targetPosition);
                }
                if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
                {
                    isIdle = true;
                    StopWalkingAnimation();
                    StartCoroutine(RandomBehavior());
                }
            }
        }
    }

    private IEnumerator RandomBehavior()
    {
        isIdle = true;
        yield return new WaitForSeconds(Random.Range(2f, 5f));
        isIdle = false;
        SetRandomTargetPosition();
        PlayWalkingAnimation();
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(targetPosition);
    }

    private IEnumerator PlayRandomGrowls()
    {
        while (true)
        {
            if (growlSounds.Count > 0 && !isChasingPlayer)
            {
                int randomIndex = Random.Range(0, growlSounds.Count);
                PlaySound(growlSounds[randomIndex]);
            }
            yield return new WaitForSeconds(Random.Range(5f, 15f));
        }
    }

    void SetRandomTargetPosition()
    {
        float randomX = Random.Range(-mazeBoundary, mazeBoundary);
        float randomZ = Random.Range(-mazeBoundary, mazeBoundary);
        targetPosition = new Vector3(randomX, transform.position.y, randomZ);
    }

    bool PlayerInDetectionRange(float range)
    {
        return Vector3.Distance(transform.position, playerTransform.position) <= range;
    }

    public void SetCrouchingStatus(bool isCrouching)
    {
        isPlayerCrouching = isCrouching;
    }

    public void MoveToFakeDoor(Vector3 fakeDoorPosition)
    {
        navMeshAgent.speed = fastMoveSpeed;
        navMeshAgent.SetDestination(fakeDoorPosition);
    }

    public void PlayWalkingAnimation()
    {
        if (walkAnimation != null && walkingClip != null && !walkAnimation.isPlaying)
        {
            walkAnimation.clip = walkingClip;
            walkAnimation.Play(walkingClip.name);
        }
    }

    public void StopWalkingAnimation()
    {
        walkAnimation?.Stop();
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void PlayDangerMusic()
    {
        if (dangerMusic != null && !musicSource.isPlaying)
        {
            musicSource.clip = dangerMusic;
            musicSource.volume = 1f;
            musicSource.Play();
        }
    }

    void FadeOutDangerMusic()
    {
        StartCoroutine(FadeOutMusic());
    }

    private IEnumerator FadeOutMusic()
    {
        float fadeDuration = 2f;
        float startVolume = musicSource.volume;
        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }
        musicSource.Stop();
        musicSource.volume = startVolume;
    }
}








