using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Thuleanx.Utils {
	/// <summary>
	/// Class <c>Calc</c> contains general utility functions.
	/// </summary>
	public class Calc {

		public static float Closest(float origin, float a, float b) {
			return Mathf.Abs(origin - a) > Mathf.Abs(origin - b) ? a : b;
		}

		public static float HeightToVelocity(float height) => height > 0 ? Mathf.Sqrt(Mathf.Abs(2 * height * Physics2D.gravity.y)) : 0;

		public static LayerMask GetPhysicsLayerMask(int currentLayer) {
			int finalMask = 0;
			for (int i = 0; i < 32; i++)
				if (!Physics.GetIgnoreLayerCollision(currentLayer, i)) finalMask = finalMask | (1 << i);
			return finalMask;
		}

		public static Vector3 ToSpherical(float r, float phi, float theta) {
			return new Vector3(
                r * Mathf.Sin(phi) * Mathf.Sin(theta),
                r * Mathf.Cos(phi),
                r * Mathf.Sin(phi) * Mathf.Cos(theta)
			);
		}

		public static void DrawWireDisk(Vector3 position, float radius, Color color) {
			Color oldColor = Gizmos.color;
			Gizmos.color = color;
			Matrix4x4 oldMatrix = Gizmos.matrix;
			Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.identity, new Vector3(1, 0.2f, 1));
			Gizmos.DrawWireSphere(Vector3.zero, radius);
			Gizmos.matrix = oldMatrix;
			Gizmos.color = oldColor;
		}
			
	}
}