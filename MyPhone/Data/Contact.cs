using MixERP.Net.VCards;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoodTimeStudio.MyPhone.Data
{
    public class Contact : IIdentifiable<string>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; } = null!;

        public string MiddleName { get; set; } = null!;

        public string NickName { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string FormattedName { get; set; } = null!;

        public string Organization { get; set; } = null!;

        public string OrganizationalUnit { get; set; } = null!;

        public VCard Detail { get; set; } = null!;
    }
}
