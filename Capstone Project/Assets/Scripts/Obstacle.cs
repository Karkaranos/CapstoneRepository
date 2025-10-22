using UnityEngine;
using NaughtyAttributes;

public class Obstacle : MonoBehaviour
{
    public enum ObstacleType {
        damage,
        slow,
        block
    }

    [SerializeField] public ObstacleType type;

    [SerializeField, ShowIf(nameof(type), ObstacleType.damage)] private float damageAmount;
    [SerializeField, ShowIf(nameof(type), ObstacleType.slow)] private int turnsRemoved;

    
}
