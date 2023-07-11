using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Thuleanx.Utils {
	/// <summary>
	/// Certified
	/// </summary>
	public class Mathx {
		static System.Random rand;
		public static System.Random Random {
			get {
				if (rand == null) rand = new System.Random();
				return rand;
			}
		}

		/// <summary>
		/// Not inclusive
		/// </summary>
		public static float RandomRange(float a, float b) 
			=> (float) Random.NextDouble() * (b-a) + a;

		public static float RandomRange(Vector2 range) => RandomRange(range.x, range.y);


		/// <summary>
		/// Not inclusive
		/// </summary>
		public static int RandomRange(int a, int b)
			=> (int) (Random.Next(a,b));

		public static float Square(float x) => x * x;

		public static Vector2 Rotate(Vector2 root, float rad)
			=> new Vector2(
				root.x * Mathf.Cos(rad) - root.y * Mathf.Sin(rad),
				root.x * Mathf.Sin(rad) + root.y * Mathf.Cos(rad)
			);

		public static T Damp<T>(Func<T, T, float, T> lerpFunction, T a, T b, float lambda, float dt)
			=> lerpFunction(a, b, 1 - Mathf.Exp(-lambda * dt));

		public static float Remap(float t, float a, float b, float c, float d) {
			Assert.AreNotEqual(b - a, 0);
			return (t - a) / (b - a) * (d - c) + c;
		}

		public static float Remap_Clamp(float t, float a, float b, float c, float d)
			=> Mathf.Clamp(Remap(t, a, b, c, d), c, d);

		public static Quaternion ToQuat(Vector2 vec) => Quaternion.Euler(0, 0, -Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg);

		/// <summary> Keep in range [0, 2pi] </summary>
		public static float NormalizeAngle(float radians) => ModInRange(radians, 0, 2*Mathf.PI);

		public static float ModInRange(float value, float start, float end)  {
			float width = end - start;
			Assert.AreNotEqual(width, 0);
			float offset = value - start;
			return (offset - (Mathf.Floor(offset/width)*width))+start;
		}

		public static bool Approximately(float a, float b, float threshold = 1e-9f) {
			Assert.IsTrue(threshold >= 0);
			return (a > b ? a - b : b - a) <= threshold;
		}

		/// <summary>
		/// cur -> target by distance, but clamped
		/// </summary>
		public static float Approach(float cur, float target, float distance) {
			float amt = Mathf.Clamp(target - cur, -distance, distance);
			cur += amt;
			if (Approximately(0, cur - target))
				cur = target;
			return cur;
		}
	}
}