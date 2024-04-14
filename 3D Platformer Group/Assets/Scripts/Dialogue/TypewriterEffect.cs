using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private float textSpeed = 30f;
    public bool IsRunning { get; private set; }
    private readonly Dictionary<HashSet<char>, float> punctuations = new Dictionary<HashSet<char>, float>
    {
        {new HashSet<char> {'.', '!', '?'}, 0.6f},
        {new HashSet<char> {',', ';', ':'}, 0.3f},
        {new HashSet<char> {' '}, 0.1f}
    };
    
    private Coroutine typingCoroutine;
    
    public void Run(string text, TMP_Text textLabel)
    {
        typingCoroutine = StartCoroutine(TypeText(text, textLabel));
    }
    
    public void Stop()
    {
        StopCoroutine(typingCoroutine);
        IsRunning = false;
    }

    private IEnumerator TypeText(string text, TMP_Text textLabel)
    {
        IsRunning = true;
        textLabel.text = string.Empty;
        
        float t = 0;
        int charIndex = 0;
        while (charIndex < text.Length)
        {
            int lastCharIndex = charIndex;
            t += Time.deltaTime * textSpeed;
            charIndex = Mathf.FloorToInt(t);
            charIndex = Mathf.Clamp(charIndex, 0, text.Length);
            for (int i = lastCharIndex; i < charIndex; i++)
            {
                bool isLast = i == text.Length - 1;
                textLabel.text = text.Substring(0, i + 1);

                if (IsPunctuation(text[i], out float waitTime) && !isLast && !IsPunctuation(text[i +1], out _))
                {
                    yield return new WaitForSeconds(waitTime);
                }
            }
            yield return null;
        }
        IsRunning = false;
    }
    private bool IsPunctuation(char c, out float waitTime)
    {
        foreach (KeyValuePair<HashSet<char>, float> punctuationCategory in punctuations)
        {
            if (punctuationCategory.Key.Contains(c))
            {
                waitTime = punctuationCategory.Value;
                return true;
            }
        }
        waitTime = default;
        return false;
    }
}
