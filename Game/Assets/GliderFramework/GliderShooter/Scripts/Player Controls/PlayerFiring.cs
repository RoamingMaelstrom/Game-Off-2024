using UnityEngine;
using UnityEngine.InputSystem;
using SOEvents;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class PlayerFiring : MonoBehaviour
{
    [SerializeField] SOEvent playerFireWeaponsEvent;
    [SerializeField] List<Weapon> playerWeapons = new();
    float isFiring;

    void OnLMB(InputValue value)
    {
        isFiring = value.Get<float>();
        StartCoroutine(CheckIfMouseClickedUIDelayed());
    }

    IEnumerator CheckIfMouseClickedUIDelayed()
    {
        yield return new WaitForEndOfFrame();
        if (EventSystem.current.IsPointerOverGameObject()) isFiring = 0f;
        yield return null;
    }

    private void FixedUpdate() => HandleFiring();

    private void HandleFiring()
    {
        if (isFiring == 0) return;
        playerFireWeaponsEvent.Invoke();
        foreach (var weapon in playerWeapons)
        {
            weapon.TryFire();
        }
    }

    public void AddWeapon(Weapon weapon) => playerWeapons.Add(weapon);
    public void RemoveWeaponAt(int index) => playerWeapons.RemoveAt(index);

}



