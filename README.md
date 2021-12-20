# MuckMod

# How To Install
Download both Assembly-CSharp.dll and MuckCoreMod.dll 
Copy them to "C:\Program Files (x86)\Steam\steamapps\common\Muck\Muck_Data\Managed" or your local install directory
Replace if asked.
launch game

# Features
- 3X Resource speed
- Homies respawn @ Noon everyday
- Inventory not lost on death
- Furnaces 3x speed

# How does it work
Core mod is loaded using a custom version of Assembly-CSharp.dll modified with DnSpy
MenuUI Loads the mod by injecting 
```
public void LoadMods()
	{
		File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "muckModLoadLog.txt"));
		string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Muck_Data/Managed/MuckCoreMod.dll");
		Type[] exportedTypes = Assembly.LoadFile(path).GetExportedTypes();
		for (int i = 0; i < exportedTypes.Length; i++)
		{
			if (exportedTypes[i].GetMethods().Any((MethodInfo o) => o.Name == "OnModLoad"))
			{
				try
				{
					this.LogToFile("found Mod Type " + exportedTypes[i].FullName);
					object obj = Activator.CreateInstance(exportedTypes[i]);
					obj.GetType().GetMethod("OnModLoad").Invoke(obj, new object[]
					{
						this,
						base.transform
					});
				}
				catch (Exception ex)
				{
					this.LogToFile("Error loading Mod " + path + " " + ex.ToString());
				}
			}
		}
	}
```
Custom static functions are used anywhere i need access to the core gameplay ex 
if i want to 3x the furnace i will see if a func<int> variable has been set , if it has i will run this and div the furnace speed.

Core Mod and the modified Assembly CSharp file then use functions like this to add changes to the game 