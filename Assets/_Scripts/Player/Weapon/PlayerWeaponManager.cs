using UnityEngine;
using UnityEngine.UIElements;

public class PlayerWeaponManager : MonoBehaviour
{
    public IPlayerWeaponBehaviour CurrentWeaponBehaviour;
    public static PlayerWeaponManager Instance;
    [HideInInspector] public bool SwitchingWeapons;

    public GameObject swordArms;
    public GameObject bowArms;


    public enum Weapon
    {
        Sword,
        Bow
    }

    private Weapon _currentWeapon;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        SwitchWeapons(Weapon.Bow);
    }

    private void Update()
    {
        var scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0 || scroll < 0) 
        {
            SwitchingWeapons = true;
        }

        if (SwitchingWeapons && CurrentWeaponBehaviour.IsIdling()) 
        {
            SwitchingWeapons = false;
            SwitchWeapons(_currentWeapon == Weapon.Sword ? Weapon.Bow : Weapon.Sword);
        }
    }

    private void SwitchWeapons(Weapon weaponToSwitch)
    {
        swordArms.SetActive(false);
        bowArms.SetActive(false);

        _currentWeapon = weaponToSwitch;
        if (weaponToSwitch == Weapon.Sword)
        {
            swordArms.SetActive(true);
            CurrentWeaponBehaviour = swordArms.GetComponent<IPlayerWeaponBehaviour>();
        }
        else if (weaponToSwitch == Weapon.Bow)
        {
            bowArms.SetActive(true);
            CurrentWeaponBehaviour = bowArms.GetComponent<IPlayerWeaponBehaviour>();
        }
    }

    public Transform GetArms()
    {
        return ((MonoBehaviour)CurrentWeaponBehaviour).transform;
    }
}
