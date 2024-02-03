using System.Collections;
using UnityEngine;

public class GhostTrail : MonoBehaviour
{
    private PlayerMovement move;
    private PlayerAnimation anim;
    private SpriteRenderer sr;
    public Transform ghostsParent;
    public Color trailColor;
    public Color fadeColor;
    public float ghostInterval;
    public float fadeTime;
    public Material ghostMaterial; // Assign a material with a shader that supports color control

    private void Start()
    {
        anim = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAnimation>();
        move = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void ShowGhost()
    {
        StartCoroutine(ShowGhostCoroutine());
    }

    private IEnumerator ShowGhostCoroutine()
    {
        for (int i = 0; i < ghostsParent.childCount; i++)
        {
            Transform currentGhost = ghostsParent.GetChild(i);

            // Create a new material instance for each ghost
            Material ghostMatInstance = new Material(ghostMaterial);
            currentGhost.GetComponent<SpriteRenderer>().material = ghostMatInstance;

            currentGhost.position = move.transform.position;
            currentGhost.GetComponent<SpriteRenderer>().flipX = anim.playerSr.flipX;
            currentGhost.GetComponent<SpriteRenderer>().sprite = anim.playerSr.sprite;

            yield return null; // Wait for one frame

            StartCoroutine(FadeSprite(currentGhost, ghostMatInstance));
            yield return new WaitForSeconds(ghostInterval);
        }
    }

    private IEnumerator FadeSprite(Transform current, Material ghostMatInstance)
    {
        float elapsedTime = 0f;
        Color startColor = trailColor;

        while (elapsedTime < fadeTime)
        {
            ghostMatInstance.color = Color.Lerp(startColor, fadeColor, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ghostMatInstance.color = fadeColor;
    }
}
