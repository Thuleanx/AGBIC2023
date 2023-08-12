using UnityEngine;
using Utils;
using System.Collections.Generic;

namespace VFX {
	[RequireComponent(typeof(Renderer))]
	public class FlashEffect : MonoBehaviour {
		[SerializeField] float flashDuration;
        [SerializeField] Material flashMaterial;
		public Renderer Renderer {get; private set;}
		Timer Flashing;
        bool lastFrameFlashing = false;

        List<Material> realMaterials = new List<Material>();
        List<Material> flashMaterials = new List<Material>();

		public void Flash() => Flashing = flashDuration;

		void Awake() {
			Renderer = GetComponent<Renderer>();

            Renderer.GetSharedMaterials(realMaterials);
            for (int i = 0; i < Renderer.materials.Length; i++)
                flashMaterials.Add( flashMaterial );
		}

		void LateUpdate() {
            if (lastFrameFlashing ^ Flashing) {
                if (Flashing)   Renderer.sharedMaterials = flashMaterials.ToArray();
                else            Renderer.materials = realMaterials.ToArray();
            }
            lastFrameFlashing = Flashing;
		}
	}
}
