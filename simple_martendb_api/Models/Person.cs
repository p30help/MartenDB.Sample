using System;
using System.Collections.Generic;

namespace simple_martendb_api.Models
{
    public class Person
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string FullName { get; set; }

        public DateTime BirthDate { get; set; }

        public string Phone { get; set; }

        public List<Address> Addresses { get; set; }

        public List<BankAccount> Accounts { get; set; }
    }

    public class Address
    {
        public Guid Id { get; set; }

        public string Street { get; set; }
    }

    public class BankAccount
    {
        public Guid Id { get; set; }

        public string Iban { get; set; }
        public string BankName { get; set; }
    }
}
