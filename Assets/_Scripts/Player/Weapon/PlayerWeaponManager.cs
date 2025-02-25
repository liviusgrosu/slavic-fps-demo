using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    public static IPlayerWeaponBehaviour CurrentWeapon { get; set; }

    public GameObject swordArms;
    public GameObject bowArms;

    public enum Weapon
    {
        Sword,
        Bow
    }

    private Weapon _currentWeapon;

    private void Start()
    {
        SwitchWeapons(Weapon.Bow);
    }

    private void SwitchWeapons(Weapon weaponToSwitch)
    {
        swordArms.SetActive(false);
        bowArms.SetActive(false);

        _currentWeapon = weaponToSwitch;
        if (weaponToSwitch == Weapon.Sword)
        {
            swordArms.SetActive(true);
            CurrentWeapon = swordArms.GetComponent<IPlayerWeaponBehaviour>();
        }
        else if (weaponToSwitch == Weapon.Bow)
        {
            bowArms.SetActive(true);
            CurrentWeapon = bowArms.GetComponent<IPlayerWeaponBehaviour>();
        }
    }
}
