using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResonanceState : PlayerBasicState
{
    private Rigidbody2D myRigidbody;
    public override void OnEnter(PlayerController player)
    {
        myRigidbody = player.GetComponent<Rigidbody2D>();
        player.MagicResonance();
    }

    public override void OnFixedUpdate(PlayerController player)
    {
        myRigidbody.velocity = Vector2.zero;
    }

    public override void OnUpdate(PlayerController player)
    {
        if (!Input.GetButton("Resonance"))
        {
            player.PlayerState.isMagicResonance = false;
            player.MagicResonanceOver();
            player.TranslateToState(player.playerMoveState);
            return;
        }
        if (player.PlayerData.CurResonanceValue >= player.PlayerData.MaxResonanceValue)
        {
            player.PlayerData.CurResonanceValue = player.PlayerData.MaxResonanceValue;
            player.PlayerState.isMagicResonance = false;
            player.MagicResonanceOver();
            player.TranslateToState(player.playerMoveState);
            return;
        }
    }
}
