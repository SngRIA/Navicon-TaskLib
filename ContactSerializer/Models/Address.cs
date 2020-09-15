using System;
using ContactSerializer.Enums;
using ContactSerializer.Attributes;

namespace ContactSerializer.Models
{
    [Description("Address")]
    [Serializable]
    public sealed class Address : ICloneable
    {
        public string Country
        {
            get; set;
        }

        public string City
        {
            get; set;
        }

        public string AddressProp
        {
            get; set;
        }

        public AddressType AddressType
        {
            get; set;
        }

        public Address() { }

        public Address(string country, string city, string addressProp, AddressType addressType)
        {
            Country = country;
            City = city;
            AddressProp = addressProp;
            AddressType = addressType;
        }

        #region ICloneable

        public object Clone()
        {
            return new Address
            {
                Country = Country,
                City = City,
                AddressProp = AddressProp,
                AddressType = AddressType
            };
        }

        #endregion
    }
}
