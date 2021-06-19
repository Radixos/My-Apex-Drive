// Jason Lui

using UnityEngine;

[RequireComponent(typeof(CarController))]
[RequireComponent(typeof(Abilities))]
[RequireComponent(typeof(CarStats))]
[RequireComponent(typeof(ComboAnalyser))]
public class CarInputHandler : MonoBehaviour
{
    [Header("Player Options")]
    [Tooltip("This is assigned on Start so this cannot be changed in run time!")]
    [SerializeField]
    [Range(1, 4)]
    public int Player;
    private string horizontalInput;
    private string accelerateInput;
    private string brakeInput;
    private string driftInput;
    private string boostInput;
    private string powerAInput;
    private string powerBInput;

    public string HorizontalInput { get => horizontalInput; set => horizontalInput = value; }
    public string AccelerateInput { get => accelerateInput; set => accelerateInput = value; }
    public string BrakeInput { get => brakeInput; set => brakeInput = value; }
    public string DriftInput { get => driftInput; set => driftInput = value; }
    public string BoostInput { get => boostInput; set => boostInput = value; }
    public string PowerAInput { get => powerAInput; set => powerAInput = value; }
    public string PowerBInput { get => powerBInput; set => powerBInput = value; }

    void Start()
    {
        //Assign controller at start. Could be done in update if we want to swap player controls mid game?

        //Input clarification: 
        //Brake is actually reverse!!! To simulate controls similar to Rocket League.
        HorizontalInput = "Horizontal " + Player;
        AccelerateInput = "Accelerate " + Player;
        BrakeInput = "Brake " + Player;
        DriftInput = "Drift " + Player;
        BoostInput = "Boost " + Player;
        powerAInput = "Power A " + Player;
        powerBInput = "Power B " + Player;
    }
}
