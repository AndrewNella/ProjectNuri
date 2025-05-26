using UnityEngine;

public class NPCPatrolState : OverworldNPCState
{
    NPCController controller;

    public NPCPatrolState(NPCController _controller)
    {
        controller = _controller;
    }


    public void Enter()
    {
        controller.npcState = NPCController.NPCState.Walking;
        Execute();
    }

    public void Execute()
    {
        if (controller.NPCCharacter == null)
        {
            Debug.LogError("Character component is missing. Cannot walk to patrol point.");
            Exit();
            return;
        }


        var _oldPosition = controller.transform.position;

        controller.StartCoroutine(controller.NPCCharacter.movementControl.Move(controller.PatrolPointList[controller.PatrolIndex], controller.NPCCharacter.movementControl.gridparentTransform));

        if (controller.transform.position != _oldPosition)
        {
            Debug.Log("Keep Walking to Patrol Point");
            controller.SetPatrolIndex((controller.PatrolIndex + 1) % controller.PatrolPointList.Count);
            Exit();
        }
    }

    public void Exit()
    {
        controller.npcState = NPCController.NPCState.WalkWaiting;
    }
}
