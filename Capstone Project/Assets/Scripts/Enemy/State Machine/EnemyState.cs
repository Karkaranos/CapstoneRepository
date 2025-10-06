/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/1/2025
Date Last Modified : 	10/1/2025
Brief Description : 		Base class for all states
External Resources : 	https://www.youtube.com/watch?v=RQd44qSaqww
***************************************************/
using UnityEngine;

public class EnemyState 
{
    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void Update() { }
}
