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

	private void Start () {
    if (DataManager.ObjectMovementThreshold == 0) {
      DataManager.ObjectMovementThreshold = 1;
    }
    threshold = DataManager.ObjectMovementThreshold;
    points = GetComponent<ObjectDataContainer>().ObjectPoints;
    knockedOver = false;
		rb = GetComponent<Rigidbody>();
    hudManager = FindObjectOfType (typeof (HUDManager)) as HUDManager;

    sceneName = SceneManager.GetActiveScene().name;
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
