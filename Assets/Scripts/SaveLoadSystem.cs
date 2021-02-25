namespace redd096
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using System.IO;

    [System.Serializable]
    public class ClassToSave
    {

    }

    [AddComponentMenu("redd096/Singletons/Save & Load System")]
    public class SaveLoadSystem : Singleton<SaveLoadSystem>
    {
        [Header("Data Directory")]
        [SerializeField] bool usePersistentDataPath = true;
        [SerializeField] string directory = "Saves/";

        string path => usePersistentDataPath ?
            Application.persistentDataPath + directory :    //return persistent data path + directory path
            directory;                                      //return only directory path

        /// <summary>
        /// Save class in directory/key.json
        /// </summary>
        /// <param name="key">Name of the file</param>
        /// <param name="value">Value to save</param>
        public static void Save(string key, ClassToSave value)
        {
            string jsonValue = JsonUtility.ToJson(value);
            File.WriteAllText(Path.Combine(instance.path, key, ".json"), jsonValue);
        }

        public static void Load(string key)
        {

        }
    }
}