using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View2DTest : MonoBehaviour {
	public List<GameObject> prefabs;
	public View2dMain _view2D;
	public List<View2dMain.ItemIndex> movableItems = new List<View2dMain.ItemIndex>(); 

	private int _size = 21;


	// Use this for initialization
	void Start () {
		if (_view2D == null)
			return;
		Dictionary<string, GameObject> name2Object = new Dictionary<string,GameObject>();
		foreach (GameObject obj in prefabs) {
			name2Object.Add (obj.name, obj);
		}
		_view2D.LoadAssets (name2Object);
		_view2D.DescribeEntireMap (BuildMap());

		AddMovingStuff (2);
	}
	
	// Update is called once per frame
	void Update () {
		if (Random.value < .5*Time.deltaTime)
			AddMovingStuff (2);
		IssueMovements ();
	}

	private string[,] BuildMap(){
		var map = new string[_size,_size];
		for (int i = 0; i < map.GetLength (0); ++i) { 
			for (int j = 0; j < map.GetLength (1); ++j) { 
				if (Random.value < .8) {
					map [i, j] = "DirtTile";
				} else {
					map [i, j] = "WaterTile";
				}
			}	
		}
		return map;
	}

	private int GetRandomPos() {
		return Mathf.FloorToInt (Random.value * _size);
	}

	private void AddMovingStuff(int count){
		for (int i = 0; i < count; i++) {
			movableItems.Add(_view2D.CreateItem ("person", GetRandomPos(), GetRandomPos(), -1));
		}
	}

	private void IssueMovements(){
		foreach (var index in movableItems) {
			if (Random.value < Time.deltaTime * .3) {
				_view2D.InterpolateToPosition(index, GetRandomPos (), GetRandomPos (), 1f);
			}
		}
	}
}
