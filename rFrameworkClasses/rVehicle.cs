using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rFrameworkClasses
{
    public class rVehicle
    {
        public string make;
        public string model;
        public int year;
        public string spawnName;
        public int price;

        public rVehicle(string make, string model, int year, string spawnName, int price)
        {
            this.make = make;
            this.model = model;
            this.year = year;
            this.spawnName = spawnName;
            this.price = price;
        }
    }
}
