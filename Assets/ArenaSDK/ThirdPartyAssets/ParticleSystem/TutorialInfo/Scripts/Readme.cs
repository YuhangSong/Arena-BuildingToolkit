using System;
using UnityEngine;

public class Readme : ScriptableObject {
	public Texture2D icon;
	public string title;
	public Section[] sections;
	public bool loadedLayout;
    //6756ed7b80223ac4e8f026ce27541de4
    [Serializable]
	public class Section {
		public string heading, text, linkText, url;
	}
}
