using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;
using ContactSerializer.Models;
using NonSerializedAttribute = ContactSerializer.Attributes.NonSerializedAttribute;

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

        public void Serialize(IEnumerable<Contact> persons)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var person in persons)
            {
                stringBuilder.Append("{");

                if (!IsSerializable(person))
                    throw new SerializationException();

                Type type = typeof(Contact);
                foreach (var prop in type.GetProperties())
                {
                    if (prop.IsDefined(typeof(NonSerializedAttribute)))
                        continue;

                    if (!prop.PropertyType.Name.Equals("Address", StringComparison.OrdinalIgnoreCase))
                    {
                        stringBuilder.Append($"[{ prop.Name }:{ prop.GetValue(person) }]");
                    }
                    else
                    {
                        stringBuilder.Append($"<{prop.Name}:");

                        foreach (var propInner in prop.PropertyType.GetProperties())
                        {
                            stringBuilder.Append($"[{ propInner.Name }:{ propInner.GetValue(person.Address) }]");
                        }

                        stringBuilder.Append(">");
                    }
                }
                stringBuilder.Append("}");
            }

            using (StreamWriter file = File.AppendText(_filePath))
            {
                file.WriteLine(stringBuilder.ToString());
            }
        }

        private IEnumerable<string> GetPropsList(string data, string startString = "[", string endString = "]")
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

        private IEnumerable<(string propName, string propValue)> GetPropsWithValue(string data, string propSplitChar = "=")
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

        private void SetContactPropsData<T>(T contact, (string propName, string propValue) prop)
        {
            Type type = typeof(T);

            var propInfo = typeof(T).GetProperty(prop.propName);

            if (type.GetProperty(prop.propName) != null)
            {
                if (propInfo.PropertyType.IsEnum)
                    propInfo?.SetValue(contact, Enum.Parse(propInfo.PropertyType, prop.propValue));
                else
                    propInfo?.SetValue(contact, Convert.ChangeType(prop.propValue, propInfo.PropertyType));
            }
            else
            {
                IEnumerable<(PropertyInfo propInfo, string name)> innerClases = contact.GetType()
                    .GetProperties()
                    .Where(pi => !pi.PropertyType.Namespace.StartsWith("System"))
                    .Where(pi => pi.PropertyType.GetProperties().FirstOrDefault(piInner => piInner.Name == prop.propName) != null)
                    .Select((pi) => (pi, pi.Name));
                // find type with true prop in current deserized class

                foreach (var trueProp in innerClases)
                {
                    Type innerType = trueProp.propInfo.PropertyType;
                    propInfo = innerType.GetProperty(prop.propName);

                    object newObject = type.GetProperty(trueProp.name).GetValue(contact) ?? Activator.CreateInstance(innerType);

                    var innerClassProp = type.GetProperty(innerType.Name);

                    if (propInfo.PropertyType.IsEnum)
                        propInfo?.SetValue(newObject, Enum.Parse(propInfo.PropertyType, prop.propValue));
                    else
                        propInfo?.SetValue(newObject, Convert.ChangeType(prop.propValue, propInfo.PropertyType));

                    innerClassProp?.SetValue(contact, newObject); // set data to inner class
                }
            }
        }

        public IEnumerable<Contact> Deserialize()
        {
            if (!File.Exists(_filePath))
                throw new FileNotFoundException();

            List<Contact> contacts = new List<Contact>();

            string data = File.ReadAllText(_filePath);
            foreach (var contactStr in GetPropsList(data, "{", "}"))
            {
                Contact contact = new Contact();
                foreach (var prop in GetPropsWithValue(contactStr))
                {
                    SetContactPropsData(contact, prop);
                }
                contacts.Add(contact);
            }

            return contacts;
        }
    }
}