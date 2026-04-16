using System.Runtime.Serialization;

namespace SchoolMangementSystem.Models
{
    [DataContract]
    internal class AppUser
    {
        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public string PasswordHash { get; set; }
    }
}
