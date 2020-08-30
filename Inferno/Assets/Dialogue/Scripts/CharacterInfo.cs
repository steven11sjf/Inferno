using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue {
	[CreateAssetMenu(menuName = "Dialogue/CharacterInfo")]
	public class CharacterInfo : ScriptableObject {
		public string m_name;
		public Color color;
	}
}