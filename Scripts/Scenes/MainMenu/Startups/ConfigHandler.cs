using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Startups {
	public class ConfigHandler : MonoBehaviour {
		void OnDestroy() {
			ConfigurationManager.Instance.CleanUp();
		}
	}
}