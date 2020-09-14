using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContactSerializer.Models;

namespace ContactSerializer.Serializers.Models
{
    /// <summary>
    /// Class helps to serialize multiple contacts
    /// </summary>
    public sealed class ContactSerializeData
    {
        public Contact Contact;

        /// <summary>
        /// Set path with file extension
        /// </summary>
        public string PathSave;

        public ContactSerializeData(Contact contact, string pathSave)
        {
            if (contact == null || string.IsNullOrEmpty(pathSave))
                throw new ArgumentException();

            Contact = contact;
            PathSave = pathSave;
        }
    }
}
