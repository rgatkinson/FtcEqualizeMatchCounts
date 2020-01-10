using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEMC.DAL
    {
    internal class LeagueData
        {
        public readonly string LeagueCode;
        public string Name;
        public string Country;
        public string State;
        public string City;

        public LeagueData(string leagueCode, string name, string country, string state, string city)
            {
            LeagueCode = leagueCode;
            Name = name;
            Country = country;
            State = state;
            City = city;
            }

        public override bool Equals(object obj)
            {
            if (GetType() == obj?.GetType())
                {
                LeagueData them = (LeagueData) obj;
                if (!Equals(LeagueCode, them.LeagueCode)) return false;
                if (!Equals(Name, them.Name)) return false;
                if (!Equals(Country, them.Country)) return false;
                if (!Equals(State, them.State)) return false;
                if (!Equals(City, them.City)) return false;
                return true;
                }

            return false;
            }

        public override int GetHashCode()
            {
            return HashCode.Combine(LeagueCode, Name, Country, State, City);
            }
        }
    }