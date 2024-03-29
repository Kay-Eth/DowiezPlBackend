using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace DowiezPlBackend.Models
{
    public class AppUser : IdentityUser<Guid>
    {
        public AppUser() : base()
        {
            CreationDate = DateTime.UtcNow;
        }

        public AppUser(string userName) : base(userName)
        {
            CreationDate = DateTime.UtcNow;
        }

        [PersonalData]
        public string FirstName { get; set; }

        [PersonalData]
        public string LastName { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }
        
        public bool Banned { get; set; }

        [InverseProperty("Issuer")]
        public ICollection<Opinion> IssuedOpinions { get; set; }

        [InverseProperty("Rated")]
        public ICollection<Opinion> OpinionsRecieved { get; set; }

        [InverseProperty("Creator")]
        public ICollection<Demand> CreatedDemands { get; set; }

        [InverseProperty("Reciever")]
        public ICollection<Demand> RecievingDemands { get; set; }

        public ICollection<Transport> PerformedTransports { get; set; }

        [InverseProperty("Reporter")]
        public ICollection<Report> ReportedReports { get; set; }

        [InverseProperty("Reported")]
        public ICollection<Report> ReferedReports { get; set; }

        public ICollection<Participant> Participations { get; set; }
        public ICollection<Member> Memberships { get; set; }

        public ICollection<Connection> Connections { get; set; }
    }
}