using UnityEngine;

public class EnemyDeathAnimation : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Rigidbody2D body;
    [SerializeField] float animationDuration = 3f;
    [SerializeField] float endSpeedMultiplier = 0.5f;
    [SerializeField] float maxVelocityDeviance = 0.5f;
    [SerializeField] float maxAngleDeviance = 90f;
    [SerializeField] float endScaleMultiplier = 0.5f;
    [SerializeField] Color startColour;
    [SerializeField] Color endColour;
    [SerializeField] ParticleSystem[] particleSystems;
    private float[] particleSystemDelays;
    [SerializeField] string explosionSfx;

    private Vector2 initialVelocity;
    private Vector2 targetVelocity;
    private Vector3 initialScale;
    private Vector3 targetScale;
    private float timer;
    private float currentAnimationDuration;

    public void Setup(Sprite sprite, Vector2 startVelocity, Vector3 startScale, float animationDurationMultiplier = 1f) {
        timer = 0;
        currentAnimationDuration = Random.Range(0.9f, 1.1f) * animationDuration * animationDurationMultiplier;

        spriteRenderer.sprite = sprite;
        initialVelocity = startVelocity;
        targetVelocity = Vector2.Perpendicular(initialVelocity) * endSpeedMultiplier;

        body.angularVelocity = Random.Range(-maxAngleDeviance, maxAngleDeviance) / currentAnimationDuration;

        transform.localScale = startScale;
        initialScale = startScale;
        targetScale = initialScale * endScaleMultiplier;

        if (particleSystems.Length == 0) return;

        foreach (var system in particleSystems)
        {
            system.gameObject.transform.localPosition = Random.insideUnitCircle + (Vector2.up * 0.25f);
            system.Stop();
        }

        particleSystemDelays = new float[particleSystems.Length];
        particleSystemDelays[0] = 0;
        for (int i = 1; i < particleSystemDelays.Length; i++)
        {
            particleSystemDelays[i] = Random.Range(0f, currentAnimationDuration / 2f);
        }
    }

    private void FixedUpdate() {
        timer += Time.fixedDeltaTime;
        spriteRenderer.color = Color.Lerp(startColour, endColour, timer / currentAnimationDuration);
        body.velocity = Vector2.Lerp(initialVelocity, targetVelocity, timer * maxVelocityDeviance / currentAnimationDuration);
        transform.localScale = Vector3.Lerp(initialScale, targetScale, timer / currentAnimationDuration);

        for (int i = 0; i < particleSystems.Length; i++)
        {
            if (timer > particleSystemDelays[i]) {
                particleSystems[i].Play();
                GliderAudio.SFX.PlayAtPoint(explosionSfx, transform.position);
                particleSystemDelays[i] = animationDuration * 2f;
            }
        }

        if (timer > currentAnimationDuration) Destroy(gameObject);
    }
}
