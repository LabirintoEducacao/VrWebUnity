using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Android;

namespace larcom.support {
	class SaveData {
		public static string basePath = "savegame";

		public static bool save(string filename, string jsondata) {
#if PLATFORM_ANDROID && !UNITY_EDITOR
			requestPermissionAsync(Permission.ExternalStorageWrite).ContinueWith( result => {
				if (result.Result) {
					effectiveSave(filename, jsondata);
				} else {
					Debug.LogError("Cannot save if write permission is not given.");
				}
			});
			return true;
#else
			effectiveSave(filename, jsondata);
			return true;
#endif
		}

		static void effectiveSave(string filename, string jsondata) {
			string dirpath = Path.Combine(Application.persistentDataPath, SaveData.basePath);
			if (!Directory.Exists(dirpath)) {
				Directory.CreateDirectory(dirpath);
			}
			string fullpath = Path.Combine(Application.persistentDataPath, SaveData.basePath, filename);
			byte[] bytesToDecode = System.Text.Encoding.UTF8.GetBytes(jsondata);
			
			using (FileStream fdp = File.Create(fullpath)) {
				using (MemoryStream mem = new MemoryStream(bytesToDecode)) {
					mem.CopyTo(fdp);
				}
			}
		}

		static async Task<bool> requestPermissionAsync(string permission) {
			if (!Permission.HasUserAuthorizedPermission(permission)) {
				Permission.RequestUserPermission(permission);
				while (!Permission.HasUserAuthorizedPermission(permission)) {
					await Task.Delay(200);
				}
			}
			return Permission.HasUserAuthorizedPermission(permission);
		}

		public static dynamic load(string filename) {
			string dirpath = Path.Combine(Application.persistentDataPath, SaveData.basePath);
			if (!Directory.Exists(dirpath)) {
				return null;
			}
			string fullpath = Path.Combine(Application.persistentDataPath, SaveData.basePath, filename);
			if (File.Exists(fullpath)) {
				string data;
				using (MemoryStream mem = new MemoryStream()) {
					using (FileStream fdp = File.OpenRead(fullpath)) {
						fdp.CopyTo(mem);
					}
					data = System.Text.Encoding.UTF8.GetString(mem.ToArray());
				}
				return JsonUtility.FromJson<dynamic>(data);
			} else {
				return null;
			}
		}

		public static T load<T>(string filename) {
			string dirpath = Path.Combine(Application.persistentDataPath, SaveData.basePath);
			if (!Directory.Exists(dirpath)) {
				return default(T);
			}
			string fullpath = Path.Combine(Application.persistentDataPath, SaveData.basePath, filename);
			if (File.Exists(fullpath)) {
				string data;
				using (MemoryStream mem = new MemoryStream()) {
					using (FileStream fdp = File.OpenRead(fullpath)) {
						fdp.CopyTo(mem);
					}
					data = System.Text.Encoding.UTF8.GetString(mem.ToArray());
				}
				return JsonUtility.FromJson<T>(data);
			} else {
				return default(T);
			}
		}

		public static string loadString(string filename) {
			string dirpath = Path.Combine(Application.persistentDataPath, SaveData.basePath);
			if (!Directory.Exists(dirpath)) {
				return null;
			}
			string fullpath = Path.Combine(Application.persistentDataPath, SaveData.basePath, filename);
			if (File.Exists(fullpath)) {
				string data;
				using (MemoryStream mem = new MemoryStream()) {
					using (FileStream fdp = File.OpenRead(fullpath)) {
						fdp.CopyTo(mem);
					}
					data = System.Text.Encoding.UTF8.GetString(mem.ToArray());
				}
				return data;
			} else {
				return null;
			}
		}

		public static bool removeFile(string filename) {
			string fullpath = Path.Combine(Application.persistentDataPath, SaveData.basePath, filename);
			if (File.Exists(fullpath)) {
				File.Delete(fullpath);
			}
			return true;
		}
	}
}
