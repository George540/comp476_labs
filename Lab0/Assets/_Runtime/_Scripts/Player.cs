using UnityEngine;

public class Player : MonoBehaviour
{
    // TODO: discuss how we can use decorator attributes to customize the inpector without any custom editor scripts

    // should be displayed as a Textfield with a tooltip that says "A common phrase that the player often says."
    // This should also have a custom label: "Special Catch Phrase!"
    [SerializeField, TextArea(4, 4), Tooltip("A phrase that's commonly used.")] 
    private string catchPhrase = "Fear is the mind killer.";

    // should be displayed as a slider with min value = 0 and max value = maxHealth
    [SerializeField, Range(0, 100)]
    private int health = 0;
    
    // The designer should not be able to assign a value less than 0 for this property
    // This property should also be indented to show that it's related to "health"
    [SerializeField, Min(0)]
    private int maxHealth = 100;

    // The "shield" property should be hidden from the designer if this property is false, otherwise it should be visible
    [SerializeField]
    private bool withShield;

    // The designer should not be able to assign a value less than 0 for this property
    [SerializeField, Min(0)]
    private int shield = 0;
}
