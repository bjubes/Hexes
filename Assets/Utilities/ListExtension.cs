using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public static class ListExtension {

	public static void AddIfNotNull<T>(this List<T> list, T element) 
	where T : class {
		if (element != null) { list.Add(element); }
	}
}
