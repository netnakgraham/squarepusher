using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]

public class DragAndDrop : MonoBehaviour {

	
    // The plane the object is currently being dragged on
    Plane dragPlane;

    // The difference between where the mouse is on the drag plane and 
    // where the origin of the object is on the drag plane
    Vector3 offset;

    Camera myMainCamera;

	bool shouldMove;

	bool isDragging;

  	Rigidbody m_Rigidbody;
    Vector3 m_YAxis;



    void Start() {
        myMainCamera = Camera.main;

 		m_Rigidbody = GetComponent<Rigidbody>();
        
		shouldMove = true;

		isDragging = false;

	}

	void Update()
    { 
		 if (shouldMove)
        {
           	m_Rigidbody.mass = 1;
        } else {
            m_Rigidbody.mass = 99999;
		}
	}

    void OnMouseDown() {
		isDragging = true;

        dragPlane = new Plane(myMainCamera.transform.forward, transform.position);
        Ray camRay = myMainCamera.ScreenPointToRay(Input.mousePosition);

        float planeDist;
        dragPlane.Raycast(camRay, out planeDist);
        offset = transform.position - camRay.GetPoint(planeDist);
		offset.y = 0;
    }

	void OnMouseUp(){

		isDragging = false;
	}

    void OnMouseDrag() {
		

		 Ray camRay = myMainCamera.ScreenPointToRay(Input.mousePosition);

        float planeDist;
        dragPlane.Raycast(camRay, out planeDist);
        transform.position = camRay.GetPoint(planeDist) + offset;

		Vector3 currentPos = transform.position;
		
		transform.position = new Vector3( Mathf.Round(currentPos.x  ),
                              0.5f,
                              Mathf.Round( currentPos.z  ) );


    }

		
	void OnCollisionEnter(Collision collision) {

		if (collision.gameObject.tag == "Cube" && isDragging == false) {            
			
			Debug.Log ("You push me!");

			shouldMove = false;
		}
	}

	void OnCollisionExit(Collision collision) {
		if (collision.gameObject.tag == "Cube" && isDragging == false) {             
			Debug.Log ("You leave me!");

			shouldMove = true;
		}
	}

}