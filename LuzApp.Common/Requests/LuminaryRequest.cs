using System;
using System.Collections.Generic;
using System.Text;

namespace LuzApp.Common.Requests
{
    public class LuminaryRequest
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Tipo { get; set; }
        public string Neighborhood { get; set; }
        public byte[] BasePhotoArray { get; set; }
        public byte[] TopPhotoArray { get; set; }
        public byte[] FullPhotoArray { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Date { get; set; }
        public string UserId { get; set; }
        public string State { get; set; }
        public string Remarks { get; set; }
    }
}
