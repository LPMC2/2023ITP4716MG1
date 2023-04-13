using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
public class ReadOnlyAttribute : PropertyAttribute
{

}

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
                                            GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position,
                               SerializedProperty property,
                               GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
#endif
public class Destructable : MonoBehaviour
{
    [SerializeField] private GameObject destroyedVersion;
    [SerializeField] private float MaxDurability = 100f;
    [SerializeField, ReadOnly] private float Durability;
    [SerializeField] private AudioClip DestroyedSound;
    private bool isDestroyed = false;
    private void Start()
    {
        Durability = MaxDurability;
    }
    public void DestroyObject(RaycastHit hit, float force, GameObject hitfx, bool isGun)
    {
        Durability -= force;
        if (Durability <= 0 && isDestroyed == false)
        {
            GameObject shattered = Instantiate(destroyedVersion, transform.position,transform.rotation) as GameObject;

            foreach (Transform child in shattered.transform)
            {

                MeshCollider meshCollider = child.GetComponent<MeshCollider>();
                if (meshCollider)
                {
                    Quaternion rotation = Quaternion.Euler(child.rotation.eulerAngles);
                    meshCollider.sharedMesh = null;
                    meshCollider.sharedMesh = child.GetComponent<MeshFilter>().mesh;
                    meshCollider.transform.rotation = rotation;
                    child.GetComponent<MeshFilter>().mesh.RecalculateNormals();
                }
                Rigidbody rb = child.GetComponent<Rigidbody>();
                if (rb)
                {
                    rb.AddForce(hit.transform.forward * force * 10, ForceMode.Impulse);
                }
            }

            isDestroyed = true;
            PlaySoundAndDestroy();
            Destroy(gameObject);
            
        } else
        {
            if (isGun == true)
            {
                if (hitfx)
                {
                    GameObject fx = Instantiate(hitfx, hit.point, Quaternion.identity);
                    fx.transform.rotation = Quaternion.LookRotation(hit.normal);
                }
            }
        }
    }
    public void PlaySoundAndDestroy()
    {
        // Create a new empty game object at the position of the original object
        GameObject soundObject = new GameObject("SoundObject");
        soundObject.transform.position = transform.position;

        // Add an AudioSource component to the new game object
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        if (audioSource)
        {
            // Set the audio clip and play it
            audioSource.clip = DestroyedSound;
            audioSource.Play();
            Destroy(soundObject, DestroyedSound.length);
        } else
        {
            Destroy(soundObject);
        }
        // Destroy the original object

        // Destroy the sound object after the sound has finished playing
      
    }
}