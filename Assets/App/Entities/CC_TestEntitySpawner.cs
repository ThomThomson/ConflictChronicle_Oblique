using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CC_TestEntitySpawner : MonoBehaviour {
	public int spawnedCount = 0;


	void Start () {
		IEnumerator coroutine = EntitySpawner(0.2f);
        StartCoroutine(coroutine);
	}
	
	private IEnumerator EntitySpawner(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
			if(spawnedCount < 10) {
				Dictionary<string, System.Object> args = new Dictionary<string, System.Object>();
				Vector2 spawnVector = Random.insideUnitCircle * 5;
				args.Add("position", new Vector3(spawnVector.x + this.transform.position.x, 0.5f, spawnVector.y + this.transform.position.z));
				args.Add("name", "SpawnedEntity");
				// ObjectForInstantiation entityObject = new ObjectForInstantiation{
				// 	createGameObject = instantiateFunction,
				// 	arguments = args,
				// };
				// CC_InstantiateController.Instance.placeInSpawnQueue(entityObject);
				spawnedCount++;
			}
        }
    }

	public GameObject instantiateFunction(Dictionary<string, System.Object> arguments) {
            GameObject folder = GameObject.FindWithTag("CC_AIEntityFolder");
            if(folder == null) {
                folder = new GameObject();
                folder.name = "CC_Entities";
                folder.tag = "CC_AIEntityFolder";
            }
            GameObject entity = GameObject.Instantiate(CC_AssetMap.assetMap.entityTypes["TEST_CHARACTER"]);
            entity.name = (string)arguments["name"];
            entity.transform.parent = folder.transform;
            entity.transform.position = (Vector3)arguments["position"];
            return entity;
        }
}
