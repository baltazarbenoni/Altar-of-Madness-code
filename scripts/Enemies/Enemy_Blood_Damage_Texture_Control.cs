using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Blood_Damage_Texture_Control : MonoBehaviour
{
    [Range(0,5)]
    [SerializeField] private float enemyHP;
    [Range(0, 5)]
    [SerializeField] private int enemyMaxHP;

    private float enemyHealthFactor;

    private MaterialPropertyBlock block;

    private Renderer rend;

    // Start is called before the first frame update
    void Awake()
    {
        block = new MaterialPropertyBlock();
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        enemyHealthFactor = Mathf.Clamp((enemyHP - 1) / (enemyMaxHP - 1), 0, 1);
        rend.GetPropertyBlock(block);
        block.SetFloat("_HealthFactor", enemyHealthFactor);
        rend.SetPropertyBlock(block);
    }
}
