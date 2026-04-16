using System;
using System.Runtime.Serialization;

namespace SchoolMangementSystem.Models
{
    [DataContract]
    public class EmployeeRecord
    {
        [DataMember]
        public string EmployeeId { get; set; }

        [DataMember]
        public string FullName { get; set; }

        [DataMember]
        public string Gender { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string JobTitle { get; set; }

        [DataMember]
        public string Department { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string PhotoPath { get; set; }

        [DataMember]
        public DateTime CreatedAtUtc { get; set; }

        [DataMember]
        public DateTime UpdatedAtUtc { get; set; }

        [DataMember]
        public DateTime? DeletedAtUtc { get; set; }
    }
}
