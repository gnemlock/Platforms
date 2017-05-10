using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode] public class PhysicsDemo : MonoBehaviour
{
    public float force;
    public Vector2 direction;

    private Rigidbody2D rigidbody;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void ApplyForce()
    {
        if(availableRigidbody())
        {
            rigidbody.AddForce(direction * force);
        }
    }

    bool availableRigidbody()
    {
        if(!rigidbody)
        {
            Debug.LogError("No rigidbody attached to PhysicsDemo");
            return false;
        }
        else
        {
            return true;
        }
    }
}

[CustomEditor(typeof(PhysicsDemo))]
public class PhysicsDemoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PhysicsDemo physicsDemo = target as PhysicsDemo;

        if(GUILayout.Button("Apply Force"))
        {
            
        }
    }
}
