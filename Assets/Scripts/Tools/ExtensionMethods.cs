using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class ExtensionMethods
{
	#region List
	public static T GetLast<T>(this List<T> list)
	{
		return list[list.Count - 1];
	}

	public static int RemoveLast<T>(this List<T> list)
	{
		list.RemoveAt(list.Count - 1);
		return list.Count;
	}

	public static T GetRandom<T>(this List<T> list)
	{
		return list.Count > 0 ? list[UnityEngine.Random.Range(0, list.Count)] : default(T);
	}
	#endregion

	#region Built-in Array
	public static T GetRandom<T>(this T[] array)
	{
		return array[UnityEngine.Random.Range(0, array.Length)];
	}
	#endregion

	#region SpriteRenderer
	public static void SetAlpha(this SpriteRenderer spriteRenderer, float value)
	{
		Color newColor = spriteRenderer.color;
		newColor.a = value;
		spriteRenderer.color = newColor;
	}
	#endregion

}
