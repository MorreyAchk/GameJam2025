using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitLevel : MonoBehaviour
{
    private LevelLoader levelLoader;
    void Start()
    {
        levelLoader = FindAnyObjectByType<LevelLoader>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        levelLoader.PlayNextLevel();
    }
}
