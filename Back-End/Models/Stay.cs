﻿using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class Stay
    {
        public Stay()
        {
            AdministratorStays = new HashSet<AdministratorStay>();
            Collects = new HashSet<Collect>();
            Favoritestays = new HashSet<Favoritestay>();
            Nears = new HashSet<Near>();
            Rooms = new HashSet<Room>();
            StayLabels = new HashSet<StayLabel>();
        }

        public int StayId { get; set; }
        public int? HostId { get; set; }
        public string StayName { get; set; }
        public string StayType { get; set; }
        public string DetailedAddress { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public byte StayCapacity { get; set; }
        public byte RoomNum { get; set; }
        public byte BedNum { get; set; }
        public decimal PublicBathroom { get; set; }
        public decimal PublicToilet { get; set; }
        public decimal NonBarrierFacility { get; set; }
        public string Characteristic { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public byte DaysMin { get; set; }
        public byte DaysMax { get; set; }
        public decimal StayStatus { get; set; }
        public int? CommentNum { get; set; }
        public int? CommentScore { get; set; }

        public virtual Host Host { get; set; }
        public virtual StayType StayTypeNavigation { get; set; }
        public virtual ICollection<AdministratorStay> AdministratorStays { get; set; }
        public virtual ICollection<Collect> Collects { get; set; }
        public virtual ICollection<Favoritestay> Favoritestays { get; set; }
        public virtual ICollection<Near> Nears { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<StayLabel> StayLabels { get; set; }
    }
}
