using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace larcom.MazeGenerator.Models {

	public class Corridor: Space {
		public Corridor (int id, Map map) : base(id, map, SPACE_TYPE.CORRIDOR) {

		}
	}
}
