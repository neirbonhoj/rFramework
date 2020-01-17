﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
namespace rFrameworkClasses
{
    public class rDealership
    {
        public string name;
        public int blipID;
        public Vector3 location;
        public List<string> vehicles;

        public rDealership(string name, int blipID, Vector3 location, List<string> vehicles)
        {
            this.name = name;
            this.blipID = blipID;
            this.location = location;
            this.vehicles = vehicles;
        }
    }
}
