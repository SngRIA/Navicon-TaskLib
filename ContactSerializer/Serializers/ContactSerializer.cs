using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;
using ContactSerializer.Models;
using ContactSerializer.Serializers.Models;

namespace ContactSerializer.Serializers
{
    public sealed class ContactSerializer
    {
        private string _filePath = string.Empty;

        public ContactSerializer(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException();

            _filePath = filePath;
        }

        public static bool IsSerializable<T>(T contact)
        {
            if (typeof(T).IsDefined(typeof(SerializableAttribute)))
                return true;

            return false;
        }

        public void Serialize(Contact person)
        {
            if (!IsSerializable(person))
                throw new SerializationException();

            StringBuilder stringBuilder = new StringBuilder();

            Type type = typeof(Contact);
            foreach (var prop in type.GetProperties())
            {
                if (prop.Name.Equals("Age", StringComparison.OrdinalIgnoreCase))
                    continue;

                stringBuilder.Append("[");
                stringBuilder.Append(prop.Name);
                stringBuilder.Append(":");
                stringBuilder.Append(prop.GetValue(person));
                stringBuilder.Append("]");
            }

            File.WriteAllText(_filePath, stringBuilder.ToString());
        }

        public static void Serialize<T>(ICollection<ContactSerializeData> persons)
        {
            foreach (var person in persons)
            {
                if (!IsSerializable(person.Contact))
                    throw new SerializationException();

                StringBuilder stringBuilder = new StringBuilder();

                foreach (var prop in typeof(T).GetProperties())
                {
                    if (prop.Name.Equals("Age", StringComparison.OrdinalIgnoreCase))
                        continue;

                    stringBuilder.Append("[");
                    stringBuilder.Append(prop.Name);
                    stringBuilder.Append(":");
                    stringBuilder.Append(prop.GetValue(person.Contact));
                    stringBuilder.Append("]");
                }

                File.WriteAllText(person.PathSave, stringBuilder.ToString());

                stringBuilder.Clear();
            }
        }

        private static List<string> GetPropsList(string data, string startString = "[", string endString = "]")
        {
            List<string> props = new List<string>();

            bool finish = false;
            while (!finish)
            {
                int startIndex = data.IndexOf(startString);
                int endIndex = data.IndexOf(endString);

                if (startIndex != -1 && endIndex != -1)
                {
                    props.Add(data.Substring(startIndex + startString.Length, endIndex - startIndex - startString.Length));
                    data = data.Substring(endIndex + endString.Length);
                }
                else
                {
                    finish = true;
                }
            }

            return props;
        }

        private static List<(string propName, string propValue)> GetPropsWithValue(string data, string propSplitChar = "=")
        {
            List<(string propName, string propValue)> props = new List<(string propName, string propValue)>();

            foreach (var prop in GetPropsList(data))
            {
                int startIndex = prop.IndexOf(":");

                props.Add((
                    prop.Substring(0, startIndex),
                    prop.Substring(startIndex + propSplitChar.Length)
                    ));
            }

            return props;
        }

        private static void SetContactPropsData<T>(T contact, (string propName, string propValue) prop)
        {
            var propInfo = typeof(T).GetProperty(prop.propName);

            if (propInfo.PropertyType.IsEnum)
                propInfo?.SetValue(contact, Enum.Parse(propInfo.PropertyType, prop.propValue));
            else
                propInfo?.SetValue(contact, Convert.ChangeType(prop.propValue, propInfo.PropertyType));
        }

        public Contact Deserialize()
        {
            if (!File.Exists(_filePath))
                throw new FileNotFoundException();

            Contact contact = new Contact();

            string data = File.ReadAllText(_filePath);
            foreach (var prop in GetPropsWithValue(data))
            {
                SetContactPropsData(contact, prop);
            }

            return contact;
        }

        /// <summary>
        /// Deserialize file from paths, path with file extension
        /// </summary>
        /// <typeparam name="T">Contact</typeparam>
        /// <param name="paths">Paths to files</param>
        /// <returns>Return deserialize collection T</returns>
        public static ICollection<T> Deserialize<T>(ICollection<string> paths)
            where T : new()
        {
            if(paths.Any(path => !File.Exists(path)))
                throw new FileNotFoundException();

            List<T> contacts = new List<T>();

            foreach (var path in paths)
            {
                T contact = new T();

                string data = File.ReadAllText(path);
                foreach (var prop in GetPropsWithValue(data))
                {
                    SetContactPropsData(contact, prop);
                }

                contacts.Add(contact);
            }

            return contacts;
        }
    }
}