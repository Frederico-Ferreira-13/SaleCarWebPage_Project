using Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Model
{
    public class Address : IEntity
    {
        [Key]
        public int AddressId { get; private set; }
        public string Street { get; private set; } = string.Empty;
        public string? Street2 { get; private set; }
        public string DoorNumber { get; private set; } = string.Empty;
        public string? Floor { get; private set; }
        public string PostalCode { get; private set; } = string.Empty;
        public string Locate { get; private set; } = string.Empty;
        public string City { get; private set; } = string.Empty;
        public string Country { get; private set; } = string.Empty;

        private Address() { }

        public Address(string street, string? street2, string doorNumber, string? floor, string postalCode,
        string city, string country, string locate)
        {
            ValidateAddressData(street, street2, doorNumber, floor, postalCode, city, country, locate);
            ValidatePostalCode(postalCode);

            Street = street;
            Street2 = street2;
            DoorNumber = doorNumber;
            Floor = floor;
            PostalCode = postalCode.Trim();
            Locate = locate;
            City = city;
            Country = country;
        }

        public void UpdateDetails(string street, string? street2, string doorNumber, string? floor, string postalCode,
             string city, string country, string locate)
        {
            ValidateAddressData(street, street2, doorNumber, floor, postalCode, city, country, locate);
            ValidatePostalCode(postalCode);

            Street = street;
            Street2 = street2;
            DoorNumber = doorNumber;
            Floor = floor;
            PostalCode = postalCode.Trim();
            Locate = locate;
            City = city;
            Country = country;
        }        

        public int GetId() => AddressId;

        public void SetId(int id)
        {
            if (AddressId != 0)
            {
                throw new InvalidOperationException("Não é permitido alterar o ID de uma Entidade já persistida.");
            }
            if (id <= 0)
            {
                throw new ArgumentException("O ID deve ser positivo.", nameof(id));
            }

            AddressId = id;
        }

        public bool GetIsActive() => true;

        private void ValidateAddressData(string street, string? street2, string doorNumber, string? floor, string postalCode,
            string city, string country, string locate)
        {
            if (string.IsNullOrWhiteSpace(street))
            {
                throw new ArgumentException("A rua é obrigatória.", nameof(street));
            }
            if (string.IsNullOrWhiteSpace(doorNumber))
            {
                throw new ArgumentException("O número da porta é obrigatório.", nameof(doorNumber));
            }
            if (string.IsNullOrWhiteSpace(postalCode))
            {
                throw new ArgumentException("O código postal é obrigatório.", nameof(postalCode));
            }
            if (string.IsNullOrWhiteSpace(locate))
            {
                throw new ArgumentException("A localidade é obrigatória.", nameof(locate));
            }
            if (string.IsNullOrWhiteSpace(city))
            {
                throw new ArgumentException("A Cidade é obrigatória.", nameof(city));
            }
            if (string.IsNullOrWhiteSpace(country))
            {
                throw new ArgumentException("O país é obrigatório.", nameof(country));
            }
        }

        private static void ValidatePostalCode(string postalCode)
        {
            if (!Regex.IsMatch(postalCode, @"^\d{4}-\d{3}$"))
            {
                throw new ArgumentException("O código postal deve ter o formato 'XXXX-YYY'.");
            }
        }           
    }
}