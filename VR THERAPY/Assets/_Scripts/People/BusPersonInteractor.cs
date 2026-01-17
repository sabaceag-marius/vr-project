using UnityEngine;

public class BusPersonInteractor : PersonInteractor
{
    public override void Interact()
    {
        //base.Interact();

        DialogueManager.Instance.SetSimpleResponse("Can't talk, I have to work for these tickets");
    }
}