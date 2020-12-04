using System;

namespace LuzApp.Common.Responses
{
    public class LuminaryResponse
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public UserResponse User { get; set; }
        public DateTime Date { get; set; }
        public string BasePhoto { get; set; }
        public string TopPhoto { get; set; }
        public string FullPhoto { get; set; }

        public string BasePhotoFullPath => string.IsNullOrEmpty(BasePhoto)
           ? "noimage"//null
           : $"http://keypress.serveftp.net:88/LuzAppApi{BasePhoto.Substring(1)}";

        public string TopPhotoFullPath => string.IsNullOrEmpty(TopPhoto)
          ? "noimage"//null
          : $"http://keypress.serveftp.net:88/LuzAppApi{TopPhoto.Substring(1)}";

        public string FullPhotoFullPath => string.IsNullOrEmpty(FullPhoto)
         ? "noimage"//null
         : $"http://keypress.serveftp.net:88/LuzAppApi{FullPhoto.Substring(1)}";

        public string State { get; set; }

        public string Remarks { get; set; }

        public int? CantFotos { get; set; }
    }
}