using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View2dMain : MonoBehaviour {
	private Dictionary<string, VisualRepresentation> _assetDictionary = new Dictionary<string, VisualRepresentation>();
	private TileMap map;

	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void Update () {
	}

	public void LoadAssets(Dictionary<string, GameObject> assets){
		foreach (string name in assets.Keys) {
			_assetDictionary [name] = new VisualRepresentation (assets [name]);
			//yield return null; //slightly annoying to use
		}
		//yield break;
	}

	// This replaces the existing map.
	public void DescribeEntireMap(string[,] groundTiles) {
		
		map = new TileMap (groundTiles.GetLength(0), groundTiles.GetLength(1));
		for (int i = 0; i < groundTiles.GetLength (0); i++) {
			for (int j = 0; j < groundTiles.GetLength (1); j++) {
				InstantiateObject (groundTiles [i, j], i, j, 0);
			}
		}
	}

	public void InterpolateToPosition(ItemIndex index, int x, int y, float duration){
		StartCoroutine(InterpolationCoroutine(index, x, y, duration));
	}

	public void MoveItem(ItemIndex index, int x, int y){
		RemoveItem (index);
		AddItem (index, x, y);
	}

	private IEnumerator InterpolationCoroutine(ItemIndex index, int endX, int endY, float duration){
		float elapsedTime=0;
		int startX = index._x;
		int startY = index._y;
		while(elapsedTime<duration){
			elapsedTime = Mathf.Min (elapsedTime + Time.deltaTime, duration);
			float fraction = elapsedTime / duration;
			float x = (1 - fraction) * startX + fraction * endX;
			float y = (1 - fraction) * startY + fraction * endY;
			MoveItem (index, Mathf.FloorToInt(x), Mathf.FloorToInt(y));
			index._item.transform.position = new Vector3 (x, y, index._item.transform.position.z);
			yield return null;
		}
		yield break;
	}

	public ItemIndex CreateItem(string name, int x, int y, float z){
		ItemIndex index = InstantiateObject (name, x, y, z);
		return AddItem (index, x, y);
	}

	private ItemIndex AddItem(ItemIndex index, int x, int y){
		index._x = x;
		index._y = y;
		map.get (index._x, index._y).Add(index._item);
		index._item.transform.position = new Vector3 (x, y, index._item.transform.position.z);
		return index;
	}



	private void RemoveItem(ItemIndex index){
		map.get (index._x, index._y).Remove(index._item);
	}
		
	private ItemIndex InstantiateObject(string asset, int x, int y, float z){
		var newThing = GameObject.Instantiate (_assetDictionary [asset].representation, new Vector3(x,y,z), Quaternion.identity);
		map.add (x, y, newThing);
		return new ItemIndex (newThing);
	}

	private class TileMap{
		List<GameObject>[,] mapArray;

		public TileMap(int xSize, int ySize){
			mapArray=new List<GameObject>[xSize,ySize];
		}

		public List<GameObject> get(int x, int y){
			if (x < 0 || x >= mapArray.GetLength (0) || y < 0 || y >= mapArray.GetLength (1))
				return null;
			return mapArray [x,y];
		}

		public bool add(int x, int y, GameObject item){
			if (x < 0 || x >= mapArray.GetLength (0) || y < 0 || y >= mapArray.GetLength (1))
				return false;
			List<GameObject> objects = mapArray [x,y];
			if (objects == null) {
				objects = new List<GameObject> ();
				mapArray [x,y] = objects;
			}
			objects.Add (item);
			//TODO move the game object on screen to the appropriate location
			//TODO position the item by sorting
			return true;
		}

	}
		

	private class VisualRepresentation {
		public GameObject representation;

		public VisualRepresentation(GameObject s) {
			representation = s;
		}
	}

	//Index fields shouldn't be accessible to outside classes
	public class ItemIndex{
		public GameObject _item;
		public int _x;
		public int _y;
		public ItemIndex(GameObject item){
			_item=item;
		}
	}
}
