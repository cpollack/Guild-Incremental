using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuildHall : MonoBehaviour
{
    protected Guild guild;

    public HallData data;
    public bool Unlocked = false;

    // Start is called before the first frame update
    protected void Start()
    {
        guild = GameObject.Find("Guild").GetComponent<Guild>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void CompleteBuild(string buildID)
    {
        //
    }
}
