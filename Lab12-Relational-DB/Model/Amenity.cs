﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab12_Relational_DB.Model
{
    public class Amenity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        // add Nab Prop: RoomAmenities

        public RoomAmenities RoomAmenities { get; set; }

    }
}
