using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//C 2024 Daniel Snapir alias Baltazar Benoni

public class PlayerHitByMagic : MonoBehaviour
{
    //Boolean to indicate whether the player has been too near the attack point at the time the attack is about to
    //start and should be disabled from moving and jumping.
   [SerializeField] public bool playerTrappedByMagic;

    //LayerMask of the ground.
    LayerMask groundLayer; 
    public List<PriestClass> priestList = new ();
    private bool playerTrappedPreviousValue;

    void Start()
    {
        //Get reference to the layermask of the ground.
        groundLayer = LayerMask.GetMask("Ground");
    }
    //If a priest is attacking the player with magic and has not yet obtained a target point, run the
    //GetMagicLoaction-method. If no priest is attacking, the boolean associated with receiving a target point
    //for the attack is always false. If the z-value of the vector of the target point is not zero, the 
    //operation to obtain the target point did not succeed. Finally, it is checked if the player should be paralyzed,
    //i.e. if the attack was successful.
    void Update()
    {
        if(priestList.Count > 0)
        {
            foreach(PriestClass priest in priestList)
            {
                CheckIfPriestAttacks(priest);
            }
        }
        UpdatePriestIndexes();
    }
    //Create a new instance of the PriestClass and store it to the priestList-field. Since this method is
    //used only by potential priests attacking, the priestHitsWithMagic-variable is set to true.
    //The vector for the target point of the attack is initialized also
    //and the index of this priest in the priest list is returned to the user of this method.
    public int CreateNewPriest()
    {
        PriestClass priest = new PriestClass();
        priestList.Add(priest);
        priest.priestIndexInList = priestList.IndexOf(priest);
        return priest.priestIndexInList;
    }
    
    //A method to disable the movement- and jumping-scripts if the player is paralyzed by the magical attack.
   /* private void PlayerMovementDisabledCheck(bool newValue)
    {
        if(playerTrappedPreviousValue != newValue)
        {
            Actions.PlayerMovementDisabledByPriest(newValue);
        }
        playerTrappedPreviousValue = newValue;
    }*/
    //Obtain the point where a ray sent downwards from the player-GO hits the ground layer. If this is successful,
    //return the point of the raycast hit. Otherwise, return a vector with a z-value unequal to zero, indicating the
    //operation failed. 
    private Vector3 GetMagicLocation(PriestClass priest)
    {
        RaycastHit2D playerShadowOnGround = Physics2D.Raycast(transform.position, Vector3.down, 7f, groundLayer);
        if(playerShadowOnGround.collider != null)
        {
            Vector3 magicLocation = playerShadowOnGround.point + new Vector2(0, 0.4f);
            return magicLocation;
        }
        else
        {
            priest.magicLocationFound = false;
            return new Vector3(0, 0, 1);
        }
    }
    public void CheckIfPriestAttacks(PriestClass priest)
    {
        if(priest.priestHitsWithMagic && priest.contactPointSelected == false)
        {
            priest.priestMagicLocation = GetMagicLocation(priest);
        }
        if(priest.priestHitsWithMagic == false) { priest.contactPointSelected = false; }
        priest.magicLocationFound = priest.priestMagicLocation.z == 0;
    }
    public void RemovePriestFromList(int priestIndex)
    {
        if(priestIndex >= priestList.Count)
        {
            priestIndex = priestList.Count - 1;
        }
        priestList.RemoveAt(priestIndex);
    }
    private void UpdatePriestIndexes()
    {
        foreach (PriestClass priest in priestList)
        {
            priest.priestIndexInList = priestList.IndexOf(priest);
        }
    }
    public class PriestClass
    {
        //Boolean to indicate whether the player-GO is under attack of a certain enemy priest.
        [SerializeField] public bool priestHitsWithMagic;
        //Boolean to indicate whether the attacking priest has obtained a target point for its attack.
        [SerializeField] public bool contactPointSelected;
        //The point where the priest will direct its attack.
        public Vector3 priestMagicLocation;
        //Boolean indicating whether the method in the parent class succeeded in finding a ground level equivalent of the player's x-axis position.
        [SerializeField] public bool magicLocationFound;
        //The index of the PriestClass-object in the list of the parent class.
        public int priestIndexInList;

        public PriestClass()
        {
            priestHitsWithMagic = true;
            contactPointSelected = false;
            priestMagicLocation = new Vector3(0, 0, 0);
        }
    }
}
