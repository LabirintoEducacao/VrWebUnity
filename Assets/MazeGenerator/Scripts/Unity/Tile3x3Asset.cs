using larcom.MazeGenerator.Models;
using UnityEngine;
using larcom.MazeGenerator.Support;

public class Tile3x3Asset : MonoBehaviour, ITileCreator {
	public GameObject representation { get => this.gameObject; }
	public Tile tile { get; set; }
	public float cellSize { get; set; } = 1f;

	[Tooltip("Todos os chãos a serem utilizadas para colocar no labirinto. 0: Quina fechada, 1: Quina aberta, 2: Parede, 3: Meio e Passagem")]
	public GameObject[] floorPrefabs;
	[Tooltip("Todas as paredes a serem utilizadas para colocar no labirinto.")]
	public GameObject[] lateralWallPrefabs;
	[Tooltip("Quinas a serem colocadas no labirinto sendo 0: Quina Fechada e 1: Quina Aberta")]
	public GameObject[] cornerWallPrefabs;

	// Propriedades da Tile
	private int[] corners = new int[] {
				Constants.DIRECTION_UP|Constants.DIRECTION_RIGHT,
				Constants.DIRECTION_RIGHT|Constants.DIRECTION_DOWN,
				Constants.DIRECTION_DOWN|Constants.DIRECTION_LEFT,
				Constants.DIRECTION_LEFT|Constants.DIRECTION_UP};
	private int area_tiles = 3;
	private float tile_size;

	private int[] middlePassages = new int[] {
				Constants.DIRECTION_UP|Constants.DIRECTION_LEFT|Constants.DIRECTION_RIGHT,
				Constants.DIRECTION_UP|Constants.DIRECTION_DOWN|Constants.DIRECTION_RIGHT,
				Constants.DIRECTION_DOWN|Constants.DIRECTION_LEFT|Constants.DIRECTION_RIGHT,
				Constants.DIRECTION_UP|Constants.DIRECTION_LEFT|Constants.DIRECTION_DOWN,
				Constants.DIRECTION_UP|Constants.DIRECTION_RIGHT,
				Constants.DIRECTION_RIGHT|Constants.DIRECTION_DOWN,
				Constants.DIRECTION_DOWN|Constants.DIRECTION_LEFT,
				Constants.DIRECTION_LEFT|Constants.DIRECTION_UP,
				Constants.DIRECTION_UP|Constants.DIRECTION_DOWN,
				Constants.DIRECTION_LEFT|Constants.DIRECTION_RIGHT,
				Constants.DIRECTION_UP,
				Constants.DIRECTION_RIGHT,
				Constants.DIRECTION_DOWN,
				Constants.DIRECTION_LEFT

	};

	private Vector3[] passagePos = new Vector3[] {
				new Vector3(0f, 0f, 1f),
				new Vector3(1f, 0f, 0f),
				new Vector3(0f, 0f, -1f),
				new Vector3(-1f, 0f, 0f)
			};

	private Vector3[] cornerPos = new Vector3[] {
				new Vector3(0.5f, 0f, 1.5f),
				new Vector3(1.5f, 0f, -0.5f),
				new Vector3(-0.5f, 0f, -1.5f),
				new Vector3(-1.5f, 0f, 0.5f)
			};

	public void create(bool hasDoors = true) {

		tile_size = cellSize / area_tiles;

		clearChildren();

		if (tile.passages == Constants.DIRECTION_NONE) {
			// inacessible path, not creating
			return;
		}

		createFloors();
		createCorners();
		createWalls();

		if (hasDoors)
			createDoors();
	}

	/// <summary>
	/// Percorre toda a matriz(Tile3x3) e coloca cada tile na sua devida posição.
	/// </summary>
	void createFloors() {

		// Floor das passagens
		for (int i = 0; i < passagePos.Length; i++) {
			if ((tile.passages & Constants.DIRECTIONS[i]) == Constants.DIRECTIONS[i]) {
				GameObject passageFloor = Instantiate(floorPrefabs[3], this.transform.position, Quaternion.identity, this.transform);
				passageFloor.transform.Rotate(0f, 90f * i, 0f);
				passageFloor.transform.localPosition = passagePos[i];
			}
		}

		// Floor do meio
		for (int i = 0; i < middlePassages.Length; i++) {
			if ((tile.passages & middlePassages[i]) == middlePassages[i]) {
				GameObject middleFloor = Instantiate(floorPrefabs[3], this.transform.position, Quaternion.identity, this.transform);
				middleFloor.transform.Rotate(0f, 90f * i, 0f);
				middleFloor.transform.localPosition = new Vector3(0f, 0f, 0f);
				break;
			}
		}

		// Floor das corners
		for (int i = 0; i < corners.Length; i++) {
			if ((Constants.DIRECTION_ALL & ~tile.passages & corners[i]) == corners[i]) {  // Quina fechada
				GameObject cornerFloor = Instantiate(floorPrefabs[0], this.transform.position + cornerPos[i], Quaternion.identity, this.transform);
				cornerFloor.transform.Rotate(0f, 90f * i, 0f);
				cornerFloor.transform.localPosition = cornerPos[i];

			} else if ((tile.passages & corners[i]) == corners[i]) { // Quina aberta
				GameObject cornerFloor = Instantiate(floorPrefabs[1], this.transform.position + cornerPos[i], Quaternion.identity, this.transform);
				cornerFloor.transform.Rotate(0f, 90f * i, 0f);
				cornerFloor.transform.localPosition = cornerPos[i];
			}
		}

		// Floor Parede
		for (int i = 0; i < Constants.DIRECTIONS.Length; i++) {
			if ((tile.passages & Constants.DIRECTIONS[i]) == 0) {

				Vector3 pos = getPositionStartWall(i);

				for (int dx = 0; dx < area_tiles; dx++) { // percorre todo lado da Tile3x3

					bool flag = hasClosedCorner(dx, i);

					if (!flag) {
						GameObject wallFloor = Instantiate(floorPrefabs[2], this.transform.position + pos, Quaternion.identity, this.transform);
						wallFloor.transform.Rotate(0f, 90f * i, 0f);
					}

					pos = getNextPositionWall(pos, i);

				}
			}
		}
	}

	/// <summary>
	/// Instancia um objeto aleatório do array na posição passada como parâmetro.
	/// </summary>
	/// <param name="prefabs">Array de objetos</param>
	/// <param name="pos">Local a ser instanciado o objeto</param>
	/// <returns></returns>
	GameObject createRandomPrefab(GameObject[] prefabs, Vector3 pos) {
		return Instantiate(prefabs[Random.Range(0, prefabs.Length)], this.transform.position + pos, Quaternion.identity, this.transform);
	}

	/// <summary>
	/// Verifica e coloca as quinas nas suas devidas posições.
	/// </summary>
	void createCorners() {

		// corredores tem cantos extras
		if (tile.occupation == Constants.TILE_TYPE.CORRIDOR) {

			// parede das corners
			for (int i = 0; i < corners.Length; i++) {
				if ((Constants.DIRECTION_ALL & ~tile.passages & corners[i]) == corners[i]) {  // Quina fechada
					GameObject go = Instantiate(cornerWallPrefabs[0], this.transform.position, Quaternion.identity, this.transform);
					go.transform.Rotate(0f, 90f * i, 0f);
					go.transform.localPosition = cornerPos[i];

				} else if ((tile.passages & corners[i]) == corners[i]) { // Quina aberta
					GameObject go = Instantiate(cornerWallPrefabs[1], this.transform.position, Quaternion.identity, this.transform);
					go.transform.Rotate(0f, 90f * i, 0f);
					go.transform.localPosition = cornerPos[i];
				}
			}
		}
	}

	/// <summary>
	/// Verifica se já foi colocado quina.
	/// </summary>
	/// <param name="partWall">O index da parede a ser colocada</param>
	/// <param name="direction">Lado da parede a ser feita no Tile3x3</param>
	/// <returns></returns>
	bool hasClosedCorner(int partWall, int direction) {

		const int UP = 0;
		const int RIGHT = 1;
		const int DOWN = 2;
		const int LEFT = 3;

		const int FIRST_WALL = 0;
		int LAST_WALL = area_tiles - 1;

		bool hasCorner = false;


		if (partWall == FIRST_WALL) {

			if (direction == UP && (Constants.DIRECTION_ALL & ~tile.passages & corners[corners.Length - 1]) == corners[corners.Length - 1]) {
				hasCorner = true;
			} else if ((direction == RIGHT || direction == DOWN) && (Constants.DIRECTION_ALL & ~tile.passages & corners[direction]) == corners[direction]) {
				hasCorner = true;
			} else if (direction == LEFT && (Constants.DIRECTION_ALL & ~tile.passages & corners[direction - 1]) == corners[direction - 1]) {
				hasCorner = true;
			}

		} else if (partWall == LAST_WALL) {

			if ((direction == UP || direction == LEFT) && (Constants.DIRECTION_ALL & ~tile.passages & corners[direction]) == corners[direction]) {
				hasCorner = true;
			} else if ((direction == RIGHT || direction == DOWN) && (Constants.DIRECTION_ALL & ~tile.passages & corners[direction - 1]) == corners[direction - 1]) {
				hasCorner = true;
			}
		}

		return hasCorner;
	}

	bool isHorizontalWall(int direction) {
		return (direction % 2 == 0); // 0 e 2 paredes na horizontal e variam apenas o x pra colocar o muro
	}

	Vector3 getPositionStartWall(int direction) {
		Vector3 pos = new Vector3(Constants.DELTA[direction].x, 0f, Constants.DELTA[direction].y - 0.5f);

		if (isHorizontalWall(direction)) { // Parede Up e Down
			pos.x -= 1f;
			pos.z = 1.5f * Mathf.Sign(pos.z);
		} else { // Parede Left e Right
			pos.x = 1.5f * Mathf.Sign(pos.x);
			pos.z -= 0.5f;
		}

		return pos;
	}

	Vector3 getNextPositionWall(Vector3 currentPos, int direction) {
		if (isHorizontalWall(direction)) //Parede Up e Down
			currentPos.x++;
		else  // Parede Left e Right
			currentPos.z++;
		return currentPos;
	}


	/// <summary>
	/// Criação das paredes da Tile3x3.
	/// </summary>
	void createWalls() {

		// paredes de fato do meio, percorre todas as direções da Tile3x3
		for (int i = 0; i < Constants.DIRECTIONS.Length; i++) {
			if ((tile.passages & Constants.DIRECTIONS[i]) == 0) { // Verifica se não tem passagem up, right, down or left

				Vector3 pos = getPositionStartWall(i);

				for (int dx = 0; dx < area_tiles; dx++) { // percorre todo lado da Tile3x3

					bool flag = hasClosedCorner(dx, i);

					if (!flag) {
						GameObject go = createRandomPrefab(lateralWallPrefabs, Vector3.zero);
						go.transform.Rotate(0f, 90f * i, 0f);
						go.transform.localPosition = pos * tile_size;
					}

					pos = getNextPositionWall(pos, i);

				}
			}
		}
	}

	void createDoors() {

	}

	void clearChildren() {
		for (int i = this.transform.childCount; i < 0; i--) {
			Destroy(this.transform.GetChild(i));
		}
	}
}
