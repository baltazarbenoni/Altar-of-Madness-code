using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
//C 2024 Daniel Snapir alias Baltazar Benoni

public class PriestMagic : MonoBehaviour
{
    //Field to indicate whether the priest should attack and if it is doing so.
    [SerializeField] public bool priestIsMagicking;
    [SerializeField] public bool priestShouldAttack;
    private int indexInPriestList;
    private float distanceToPlayer;
    [SerializeField] private float magicRange;
    //Variable sets the delay after which the player can't escape the priest's attack.
    [SerializeField] private float magicOnsetDelay;
    //Time it takes for the priest to attack again.
    [SerializeField] private float magicOffsetDelay;
    private Vector3 magicTargetLocation;
    private LayerMask groundLayer;

    //Fields relating to player-GO.
    [SerializeField] private GameObject player;
    private Collider2D playerCollider;
    private Vector3 playerPosition;
    [SerializeField] private int priestSanityDamage;
    private PlayerHitByMagic playerHitByMagicScript;
    private PlayerHitByMagic.PriestClass thisPriest;

    //Fields relating to magicTrap-GO.
    private Collider2D magicTrapCollider;
    [SerializeField] AudioSource priestMagicAudio;
    [SerializeField] private GameObject magicTrapVFX;
    private VisualEffect magicTrapVFXClip;

    void Start()
    {
        //Get reference to different scripts and components of the player-GO.
        playerHitByMagicScript = player.GetComponent<PlayerHitByMagic>();
        groundLayer = LayerMask.GetMask("Ground");
        playerCollider = player.GetComponent<Collider2D>();
        magicTrapVFXClip = magicTrapVFX.GetComponent<VisualEffect>();

        //Handle the case where the magic onset delay has not been set and is left to zero.
        if(magicOnsetDelay == 0) { magicOnsetDelay = 0.1f; }
    }
    void Update()
    {
        priestShouldAttack = IsPlayerVisibleToPriest();
        //If the priest is not attacking but should attack, start the attack.
        if(priestShouldAttack && !priestIsMagicking)
        {
            CreateNewPriestAttackingPlayer();
            if(indexInPriestList > -1) { StartCoroutine(PrepareMagicAttack()); }
        }
        if(priestShouldAttack) { GetPriestIndexInList();}
    }
    //A method to check whether the priest can see the player.
    private bool IsPlayerVisibleToPriest()
    {
        playerPosition = player.transform.position;
        distanceToPlayer = Vector3.Distance(transform.position, playerPosition);
        bool groundBetweenPlayerAndPriest = Physics2D.Raycast(transform.position, playerPosition - transform.position, distanceToPlayer, groundLayer); 

        if(!groundBetweenPlayerAndPriest && distanceToPlayer < magicRange)
        { return true; }
        else { return false; }
    }
    //Update the index of this priest based on the list of all the priests currently attacking the player.
    private void GetPriestIndexInList()
    {
        try
        {
            indexInPriestList = playerHitByMagicScript.priestList.IndexOf(thisPriest);
        }
        catch
        {
            indexInPriestList = -1;
        }
    }
    //If the priest is attacking, the "CreateNewPriest"-method of the "playerHitByMagic"-script is run.
    //The method creates a priest in the list where player's script stores the multiple
    //priests that are possibly attacking it. The index of the created priest is returned to this method.
    private void CreateNewPriestAttackingPlayer()
    {
        indexInPriestList = playerHitByMagicScript.CreateNewPriest();
        thisPriest = playerHitByMagicScript.priestList[indexInPriestList];
    }
    //A Coroutine preparing the actual magical attack. First, it sets true the booleans indicating
    //whether the priest is attacking. After pausing for half a second, the script tries to get the target point
    //of the attack from the script on the player-GO. If this does not succeed (say if the player was jumping over
    // an abyss for example) the script will try again after half a second until successful. However, if the player has by this time
    //exited the priest's field of vision, no attack is started, and there is a breakpoint. 
    //Once the target point has been obtained, the coroutine for the attack itself is started.

    IEnumerator PrepareMagicAttack()
    {
        ChangeMagickingBooleans(true);
        yield return new WaitForSeconds(0.2f);
        bool playerAboveGround;

        do
        {
            playerAboveGround = GetMagicTargetLocation();
            yield return new WaitForSeconds(0.1f);
            if(!priestShouldAttack) { break; }

        } while (!playerAboveGround);
        StartCoroutine(InitializeRingOfFireAndAttackIfPossible());
    }
    //The coroutine for the attack itself. A "ring of fire" is instantiated in the location
    //obtained from the script on the player-GO. Then the iterator pauses for the time the player has before
    //they are trapped and cannot escape the attack. Then the program runs a method checking if the player is still
    //"in" the "ring of fire", that is, if they remain at the point where the attack is about to start.
    //If the player is still within the target area, a boolean (in a script on the player-GO) is set so
    //that the movement scripts of the player are disabled. The program pauses for the assigned time before
    //setting the pertaining booleans so that the attack can start again.
    IEnumerator InitializeRingOfFireAndAttackIfPossible()
    {
        magicTrapVFX.transform.position = magicTargetLocation;
        magicTrapVFX.SetActive(true);
        yield return new WaitForSeconds(magicOnsetDelay);

        bool attackForReal = ShouldPlayerBeDisabled(magicTrapVFX);
        if(attackForReal)
        {
            DisablePlayerMovement(true);
            priestMagicAudio.Play();
            magicTrapVFXClip.pause = true;
            yield return new WaitForSeconds(magicOffsetDelay);
            Actions.PlayerLosesSanity(priestSanityDamage);
            
            magicTrapVFX.SetActive(false);
        }
        else
        {
            magicTrapVFX.SetActive(false);
            yield return new WaitForSeconds(magicOffsetDelay);
        }

        EndAttack();
    }
    private void DisablePlayerMovement(bool newValue)
    {
        playerHitByMagicScript.playerTrappedByMagic = newValue;
        Actions.PlayerMovementDisabledByPriest(newValue);
    }
    public void PriestParalyzedWhileAttacking()
    {
        magicTrapVFX.SetActive(false);
        priestIsMagicking = false;
    }

    //Method to end the magical attack. returns to false two booleans of the player's script indicating
    //whether the target location of the priest's attack is selected, and whether the player is trapped by
    //the priest's magic and cannot escape. 
    //Also changes the booleans indicating whether the priest can attack again.
    //The index variable of this script is set to -1.
    public void EndAttack()
    {
        thisPriest.contactPointSelected = false;
        DisablePlayerMovement(false);
        ChangeMagickingBooleans(false);
        playerHitByMagicScript.RemovePriestFromList(thisPriest.priestIndexInList);
        indexInPriestList = -1;
    }

    //Method changing the values of the boolean variables, both in this script and the one associated with the player GO,
    //indicating whether the priest is attacking.
    private void ChangeMagickingBooleans(bool boolean)
    {
        priestIsMagicking = boolean;
        thisPriest.priestHitsWithMagic = boolean;
    }
    //A method to check whether the player is still within the "ring of fire" and should be paralyzed
    //as the attack commences.
    private bool ShouldPlayerBeDisabled(GameObject gameObject)
    {
        magicTrapCollider = gameObject.GetComponent<Collider2D>();
        bool priestIsParalyzed = GetComponent<DisableEnemy>().enemyIsParalyzed;

        if(magicTrapCollider == null)
        { return false; }

        else if(magicTrapCollider.IsTouching(playerCollider) && !priestIsParalyzed)
        { return true; }

        else { return false; }
    }
    //Method to get the ground-level coordinates of the player at the time of the attack.
    //This serves as the target location of the attack. Then the boolean variable (in the script of the player-GO)
    //indicating whether the target location was selected is set to true.
    private bool GetMagicTargetLocation()
    {
        if(thisPriest.magicLocationFound)
        { 
            magicTargetLocation = thisPriest.priestMagicLocation;
            thisPriest.contactPointSelected = true;  
        }
        return thisPriest.magicLocationFound;
    }
}