using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyBehaviour), true)]
public class EnemyBehaviourEditor : Editor
{
    private void OnSceneGUI()
    {
        EnemyBehaviour enemyStats = (EnemyBehaviour)target;
     
        // Show the enemies player behaviour circles
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(enemyStats.transform.position, new Vector3(0, 1, 0), enemyStats.stats.pursueRadius);
        
        Handles.color = Color.red;
        Handles.DrawWireDisc(enemyStats.transform.position, new Vector3(0, 1, 0), enemyStats.stats.engageRadius);
    }
}