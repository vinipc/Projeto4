using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class ExtensionMethods
{
	public static T GetLast<T>(this List<T> list)
	{
		return list[list.Count - 1];
	}

	public static int RemoveLast<T>(this List<T> list)
	{
		list.RemoveAt(list.Count - 1);
		return list.Count;
	}
}
