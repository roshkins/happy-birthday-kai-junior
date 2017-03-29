﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RigidbodySleepCheck : MonoBehaviour {

  private Rigidbody rb;
  private bool knockedOver;
  private int points;
  private HUDManager hudManager;
  private float threshold;
  private string sceneName;
  private string objName;

	private void Start () {

    objName = gameObject.name;

    if (objName.LastIndexOf(" ") > 0) {
      objName = objName.Substring(0, objName.LastIndexOf(" "));
    }

    if (DataManager.ObjectMovementThreshold == 0) {
      DataManager.ObjectMovementThreshold = 1;
    }
    threshold = DataManager.ObjectMovementThreshold;
    knockedOver = false;
    rb = GetComponent<Rigidbody>();
    hudManager = FindObjectOfType (typeof (HUDManager)) as HUDManager;

    sceneName = SceneManager.GetActiveScene().name;

    int _mass = 0;
    int _points = 0;
    int i = 0;

    List<ObjectData> ObjectProperties = DataManager.ObjectProperties;

    while (_mass == 0 && i < ObjectProperties.Count) {
      if (ObjectProperties[i].name == objName) {
        _mass = ObjectProperties[i].mass;
        _points = ObjectProperties[i].points;
      }
      i++;
    }

    float volume = VolumeOfMesh(GetComponent<MeshFilter>().mesh);

    // volume *= ((transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3);

    Debug.Log (objName + " volume: " + volume);

    rb.mass = (_mass != 0) ? _mass : 1;
    points = (_points != 0) ? _points : 1;
	}
	
	private void OnCollisionStay (Collision other) {
    if (other.gameObject.GetComponent<Rigidbody>() != null && rb != null) {
      if (!knockedOver) {
    		if (rb.velocity.magnitude > 2) {
          knockedOver = true;
          Renderer rend = null;
          if (GetComponent<Renderer>() != null) {
            rend = GetComponent<Renderer>();
            rend.material.color = new Color(0.8F, 0.8F, 0.8F, 1F);;
          }

          int _points = points;

          // _points *= DataManager.Combo;

          DataManager.Score += _points;
          DataManager.CumulativeScore += _points;

          hudManager.ScoreChange();
          // hudManager.CumulativeScoreChange();

          if (DataManager.Score > DataManager.HighScore) {

            DataManager.HighScore = DataManager.Score;

            DataManager.NewHighScore = true;
            hudManager.HighScoreChange();
          }

          StartCoroutine(CheckMoveState());

        }
      }
    }
	}

  public float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3) {
    float v321 = p3.x * p2.y * p1.z;
    float v231 = p2.x * p3.y * p1.z;
    float v312 = p3.x * p1.y * p2.z;
    float v132 = p1.x * p3.y * p2.z;
    float v213 = p2.x * p1.y * p3.z;
    float v123 = p1.x * p2.y * p3.z;
    return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
  }

  public float VolumeOfMesh(Mesh mesh) {
    float volume = 0;
    Vector3[] vertices = mesh.vertices;
    int[] triangles = mesh.triangles;
    for (int i = 0; i < mesh.triangles.Length; i += 3) {
      Vector3 p1 = vertices[triangles[i + 0]];
      Vector3 p2 = vertices[triangles[i + 1]];
      Vector3 p3 = vertices[triangles[i + 2]];
      volume += SignedVolumeOfTriangle(p1, p2, p3);
    }
    return Mathf.Abs(volume);
  }

  private IEnumerator DestroyObject() {
    float t = 1f;
    while (t > 0) {
      transform.localScale = new Vector3(Mathf.Lerp(transform.localScale.x, 0, t), Mathf.Lerp(transform.localScale.y, 0, t), Mathf.Lerp(transform.localScale.z, 0, t));

      t -= Time.deltaTime;
      yield return new WaitForEndOfFrame();
    }
  }



  // @REFACTOR: this whole script can be done betttttttttter

  private IEnumerator CheckMoveState() {
		while (rb.velocity.magnitude > threshold && !gameObject.name.StartsWith("Jeffu") && sceneName == "kort-test") {
      DataManager.ObjectIsStillMoving = true;
      yield return null;
    }

    DataManager.ObjectIsStillMoving = false;

    this.enabled = false;
  }

}