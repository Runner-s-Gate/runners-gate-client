using Godot;
using System;

public class ConfigLoader
{
	private static ConfigFile CONFIG_FILE;
	
	public static object GetValue(string section, string val)
	{
		if(CONFIG_FILE == null) {
			CONFIG_FILE = new ConfigFile();
			CONFIG_FILE.Load("res://config.cfg");
		}
		return CONFIG_FILE.GetValue(section, val);
	}
}
